using System;
using System.Security.Cryptography.X509Certificates;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A pulsar client building abstraction.
    /// </summary>
    public interface IPulsarClientBuilder
    {
        /// <summary>
        /// Set connection encryption policy. The default is 'EnforceUnencrypted' if the ServiceUrl scheme is 'pulsar' and 'EnforceEncrypted' if it's 'pulsar+ssl'.
        /// </summary>
        IPulsarClientBuilder ConnectionSecurity(EncryptionPolicy encryptionPolicy);

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
        /// Create the client.
        /// </summary>
        IPulsarClient Build();
    }
}
