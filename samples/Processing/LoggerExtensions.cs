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

namespace Processing;

using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Exceptions;

#pragma warning disable IDE0079 // Remove unnecessary suppression... Ehm... *sigh*
#pragma warning disable IDE0060 // Remove unused parameter... Why Microsoft? Why do you force me to do this?

public static partial class LoggerExtensions
{
    // ConsumerChangedState
    public static void ConsumerChangedState(this ILogger logger, ConsumerStateChanged stateChanged)
    {
        var logLevel = stateChanged.ConsumerState switch
        {
            ConsumerState.Disconnected => LogLevel.Warning,
            ConsumerState.Faulted => LogLevel.Error,
            _ => LogLevel.Information
        };

        logger.ConsumerChangedState(logLevel, stateChanged.Consumer.Topic, stateChanged.ConsumerState.ToString());
    }

    [LoggerMessage(EventId = 0, Message = "The consumer for topic '{topic}' changed state to '{state}'")]
    static partial void ConsumerChangedState(this ILogger logger, LogLevel logLevel, string topic, string state);

    // OutputMessage
    public static void OutputMessage(this ILogger logger, IMessage<string> message)
    {
        var publishedOn = message.PublishTimeAsDateTime;
        var payload = message.Value();
        logger.OutputMessage(publishedOn, payload);
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Received: '{payload}' published on {publishedOn}")]
    static partial void OutputMessage(this ILogger logger, DateTime publishedOn, string payload);

    // PulsarClientException
    public static void PulsarClientException(this ILogger logger, ExceptionContext exceptionContext)
    {
        if (exceptionContext.Exception is not ChannelNotReadyException)
            logger.PulsarClientException(exceptionContext.Exception);
    }

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "The PulsarClient got an exception")]
    static partial void PulsarClientException(this ILogger logger, Exception exception);
}

#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0079 // Remove unnecessary suppression
