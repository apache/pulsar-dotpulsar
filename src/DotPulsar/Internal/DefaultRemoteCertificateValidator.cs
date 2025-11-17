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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public sealed class DefaultRemoteCertificateValidator : IValidateRemoteCertificate
{
    private readonly X509Certificate2? _trustedCertificateAuthority;
    private readonly bool _verifyCertificateAuthority;
    private readonly bool _verifyCertificateName;

    public DefaultRemoteCertificateValidator(X509Certificate2? trustedCertificateAuthority, bool verifyCertificateAuthority, bool verifyCertificateName)
    {
        _trustedCertificateAuthority = trustedCertificateAuthority;
        _verifyCertificateAuthority = verifyCertificateAuthority;
        _verifyCertificateName = verifyCertificateName;
    }

    public bool Validate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
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
