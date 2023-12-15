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

namespace DotPulsar;

using DotPulsar.Abstractions;
using DotPulsar.Internal;

/// <summary>
/// Factory for creating IAuthentication instances for all the supported authentication methods.
/// </summary>
public static class AuthenticationFactory
{
    /// <summary>
    /// Create an authentication provider for token based authentication.
    /// </summary>
    public static IAuthentication Token(string token) => new TokenAuthentication(token);

    /// <summary>
    /// Create an authentication provider for token based authentication.
    /// </summary>
    public static IAuthentication Token(Func<CancellationToken, ValueTask<string>> tokenSupplier) => new TokenAuthentication(tokenSupplier);
}
