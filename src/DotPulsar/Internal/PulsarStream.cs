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

namespace DotPulsar.Internal;

using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Exceptions;
using DotPulsar.Internal.Extensions;
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
    private const int FrameSizePrefix = 4;
    private const int UnknownFrameSize = 0;
    private const long PauseAtMoreThan10Mb = 10485760;
    private const long ResumeAt5MbOrLess = 5242881;
    private const int ChunkSize = 75000;

    private readonly Stream _stream;
    private readonly ChunkingPipeline _pipeline;
    private readonly PipeReader _reader;
    private readonly PipeWriter _writer;
    private int _isDisposed;

    public PulsarStream(Stream stream)
    {
        _stream = stream;
        _pipeline = new ChunkingPipeline(stream, ChunkSize);
        var options = new PipeOptions(pauseWriterThreshold: PauseAtMoreThan10Mb, resumeWriterThreshold: ResumeAt5MbOrLess);
        var pipe = new Pipe(options);
        _reader = pipe.Reader;
        _writer = pipe.Writer;
    }

    public async Task Send(ReadOnlySequence<byte> sequence)
    {
        ThrowIfDisposed();
        await _pipeline.Send(sequence).ConfigureAwait(false);
    }

#if NETSTANDARD2_0
    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
            _stream.Dispose();

        return new ValueTask();
    }
#else
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
            await _stream.DisposeAsync().ConfigureAwait(false);
    }
#endif

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
                var memory = _writer.GetMemory(84999);
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
            var frameSize = UnknownFrameSize;
            var totalSize = 0;

            while (true)
            {
                var minimumSize = FrameSizePrefix + frameSize;
                var readResult = await _reader.ReadAtLeastAsync(minimumSize, cancellationToken).ConfigureAwait(false);
                var buffer = readResult.Buffer;

                while (true)
                {
                    if (buffer.Length < FrameSizePrefix)
                        break;

                    if (frameSize == UnknownFrameSize)
                    {
                        frameSize = (int) buffer.ReadUInt32(0, true);
                        totalSize = FrameSizePrefix + frameSize;
                    }

                    if (buffer.Length < totalSize)
                        break;

                    yield return buffer.Slice(FrameSizePrefix, frameSize);

                    buffer = buffer.Slice(totalSize);
                    frameSize = UnknownFrameSize;
                }

                if (readResult.IsCompleted)
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
