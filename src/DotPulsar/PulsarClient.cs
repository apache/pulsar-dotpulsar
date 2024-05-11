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

namespace DotPulsar;

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Extensions;
using DotPulsar.Internal;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Compression;
using DotPulsar.Internal.Encryption;

/// <summary>
/// Pulsar client for creating producers, consumers and readers.
/// </summary>
public sealed class PulsarClient : IPulsarClient
{
    private readonly object _lock = new();
    private readonly HashSet<IAsyncDisposable> _disposables;
    private readonly IConnectionPool _connectionPool;
    private readonly IHandleException _exceptionHandler;
    private int _isDisposed;

    public Uri ServiceUrl { get; }

    internal PulsarClient(
        IConnectionPool connectionPool,
        IHandleException exceptionHandler,
        Uri serviceUrl)
    {
        _disposables = [];
        _connectionPool = connectionPool;
        _exceptionHandler = exceptionHandler;
        ServiceUrl = serviceUrl;
        _isDisposed = 0;
        DotPulsarMeter.ClientCreated();
    }

    /// <summary>
    /// Get a builder that can be used to configure and build a PulsarClient instance.
    /// </summary>
    public static IPulsarClientBuilder Builder()
        => new PulsarClientBuilder();

    /// <summary>
    /// Create a producer.
    /// </summary>
    public IProducer<TMessage> CreateProducer<TMessage>(ProducerOptions<TMessage> options)
    {
        ThrowIfDisposed();

        ICompressorFactory? compressorFactory = null;
        if (options.CompressionType != CompressionType.None)
        {
            var compressionType = (Internal.PulsarApi.CompressionType) options.CompressionType;
            compressorFactory = CompressionFactories.CompressorFactories().SingleOrDefault(f => f.CompressionType == compressionType);

            if (compressorFactory is null)
                throw new CompressionException($"Support for {compressionType} compression was not found");
        }

        var encryptorFactory = EncryptionFactories.GetEncryptorFactory(options.EncryptionAlgo);

        var producer = new Producer<TMessage>(ServiceUrl, options, _exceptionHandler, _connectionPool, compressorFactory, encryptorFactory);

        if (options.StateChangedHandler is not null)
            _ = StateMonitor.MonitorProducer(producer, options.StateChangedHandler);

        AddDisposable(producer);
        producer.StateChangedTo(ProducerState.Closed).AsTask().ContinueWith(_ => RemoveDisposable(producer));
        return producer;
    }

    /// <summary>
    /// Create a consumer.
    /// </summary>
    public IConsumer<TMessage> CreateConsumer<TMessage>(ConsumerOptions<TMessage> options)
    {
        ThrowIfDisposed();

        var consumer = new Consumer<TMessage>(ServiceUrl, options, _connectionPool, _exceptionHandler);
        if (options.StateChangedHandler is not null)
            _ = StateMonitor.MonitorConsumer(consumer, options.StateChangedHandler);

        AddDisposable(consumer);
        consumer.StateChangedTo(ConsumerState.Closed).AsTask().ContinueWith(_ => RemoveDisposable(consumer));
        return consumer;
    }

    /// <summary>
    /// Create a reader.
    /// </summary>
    public IReader<TMessage> CreateReader<TMessage>(ReaderOptions<TMessage> options)
    {
        ThrowIfDisposed();

        var reader = new Reader<TMessage>(ServiceUrl, options, _exceptionHandler, _connectionPool);
        if (options.StateChangedHandler is not null)
            _ = StateMonitor.MonitorReader(reader, options.StateChangedHandler);

        AddDisposable(reader);
        reader.StateChangedTo(ReaderState.Closed).AsTask().ContinueWith(_ => RemoveDisposable(reader));
        return reader;
    }

    /// <summary>
    /// Dispose the client and all its producers, consumers and readers.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        IEnumerable<IAsyncDisposable> disposables;

        lock (_lock)
        {
            disposables = _disposables.ToArray();
            _disposables.Clear();
        }

        foreach (var item in disposables)
            await item.DisposeAsync().ConfigureAwait(false);

        await _connectionPool.DisposeAsync().ConfigureAwait(false);

        DotPulsarMeter.ClientDisposed();
    }

    private void AddDisposable(IAsyncDisposable disposable)
    {
        lock (_lock)
        {
            _disposables.Add(disposable);
        }
    }

    internal void RemoveDisposable(IAsyncDisposable disposable)
    {
        lock (_lock)
        {
            _disposables.Remove(disposable);
        }
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed != 0)
            throw new PulsarClientDisposedException();
    }
}
