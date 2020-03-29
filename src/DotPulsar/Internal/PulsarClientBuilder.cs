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

namespace DotPulsar.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;
    using DotPulsar.Abstractions;
    using DotPulsar.Exceptions;
    using PulsarApi;

    public sealed class PulsarClientBuilder : IPulsarClientBuilder
    {
        private readonly CommandConnect _commandConnect;
        private readonly List<IHandleException> _exceptionHandlers;
        private EncryptionPolicy? _encryptionPolicy;
        private TimeSpan _retryInterval;
        private Uri _serviceUrl;
        private X509Certificate2? _trustedCertificateAuthority;
        private readonly X509Certificate2Collection _clientCertificates;
        private bool _verifyCertificateAuthority;
        private bool _verifyCertificateName;
        private TimeSpan _closeInactiveConnectionsInterval;

        public PulsarClientBuilder()
        {
            _commandConnect = new CommandConnect
            {
                ProtocolVersion = Constants.ProtocolVersion,
                ClientVersion = Constants.ClientVersion
            };

            _exceptionHandlers = new List<IHandleException>();
            _retryInterval = TimeSpan.FromSeconds(3);
            _serviceUrl = new Uri($"{Constants.PulsarScheme}://localhost:{Constants.DefaultPulsarPort}");
            _clientCertificates = new X509Certificate2Collection();
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

        public IPulsarClientBuilder ExceptionHandler(IHandleException exceptionHandler)
        {
            _exceptionHandlers.Add(exceptionHandler);
            return this;
        }

        public IPulsarClientBuilder ExceptionHandler(Func<ExceptionContext, ValueTask> exceptionHandler)
        {
            _exceptionHandlers.Add(new FuncExceptionHandler(exceptionHandler));
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
                    throw new ConnectionSecurityException(
                        $"The scheme of the ServiceUrl ({_serviceUrl}) is '{Constants.PulsarScheme}' and cannot be used with an encryption policy of 'EnforceEncrypted'");
            }
            else if (scheme == Constants.PulsarSslScheme)
            {
                _encryptionPolicy ??= EncryptionPolicy.EnforceEncrypted;

                if (_encryptionPolicy.Value == EncryptionPolicy.EnforceUnencrypted)
                    throw new ConnectionSecurityException(
                        $"The scheme of the ServiceUrl ({_serviceUrl}) is '{Constants.PulsarSslScheme}' and cannot be used with an encryption policy of 'EnforceUnencrypted'");
            }
            else
            {
                throw new InvalidSchemeException($"Invalid scheme '{scheme}'. Expected '{Constants.PulsarScheme}' or '{Constants.PulsarSslScheme}'");
            }
            
            var connector = new Connector(_clientCertificates, _trustedCertificateAuthority, _verifyCertificateAuthority, _verifyCertificateName);

            var connectionPool = new ConnectionPool(_commandConnect, _serviceUrl, connector, _encryptionPolicy.Value, _closeInactiveConnectionsInterval);

            var processManager = new ProcessManager(connectionPool);

            var exceptionHandlerPipeline = new ExceptionHandlerPipeline(new List<IHandleException>(_exceptionHandlers)
            {
                new DefaultExceptionHandler(_retryInterval)
            });

            return new PulsarClient(connectionPool, processManager, exceptionHandlerPipeline);
        }
    }
}
