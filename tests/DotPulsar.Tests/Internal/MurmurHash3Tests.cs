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

using DotPulsar.Internal;

[Trait("Category", "Unit")]
public sealed class MurmurHash3Tests
{
    [Theory]
    [InlineAutoData("", 0U, 0U)]
    [InlineAutoData("", 1U, 0x514E28B7U)]
    [InlineAutoData("", 0xffffffffU, 0x81F16F39U)]
    [InlineAutoData("\0\0\0\0", 0U, 0x2362F9DEU)]
    [InlineAutoData("aaaa", 0x9747b28cU, 0x5A97808AU)]
    [InlineAutoData("aaa", 0x9747b28cU, 0x283E0130U)]
    [InlineAutoData("aa", 0x9747b28cU, 0x5D211726U)]
    [InlineAutoData("a", 0x9747b28cU, 0x7FA09EA6U)]
    [InlineAutoData("abcd", 0x9747b28cU, 0xF0478627U)]
    [InlineAutoData("abc", 0x9747b28cU, 0xC84A62DDU)]
    [InlineAutoData("ab", 0x9747b28cU, 0x74875592U)]
    public void Hash32_GivenInput_ShouldCalculateHash(string text, uint seed, uint expected)
    {
        // Arrange
        var data = System.Text.Encoding.UTF8.GetBytes(text);

        // Act
        var actual = MurmurHash3.Hash32(data, seed);

        // Assert
        actual.ShouldBe(expected);
    }
}
