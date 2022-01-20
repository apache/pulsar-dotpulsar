namespace DotPulsar.Internal;

using Abstractions;
using PulsarApi;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class AccessTokenConnectionPool : IConnectionPool
{
    private readonly Func<Task<string>> _accessTokenFactory;
    private readonly CommandConnect _commandConnect;
    private readonly Func<CommandConnect, ConnectionPool> _poolFactory;
    private readonly AutoResetEvent _resetEvent = new(true);

    private ConnectionPool? _currentConnectionPool;

    public AccessTokenConnectionPool(Func<Task<string>> accessTokenFactory,
        CommandConnect commandConnect,
        Func<CommandConnect, ConnectionPool> poolFactory)
    {
        _accessTokenFactory = accessTokenFactory;
        _commandConnect = commandConnect;
        _poolFactory = poolFactory;
    }

    public ValueTask DisposeAsync()
        => _currentConnectionPool?.DisposeAsync() ?? new ValueTask(Task.CompletedTask);

    public async ValueTask<IConnection> FindConnectionForTopic(string topic, CancellationToken cancellationToken = default)
    {
        if (_currentConnectionPool == null)
        {
            while (!_resetEvent.WaitOne(TimeSpan.Zero))
            {
                await Task.Delay(1000, cancellationToken);
            }

            if (_currentConnectionPool == null)
            {
                try
                {
                    var token = await _accessTokenFactory();
                    _commandConnect.AuthMethodName = "token";
                    _commandConnect.AuthData = Encoding.UTF8.GetBytes(token);
                    _currentConnectionPool = _poolFactory(_commandConnect);
                }
                finally
                {
                    _resetEvent.Set();
                }
            }
        }

        return await _currentConnectionPool.FindConnectionForTopic(topic, cancellationToken);
    }
}
