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

// We cannot assume consumers of this library will obey the nullability guarantees
#nullable disable

namespace DotPulsar.Tests
{
    using DotPulsar;
    using Xunit;

    public class MessageIdTests
    {
        [Fact]
        public void Equals_GivenTheSameObject_ShouldBeEqual()
        {
            var m1 = new MessageId(1234, 5678, 9876, 5432);
            var m2 = m1;

            Assert.True(m1.Equals(m2));
            Assert.True(m1 == m2);
            Assert.False(m1 != m2);
        }

        [Fact]
        public void Equals_GivenTheSameValues_ShouldBeEqual()
        {
            var m1 = new MessageId(1234, 5678, 9876, 5432);
            var m2 = new MessageId(1234, 5678, 9876, 5432);

            Assert.True(m1.Equals(m2));
            Assert.True(m1 == m2);
            Assert.False(m1 != m2);
        }

        [Fact]
        public void Equals_GivenDifferentValues_ShouldNotBeEqual()
        {
            var m1 = new MessageId(1234, 5678, 9876, 5432);
            var m2 = new MessageId(9876, 6432, 1234, 6678);

            Assert.False(m1.Equals(m2));
            Assert.False(m1 == m2);
            Assert.True(m1 != m2);
        }

        [Fact]
        public void Equals_GivenAllNull_ShouldBeEqual()
        {
            MessageId m1 = null;
            MessageId m2 = null;

            Assert.True(m1 == m2);
            Assert.True(m1 == null);
            Assert.False(m1 != m2);
        }

        [Fact]
        public void Equals_GivenOneNull_ShouldNotBeEqual()
        {
            var m1 = new MessageId(1234, 5678, 9876, 5432);
            MessageId m2 = null;

            Assert.False(m1 == null);
            Assert.False(m1 == m2);
            Assert.False(m1.Equals(m2));
            Assert.True(m1 != m2);
        }
    }
}
