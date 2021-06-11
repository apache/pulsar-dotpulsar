
namespace DotPulsar.IntegrationTests
{
    using Extensions;
    using Fixtures;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    [Collection(nameof(StandaloneClusterTest))]
    public class ProducerTests
    {
        [Fact]
        public async void SimpleProduceConsume_WhenSendingMessagesFromProducer_ThenReceiveMessagesFromConsumer()
        {
            //Arrange
            await using var client = PulsarClient.Builder().Build();
            string topicName = $"simple-produce-consume{Guid.NewGuid():N}";;
            string content = "test-message";

            //Act
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();

            await using var consumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName("test-sub")
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .Create();

            //Assert
            await producer.StateChangedTo(ProducerState.Connected);
            await producer.Send(content);
            Assert.Equal(content,(await consumer.Receive()).Value());
        }
    }
}
