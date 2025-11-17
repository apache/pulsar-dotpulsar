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

namespace DotPulsar.Extensions;

using DotPulsar.Abstractions;
using DotPulsar.Internal;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Extensions for IPulsarClientBuilder.
/// </summary>
public static class PulsarClientBuilderExtensions
{
    /// <summary>
    /// Register a custom exception handler that will be invoked before the default exception handler.
    /// </summary>
    public static IPulsarClientBuilder ExceptionHandler(this IPulsarClientBuilder builder, Action<ExceptionContext> exceptionHandler)
        => builder.ExceptionHandler(new ActionExceptionHandler(exceptionHandler));

    /// <summary>
    /// Register a custom exception handler that will be invoked before the default exception handler.
    /// </summary>
    public static IPulsarClientBuilder ExceptionHandler(this IPulsarClientBuilder builder, Func<ExceptionContext, ValueTask> exceptionHandler)
        => builder.ExceptionHandler(new FuncExceptionHandler(exceptionHandler));

    /// <summary>
    /// Register a custom remote certificate validator.
    /// </summary>
    public static IPulsarClientBuilder RemoteCertificateValidation(this IPulsarClientBuilder builder, Func<object, X509Certificate?, X509Chain?, SslPolicyErrors, bool> validator)
        => builder.RemoteCertificateValidation(new FuncRemoteCertificateValidator(validator));
}
