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

using DotPulsar.Exceptions;

[Trait("Category", "Unit")]
public sealed class PulsarClientProducerValidationTests
{
    [Fact]
    public async Task CreateProducer_GivenMaxPendingMessagesIsZero_ShouldThrowConfigurationException()
    {
        // Arrange
        await using var client = PulsarClient
            .Builder()
            .ServiceUrl(new Uri("pulsar://localhost:6650"))
            .Build();

        var options = new ProducerOptions<string>("persistent://public/default/my-topic", Schema.String)
        {
            MaxPendingMessages = 0
        };

        // Act
        var act = () => client.CreateProducer(options);

        // Assert
        var exception = act.ShouldThrow<ConfigurationException>();
        exception.Message.ShouldBe("ProducerOptions.MaxPendingMessages must be greater than 0");
    }
}
