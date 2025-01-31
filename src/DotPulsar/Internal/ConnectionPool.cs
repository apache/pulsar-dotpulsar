/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar.Internal;

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

public sealed class ConnectionPool : IConnectionPool
{
    private readonly CommandConnect _commandConnect;
    private readonly Uri _serviceUrl;
    private readonly Connector _connector;
    private readonly EncryptionPolicy _encryptionPolicy;
    private readonly ConcurrentDictionary<PulsarUrl, Connection> _connections;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly string? _listenerName;
    private readonly TimeSpan _closeInactiveConnectionsInterval;
    private readonly TimeSpan _keepAliveInterval;
    private readonly IAuthentication? _authentication;

    public ConnectionPool(
        CommandConnect commandConnect,
        Uri serviceUrl,
        Connector connector,
        EncryptionPolicy encryptionPolicy,
        TimeSpan closeInactiveConnectionsInterval,
        string? listenerName,
        TimeSpan keepAliveInterval,
        IAuthentication? authentication)
    {
        _commandConnect = commandConnect;
        _serviceUrl = serviceUrl;
        _connector = connector;
        _encryptionPolicy = encryptionPolicy;
        _listenerName = listenerName;
        _connections = new ConcurrentDictionary<PulsarUrl, Connection>();
        _cancellationTokenSource = new CancellationTokenSource();
        _closeInactiveConnectionsInterval = closeInactiveConnectionsInterval;
        _keepAliveInterval = keepAliveInterval;
        _authentication = authentication;
    }

    public async ValueTask DisposeAsync()
    {
        _cancellationTokenSource.Cancel();

        foreach (var entry in _connections.ToArray())
        {
            await DisposeConnection(entry.Key, entry.Value).ConfigureAwait(false);
        }
    }

    public async ValueTask<IConnection> FindConnectionForTopic(string topic, CancellationToken cancellationToken)
    {
        var lookup = new CommandLookupTopic
        {
            Topic = topic,
            Authoritative = false,
            AdvertisedListenerName = _listenerName
        };

        var physicalUrl = _serviceUrl;

        while (true)
        {
            var connection = await GetConnection(physicalUrl, cancellationToken).ConfigureAwait(false);
            var response = await connection.Send(lookup, cancellationToken).ConfigureAwait(false);

            response.Expect(BaseCommand.Type.LookupResponse);

            if (response.LookupTopicResponse.Response == CommandLookupTopicResponse.LookupType.Failed)
                response.LookupTopicResponse.Throw();

            lookup.Authoritative = response.LookupTopicResponse.Authoritative;

            var lookupResponseServiceUrl = new Uri(GetBrokerServiceUrl(response.LookupTopicResponse));

            if (response.LookupTopicResponse.Response == CommandLookupTopicResponse.LookupType.Redirect || !response.LookupTopicResponse.Authoritative)
            {
                physicalUrl = lookupResponseServiceUrl;
                continue;
            }

            if (response.LookupTopicResponse.ProxyThroughServiceUrl)
            {
                var url = new PulsarUrl(physicalUrl, lookupResponseServiceUrl);
                return await GetConnection(url, cancellationToken).ConfigureAwait(false);
            }

            // LookupType is 'Connect', ServiceUrl is local and response is authoritative. Assume the Pulsar server is a standalone docker.
            return lookupResponseServiceUrl.IsLoopback
                ? connection
                : await GetConnection(lookupResponseServiceUrl, cancellationToken).ConfigureAwait(false);
        }
    }

    private string GetBrokerServiceUrl(CommandLookupTopicResponse response)
    {
        var hasBrokerServiceUrl = !string.IsNullOrEmpty(response.BrokerServiceUrl);
        var hasBrokerServiceUrlTls = !string.IsNullOrEmpty(response.BrokerServiceUrlTls);

        switch (_encryptionPolicy)
        {
            case EncryptionPolicy.EnforceEncrypted:
                if (!hasBrokerServiceUrlTls)
                    throw new ConnectionSecurityException("Cannot enforce encrypted connections. The lookup topic response from broker gave no secure alternative.");
                return response.BrokerServiceUrlTls;
            case EncryptionPolicy.EnforceUnencrypted:
                if (!hasBrokerServiceUrl)
                    throw new ConnectionSecurityException("Cannot enforce unencrypted connections. The lookup topic response from broker gave no unsecure alternative.");
                return response.BrokerServiceUrl;
            case EncryptionPolicy.PreferEncrypted:
                return hasBrokerServiceUrlTls ? response.BrokerServiceUrlTls : response.BrokerServiceUrl;
            default:
                return hasBrokerServiceUrl ? response.BrokerServiceUrl : response.BrokerServiceUrlTls;
        }
    }

    private async ValueTask<Connection> GetConnection(Uri serviceUrl, CancellationToken cancellationToken)
        => await GetConnection(new PulsarUrl(serviceUrl, serviceUrl), cancellationToken).ConfigureAwait(false);

    private async ValueTask<Connection> GetConnection(PulsarUrl url, CancellationToken cancellationToken)
    {
        if (_connections.TryGetValue(url, out var connection) && connection is not null)
            return connection;

        return await EstablishNewConnection(url, cancellationToken).ConfigureAwait(false);
    }

    private async Task<Connection> EstablishNewConnection(PulsarUrl url, CancellationToken cancellationToken)
    {
        var stream = await _connector.Connect(url.Physical, cancellationToken).ConfigureAwait(false);

        var commandConnect = _commandConnect;
        if (url.ProxyThroughServiceUrl)
            commandConnect = WithProxyToBroker(commandConnect, url.Logical);

        var connection = Connection.Connect(new PulsarStream(stream), _authentication, _keepAliveInterval, _closeInactiveConnectionsInterval);
        _ = connection.State.OnStateChangeFrom(ConnectionState.Connected, CancellationToken.None).AsTask().ContinueWith(t => DisposeConnection(url, connection), CancellationToken.None);
        var response = await connection.Send(commandConnect, cancellationToken).ConfigureAwait(false);
        response.Expect(BaseCommand.Type.Connected);
        _connections[url] = connection;
        connection.MaxMessageSize = response.Connected.MaxMessageSize;
        return connection;
    }

    private async ValueTask DisposeConnection(PulsarUrl serviceUrl, Connection connection)
    {
        _connections.TryRemove(serviceUrl, out var _);
        await connection.DisposeAsync().ConfigureAwait(false);
    }

    private static CommandConnect WithProxyToBroker(CommandConnect commandConnect, Uri logicalUrl)
    {
        return new CommandConnect
        {
            AuthData = commandConnect.ShouldSerializeAuthData() ? commandConnect.AuthData : null,
            AuthMethod = commandConnect.ShouldSerializeAuthMethod() ? commandConnect.AuthMethod : AuthMethod.AuthMethodNone,
            AuthMethodName = commandConnect.ShouldSerializeAuthMethodName() ? commandConnect.AuthMethodName : null,
            ClientVersion = commandConnect.ClientVersion,
            OriginalPrincipal = commandConnect.ShouldSerializeOriginalPrincipal() ? commandConnect.OriginalPrincipal : null,
            ProtocolVersion = commandConnect.ProtocolVersion,
            OriginalAuthData = commandConnect.ShouldSerializeOriginalAuthData() ? commandConnect.OriginalAuthData : null,
            OriginalAuthMethod = commandConnect.ShouldSerializeOriginalAuthMethod() ? commandConnect.OriginalAuthMethod : null,
            ProxyToBrokerUrl = $"{logicalUrl.Host}:{logicalUrl.Port}",
            FeatureFlags = commandConnect.FeatureFlags
        };
    }

    private sealed class PulsarUrl : IEquatable<PulsarUrl>
    {
        public PulsarUrl(Uri physical, Uri logical)
        {
            Physical = physical;
            Logical = logical;
            ProxyThroughServiceUrl = physical != logical;
        }

        public Uri Physical { get; }

        public Uri Logical { get; }

        public bool ProxyThroughServiceUrl { get; }

        public bool Equals(PulsarUrl? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Physical.Equals(other.Physical) && Logical.Equals(other.Logical);
        }

        public override bool Equals(object? obj)
            => obj is PulsarUrl url && Equals(url);

        public override int GetHashCode()
            => HashCode.Combine(Physical, Logical);

        public override string ToString()
            => $"{nameof(Physical)}: {Physical}, {nameof(Logical)}: {Logical}, {nameof(ProxyThroughServiceUrl)}: {ProxyThroughServiceUrl}";
    }

    public async ValueTask<uint> GetNumberOfPartitions(string topic, CancellationToken cancellationToken = default)
    {
        var connection = await FindConnectionForTopic(topic, cancellationToken).ConfigureAwait(false);
        var commandPartitionedMetadata = new CommandPartitionedTopicMetadata { Topic = topic };
        var response = await connection.Send(commandPartitionedMetadata, cancellationToken).ConfigureAwait(false);

        response.Expect(BaseCommand.Type.PartitionedMetadataResponse);

        if (response.PartitionMetadataResponse.Response == CommandPartitionedTopicMetadataResponse.LookupType.Failed)
            response.PartitionMetadataResponse.Throw();

        return response.PartitionMetadataResponse.Partitions;
    }

    public async ValueTask<IEnumerable<string>> GetTopicsOfNamespace(RegexSubscriptionMode mode, Regex topicsPattern, CancellationToken cancellationToken = default)
    {
        var topicUriPattern = new Regex(@"^(persistent|non-persistent)://([^/]+)/([^/]+)/(.+)$", RegexOptions.Compiled);

        var patternString = topicsPattern.ToString();

        var match = topicUriPattern.Match(patternString);
        if (!match.Success)
            throw new InvalidTopicsPatternException($"The topics pattern '{patternString}' is not valid");

        var persistence = match.Groups[1].Value;
        var tenant = match.Groups[2].Value;
        var ns = match.Groups[3].Value;

        if (!string.IsNullOrEmpty(persistence))
        {
            if (persistence.Equals("persistent"))
                mode = RegexSubscriptionMode.Persistent;
            else
                mode = RegexSubscriptionMode.NonPersistent;
        }

        var getTopicsOfNamespace = new CommandGetTopicsOfNamespace
        {
            mode = (CommandGetTopicsOfNamespace.Mode) mode,
            Namespace =$"{tenant}/{ns}",
            TopicsPattern = patternString
        };

        var connection = await GetConnection(_serviceUrl, cancellationToken).ConfigureAwait(false);
        var response = await connection.Send(getTopicsOfNamespace, cancellationToken).ConfigureAwait(false);

        response.Expect(BaseCommand.Type.GetTopicsOfNamespaceResponse);

        if (response.getTopicsOfNamespaceResponse.Filtered)
            return response.getTopicsOfNamespaceResponse.Topics;

        var topics = new List<string>();

        foreach (var topic in response.getTopicsOfNamespaceResponse.Topics)
        {
            if (topicsPattern.Match(topic).Success)
                topics.Add(topic);
        }

        return topics;
    }
}
