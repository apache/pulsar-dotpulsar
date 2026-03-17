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

public sealed class BasicAuthentication : IAuthentication
{
    private readonly Func<CancellationToken, ValueTask<string>> _tokenSupplier;

    public BasicAuthentication(string userId, string password)
    {
        _tokenSupplier = (cancellationToken) => new ValueTask<string>($"{userId}:{password}");
    }

    public BasicAuthentication(Func<CancellationToken, ValueTask<string>> tokenSupplier) => _tokenSupplier = tokenSupplier;

    public string AuthenticationMethodName => "basic";

    public async ValueTask<byte[]> GetAuthenticationData(CancellationToken cancellationToken)
    {
        var token = await _tokenSupplier(cancellationToken).ConfigureAwait(false);
        return System.Text.Encoding.UTF8.GetBytes(token);
    }
}
