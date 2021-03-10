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
    using Abstractions;
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    public sealed class AsyncQueue<T> : IEnqueue<T>, IDequeue<T>, IDisposable
    {
        private readonly Channel<T> _channel;
        private readonly ChannelReader<T> _reader;
        private readonly ChannelWriter<T> _writer;

        public AsyncQueue()
        {
            _channel = System.Threading.Channels.Channel.CreateUnbounded<T>();
            _reader = _channel.Reader;
            _writer = _channel.Writer;
        }

        public void Enqueue(T item)
        {
            if (!_writer.TryWrite(item))
            {
                throw new InvalidOperationException("Channel did not accept new message");
            }
        }

        public ValueTask<T> Dequeue(CancellationToken cancellationToken = default)
        {
            return _reader.ReadAsync(cancellationToken);
        }

        public void Dispose()
        {
            _writer.TryComplete();
        }
    }
}
