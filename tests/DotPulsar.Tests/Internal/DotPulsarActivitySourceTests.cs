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
using System.Diagnostics;

[Trait("Category", "Unit")]
public sealed class DotPulsarActivitySourceTests : IDisposable
{
    private const string OperationName = "test process";
    private static readonly KeyValuePair<string, object?>[] _tags = [];

    private readonly ActivityListener _listener;
    private readonly ActivityTraceId _traceId;
    private readonly ActivitySpanId _spanId;
    private readonly IMessage _message;

    public DotPulsarActivitySourceTests()
    {
        var activitySource = DotPulsarActivitySource.ActivitySource;
        _listener = new ActivityListener
        {
            ShouldListenTo = source => ReferenceEquals(source, activitySource),
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(_listener);

        _traceId = ActivityTraceId.CreateRandom();
        _spanId = ActivitySpanId.CreateRandom();

        _message = Substitute.For<IMessage>();
        _message.Properties.Returns(new Dictionary<string, string>
        {
            [Constants.TraceParent] = $"00-{_traceId.ToHexString()}-{_spanId.ToHexString()}-01",
            [Constants.TraceState] = "vendor=value"
        });

        Activity.Current = null;
    }

    [Fact]
    public void StartConsumerActivity_GivenNone_ShouldNotCorrelate()
    {
        //Act
        using var activity = DotPulsarActivitySource.StartConsumerActivity(_message, OperationName, _tags, TraceCorrelation.None);

        //Assert
        activity.ShouldNotBeNull();
        activity.TraceId.ShouldNotBe(_traceId);
        activity.ParentId.ShouldBeNull();
        activity.Links.ShouldBeEmpty();
    }

    [Fact]
    public void StartConsumerActivity_GivenLink_ShouldLinkToCreationContextAndNotUseItAsParent()
    {
        //Act
        using var activity = DotPulsarActivitySource.StartConsumerActivity(_message, OperationName, _tags, TraceCorrelation.Link);

        //Assert
        activity.ShouldNotBeNull();
        activity.TraceId.ShouldNotBe(_traceId);
        activity.ParentId.ShouldBeNull();
        var link = activity.Links.ShouldHaveSingleItem();
        link.Context.TraceId.ShouldBe(_traceId);
        link.Context.SpanId.ShouldBe(_spanId);
        link.Context.TraceState.ShouldBe("vendor=value");
    }

    [Fact]
    public void StartConsumerActivity_GivenLinkAndAmbientActivity_ShouldBeChildOfAmbientActivity()
    {
        //Arrange
        using var ambient = new Activity("ambient").Start();

        //Act
        using var activity = DotPulsarActivitySource.StartConsumerActivity(_message, OperationName, _tags, TraceCorrelation.Link);

        //Assert
        activity.ShouldNotBeNull();
        activity.TraceId.ShouldBe(ambient.TraceId);
        activity.ParentSpanId.ShouldBe(ambient.SpanId);
        var link = activity.Links.ShouldHaveSingleItem();
        link.Context.TraceId.ShouldBe(_traceId);
        link.Context.SpanId.ShouldBe(_spanId);
    }

    [Fact]
    public void StartConsumerActivity_GivenParent_ShouldUseCreationContextAsParentAndLinkToIt()
    {
        //Act
        using var activity = DotPulsarActivitySource.StartConsumerActivity(_message, OperationName, _tags, TraceCorrelation.Parent);

        //Assert
        activity.ShouldNotBeNull();
        activity.TraceId.ShouldBe(_traceId);
        activity.ParentSpanId.ShouldBe(_spanId);
        activity.TraceStateString.ShouldBe("vendor=value");
        var link = activity.Links.ShouldHaveSingleItem();
        link.Context.TraceId.ShouldBe(_traceId);
        link.Context.SpanId.ShouldBe(_spanId);
    }

    [Fact]
    public void StartConsumerActivity_GivenParentAndAmbientActivity_ShouldLinkToCreationContextAndAmbientActivity()
    {
        //Arrange
        using var ambient = new Activity("ambient").Start();

        //Act
        using var activity = DotPulsarActivitySource.StartConsumerActivity(_message, OperationName, _tags, TraceCorrelation.Parent);

        //Assert
        activity.ShouldNotBeNull();
        activity.TraceId.ShouldBe(_traceId);
        activity.ParentSpanId.ShouldBe(_spanId);
        var links = activity.Links.ToList();
        links.Count.ShouldBe(2);
        links.ShouldContain(link => link.Context.TraceId == _traceId && link.Context.SpanId == _spanId);
        links.ShouldContain(link => link.Context.TraceId == ambient.TraceId && link.Context.SpanId == ambient.SpanId);
    }

    [Theory]
    [InlineData(TraceCorrelation.Link)]
    [InlineData(TraceCorrelation.Parent)]
    public void StartConsumerActivity_GivenNoCreationContextInMessage_ShouldNotCorrelate(TraceCorrelation traceCorrelation)
    {
        //Arrange
        _message.Properties.Returns(new Dictionary<string, string>());

        //Act
        using var activity = DotPulsarActivitySource.StartConsumerActivity(_message, OperationName, _tags, traceCorrelation);

        //Assert
        activity.ShouldNotBeNull();
        activity.TraceId.ShouldNotBe(_traceId);
        activity.ParentId.ShouldBeNull();
        activity.Links.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(TraceCorrelation.Link)]
    [InlineData(TraceCorrelation.Parent)]
    public void StartConsumerActivity_GivenInvalidTraceParentInMessage_ShouldNotCorrelate(TraceCorrelation traceCorrelation)
    {
        //Arrange
        _message.Properties.Returns(new Dictionary<string, string> { [Constants.TraceParent] = "not-a-traceparent" });

        //Act
        using var activity = DotPulsarActivitySource.StartConsumerActivity(_message, OperationName, _tags, traceCorrelation);

        //Assert
        activity.ShouldNotBeNull();
        activity.ParentId.ShouldBeNull();
        activity.Links.ShouldBeEmpty();
    }

    public void Dispose()
    {
        Activity.Current = null;
        _listener.Dispose();
    }
}
