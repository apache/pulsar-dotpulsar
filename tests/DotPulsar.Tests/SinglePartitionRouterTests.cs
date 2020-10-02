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

namespace DotPulsar.Tests
{
    using DotPulsar.Internal;
    using Xunit;

    public class SinglePartitionRouterTests
    {
        [Fact]
        public void ChoosePartition_GivenMessageWithKey_ShouldChoosePartitionBasedOnKeyHash()
        {
            //Arrange
            var message = new MessageMetadata()
            {
                Key = "test_key"
            };
            var partitionTopicMetadata = new PartitionedTopicMetadata(3);

            //Act
            var router = new SinglePartitionRouter();
            var result = router.ChoosePartition(message, partitionTopicMetadata);

            //Assert
            Assert.Equal(Murmur3_32Hash.Instance.MakeHash(message.Key) % 3, result);
        }

        [Fact]
        public void ChoosePartition_GivenMessageWithoutKey_ShouldSinglePartitionRoute()
        {
            //Arrange
            var message = new MessageMetadata();
            var partitionTopicMetadata = new PartitionedTopicMetadata(3);

            //Act
            var router = new SinglePartitionRouter(1);
            var result = router.ChoosePartition(message, partitionTopicMetadata);

            //Assert
            Assert.Equal(1, result);
        }
    }
}
