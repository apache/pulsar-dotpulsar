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
        private readonly X509Certificate2? _trustedCertificateAuthority;
        private readonly bool _verifyCertificateAuthority;
        private readonly bool _verifyCertificateName;

        public Connector(X509Certificate2? trustedCertificateAuthority, bool verifyCertificateAuthority, bool verifyCertificateName)
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
            var encrypt = scheme == Constants.PulsarSslScheme;

            if (port == -1)
                port = encrypt ? Constants.DefaultPulsarSSLPort : Constants.DefaultPulsarPort;

            var stream = await GetStream(host, port);

            if (encrypt)
                stream = await EncryptStream(stream, host);

            return stream;
        }

        private async Task<Stream> GetStream(string host, int port)
        {
            var tcpClient = new TcpClient();

            try
            {
                var type = Uri.CheckHostName(host);

                if (type == UriHostNameType.IPv4 || type == UriHostNameType.IPv6)
                    await tcpClient.ConnectAsync(IPAddress.Parse(host), port);
                else
                    await tcpClient.ConnectAsync(host, port);

                return tcpClient.GetStream();
            }
            catch
            {
                tcpClient.Dispose();
                throw;
            }
        }

        private async Task<Stream> EncryptStream(Stream stream, string host)
        {
            SslStream? sslStream = null;

            try
            {
                sslStream = new SslStream(stream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                await sslStream.AuthenticateAsClientAsync(host);
                return sslStream;
            }
            catch (System.Security.Authentication.AuthenticationException exception)
            {
                if (sslStream == null)
                    stream.Dispose();
                else
                    sslStream.Dispose();

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
