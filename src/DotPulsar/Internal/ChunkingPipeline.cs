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
    using System;
    using System.Buffers;
    using System.IO;
    using System.Threading.Tasks;

    public sealed class ChunkingPipeline
    {
        private readonly Stream _stream;
        private readonly int _chunkSize;
        private readonly byte[] _buffer;
        private int _bufferCount;

        public ChunkingPipeline(Stream stream, int chunkSize)
        {
            _stream = stream;
            _chunkSize = chunkSize;
            _buffer = new byte[_chunkSize];
        }

        private void CopyToBuffer(ReadOnlySequence<byte> sequence) => sequence.CopyTo(_buffer.AsSpan());

        private void CopyToBuffer(ReadOnlyMemory<byte> memory) => memory.CopyTo(_buffer.AsMemory(_bufferCount));

        public async ValueTask Send(ReadOnlySequence<byte> sequence)
        {
            var sequenceLength = sequence.Length;

            if (sequenceLength <= _chunkSize)
            {
                CopyToBuffer(sequence);
                _bufferCount = (int) sequenceLength;
                await SendBuffer().ConfigureAwait(false);
                return;
            }

            var enumerator = sequence.GetEnumerator();
            var hasNext = true;

            while (hasNext)
            {
                var current = enumerator.Current;
                var currentLength = current.Length;
                hasNext = enumerator.MoveNext();

                if (currentLength > _chunkSize)
                {
                    await Send(current).ConfigureAwait(false);
                    continue;
                }

                var total = currentLength + _bufferCount;

                if (total > _chunkSize)
                    await SendBuffer().ConfigureAwait(false);

                if (_bufferCount != 0 || (hasNext && enumerator.Current.Length + total <= _chunkSize))
                {
                    CopyToBuffer(current);
                    _bufferCount = total;
                    continue;
                }

                await Send(current).ConfigureAwait(false);
            }

            await SendBuffer().ConfigureAwait(false);
        }

        private async ValueTask SendBuffer()
        {
            if (_bufferCount != 0)
            {
                await _stream.WriteAsync(_buffer, 0, _bufferCount).ConfigureAwait(false);
                _bufferCount = 0;
            }
        }

        private async ValueTask Send(ReadOnlyMemory<byte> memory)
        {
            await SendBuffer().ConfigureAwait(false);

#if NETSTANDARD2_0
            var data = memory.ToArray();
            await _stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
#else
            await _stream.WriteAsync(memory).ConfigureAwait(false);
#endif
        }
    }
}
