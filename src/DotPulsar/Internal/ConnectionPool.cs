using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class ConnectionPool : IDisposable
    {
        private readonly AsyncLock _lock;
        private readonly int _protocolVersion;
        private readonly string _clientVersion;
        private readonly Uri _serviceUrl;
        private readonly Connector _connector;
        private readonly ConcurrentDictionary<Uri, Connection> _connections;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _closeInactiveConnections;

        public ConnectionPool(int protocolVersion, string clientVersion, Uri serviceUrl, Connector connector)
        {
            _lock = new AsyncLock();
            _protocolVersion = protocolVersion;
            _clientVersion = clientVersion;
            _serviceUrl = serviceUrl;
            _connector = connector;
            _connections = new ConcurrentDictionary<Uri, Connection>();
            _cancellationTokenSource = new CancellationTokenSource();
            _closeInactiveConnections = CloseInactiveConnections(TimeSpan.FromSeconds(60), _cancellationTokenSource.Token);
        }

        public void Dispose() //While we wait for IAsyncDisposable
        {
            _cancellationTokenSource.Cancel();
            _closeInactiveConnections.Wait();

            _lock.Dispose();

            foreach (var serviceUrl in _connections.Keys.ToArray())
            {
                Deregister(serviceUrl);
            }
        }

        public async Task<Connection> FindConnectionForTopic(string topic, CancellationToken cancellationToken)
        {
            var lookup = new CommandLookupTopic
            {
                Topic = topic,
                Authoritative = false
            };

            var serviceUrl = _serviceUrl;

            while (true)
            {
                var connection = await CreateConnection(serviceUrl, cancellationToken);
                var response = await connection.Send(lookup);
                response.Expect(BaseCommand.Type.LookupResponse);

                if (response.LookupTopicResponse.Response == CommandLookupTopicResponse.LookupType.Failed)
                    response.LookupTopicResponse.Throw();

                lookup.Authoritative = response.LookupTopicResponse.Authoritative;
                serviceUrl = new Uri(response.LookupTopicResponse.BrokerServiceUrl);

                if (response.LookupTopicResponse.Response == CommandLookupTopicResponse.LookupType.Redirect || !response.LookupTopicResponse.Authoritative)
                    continue;

                if (_serviceUrl.IsLoopback) // LookupType is 'Connect', ServiceUrl is local and response is authoritative. Assume the Pulsar server is a standalone docker.
                    return connection;
                else
                    return await CreateConnection(serviceUrl, cancellationToken);
            }
        }

        private async Task<Connection> CreateConnection(Uri serviceUrl, CancellationToken cancellationToken)
        {

            using (await _lock.Lock(cancellationToken))
            {
                if (_connections.TryGetValue(serviceUrl, out Connection connection))
                    return connection;

                var stream = await _connector.Connect(serviceUrl);

                connection = new Connection(stream);
                Register(serviceUrl, connection);

                var connect = new CommandConnect
                {
                    ProtocolVersion = _protocolVersion,
                    ClientVersion = _clientVersion
                };

                var response = await connection.Send(connect);
                response.Expect(BaseCommand.Type.Connected);

                return connection;
            }
        }

        private void Register(Uri serviceUrl, Connection connection)
        {
            _connections[serviceUrl] = connection;
            connection.IsClosed.ContinueWith(t => Deregister(serviceUrl));
        }

        private void Deregister(Uri serviceUrl)
        {
            if (_connections.TryRemove(serviceUrl, out Connection connection))
                connection.Dispose();
        }

        private async Task CloseInactiveConnections(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(interval, cancellationToken);

                    using (await _lock.Lock(cancellationToken))
                    {
                        var serviceUrls = _connections.Keys;
                        foreach (var serviceUrl in serviceUrls)
                        {
                            var connection = _connections[serviceUrl];
                            if (connection == null)
                                continue;
                            if (!await connection.IsActive())
                                Deregister(serviceUrl);
                        }
                    }
                }
                catch { }
            }
        }
    }
}
