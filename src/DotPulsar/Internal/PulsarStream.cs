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
    using Exceptions;
    using Extensions;
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Pipelines;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class PulsarStream : IPulsarStream
    {
        private const long _pauseAtMoreThan10Mb = 10485760;
        private const long _resumeAt5MbOrLess = 5242881;

        private readonly Stream _stream;
        private readonly PipeReader _reader;
        private readonly PipeWriter _writer;
        private int _isDisposed;

        public PulsarStream(Stream stream)
        {
            _stream = stream;
            var options = new PipeOptions(pauseWriterThreshold: _pauseAtMoreThan10Mb, resumeWriterThreshold: _resumeAt5MbOrLess);
            var pipe = new Pipe(options);
            _reader = pipe.Reader;
            _writer = pipe.Writer;
        }

        public async Task Send(ReadOnlySequence<byte> sequence)
        {
            ThrowIfDisposed();

#if NETSTANDARD2_0
            foreach (var segment in sequence)
            {
                var data = segment.ToArray();
                await _stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
            }
#else
            foreach (var segment in sequence)
            {
                await _stream.WriteAsync(segment).ConfigureAwait(false);
            }
#endif
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async ValueTask DisposeAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

#if NETSTANDARD2_0
            _stream.Dispose();
#else
            await _stream.DisposeAsync().ConfigureAwait(false);
#endif
        }

        private async Task FillPipe(CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
#if NETSTANDARD2_0
                var buffer = new byte[84999];
#endif
                while (true)
                {
                    var memory = _writer.GetMemory(84999); // LOH - 1 byte
#if NETSTANDARD2_0
                    var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                    new Memory<byte>(buffer, 0, bytesRead).CopyTo(memory);
#else
                    var bytesRead = await _stream.ReadAsync(memory, cancellationToken).ConfigureAwait(false);
#endif
                    if (bytesRead == 0)
                        break;

                    _writer.Advance(bytesRead);

                    var result = await _writer.FlushAsync(cancellationToken).ConfigureAwait(false);

                    if (result.IsCompleted)
                        break;
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                await _writer.CompleteAsync().ConfigureAwait(false);
            }
        }

        public async IAsyncEnumerable<ReadOnlySequence<byte>> Frames([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            _ = FillPipe(cancellationToken);

            try
            {
                while (true)
                {
                    var result = await _reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                    var buffer = result.Buffer;

                    while (true)
                    {
                        if (buffer.Length < 4)
                            break;

                        var frameSize = buffer.ReadUInt32(0, true);
                        var totalSize = frameSize + 4;

                        if (buffer.Length < totalSize)
                            break;

                        yield return buffer.Slice(4, frameSize);

                        buffer = buffer.Slice(totalSize);
                    }

                    if (result.IsCompleted)
                        break;

                    _reader.AdvanceTo(buffer.Start);
                }
            }
            finally
            {
                await _reader.CompleteAsync().ConfigureAwait(false);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed != 0)
                throw new PulsarStreamDisposedException();
        }
    }
}
