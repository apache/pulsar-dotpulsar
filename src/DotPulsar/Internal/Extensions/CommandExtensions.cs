using DotPulsar.Exceptions;
using DotPulsar.Internal.Exceptions;
using DotPulsar.Internal.PulsarApi;

namespace DotPulsar.Internal.Extensions
{
    public static class CommandExtensions
    {
        public static void Expect(this BaseCommand command, params BaseCommand.Type[] types)
        {
            var actual = command.CommandType;

            foreach (var type in types)
            {
                if (type == actual)
                    return;
            }

            switch (actual)
            {
                case BaseCommand.Type.Error:
                    command.Error.Throw();
                    return;
                case BaseCommand.Type.SendError:
                    command.SendError.Throw();
                    return;
            }

            throw new UnexpectedResponseException($"Expected '{string.Join(",", types)}' but got '{actual}'");
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
