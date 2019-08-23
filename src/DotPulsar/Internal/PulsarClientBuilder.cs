using DotPulsar.Abstractions;
using System;
using System.Reflection;

namespace DotPulsar.Internal
{
    public sealed class PulsarClientBuilder : IPulsarClientBuilder
    {
        private static readonly int ProtocolVersion;
        private static readonly string ClientVersion;

        static PulsarClientBuilder()
        {
            ProtocolVersion = 12;
            var assemblyName = Assembly.GetCallingAssembly().GetName();
            ClientVersion = assemblyName.Name + " " + assemblyName.Version.ToString(3);
        }

        private Uri _serviceUrl;

        public PulsarClientBuilder() => _serviceUrl = new Uri("pulsar://localhost:6650");

        public IPulsarClientBuilder ServiceUrl(Uri uri)
        {
            _serviceUrl = uri;
            return this;
        }

        public IPulsarClient Build()
        {
            var connectionPool = new ConnectionPool(ProtocolVersion, ClientVersion, _serviceUrl);
            return new PulsarClient(connectionPool);
        }
    }
}
