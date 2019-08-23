using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
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
        private readonly ConcurrentDictionary<Uri, Connection> _connections;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _closeInactiveConnections;

        public ConnectionPool(int protocolVersion, string clientVersion, Uri serviceUrl)
        {
            _lock = new AsyncLock();
            _protocolVersion = protocolVersion;
            _clientVersion = clientVersion;
            _serviceUrl = serviceUrl;
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
            var connection = await CreateConnection(_serviceUrl, cancellationToken);

            var authoritative = false;

            while (true)
            {
                var lookup = new CommandLookupTopic
                {
                    Topic = topic,
                    Authoritative = authoritative
                };
                var response = await connection.Send(lookup);
                response.Expect(BaseCommand.Type.LookupResponse);

                switch (response.LookupTopicResponse.Response)
                {
                    case CommandLookupTopicResponse.LookupType.Connect:
                        return connection;
                    case CommandLookupTopicResponse.LookupType.Redirect:
                        authoritative = response.LookupTopicResponse.Authoritative;
                        connection = await CreateConnection(new Uri(response.LookupTopicResponse.BrokerServiceUrl), cancellationToken);
                        continue;
                    case CommandLookupTopicResponse.LookupType.Failed:
                        response.LookupTopicResponse.Throw();
                        break;
                    default:
                        throw new InvalidEnumArgumentException("LookupType", (int)response.LookupTopicResponse.Response, typeof(CommandLookupTopicResponse.LookupType));
                }
            }
        }

        private async Task<Connection> CreateConnection(Uri serviceUrl, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken))
            {
                if (_connections.TryGetValue(serviceUrl, out Connection connection))
                    return connection;

                var tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serviceUrl.Host, serviceUrl.Port);

                connection = new Connection(tcpClient.GetStream());
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
