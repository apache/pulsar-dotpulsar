﻿/*
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

namespace DotPulsar.Tests.Internal.Compression;

using DotPulsar.Internal.Compression;
using FluentAssertions;
using Xunit;

[Trait("Category", "Unit")]
public class ZlibCompressionTests
{
    [Fact]
    public void Compression_GivenDataToCompressAndDecompress_ShouldReturnOriginalData()
    {
        // Arrange
        var couldLoad = ZlibCompression.TryLoading(out var compressorFactory, out var decompressorFactory);
        couldLoad.Should().BeTrue();
        using var compressor = compressorFactory!.Create();
        using var decompressor = decompressorFactory!.Create();

        // Act
        var compressionWorks = CompressionTester.TestCompression(compressorFactory, decompressorFactory);

        // Assert
        compressionWorks.Should().BeTrue();
    }
}
