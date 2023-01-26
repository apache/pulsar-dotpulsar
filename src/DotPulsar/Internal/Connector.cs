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

using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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

    public async Task<Stream> Connect(Uri serviceUrl)
    {
        var scheme = serviceUrl.Scheme;
        var host = serviceUrl.Host;
        var port = serviceUrl.Port;
        var encrypt = scheme == Constants.PulsarSslScheme;

        if (port == -1)
            port = encrypt ? Constants.DefaultPulsarSSLPort : Constants.DefaultPulsarPort;

        var stream = await GetStream(host, port).ConfigureAwait(false);

        if (encrypt)
            stream = await EncryptStream(stream, host).ConfigureAwait(false);

        return stream;
    }

    private static async Task<Stream> GetStream(string host, int port)
    {
        var tcpClient = new TcpClient();

        try
        {
            var type = Uri.CheckHostName(host);

            if (type == UriHostNameType.IPv4 || type == UriHostNameType.IPv6)
                await tcpClient.ConnectAsync(IPAddress.Parse(host), port).ConfigureAwait(false);
            else
                await tcpClient.ConnectAsync(host, port).ConfigureAwait(false);

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
            sslStream = new SslStream(stream, false, ValidateServerCertificate, null);
            await sslStream.AuthenticateAsClientAsync(host, _clientCertificates, SslProtocols.None, _checkCertificateRevocation).ConfigureAwait(false);
            return sslStream;
        }
        catch
        {
#if NETSTANDARD2_0
            if (sslStream is null)
                stream.Dispose();
            else
                sslStream.Dispose();
#else
            if (sslStream is null)
                await stream.DisposeAsync().ConfigureAwait(false);
            else
                await sslStream.DisposeAsync().ConfigureAwait(false);
#endif
            throw;
        }
    }

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
