using DotPulsar.Internal.PulsarApi;

namespace DotPulsar.Internal
{
    public sealed class PingPongHandler
    {
        private readonly Connection _connection;
        private readonly CommandPong _pong;

        public PingPongHandler(Connection connection)
        {
            _connection = connection;
            _pong = new CommandPong();
        }

        public void Incoming(CommandPing ping)
        {
            _ = _connection.Send(_pong);
        }
    }
}
