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

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal.PulsarApi;
using System.Security.Cryptography.X509Certificates;

public sealed class PulsarClientBuilder : IPulsarClientBuilder
{
    private readonly CommandConnect _commandConnect;
    private readonly List<IHandleException> _exceptionHandlers;
    private EncryptionPolicy? _encryptionPolicy;
    private TimeSpan _keepAliveInterval;
    private string? _listenerName;
    private TimeSpan _retryInterval;
    private Uri _serviceUrl;
    private X509Certificate2? _trustedCertificateAuthority;
    private readonly X509Certificate2Collection _clientCertificates;
    private bool _checkCertificateRevocation;
    private bool _verifyCertificateAuthority;
    private bool _verifyCertificateName;
    private TimeSpan _closeInactiveConnectionsInterval;
    private IAuthentication? _authentication;
    private IValidateRemoteCertificate? _remoteCertificateValidator;

    public PulsarClientBuilder()
    {
        _commandConnect = new CommandConnect
        {
            ProtocolVersion = Constants.ProtocolVersion,
            ClientVersion = $"{Constants.ClientName} {Constants.ClientVersion}",
            FeatureFlags = new FeatureFlags
            {
                SupportsAuthRefresh = true
            }
        };

        _exceptionHandlers = [];
        _keepAliveInterval = TimeSpan.FromSeconds(30);
        _retryInterval = TimeSpan.FromSeconds(3);
        _serviceUrl = new Uri($"{Constants.PulsarScheme}://localhost:{Constants.DefaultPulsarPort}");
        _clientCertificates = [];
        _checkCertificateRevocation = true;
        _verifyCertificateAuthority = true;
        _verifyCertificateName = false;
        _closeInactiveConnectionsInterval = TimeSpan.FromSeconds(60);
    }

    public IPulsarClientBuilder AuthenticateUsingClientCertificate(X509Certificate2 clientCertificate)
    {
        _commandConnect.AuthMethodName = "tls";
        _clientCertificates.Add(clientCertificate);
        return this;
    }

    public IPulsarClientBuilder Authentication(IAuthentication authentication)
    {
        _authentication = authentication;
        return this;
    }

    public IPulsarClientBuilder CheckCertificateRevocation(bool checkCertificateRevocation)
    {
        _checkCertificateRevocation = checkCertificateRevocation;
        return this;
    }

    public IPulsarClientBuilder ConnectionSecurity(EncryptionPolicy encryptionPolicy)
    {
        _encryptionPolicy = encryptionPolicy;
        return this;
    }

    public IPulsarClientBuilder ExceptionHandler(IHandleException exceptionHandler)
    {
        _exceptionHandlers.Add(exceptionHandler);
        return this;
    }

    public IPulsarClientBuilder KeepAliveInterval(TimeSpan interval)
    {
        _keepAliveInterval = interval;
        return this;
    }

    public IPulsarClientBuilder ListenerName(string listenerName)
    {
        _listenerName = listenerName;
        return this;
    }

    public IPulsarClientBuilder RemoteCertificateValidation(IValidateRemoteCertificate remoteCertificateValidator)
    {
        _remoteCertificateValidator = remoteCertificateValidator;
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

    public IPulsarClientBuilder CloseInactiveConnectionsInterval(TimeSpan interval)
    {
        _closeInactiveConnectionsInterval = interval;
        return this;
    }

    public IPulsarClient Build()
    {
        var scheme = _serviceUrl.Scheme;

        if (scheme == Constants.PulsarScheme)
        {
            _encryptionPolicy ??= EncryptionPolicy.EnforceUnencrypted;

            if (_encryptionPolicy.Value == EncryptionPolicy.EnforceEncrypted)
                throw new ConnectionSecurityException($"The scheme of the ServiceUrl ({_serviceUrl}) is '{Constants.PulsarScheme}' and cannot be used with an encryption policy of 'EnforceEncrypted'");
        }
        else if (scheme == Constants.PulsarSslScheme)
        {
            _encryptionPolicy ??= EncryptionPolicy.EnforceEncrypted;

            if (_encryptionPolicy.Value == EncryptionPolicy.EnforceUnencrypted)
                throw new ConnectionSecurityException($"The scheme of the ServiceUrl ({_serviceUrl}) is '{Constants.PulsarSslScheme}' and cannot be used with an encryption policy of 'EnforceUnencrypted'");
        }
        else
            throw new InvalidSchemeException($"Invalid scheme '{scheme}'. Expected '{Constants.PulsarScheme}' or '{Constants.PulsarSslScheme}'");

        if (_remoteCertificateValidator is null)
            _remoteCertificateValidator = new DefaultRemoteCertificateValidator(_trustedCertificateAuthority, _verifyCertificateAuthority, _verifyCertificateName);

        var connector = new Connector(_remoteCertificateValidator, _clientCertificates, _checkCertificateRevocation);

        var exceptionHandlers = new List<IHandleException>(_exceptionHandlers) { new DefaultExceptionHandler() };
        var exceptionHandlerPipeline = new ExceptionHandlerPipeline(_retryInterval, exceptionHandlers);
        var connectionPool = new ConnectionPool(_commandConnect, _serviceUrl, connector, _encryptionPolicy.Value, _closeInactiveConnectionsInterval, _listenerName, _keepAliveInterval, _authentication);

        return new PulsarClient(connectionPool, exceptionHandlerPipeline, _serviceUrl);
    }
}
