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
using DotPulsar.Exceptions;
using DotPulsar.Internal.Exceptions;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public sealed class DefaultExceptionHandler : IHandleException
{
    public ValueTask OnException(ExceptionContext exceptionContext)
    {
        exceptionContext.Result = DetermineFaultAction(exceptionContext.Exception, exceptionContext.CancellationToken);
        exceptionContext.ExceptionHandled = true;
#if NET6_0_OR_GREATER
        return ValueTask.CompletedTask;
#else
        return new ValueTask();
#endif
    }

    private static FaultAction DetermineFaultAction(Exception exception, CancellationToken cancellationToken)
        => exception switch
        {
            TooManyRequestsException _ => FaultAction.Retry,
            ChannelNotReadyException _ => FaultAction.Retry,
            ServiceNotReadyException _ => FaultAction.Retry,
            MetadataException _ => FaultAction.Rethrow,
            ConsumerNotFoundException _ => FaultAction.Retry,
            ConsumerNotActiveException _ => FaultAction.Retry,
            ConsumerBusyException _ => FaultAction.Retry,
            ProducerBusyException _ => FaultAction.Retry,
            ProducerFencedException _ => FaultAction.Rethrow,
            ConnectionDisposedException _ => FaultAction.Retry,
            AsyncLockDisposedException _ => FaultAction.Retry,
            PulsarStreamDisposedException _ => FaultAction.Retry,
            AsyncQueueDisposedException _ => FaultAction.Retry,
            AsyncQueueWithCursorDisposedException => FaultAction.Retry,
            AsyncQueueWithCursorNoItemException => FaultAction.Rethrow,
            ProducerSendReceiptOrderingException _ => FaultAction.Retry,
            OperationCanceledException _ => cancellationToken.IsCancellationRequested ? FaultAction.Rethrow : FaultAction.Retry,
            DotPulsarException _ => FaultAction.Rethrow,
            IOException _ => FaultAction.Retry,
            SocketException socketException => socketException.SocketErrorCode switch
            {
                SocketError.HostNotFound => FaultAction.Rethrow,
                SocketError.HostUnreachable => FaultAction.Rethrow,
                SocketError.NetworkUnreachable => FaultAction.Rethrow,
                _ => FaultAction.Retry
            },
            _ => FaultAction.Rethrow
        };
}
