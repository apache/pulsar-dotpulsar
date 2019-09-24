using DotPulsar.Internal.Extensions;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class PulsarStream : IAsyncDisposable
    {
        private const long PauseAtMoreThan10Mb = 10485760;
        private const long ResumeAt5MbOrLess = 5242881;

        private readonly Stream _stream;
        private readonly Action<uint, ReadOnlySequence<byte>> _handler;
        private readonly CancellationTokenSource _tokenSource;

        public PulsarStream(Stream stream, Action<uint, ReadOnlySequence<byte>> handler)
        {
            _stream = stream;
            _handler = handler;
            _tokenSource = new CancellationTokenSource();
            var options = new PipeOptions(pauseWriterThreshold: PauseAtMoreThan10Mb, resumeWriterThreshold: ResumeAt5MbOrLess);
            var pipe = new Pipe(options);
            var fill = FillPipe(_stream, pipe.Writer, _tokenSource.Token);
            var read = ReadPipe(pipe.Reader, _tokenSource.Token);
            IsClosed = Task.WhenAny(fill, read);
        }

        public Task IsClosed { get; }

        public async Task Send(ReadOnlySequence<byte> sequence)
        {
            try
            {
#if NETSTANDARD2_0
                var data = sequence.ToArray();
                await _stream.WriteAsync(data, 0, data.Length);
#else
                foreach (var segment in sequence)
                {
                    await _stream.WriteAsync(segment);
                }
#endif
            }
            catch
            {
                _tokenSource.Cancel();
                throw;
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async ValueTask DisposeAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _tokenSource.Cancel();
#if NETSTANDARD2_0
            _stream.Dispose();
#else
            await _stream.DisposeAsync();
#endif
        }

        private async Task FillPipe(Stream stream, PipeWriter writer, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var memory = writer.GetMemory(84999); // LOH - 1 byte
#if NETSTANDARD2_0
                    var buffer = new byte[memory.Length];
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    buffer.CopyTo(memory);
#else
                    var bytesRead = await stream.ReadAsync(memory, cancellationToken);
#endif
                    if (bytesRead == 0)
                        break;

                    writer.Advance(bytesRead);

                    var result = await writer.FlushAsync(cancellationToken);
                    if (result.IsCompleted)
                        break;
                }
            }
            catch
            {
                _tokenSource.Cancel();
            }

            writer.Complete();
        }

        private async Task ReadPipe(PipeReader reader, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await reader.ReadAsync(cancellationToken);
                    var buffer = result.Buffer;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        if (buffer.Length < 4)
                            break;

                        var frameSize = buffer.ReadUInt32(0, true);
                        var totalSize = frameSize + 4;
                        if (buffer.Length < totalSize)
                            break;

                        var commandSize = buffer.ReadUInt32(4, true);

                        _handler(commandSize, buffer.Slice(8, totalSize - 8));

                        buffer = buffer.Slice(totalSize);
                    }

                    if (result.IsCompleted)
                        break;

                    reader.AdvanceTo(buffer.Start);
                }
            }
            catch
            {
                _tokenSource.Cancel();
            }

            reader.Complete();
        }
    }
}
