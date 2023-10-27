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

namespace DotPulsar.Tests;

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Extensions;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

[Collection("Integration"), Trait("Category", "Integration")]
public class PulsarClientTests : IDisposable
{
    private readonly CancellationTokenSource _cts;
    private readonly IntegrationFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public PulsarClientTests(IntegrationFixture fixture, ITestOutputHelper outputHelper)
    {
        _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
        _fixture = fixture;
        _testOutputHelper = outputHelper;
    }

    [Fact]
    public async Task TokenSupplier_WhenTokenSupplierInitiallyThrowsAnException_ShouldFaultProducer()
    {
        // Arrange
        await using var client = CreateClient(ct => throw new Exception());
        await using var producer = CreateProducer(client);

        // Act
        var exception = await Record.ExceptionAsync(() => producer.Send("Test", _cts.Token).AsTask());
        var state = await producer.OnStateChangeTo(ProducerState.Faulted, _cts.Token);

        // Assert
        exception.Should().BeOfType<ProducerFaultedException>();
        state.Should().Be(ProducerState.Faulted);
    }

    [Fact]
    public async Task TokenSupplier_WhenTokenSupplierThrowsAnExceptionOnAuthChallenge_ShouldFaultProducer()
    {
        // Arrange
        var throwException = false;
        await using var client = CreateClient(ct =>
        {
            if (throwException)
                throw new Exception();
            var token = _fixture.CreateToken(TimeSpan.FromSeconds(10));
            _testOutputHelper.Log($"Received token: {token}");
            return ValueTask.FromResult(token);
        });

        await using var producer = CreateProducer(client);

        // Act
        _ = await producer.Send("Test", _cts.Token); // Make sure we have a working connection
        throwException = true;
        var state = await producer.OnStateChangeTo(ProducerState.Faulted, _cts.Token);

        // Assert
        state.Should().Be(ProducerState.Faulted);
    }

    [Fact]
    public async Task TokenSupplier_WhenTokenSupplierReturnsToLate_ShouldFaultProducer()
    {
        // Arrange
        await using var client = CreateClient(async ct =>
        {
            await Task.Delay(TimeSpan.FromSeconds(10), ct);
            return string.Empty;
        });

        await using var producer = CreateProducer(client);

        // Act
        var exception = await Record.ExceptionAsync(() => producer.Send("Test", _cts.Token).AsTask());
        var state = await producer.OnStateChangeTo(ProducerState.Faulted, _cts.Token);

        // Assert
        exception.Should().BeOfType<ProducerFaultedException>();
        exception.InnerException.Should().BeOfType<AuthenticationException>();
        state.Should().Be(ProducerState.Faulted);
    }

    [Fact]
    public async Task TokenSupplier_WhenTokenSupplierReturnValidToken_ShouldStayConnected()
    {
        // Arrange
        var refreshCount = 0;
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await using var client = CreateClient(ct =>
        {
            ++refreshCount;
            if (refreshCount == 3)
                tcs.SetResult();

            var token = _fixture.CreateToken(TimeSpan.FromSeconds(10));
            _testOutputHelper.Log($"Received token: {token}");
            return ValueTask.FromResult(token);
        });

        await using var producer = CreateProducer(client);

        // Act
        _ = await producer.Send("Test", _cts.Token); // Make sure we have a working connection
        await tcs.Task;
        var state = await producer.OnStateChangeTo(ProducerState.Connected, _cts.Token);

        // Assert
        state.Should().Be(ProducerState.Connected);
    }

    private IPulsarClient CreateClient(Func<CancellationToken, ValueTask<string>> tokenSupplier)
        => PulsarClient
        .Builder()
        .Authentication(AuthenticationFactory.Token(tokenSupplier))
        .ExceptionHandler(_testOutputHelper.Log)
        .ServiceUrl(_fixture.ServiceUrl)
        .Build();

    private IProducer<string> CreateProducer(IPulsarClient client)
        => client
        .NewProducer(Schema.String)
        .Topic(_fixture.CreateTopic())
        .StateChangedHandler(_testOutputHelper.Log)
        .Create();

    public void Dispose() => _cts.Dispose();
}
