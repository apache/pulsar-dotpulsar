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

namespace DotPulsar.Internal.Extensions
{
    using DotPulsar.Exceptions;
    using Exceptions;
    using PulsarApi;

    public static class CommandExtensions
    {
        public static void Expect(this BaseCommand command, BaseCommand.Type type)
        {
            if (command.CommandType == type)
                return;

            if (command.CommandType == BaseCommand.Type.Error)
                command.Error.Throw();

            if (command.CommandType == BaseCommand.Type.SendError)
                command.SendError.Throw();

            throw new UnexpectedResponseException($"Expected '{type}' but got '{command.CommandType}'");
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
                CommandType = BaseCommand.Type.Ack,
                Ack = command
            };

        public static BaseCommand AsBaseCommand(this CommandConnect command)
            => new()
            {
                CommandType = BaseCommand.Type.Connect,
                Connect = command
            };

        public static BaseCommand AsBaseCommand(this CommandPing command)
            => new()
            {
                CommandType = BaseCommand.Type.Ping,
                Ping = command
            };

        public static BaseCommand AsBaseCommand(this CommandPong command)
            => new()
            {
                CommandType = BaseCommand.Type.Pong,
                Pong = command
            };

        public static BaseCommand AsBaseCommand(this CommandProducer command)
            => new()
            {
                CommandType = BaseCommand.Type.Producer,
                Producer = command
            };

        public static BaseCommand AsBaseCommand(this CommandGetLastMessageId command)
            => new()
            {
                CommandType = BaseCommand.Type.GetLastMessageId,
                GetLastMessageId = command
            };

        public static BaseCommand AsBaseCommand(this CommandUnsubscribe command)
            => new()
            {
                CommandType = BaseCommand.Type.Unsubscribe,
                Unsubscribe = command
            };

        public static BaseCommand AsBaseCommand(this CommandSubscribe command)
            => new()
            {
                CommandType = BaseCommand.Type.Subscribe,
                Subscribe = command
            };

        public static BaseCommand AsBaseCommand(this CommandLookupTopic command)
            => new()
            {
                CommandType = BaseCommand.Type.Lookup,
                LookupTopic = command
            };

        public static BaseCommand AsBaseCommand(this CommandSend command)
            => new()
            {
                CommandType = BaseCommand.Type.Send,
                Send = command
            };

        public static BaseCommand AsBaseCommand(this CommandFlow command)
            => new()
            {
                CommandType = BaseCommand.Type.Flow,
                Flow = command
            };

        public static BaseCommand AsBaseCommand(this CommandCloseProducer command)
            => new()
            {
                CommandType = BaseCommand.Type.CloseProducer,
                CloseProducer = command
            };

        public static BaseCommand AsBaseCommand(this CommandCloseConsumer command)
            => new()
            {
                CommandType = BaseCommand.Type.CloseConsumer,
                CloseConsumer = command
            };

        public static BaseCommand AsBaseCommand(this CommandSeek command)
            => new()
            {
                CommandType = BaseCommand.Type.Seek,
                Seek = command
            };
        
        public static BaseCommand AsBaseCommand(this CommandRedeliverUnacknowledgedMessages command)
            => new()
            {
                CommandType = BaseCommand.Type.RedeliverUnacknowledgedMessages,
                RedeliverUnacknowledgedMessages = command
            };

        public static BaseCommand AsBaseCommand(this CommandGetOrCreateSchema command)
            => new()
            {
                CommandType = BaseCommand.Type.GetOrCreateSchema,
                GetOrCreateSchema = command
            };

        public static BaseCommand AsBaseCommand(this CommandPartitionedTopicMetadata command)
            => new()
            {
                CommandType = BaseCommand.Type.PartitionedMetadata, 
                PartitionMetadata = command
            };

        public static BaseCommand AsBaseCommand(this CommandGetTopicsOfNamespace command)
            => new()
            {
                CommandType = BaseCommand.Type.GetTopicsOfNamespace, 
                GetTopicsOfNamespace = command
            };
    }
}
