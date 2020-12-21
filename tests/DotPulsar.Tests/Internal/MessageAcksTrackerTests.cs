namespace DotPulsar.Tests.Internal
{
    using DotPulsar.Internal;
    using FluentAssertions;
    using System.Buffers;
    using System.Linq;
    using Xunit;

    public class MessageAcksTrackerTests
    {
        [Fact]
        public void Test_Instance()
        {
            var tracker = new MessageAcksTracker(1, 2, 3);
            tracker.Should().BeOfType<MessageAcksTracker>();
        }
    }
}