namespace DotPulsar.Tests
{
    using DotPulsar.Extensions;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Xunit;

    public class DeadLetterTests
    {
        [Fact]
        public void CreateConsumerWithDeadLetterConfig()
        {
            var client = PulsarClient.Builder().Build();

            var consumer = client.NewConsumer<string>(Schema.String)
                .DeadLetterPolicy(b =>
                    b.DeadLetterTopic("DLQ")
                     .RetryLetterTopic("RLQ")
                     .MaxRedeliveryCount(10))
                .Topic("Topic")
                .Create();
        }
    }
}
