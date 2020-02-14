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

using System;
using System.Threading;

namespace DotPulsar
{
    public sealed class ExceptionContext
    {
        internal ExceptionContext(Exception exception, CancellationToken cancellationToken)
        {
            Exception = exception;
            CancellationToken = cancellationToken;
            ExceptionHandled = false;
            Result = FaultAction.Rethrow;
        }

        public Exception Exception { set; get; }
        public CancellationToken CancellationToken { get; }
        public bool ExceptionHandled { get; set; }
        public FaultAction Result { get; set; }
    }
}
