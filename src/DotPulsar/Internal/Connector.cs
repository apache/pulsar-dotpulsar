using DotPulsar.Exceptions;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class Connector
    {
        private const string PulsarScheme = "pulsar";
        private const string PulsarSslScheme = "pulsar+ssl";
        private const int DefaultPulsarPort = 6650;
        private const int DefaultPulsarSSLPort = 6651;

        private readonly X509Certificate2 _trustedCertificateAuthority;
        private readonly bool _verifyCertificateAuthority;
        private readonly bool _verifyCertificateName;

        public Connector(X509Certificate2 trustedCertificateAuthority, bool verifyCertificateAuthority, bool verifyCertificateName)
        {
            _trustedCertificateAuthority = trustedCertificateAuthority;
            _verifyCertificateAuthority = verifyCertificateAuthority;
            _verifyCertificateName = verifyCertificateName;
        }

        public async Task<Stream> Connect(Uri serviceUrl)
        {
            var scheme = serviceUrl.Scheme;
            var host = serviceUrl.Host;
            var port = serviceUrl.Port;
            var encrypt = false;

            switch (scheme)
            {
                case PulsarScheme:
                    if (port == -1)
                        port = DefaultPulsarPort;
                    break;
                case PulsarSslScheme:
                    if (port == -1)
                        port = DefaultPulsarSSLPort;
                    encrypt = true;
                    break;
                default:
                    throw new InvalidSchemeException($"Invalid scheme '{scheme}'. Expected '{PulsarScheme}' or '{PulsarSslScheme}'");
            }

            var tcpClient = new TcpClient();

            switch (Uri.CheckHostName(host))
            {
                case UriHostNameType.IPv4:
                case UriHostNameType.IPv6:
                    await tcpClient.ConnectAsync(IPAddress.Parse(host), port);
                    break;
                default:
                    await tcpClient.ConnectAsync(host, port);
                    break;
            }

            if (!encrypt)
                return tcpClient.GetStream();

            try
            {
                var sslStream = new SslStream(tcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                await sslStream.AuthenticateAsClientAsync(host);
                return sslStream;
            }
            catch (System.Security.Authentication.AuthenticationException exception)
            {
                throw new AuthenticationException("Got an authentication exception while trying to establish an encrypted connection. See inner exception for details.", exception);
            }
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
                return false;

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch) && _verifyCertificateName)
                return false;

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors) && _verifyCertificateAuthority)
            {
                if (_trustedCertificateAuthority is null)
                    return false;

                chain.ChainPolicy.ExtraStore.Add(_trustedCertificateAuthority);
                _ = chain.Build((X509Certificate2)certificate);
                for (var i = 0; i < chain.ChainElements.Count; i++)
                {
                    if (chain.ChainElements[i].Certificate.Thumbprint == _trustedCertificateAuthority.Thumbprint)
                        return true;
                }

                return false;
            }

            return true;
        }
    }
}
