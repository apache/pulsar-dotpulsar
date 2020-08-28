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

namespace DotPulsar.Internal
{
    using DotPulsar.Abstractions;
    using DotPulsar.Exceptions;

    public sealed class ConsumerBuilder : IConsumerBuilder
    {
        private readonly IPulsarClient _pulsarClient;
        private string? _consumerName;
        private SubscriptionInitialPosition _initialPosition;
        private int _priorityLevel;
        private uint _messagePrefetchCount;
        private bool _readCompacted;
        private string? _subscriptionName;
        private SubscriptionType _subscriptionType;
        private string? _topic;
        private bool _autoUpdatePartitions;
        private int _autoUpdatePartitionsInterval;

        public ConsumerBuilder(IPulsarClient pulsarClient)
        {
            _pulsarClient = pulsarClient;
            _initialPosition = ConsumerOptions.DefaultInitialPosition;
            _priorityLevel = ConsumerOptions.DefaultPriorityLevel;
            _messagePrefetchCount = ConsumerOptions.DefaultMessagePrefetchCount;
            _readCompacted = ConsumerOptions.DefaultReadCompacted;
            _subscriptionType = ConsumerOptions.DefaultSubscriptionType;
            _autoUpdatePartitions = ConsumerOptions.DefaultAutoUpdatePartitions;
            _autoUpdatePartitionsInterval = ConsumerOptions.DefaultAutoUpdatePartitionsInterval;
        }

        public IConsumerBuilder ConsumerName(string name)
        {
            _consumerName = name;
            return this;
        }

        public IConsumerBuilder InitialPosition(SubscriptionInitialPosition initialPosition)
        {
            _initialPosition = initialPosition;
            return this;
        }

        public IConsumerBuilder PriorityLevel(int priorityLevel)
        {
            _priorityLevel = priorityLevel;
            return this;
        }

        public IConsumerBuilder MessagePrefetchCount(uint count)
        {
            _messagePrefetchCount = count;
            return this;
        }

        public IConsumerBuilder ReadCompacted(bool readCompacted)
        {
            _readCompacted = readCompacted;
            return this;
        }

        public IConsumerBuilder SubscriptionName(string name)
        {
            _subscriptionName = name;
            return this;
        }

        public IConsumerBuilder SubscriptionType(SubscriptionType type)
        {
            _subscriptionType = type;
            return this;
        }

        public IConsumerBuilder Topic(string topic)
        {
            _topic = topic;
            return this;
        }

        public IConsumerBuilder AutoUpdatePartitions(bool autoUpdatePartitions)
        {
            _autoUpdatePartitions = autoUpdatePartitions;
            return this;
        }

        public IConsumerBuilder AutoUpdatePartitionsInterval(int autoUpdatePartitionsInterval)
        {
            _autoUpdatePartitionsInterval = autoUpdatePartitionsInterval;
            return this;
        }

        public IConsumer Create()
        {
            if (string.IsNullOrEmpty(_subscriptionName))
                throw new ConfigurationException("SubscriptionName may not be null or empty");

            if (string.IsNullOrEmpty(_topic))
                throw new ConfigurationException("Topic may not be null or empty");

            var options = new ConsumerOptions(_subscriptionName!, _topic!)
            {
                ConsumerName = _consumerName,
                InitialPosition = _initialPosition,
                MessagePrefetchCount = _messagePrefetchCount,
                PriorityLevel = _priorityLevel,
                ReadCompacted = _readCompacted,
                SubscriptionType = _subscriptionType,
                AutoUpdatePartitions = _autoUpdatePartitions,
                AutoUpdatePartitionsInterval = _autoUpdatePartitionsInterval
            };

            return _pulsarClient.CreateConsumer(options);
        }
    }
}
