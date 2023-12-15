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

using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

public sealed class Connector
{
    private readonly X509Certificate2Collection _clientCertificates;
    private readonly X509Certificate2? _trustedCertificateAuthority;
    private readonly bool _verifyCertificateAuthority;
    private readonly bool _verifyCertificateName;
    private readonly bool _checkCertificateRevocation;

    public Connector(
        X509Certificate2Collection clientCertificates,
        X509Certificate2? trustedCertificateAuthority,
        bool verifyCertificateAuthority,
        bool verifyCertificateName,
        bool checkCertificateRevocation)
    {
        _clientCertificates = clientCertificates;
        _trustedCertificateAuthority = trustedCertificateAuthority;
        _verifyCertificateAuthority = verifyCertificateAuthority;
        _verifyCertificateName = verifyCertificateName;
        _checkCertificateRevocation = checkCertificateRevocation;
    }

    public async Task<Stream> Connect(Uri serviceUrl, CancellationToken cancellationToken)
    {
        var scheme = serviceUrl.Scheme;
        var host = serviceUrl.Host;
        var port = serviceUrl.Port;
        var encrypt = scheme == Constants.PulsarSslScheme;

        if (port == -1)
            port = encrypt ? Constants.DefaultPulsarSSLPort : Constants.DefaultPulsarPort;

        var stream = await GetStream(host, port, cancellationToken).ConfigureAwait(false);

        if (encrypt)
            stream = await EncryptStream(stream, host, cancellationToken).ConfigureAwait(false);

        return stream;
    }

    private static async Task<Stream> GetStream(string host, int port, CancellationToken cancellationToken)
    {
        var tcpClient = new TcpClient();

        try
        {
            var type = Uri.CheckHostName(host);

#if NETSTANDARD2_0 || NETSTANDARD2_1
            if (type == UriHostNameType.IPv4 || type == UriHostNameType.IPv6)
                await tcpClient.ConnectAsync(IPAddress.Parse(host), port).ConfigureAwait(false);
            else
                await tcpClient.ConnectAsync(host, port).ConfigureAwait(false);
#else
            if (type == UriHostNameType.IPv4 || type == UriHostNameType.IPv6)
                await tcpClient.ConnectAsync(IPAddress.Parse(host), port, cancellationToken).ConfigureAwait(false);
            else
                await tcpClient.ConnectAsync(host, port, cancellationToken).ConfigureAwait(false);
#endif

            return tcpClient.GetStream();
        }
        catch
        {
            tcpClient.Dispose();
            throw;
        }
    }

#if NETSTANDARD2_0
    private async Task<Stream> EncryptStream(Stream stream, string host, CancellationToken _)
    {
        SslStream? sslStream = null;

        try
        {
            sslStream = new SslStream(stream, false, ValidateServerCertificate, null);
            await sslStream.AuthenticateAsClientAsync(host, _clientCertificates, SslProtocols.None, _checkCertificateRevocation).ConfigureAwait(false);
            return sslStream;
        }
        catch
        {
            if (sslStream is null)
                stream.Dispose();
            else
                sslStream.Dispose();

            throw;
        }
    }
#else
    private async Task<Stream> EncryptStream(Stream stream, string host, CancellationToken cancellationToken)
    {
        SslStream? sslStream = null;

        try
        {
            sslStream = new SslStream(stream, false, ValidateServerCertificate, null);
            var options = new SslClientAuthenticationOptions
            {
                TargetHost = host,
                ClientCertificates = _clientCertificates,
                EnabledSslProtocols = SslProtocols.None,
                CertificateRevocationCheckMode = _checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck
            };
            await sslStream.AuthenticateAsClientAsync(options, cancellationToken).ConfigureAwait(false);
            return sslStream;
        }
        catch
        {
            if (sslStream is null)
                await stream.DisposeAsync().ConfigureAwait(false);
            else
                await sslStream.DisposeAsync().ConfigureAwait(false);

            throw;
        }
    }
#endif

    private bool ValidateServerCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
            return false;

        if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch) && _verifyCertificateName)
            return false;

        if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors) && _verifyCertificateAuthority)
        {
            if (_trustedCertificateAuthority is null || chain is null || certificate is null)
                return false;

            chain.ChainPolicy.ExtraStore.Add(_trustedCertificateAuthority);
            _ = chain.Build((X509Certificate2) certificate);

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
