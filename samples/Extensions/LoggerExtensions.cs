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

namespace Extensions;

using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using Microsoft.Extensions.Logging;

/// <summary>
/// Extensions for ILogger
/// </summary>
public static partial class LoggerExtensions
{
    /// <summary>
    /// Default logger for the PulsarClient exception handler
    /// </summary>
    public static void PulsarClientException(this ILogger logger, ExceptionContext exceptionContext)
    {
        if (exceptionContext.Exception is not ChannelNotReadyException && exceptionContext.Exception is not TaskCanceledException)
            logger.PulsarClientException(exceptionContext.Exception);
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "The PulsarClient got an exception")]
    static partial void PulsarClientException(this ILogger logger, Exception exception);

    /// <summary>
    /// Default logger for an IMessage with a string value
    /// </summary>
    public static void OutputMessage(this ILogger logger, IMessage<string> message)
        => logger.OutputMessage(message.Value(), message.PublishTimeAsDateTime, message.MessageId);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Received: '{payload}' published on '{publishedOn}' with message id '{messageId}'")]
    static partial void OutputMessage(this ILogger logger, string payload, DateTime publishedOn, MessageId messageId);

    /// <summary>
    /// Default logger for consumer state monitoring
    /// </summary>
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

    [LoggerMessage(EventId = 2, Message = "The consumer for topic '{topic}' changed state to '{state}'")]
    static partial void ConsumerChangedState(this ILogger logger, LogLevel logLevel, string topic, string state);

    /// <summary>
    /// Default logger for producer state monitoring
    /// </summary>
    public static void ProducerChangedState(this ILogger logger, ProducerStateChanged stateChanged)
    {
        var logLevel = stateChanged.ProducerState switch
        {
            ProducerState.Disconnected => LogLevel.Warning,
            ProducerState.Faulted => LogLevel.Error,
            _ => LogLevel.Information
        };

        logger.ProducerChangedState(logLevel, stateChanged.Producer.Topic, stateChanged.ProducerState.ToString());
    }

    [LoggerMessage(EventId = 3, Message = "The producer for topic '{topic}' changed state to '{state}'")]
    static partial void ProducerChangedState(this ILogger logger, LogLevel logLevel, string topic, string state);

    /// <summary>
    /// Default logger for reader state monitoring
    /// </summary>
    public static void ReaderChangedState(this ILogger logger, ReaderStateChanged stateChanged)
    {
        var logLevel = stateChanged.ReaderState switch
        {
            ReaderState.Disconnected => LogLevel.Warning,
            ReaderState.Faulted => LogLevel.Error,
            _ => LogLevel.Information
        };

        logger.ReaderChangedState(logLevel, stateChanged.Reader.Topic, stateChanged.ReaderState.ToString());
    }

    [LoggerMessage(EventId = 4, Message = "The reader for topic '{topic}' changed state to '{state}'")]
    static partial void ReaderChangedState(this ILogger logger, LogLevel logLevel, string topic, string state);

    // Consumer lost connection
    public static ValueTask<string> ConsumerLostConnection(this ILogger logger, IConsumer consumer, ConsumerState state, CancellationToken cancellationToken)
    {
        logger.ConsumerLostConnection(consumer.Topic);
        return ValueTask.FromResult("TicketId"); // Simulating a ticket-id after sending an alert. This is optional.
    }

    [LoggerMessage(EventId = 5, Level = LogLevel.Warning, Message = "The consumer for topic '{topic}' has lost the connection")]
    static partial void ConsumerLostConnection(this ILogger logger, string topic);

    // Consumer regained connection
    public static ValueTask ConsumerRegainedConnection(this ILogger logger, IConsumer consumer, ConsumerState state, string ticketId, CancellationToken cancellationToken)
    {
        logger.ConsumerRegainedConnection(consumer.Topic);
        return ValueTask.CompletedTask; // If an alert has been opened, this is where we can close it again
    }

    [LoggerMessage(EventId = 6, Level = LogLevel.Warning, Message = "The consumer for topic '{topic}' has regained the connection")]
    static partial void ConsumerRegainedConnection(this ILogger logger, string topic);
}
