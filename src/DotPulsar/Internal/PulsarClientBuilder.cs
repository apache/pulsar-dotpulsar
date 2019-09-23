using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DotPulsar.Internal
{
    public sealed class PulsarClientBuilder : IPulsarClientBuilder
    {
        private readonly CommandConnect _commandConnect;
        private EncryptionPolicy? _encryptionPolicy;
        private TimeSpan _retryInterval;
        private Uri _serviceUrl;
        private X509Certificate2 _trustedCertificateAuthority;
        private bool _verifyCertificateAuthority;
        private bool _verifyCertificateName;

        public PulsarClientBuilder()
        {
            _commandConnect = new CommandConnect
            {
                ProtocolVersion = Constants.ProtocolVersion,
                ClientVersion = Constants.ClientVersion
            };
            _retryInterval = TimeSpan.FromSeconds(3);
            _serviceUrl = new Uri(Constants.PulsarScheme + "://localhost:" + Constants.DefaultPulsarPort);
            _verifyCertificateAuthority = true;
            _verifyCertificateName = false;
        }

        public IPulsarClientBuilder AuthenticateUsingToken(string token)
        {
            _commandConnect.AuthMethodName = "token";
            _commandConnect.AuthData = Encoding.ASCII.GetBytes(token);
            return this;
        }

        public IPulsarClientBuilder ConnectionSecurity(EncryptionPolicy encryptionPolicy)
        {
            _encryptionPolicy = encryptionPolicy;
            return this;
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
            var scheme = _serviceUrl.Scheme;

            if (scheme == Constants.PulsarScheme)
            {
                if (!_encryptionPolicy.HasValue)
                    _encryptionPolicy = EncryptionPolicy.EnforceUnencrypted;

                if (_encryptionPolicy.Value == EncryptionPolicy.EnforceEncrypted)
                    throw new ConnectionSecurityException($"The scheme of the ServiceUrl ({_serviceUrl}) is '{Constants.PulsarScheme}' and cannot be used with an encryption policy of 'EnforceEncrypted'");
            }
            else if (scheme == Constants.PulsarSslScheme)
            {
                if (!_encryptionPolicy.HasValue)
                    _encryptionPolicy = EncryptionPolicy.EnforceEncrypted;

                if (_encryptionPolicy.Value == EncryptionPolicy.EnforceUnencrypted)
                    throw new ConnectionSecurityException($"The scheme of the ServiceUrl ({_serviceUrl}) is '{Constants.PulsarSslScheme}' and cannot be used with an encryption policy of 'EnforceUnencrypted'");
            }
            else
                throw new InvalidSchemeException($"Invalid scheme '{scheme}'. Expected '{Constants.PulsarScheme}' or '{Constants.PulsarSslScheme}'");

            var connector = new Connector(_trustedCertificateAuthority, _verifyCertificateAuthority, _verifyCertificateName);
            var connectionPool = new ConnectionPool(_commandConnect, _serviceUrl, connector, _encryptionPolicy.Value);
            return new PulsarClient(connectionPool, new FaultStrategy(_retryInterval));
        }
    }
}
