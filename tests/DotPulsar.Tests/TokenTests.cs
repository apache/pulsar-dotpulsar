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
public class TokenTests
{
    private const string MyTopic = "persistent://public/default/mytopic";

    private readonly IntegrationFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public TokenTests(IntegrationFixture fixture, ITestOutputHelper outputHelper)
    {
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
        var exception = await Record.ExceptionAsync(() => producer.Send("Test").AsTask());
        var state = await producer.OnStateChangeTo(ProducerState.Faulted);

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
            _testOutputHelper.WriteLine($"Received token: {token}");
            return ValueTask.FromResult(token);
        });

        await using var producer = CreateProducer(client);

        // Act
        _ = await producer.Send("Test"); // Make sure we have a working connection
        throwException = true;
        var state = await producer.OnStateChangeTo(ProducerState.Faulted);

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
        var exception = await Record.ExceptionAsync(() => producer.Send("Test").AsTask());
        var state = await producer.OnStateChangeTo(ProducerState.Faulted);

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
            _testOutputHelper.WriteLine($"Received token: {token}");
            return ValueTask.FromResult(token);
        });

        await using var producer = CreateProducer(client);

        // Act
        _ = await producer.Send("Test"); // Make sure we have a working connection
        await tcs.Task;
        var state = await producer.OnStateChangeTo(ProducerState.Connected);

        // Assert
        state.Should().Be(ProducerState.Connected);
    }

    private IPulsarClient CreateClient(Func<CancellationToken, ValueTask<string>> tokenSupplier)
        => PulsarClient
            .Builder()
            .Authentication(AuthenticationFactory.Token(tokenSupplier))
            .ExceptionHandler(ec => _testOutputHelper.WriteLine($"Exception: {ec.Exception}"))
            .ServiceUrl(_fixture.ServiceUrl)
            .Build();

    private IProducer<string> CreateProducer(IPulsarClient client)
        => client
            .NewProducer(Schema.String)
            .Topic(MyTopic)
            .StateChangedHandler(Monitor)
            .Create();

    private void Monitor(ProducerStateChanged stateChanged, CancellationToken _)
    {
        var stateMessage = stateChanged.ProducerState switch
        {
            ProducerState.Connected => "is connected",
            ProducerState.Disconnected => "is disconnected",
            ProducerState.PartiallyConnected => "is partially connected",
            ProducerState.Closed => "has closed",
            ProducerState.Faulted => "has faulted",
            _ => $"has an unknown state '{stateChanged.ProducerState}'"
        };

        var topic = stateChanged.Producer.Topic;
        _testOutputHelper.WriteLine($"The producer for topic '{topic}' {stateMessage}");
    }
}
