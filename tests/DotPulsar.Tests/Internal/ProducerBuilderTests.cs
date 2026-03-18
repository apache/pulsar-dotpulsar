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
using DotPulsar.Exceptions;
using DotPulsar.Extensions;

[Trait("Category", "Unit")]
public sealed class ProducerBuilderTests
{
    [Fact]
    public async Task Create_GivenMaxPendingMessagesIsZero_ShouldThrowConfigurationException()
    {
        // Arrange
        await using var client = CreateClient();

        // Act
        var act = () => client
            .NewProducer(Schema.String)
            .Topic("persistent://public/default/my-topic")
            .MaxPendingMessages(0)
            .Create();

        // Assert
        var exception = act.ShouldThrow<ConfigurationException>();
        exception.Message.ShouldBe("ProducerOptions.MaxPendingMessages must be greater than 0");
    }

    private static IPulsarClient CreateClient()
        => PulsarClient
            .Builder()
            .ServiceUrl(new Uri("pulsar://localhost:6650"))
            .Build();
}
