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

public sealed class FuncRemoteCertificateValidator : IValidateRemoteCertificate
{
    private readonly Func<object, X509Certificate?, X509Chain?, SslPolicyErrors, bool> _validator;

    public FuncRemoteCertificateValidator(Func<object, X509Certificate?, X509Chain?, SslPolicyErrors, bool> validator) => _validator = validator;

    public bool Validate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) => _validator(sender, certificate, chain, sslPolicyErrors);
}
