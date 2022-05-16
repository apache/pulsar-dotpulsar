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

namespace DotPulsar.Abstractions;

using System;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// A pulsar client building abstraction.
/// </summary>
public interface IPulsarClientBuilder
{
    /// <summary>
    /// Authenticate using a client certificate. This is optional.
    /// </summary>
    IPulsarClientBuilder AuthenticateUsingClientCertificate(X509Certificate2 clientCertificate);

    /// <summary>
    /// Authenticate using a (JSON Web) token. This is optional.
    /// </summary>
    [Obsolete("This method is obsolete. Call Authentication(AuthenticationFactory.Token(...)) instead.", false)]
    IPulsarClientBuilder AuthenticateUsingToken(string token);

    /// <summary>
    /// Set the authentication provider. This is optional.
    /// </summary>
    IPulsarClientBuilder Authentication(IAuthentication authentication);

    /// <summary>
    /// Set connection encryption policy. The default is 'EnforceUnencrypted' if the ServiceUrl scheme is 'pulsar' and 'EnforceEncrypted' if it's 'pulsar+ssl'.
    /// </summary>
    IPulsarClientBuilder ConnectionSecurity(EncryptionPolicy encryptionPolicy);

    /// <summary>
    /// Register a custom exception handler that will be invoked before the default exception handler.
    /// </summary>
    IPulsarClientBuilder ExceptionHandler(IHandleException exceptionHandler);

    /// <summary>
    /// The time to wait before sending a 'ping' if there has been no activity on the connection. The default is 30 seconds.
    /// </summary>
    IPulsarClientBuilder KeepAliveInterval(TimeSpan interval);

    /// <summary>
    /// The maximum amount of time to wait without receiving any message from the server at
    /// which point the connection is assumed to be dead or the server is not responding.
    /// As we are sending pings the server should respond to those at a minimum within this specified timeout period.
    /// Once this happens the connection will be torn down and all consumers/producers will enter
    /// the disconnected state and attempt to reconnect
    /// The default is 60 seconds.
    /// </summary>
    IPulsarClientBuilder ServerResponseTimeout(TimeSpan interval);

    /// <summary>
    /// Set the listener name. This is optional.
    /// </summary>
    IPulsarClientBuilder ListenerName(string listenerName);

    /// <summary>
    /// The time to wait before retrying an operation or a reconnect. The default is 3 seconds.
    /// </summary>
    IPulsarClientBuilder RetryInterval(TimeSpan interval);

    /// <summary>
    /// The service URL for the Pulsar cluster. The default is "pulsar://localhost:6650".
    /// </summary>
    IPulsarClientBuilder ServiceUrl(Uri uri);

    /// <summary>
    /// Add a trusted certificate authority. This is optional.
    /// </summary>
    IPulsarClientBuilder TrustedCertificateAuthority(X509Certificate2 trustedCertificateAuthority);

    /// <summary>
    /// Verify the certificate authority. The default is 'true'.
    /// </summary>
    IPulsarClientBuilder VerifyCertificateAuthority(bool verifyCertificateAuthority);

    /// <summary>
    /// Verify the certificate name with the hostname. The default is 'false'.
    /// </summary>
    IPulsarClientBuilder VerifyCertificateName(bool verifyCertificateName);

    /// <summary>
    /// The time to wait before checking for inactive connections that can be closed. The default is 60 seconds.
    /// </summary>
    IPulsarClientBuilder CloseInactiveConnectionsInterval(TimeSpan interval);

    /// <summary>
    /// Create the client.
    /// </summary>
    IPulsarClient Build();
}
