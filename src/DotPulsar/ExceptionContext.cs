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

using System;
using System.Threading;

public sealed class ExceptionContext
{
    internal ExceptionContext(Exception exception, CancellationToken cancellationToken)
    {
        Exception = exception;
        CancellationToken = cancellationToken;
        ExceptionHandled = false;
        Result = FaultAction.Rethrow;
    }

    /// <summary>
    /// The exception caught while executing the operation. This exception will be thrown if the fault action Result is set to 'ThrowException'.
    /// </summary>
    public Exception Exception { set; get; }

    /// <summary>
    /// The cancellation token given to the operation.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets or sets an indication that the exception has been handled. If 'true' no other exception handlers will be invoked.
    /// </summary>
    public bool ExceptionHandled { get; set; }

    /// <summary>
    /// Gets or sets the FaultAction.
    /// </summary>
    public FaultAction Result { get; set; }
}
