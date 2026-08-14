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

namespace DotPulsar.Tests.Internal;

using DotPulsar.Internal;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Events;

[Trait("Category", "Unit")]
public sealed class ProcessReconnectTests
{
    [Theory]
    [InlineData(ProcessKind.Producer)]
    [InlineData(ProcessKind.Consumer)]
    [InlineData(ProcessKind.Reader)]
    public async Task Start_WhenInitiallyDisconnected_EstablishesInitialChannel(ProcessKind processKind)
    {
        //Arrange
        var correlationId = Guid.NewGuid();
        var channel = new TrackingChannelContainer();
        await using var harness = CreateHarness(processKind, correlationId, channel);
        channel.OnEstablished = _ => harness.Process.Handle(new ChannelConnected(correlationId));

        //Act
        harness.Process.Start();
        await channel.WaitForEstablishAsync(Current.CancellationToken);
        await harness.WaitForConnected(Current.CancellationToken);

        //Assert
        channel.CurrentGeneration.ShouldBe(1);
        channel.EstablishCount.ShouldBe(1);
    }

    [Theory]
    [InlineData(ProcessKind.Producer)]
    [InlineData(ProcessKind.Consumer)]
    [InlineData(ProcessKind.Reader)]
    public async Task Handle_WhenChannelDisconnectedTwice_DoesNotCloseReplacementChannel(ProcessKind processKind)
    {
        //Arrange
        var correlationId = Guid.NewGuid();
        var channel = new TrackingChannelContainer();
        await using var harness = CreateHarness(processKind, correlationId, channel);
        channel.OnEstablished = _ => harness.Process.Handle(new ChannelConnected(correlationId));

        harness.Process.Start();
        await channel.WaitForEstablishAsync(Current.CancellationToken);
        await harness.WaitForConnected(Current.CancellationToken);

        //Act
        var closeBlocker = channel.BlockNextClose();
        harness.Process.Handle(new ChannelDisconnected(correlationId));
        await closeBlocker.WaitForCloseStarted(Current.CancellationToken);
        harness.Process.Handle(new ChannelDisconnected(correlationId));
        closeBlocker.Release();

        await channel.WaitForEstablishAsync(Current.CancellationToken);
        var replacementGeneration = channel.CurrentGeneration;
        var replacementClosed = await Task.WhenAny(
            channel.ReplacementClosed,
            Task.Delay(TimeSpan.FromMilliseconds(250), Current.CancellationToken)) == channel.ReplacementClosed;

        //Assert
        replacementGeneration.ShouldBe(2);
        replacementClosed.ShouldBeFalse();
        channel.EstablishCount.ShouldBe(2);
    }

    [Theory]
    [InlineData(ProcessKind.Producer)]
    [InlineData(ProcessKind.Consumer)]
    [InlineData(ProcessKind.Reader)]
    public async Task Handle_WhenReplacementDisconnectsDuringReconnect_ReconnectsAgain(ProcessKind processKind)
    {
        //Arrange
        var correlationId = Guid.NewGuid();
        var channel = new TrackingChannelContainer();
        await using var harness = CreateHarness(processKind, correlationId, channel);
        channel.OnEstablished = generation =>
        {
            harness.Process.Handle(new ChannelConnected(correlationId));
            if (generation == 2)
                harness.Process.Handle(new ChannelDisconnected(correlationId));
        };

        harness.Process.Start();
        await channel.WaitForEstablishAsync(Current.CancellationToken);
        await harness.WaitForConnected(Current.CancellationToken);

        //Act
        harness.Process.Handle(new ChannelDisconnected(correlationId));

        await channel.WaitForEstablishAsync(Current.CancellationToken);
        await channel.WaitForEstablishAsync(Current.CancellationToken);

        //Assert
        channel.CurrentGeneration.ShouldBe(3);
        channel.EstablishCount.ShouldBe(3);
    }

    private static ProcessHarness CreateHarness(
        ProcessKind processKind,
        Guid correlationId,
        TrackingChannelContainer channel)
    {
        switch (processKind)
        {
            case ProcessKind.Producer:
                {
                    var stateManager = new StateManager<ProducerState>(
                        ProducerState.Disconnected,
                        ProducerState.Closed,
                        ProducerState.Faulted,
                        ProducerState.Fenced);
                    return new ProcessHarness(
                        new ProducerProcess(correlationId, stateManager, channel),
                        channel,
                        cancellationToken => stateManager
                            .OnStateChangeTo(ProducerState.Connected, cancellationToken)
                            .AsTask());
                }
            case ProcessKind.Consumer:
                {
                    var stateManager = new StateManager<ConsumerState>(
                        ConsumerState.Disconnected,
                        ConsumerState.Closed,
                        ConsumerState.ReachedEndOfTopic,
                        ConsumerState.Faulted);
                    return new ProcessHarness(
                        new ConsumerProcess(correlationId, stateManager, channel, false),
                        channel,
                        cancellationToken => stateManager
                            .OnStateChangeTo(ConsumerState.Active, cancellationToken)
                            .AsTask());
                }
            case ProcessKind.Reader:
                {
                    var stateManager = new StateManager<ReaderState>(
                        ReaderState.Disconnected,
                        ReaderState.Closed,
                        ReaderState.ReachedEndOfTopic,
                        ReaderState.Faulted);
                    return new ProcessHarness(
                        new ReaderProcess(correlationId, stateManager, channel),
                        channel,
                        cancellationToken => stateManager
                            .OnStateChangeTo(ReaderState.Connected, cancellationToken)
                            .AsTask());
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(processKind), processKind, null);
        }
    }

    public enum ProcessKind
    {
        Producer,
        Consumer,
        Reader
    }

    private sealed class ProcessHarness : IAsyncDisposable
    {
        private readonly TrackingChannelContainer _channel;
        private readonly Func<CancellationToken, Task> _waitForConnected;

        public ProcessHarness(
            IProcess process,
            TrackingChannelContainer channel,
            Func<CancellationToken, Task> waitForConnected)
        {
            Process = process;
            _channel = channel;
            _waitForConnected = waitForConnected;
        }

        public IProcess Process { get; }

        public Task WaitForConnected(CancellationToken cancellationToken)
            => _waitForConnected(cancellationToken);

        public async ValueTask DisposeAsync()
        {
            await Process.DisposeAsync();
            await _channel.DisposeAsync();
        }
    }

    private sealed class TrackingChannelContainer : IContainsChannel
    {
        private readonly Lock _lock = new();
        private readonly SemaphoreSlim _established = new(0);
        private readonly TaskCompletionSource _replacementClosed = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private CloseBlocker? _nextCloseBlocker;
        private int _currentGeneration;
        private int _establishCount;

        public Action<int>? OnEstablished { get; set; }

        public int CurrentGeneration
        {
            get
            {
                lock (_lock)
                    return _currentGeneration;
            }
        }

        public int EstablishCount
        {
            get
            {
                lock (_lock)
                    return _establishCount;
            }
        }

        public Task ReplacementClosed => _replacementClosed.Task;

        public Task EstablishNewChannel(CancellationToken cancellationToken)
        {
            int generation;
            lock (_lock)
            {
                _currentGeneration++;
                _establishCount++;
                generation = _currentGeneration;
            }

            _established.Release();
            OnEstablished?.Invoke(generation);
            return Task.CompletedTask;
        }

        public async ValueTask CloseChannel(CancellationToken cancellationToken)
        {
            CloseBlocker? closeBlocker;
            int generation;
            lock (_lock)
            {
                generation = _currentGeneration;
                closeBlocker = _nextCloseBlocker;
                _nextCloseBlocker = null;
            }

            if (closeBlocker is not null)
            {
                closeBlocker.CloseStarted.TrySetResult();
                await closeBlocker.Continue.Task.WaitAsync(cancellationToken);
            }

            if (generation == 2)
                _replacementClosed.TrySetResult();
        }

        public ValueTask ChannelFaulted(Exception exception) => ValueTask.CompletedTask;

        public ValueTask DisposeAsync()
        {
            _established.Dispose();
            return ValueTask.CompletedTask;
        }

        public async Task WaitForEstablishAsync(CancellationToken cancellationToken)
            => await _established.WaitAsync(cancellationToken);

        public CloseBlocker BlockNextClose()
        {
            var closeBlocker = new CloseBlocker();
            lock (_lock)
                _nextCloseBlocker = closeBlocker;
            return closeBlocker;
        }
    }

    private sealed class CloseBlocker
    {
        public TaskCompletionSource CloseStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource Continue { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task WaitForCloseStarted(CancellationToken cancellationToken)
            => await CloseStarted.Task.WaitAsync(cancellationToken);

        public void Release() => Continue.TrySetResult();
    }
}
