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

using DotPulsar.Exceptions;
using DotPulsar.Internal.Exceptions;
using DotPulsar.Internal.PulsarApi;

namespace DotPulsar.Internal.Extensions
{
    public static class CommandExtensions
    {
        public static void Expect(this BaseCommand command, BaseCommand.Type type)
        {
            if (command.CommandType == type)
                return;

            switch (command.CommandType)
            {
                case BaseCommand.Type.Error:
                    command.Error.Throw();
                    return;
                case BaseCommand.Type.SendError:
                    command.SendError.Throw();
                    return;
            }

            throw new UnexpectedResponseException($"Expected '{type}' but got '{command.CommandType}'");
        }

        public static void Throw(this CommandSendError command) => Throw(command.Error, command.Message);

        public static void Throw(this CommandLookupTopicResponse command) => Throw(command.Error, command.Message);

        public static void Throw(this CommandError error) => Throw(error.Error, error.Message);

        private static void Throw(ServerError error, string message)
        {
            switch (error)
            {
                case ServerError.AuthenticationError: throw new AuthenticationException(message);
                case ServerError.AuthorizationError: throw new AuthorizationException(message);
                case ServerError.ChecksumError: throw new ChecksumException(message);
                case ServerError.ConsumerAssignError: throw new ConsumerAssignException(message);
                case ServerError.ConsumerBusy: throw new ConsumerBusyException(message);
                case ServerError.ConsumerNotFound: throw new ConsumerNotFoundException(message);
                case ServerError.IncompatibleSchema: throw new IncompatibleSchemaException(message);
                case ServerError.InvalidTopicName: throw new InvalidTopicNameException(message);
                case ServerError.MetadataError: throw new MetadataException(message);
                case ServerError.PersistenceError: throw new PersistenceException(message);
                case ServerError.ProducerBlockedQuotaExceededError:
                case ServerError.ProducerBlockedQuotaExceededException:
                    throw new ProducerBlockedQuotaExceededException(message + ". Error code: " + error);
                case ServerError.ProducerBusy: throw new ProducerBusyException(message);
                case ServerError.ServiceNotReady: throw new ServiceNotReadyException(message);
                case ServerError.SubscriptionNotFound: throw new SubscriptionNotFoundException(message);
                case ServerError.TooManyRequests: throw new TooManyRequestsException(message);
                case ServerError.TopicNotFound: throw new TopicNotFoundException(message);
                case ServerError.TopicTerminatedError: throw new TopicTerminatedException(message);
                case ServerError.UnsupportedVersionError: throw new UnsupportedVersionException(message);
                case ServerError.UnknownError:
                default: throw new UnknownException(message + ". Error code: " + error);
            }
        }

        public static BaseCommand AsBaseCommand(this CommandAck command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.Ack,
                Ack = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandConnect command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.Connect,
                Connect = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandPing command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.Ping,
                Ping = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandPong command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.Pong,
                Pong = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandProducer command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.Producer,
                Producer = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandGetLastMessageId command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.GetLastMessageId,
                GetLastMessageId = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandUnsubscribe command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.Unsubscribe,
                Unsubscribe = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandSubscribe command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.Subscribe,
                Subscribe = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandLookupTopic command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.Lookup,
                LookupTopic = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandSend command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.Send,
                Send = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandFlow command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.Flow,
                Flow = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandCloseProducer command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.CloseProducer,
                CloseProducer = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandCloseConsumer command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.CloseConsumer,
                CloseConsumer = command
            };
        }

        public static BaseCommand AsBaseCommand(this CommandSeek command)
        {
            return new BaseCommand
            {
                CommandType = BaseCommand.Type.Seek,
                Seek = command
            };
        }
    }
}
