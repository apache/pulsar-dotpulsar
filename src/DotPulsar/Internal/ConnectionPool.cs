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

using Abstractions;
using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using Extensions;
using PulsarApi;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed class ConnectionPool : IConnectionPool
{
    private readonly AsyncLock _lock;
    private readonly CommandConnect _commandConnect;
    private readonly Uri _serviceUrl;
    private readonly Connector _connector;
    private readonly EncryptionPolicy _encryptionPolicy;
    private readonly ConcurrentDictionary<PulsarUrl, Connection> _connections;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _closeInactiveConnections;
    private readonly string? _listenerName;
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
        _lock = new AsyncLock();
        _commandConnect = commandConnect;
        _serviceUrl = serviceUrl;
        _connector = connector;
        _encryptionPolicy = encryptionPolicy;
        _listenerName = listenerName;
        _connections = new ConcurrentDictionary<PulsarUrl, Connection>();
        _cancellationTokenSource = new CancellationTokenSource();
        _closeInactiveConnections = CloseInactiveConnections(closeInactiveConnectionsInterval, _cancellationTokenSource.Token);
        _keepAliveInterval = keepAliveInterval;
        _authentication = authentication;
    }

    public async ValueTask DisposeAsync()
    {
        _cancellationTokenSource.Cancel();

        await _closeInactiveConnections.ConfigureAwait(false);

        await _lock.DisposeAsync().ConfigureAwait(false);

        foreach (var serviceUrl in _connections.Keys.ToArray())
        {
            await DisposeConnection(serviceUrl).ConfigureAwait(false);
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
        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            if (_connections.TryGetValue(url, out Connection? connection) && connection is not null)
                return connection;

            return await EstablishNewConnection(url, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<Connection> EstablishNewConnection(PulsarUrl url, CancellationToken cancellationToken)
    {
        var stream = await _connector.Connect(url.Physical).ConfigureAwait(false);
        var connection = new Connection(new PulsarStream(stream), _keepAliveInterval, _authentication);
        DotPulsarMeter.ConnectionCreated();
        _connections[url] = connection;
        _ = connection.ProcessIncommingFrames(_cancellationTokenSource.Token).ContinueWith(t => DisposeConnection(url));
        var commandConnect = _commandConnect;

        if (url.ProxyThroughServiceUrl)
            commandConnect = WithProxyToBroker(commandConnect, url.Logical);

        var response = await connection.Send(commandConnect, cancellationToken).ConfigureAwait(false);
        response.Expect(BaseCommand.Type.Connected);
        return connection;
    }

    private async ValueTask DisposeConnection(PulsarUrl serviceUrl)
    {
        if (_connections.TryRemove(serviceUrl, out Connection? connection) && connection is not null)
        {
            await connection.DisposeAsync().ConfigureAwait(false);
            DotPulsarMeter.ConnectionDisposed();
        }
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

    private async Task CloseInactiveConnections(TimeSpan interval, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(interval, cancellationToken).ConfigureAwait(false);

                using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
                {
                    var serviceUrls = _connections.Keys;
                    foreach (var serviceUrl in serviceUrls)
                    {
                        var connection = _connections[serviceUrl];
                        if (connection is null)
                            continue;

                        if (!await connection.HasChannels(cancellationToken).ConfigureAwait(false))
                            await DisposeConnection(serviceUrl).ConfigureAwait(false);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
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
}
