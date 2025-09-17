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

namespace DotPulsar.Internal.Extensions;

using DotPulsar.Exceptions;
using DotPulsar.Internal.Exceptions;
using Pulsar.Proto;

public static class CommandExtensions
{
    public static void Expect(this BaseCommand command, BaseCommand.Types.Type type)
    {
        if (command.Type == type)
            return;

        if (command.Type == BaseCommand.Types.Type.Error)
            command.Error.Throw();

        if (command.Type == BaseCommand.Types.Type.SendError)
            command.SendError.Throw();

        throw new UnexpectedResponseException($"Expected '{type}' but got '{command.Type}'");
    }

    public static void Throw(this CommandSendError command)
        => Throw(command.Error, command.Message);

    public static void Throw(this CommandLookupTopicResponse command)
        => Throw(command.Error, command.Message);

    public static void Throw(this CommandError command)
        => Throw(command.Error, command.Message);

    public static void Throw(this CommandGetOrCreateSchemaResponse command)
        => Throw(command.ErrorCode, command.ErrorMessage);

    public static void Throw(this CommandPartitionedTopicMetadataResponse command)
        => Throw(command.Error, command.Message);

    private static void Throw(ServerError error, string message)
        => throw (error switch
        {
            ServerError.AuthenticationError => new AuthenticationException(message),
            ServerError.AuthorizationError => new AuthorizationException(message),
            ServerError.ChecksumError => new ChecksumException(message),
            ServerError.ConsumerAssignError => new ConsumerAssignException(message),
            ServerError.ConsumerBusy => new ConsumerBusyException(message),
            ServerError.ConsumerNotFound => new ConsumerNotFoundException(message),
            ServerError.IncompatibleSchema => new IncompatibleSchemaException(message),
            ServerError.InvalidTopicName => new InvalidTopicNameException(message),
            ServerError.InvalidTxnStatus => new InvalidTransactionStatusException(message),
            ServerError.MetadataError => new MetadataException(message),
            ServerError.NotAllowedError => new NotAllowedException(message),
            ServerError.PersistenceError => new PersistenceException(message),
            ServerError.ProducerBlockedQuotaExceededError => new ProducerBlockedQuotaExceededException($"{message}. Error code: {error}"),
            ServerError.ProducerBlockedQuotaExceededException => new ProducerBlockedQuotaExceededException($"{message}. Error code: {error}"),
            ServerError.ProducerBusy => new ProducerBusyException(message),
            ServerError.ProducerFenced => new ProducerFencedException(message),
            ServerError.ServiceNotReady => new ServiceNotReadyException(message),
            ServerError.SubscriptionNotFound => new SubscriptionNotFoundException(message),
            ServerError.TooManyRequests => new TooManyRequestsException(message),
            ServerError.TopicNotFound => new TopicNotFoundException(message),
            ServerError.TopicTerminatedError => new TopicTerminatedException(message),
            ServerError.TransactionConflict => new TransactionConflictException(message),
            ServerError.TransactionCoordinatorNotFound => new TransactionCoordinatorNotFoundException(message),
            ServerError.UnknownError => new UnknownException($"{message}. Error code: {error}"),
            ServerError.UnsupportedVersionError => new UnsupportedVersionException(message),
            _ => new UnknownException($"{message}. Error code: {error}")
        });

    public static BaseCommand AsBaseCommand(this CommandAck command)
        => new()
        {
            Type = BaseCommand.Types.Type.Ack,
            Ack = command
        };

    public static BaseCommand AsBaseCommand(this CommandConnect command)
        => new()
        {
            Type = BaseCommand.Types.Type.Connect,
            Connect = command
        };

    public static BaseCommand AsBaseCommand(this CommandPing command)
        => new()
        {
            Type = BaseCommand.Types.Type.Ping,
            Ping = command
        };

    public static BaseCommand AsBaseCommand(this CommandAuthResponse command)
        => new()
        {
            Type = BaseCommand.Types.Type.AuthResponse,
            AuthResponse = command
        };

    public static BaseCommand AsBaseCommand(this CommandPong command)
        => new()
        {
            Type = BaseCommand.Types.Type.Pong,
            Pong = command
        };

    public static BaseCommand AsBaseCommand(this CommandProducer command)
        => new()
        {
            Type = BaseCommand.Types.Type.Producer,
            Producer = command
        };

    public static BaseCommand AsBaseCommand(this CommandGetLastMessageId command)
        => new()
        {
            Type = BaseCommand.Types.Type.GetLastMessageId,
            GetLastMessageId = command
        };

    public static BaseCommand AsBaseCommand(this CommandUnsubscribe command)
        => new()
        {
            Type = BaseCommand.Types.Type.Unsubscribe,
            Unsubscribe = command
        };

    public static BaseCommand AsBaseCommand(this CommandSubscribe command)
        => new()
        {
            Type = BaseCommand.Types.Type.Subscribe,
            Subscribe = command
        };

    public static BaseCommand AsBaseCommand(this CommandLookupTopic command)
        => new()
        {
            Type = BaseCommand.Types.Type.Lookup,
            LookupTopic = command
        };

    public static BaseCommand AsBaseCommand(this CommandSend command)
        => new()
        {
            Type = BaseCommand.Types.Type.Send,
            Send = command
        };

    public static BaseCommand AsBaseCommand(this CommandFlow command)
        => new()
        {
            Type = BaseCommand.Types.Type.Flow,
            Flow = command
        };

    public static BaseCommand AsBaseCommand(this CommandCloseProducer command)
        => new()
        {
            Type = BaseCommand.Types.Type.CloseProducer,
            CloseProducer = command
        };

    public static BaseCommand AsBaseCommand(this CommandCloseConsumer command)
        => new()
        {
            Type = BaseCommand.Types.Type.CloseConsumer,
            CloseConsumer = command
        };

    public static BaseCommand AsBaseCommand(this CommandSeek command)
        => new()
        {
            Type = BaseCommand.Types.Type.Seek,
            Seek = command
        };

    public static BaseCommand AsBaseCommand(this CommandRedeliverUnacknowledgedMessages command)
        => new()
        {
            Type = BaseCommand.Types.Type.RedeliverUnacknowledgedMessages,
            RedeliverUnacknowledgedMessages = command
        };

    public static BaseCommand AsBaseCommand(this CommandGetOrCreateSchema command)
        => new()
        {
            Type = BaseCommand.Types.Type.GetOrCreateSchema,
            GetOrCreateSchema = command
        };

    public static BaseCommand AsBaseCommand(this CommandPartitionedTopicMetadata command)
        => new()
        {
            Type = BaseCommand.Types.Type.PartitionedMetadata,
            PartitionMetadata = command
        };

    public static BaseCommand AsBaseCommand(this CommandGetTopicsOfNamespace command)
        => new()
        {
            Type = BaseCommand.Types.Type.GetTopicsOfNamespace,
            GetTopicsOfNamespace = command
        };
}
