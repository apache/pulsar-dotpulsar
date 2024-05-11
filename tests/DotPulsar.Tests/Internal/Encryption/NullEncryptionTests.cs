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

namespace DotPulsar.Tests.Internal.Encryption;

using DotPulsar.Internal.Encryption;

[Trait("Category", "Unit")]
public class NullEncryptionTests
{
    [Fact]
    public void Compression_GivenDataToCompressAndDecompress_ShouldReturnOriginalData()
    {
        // Arrange
        var couldLoad = NullEncryption.TryLoading(out var encryptorFactory, out var decryptorFactory);
        couldLoad.Should().BeTrue();
        using var compressor = encryptorFactory!.Create();
        using var decompressor = decryptorFactory!.Create();

        // Act
        var compressionWorks = EncryptionTester.TestEncryption(encryptorFactory, decryptorFactory);

        // Assert
        compressionWorks.Should().BeTrue();
    }
}
