﻿/*
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

    public sealed class ProducerBuilder : IProducerBuilder
    {
        private readonly IPulsarClient _pulsarClient;
        private string? _producerName;
        private ulong _initialSequenceId;
        private string? _topic;

        public ProducerBuilder(IPulsarClient pulsarClient)
        {
            _pulsarClient = pulsarClient;
            _initialSequenceId = ProducerOptions.DefaultInitialSequenceId;
        }

        public IProducerBuilder ProducerName(string name)
        {
            _producerName = name;
            return this;
        }

        public IProducerBuilder InitialSequenceId(ulong initialSequenceId)
        {
            _initialSequenceId = initialSequenceId;
            return this;
        }

        public IProducerBuilder Topic(string topic)
        {
            _topic = topic;
            return this;
        }

        public IProducer Create()
        {
            if (string.IsNullOrEmpty(_topic))
                throw new ConfigurationException("ProducerOptions.Topic may not be null or empty");

            var options = new ProducerOptions(_topic!)
            {
                InitialSequenceId = _initialSequenceId,
                ProducerName = _producerName
            };

            return _pulsarClient.CreateProducer(options);
        }
    }
}
