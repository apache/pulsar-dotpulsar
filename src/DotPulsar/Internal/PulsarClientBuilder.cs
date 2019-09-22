using DotPulsar.Abstractions;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

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

        private TimeSpan _retryInterval;
        private Uri _serviceUrl;
        private X509Certificate2 _trustedCertificateAuthority;
        private bool _verifyCertificateAuthority;
        private bool _verifyCertificateName;

        public PulsarClientBuilder()
        {
            _retryInterval = TimeSpan.FromSeconds(3);
            _serviceUrl = new Uri("pulsar://localhost:6650");
            _verifyCertificateAuthority = true;
            _verifyCertificateName = false;
        }

        public IPulsarClientBuilder RetryInterval(TimeSpan interval)
        {
            _retryInterval = interval;
            return this;
        }

        public IPulsarClientBuilder ServiceUrl(Uri uri)
        {
            _serviceUrl = uri;
            return this;
        }

        public IPulsarClientBuilder TrustedCertificateAuthority(X509Certificate2 trustedCertificateAuthority)
        {
            _trustedCertificateAuthority = trustedCertificateAuthority;
            return this;
        }

        public IPulsarClientBuilder VerifyCertificateAuthority(bool verifyCertificateAuthority)
        {
            _verifyCertificateAuthority = verifyCertificateAuthority;
            return this;
        }

        public IPulsarClientBuilder VerifyCertificateName(bool verifyCertificateName)
        {
            _verifyCertificateName = verifyCertificateName;
            return this;
        }

        public IPulsarClient Build()
        {
            var connector = new Connector(_trustedCertificateAuthority, _verifyCertificateAuthority, _verifyCertificateName);
            var connectionPool = new ConnectionPool(ProtocolVersion, ClientVersion, _serviceUrl, connector);
            return new PulsarClient(connectionPool, new FaultStrategy(_retryInterval));
        }
    }
}
