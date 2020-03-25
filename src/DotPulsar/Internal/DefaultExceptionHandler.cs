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

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal.Exceptions;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class DefaultExceptionHandler : IHandleException
    {
        private readonly TimeSpan _retryInterval;

        public DefaultExceptionHandler(TimeSpan retryInterval) => _retryInterval = retryInterval;

        public async ValueTask OnException(ExceptionContext exceptionContext)
        {
            exceptionContext.Result = DetermineFaultAction(exceptionContext.Exception, exceptionContext.CancellationToken);
            if (exceptionContext.Result == FaultAction.Retry)
                await Task.Delay(_retryInterval, exceptionContext.CancellationToken).ConfigureAwait(false);

            exceptionContext.ExceptionHandled = true;
        }

        private FaultAction DetermineFaultAction(Exception exception, CancellationToken cancellationToken)
        {
            switch (exception)
            {
                case TooManyRequestsException _: return FaultAction.Retry;
                case ChannelNotReadyException _: return FaultAction.Retry;
                case ServiceNotReadyException _: return FaultAction.Retry;
                case ConnectionDisposedException _: return FaultAction.Retry;
                case AsyncLockDisposedException _: return FaultAction.Retry;
                case PulsarStreamDisposedException _: return FaultAction.Retry;
                case AsyncQueueDisposedException _: return FaultAction.Retry;
                case OperationCanceledException _: return cancellationToken.IsCancellationRequested ? FaultAction.Rethrow : FaultAction.Retry;
                case DotPulsarException _: return FaultAction.Rethrow;
                case SocketException socketException:
                    switch (socketException.SocketErrorCode)
                    {
                        case SocketError.HostNotFound:
                        case SocketError.HostUnreachable:
                        case SocketError.NetworkUnreachable:
                            return FaultAction.Rethrow;
                    }
                    return FaultAction.Retry;
            }

            return FaultAction.Rethrow;
        }
    }
}
