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

using DotPulsar.Abstractions;
using DotPulsar.Internal;
using DotPulsar.Internal.Abstractions;

[Trait("Category", "Unit")]
public class SubProducerTests
{
    [Fact]
    public async Task EstablishNewChannel_WhenCalledThreeTimesInSuccession_ShouldNotThrowObjectDisposedException()
    {
        //Arrange
        var factory = Substitute.For<IProducerChannelFactory>();
        factory.Create(Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(Substitute.For<IProducerChannel>()));

        await using var sut = CreateSubProducer(factory);

        //Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await sut.EstablishNewChannel(CancellationToken.None);
            await sut.EstablishNewChannel(CancellationToken.None);
            await sut.EstablishNewChannel(CancellationToken.None);
        });

        //Assert
        exception.ShouldNotBeOfType<ObjectDisposedException>();
    }

    private static SubProducer CreateSubProducer(IProducerChannelFactory factory)
        => new(
            correlationId: Guid.NewGuid(),
            registerEvent: Substitute.For<IRegisterEvent>(),
            initialChannel: Substitute.For<IProducerChannel>(),
            executor: new DirectExecutor(),
            state: Substitute.For<IState<ProducerState>>(),
            factory: factory,
            partition: 0,
            maxPendingMessages: 1000,
            topic: "persistent://public/default/test");

    private sealed class DirectExecutor : IExecute
    {
        public ValueTask Execute(Action action, CancellationToken cancellationToken = default)
        {
            action();
            return ValueTask.CompletedTask;
        }

        public async ValueTask Execute(Func<Task> func, CancellationToken cancellationToken = default)
            => await func().ConfigureAwait(false);

        public async ValueTask Execute(Func<ValueTask> func, CancellationToken cancellationToken = default)
            => await func().ConfigureAwait(false);

        public ValueTask<TResult> Execute<TResult>(Func<TResult> func, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(func());

        public async ValueTask<TResult> Execute<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default)
            => await func().ConfigureAwait(false);

        public async ValueTask<TResult> Execute<TResult>(Func<ValueTask<TResult>> func, CancellationToken cancellationToken = default)
            => await func().ConfigureAwait(false);

        public ValueTask<bool> TryExecuteOnce(Action action, CancellationToken cancellationToken = default)
        {
            action();
            return ValueTask.FromResult(true);
        }

        public async ValueTask<bool> TryExecuteOnce(Func<Task> func, CancellationToken cancellationToken = default)
        {
            try
            {
                await func().ConfigureAwait(false);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        public async ValueTask<bool> TryExecuteOnce(Func<ValueTask> func, CancellationToken cancellationToken = default)
        {
            await func().ConfigureAwait(false);
            return true;
        }

        public ValueTask<ExecutionResult<TResult>> TryExecuteOnce<TResult>(Func<TResult> func, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(new ExecutionResult<TResult>(true, func()));

        public async ValueTask<ExecutionResult<TResult>> TryExecuteOnce<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default)
            => new ExecutionResult<TResult>(true, await func().ConfigureAwait(false));

        public async ValueTask<ExecutionResult<TResult>> TryExecuteOnce<TResult>(Func<ValueTask<TResult>> func, CancellationToken cancellationToken = default)
            => new ExecutionResult<TResult>(true, await func().ConfigureAwait(false));
    }
}
