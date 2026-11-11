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
using DotPulsar.Internal.Compression;
using IronSnappy;
using System.Buffers;

[Trait("Category", "Unit")]
public class CompressionCodecsTests
{
    [Fact]
    public void Register_GivenWorkingCodec_ShouldTakePrecedenceOverDiscoveredCodec()
    {
        // Arrange
        var compressCalls = 0;
        var decompressCalls = 0;
        CompressionCodecs.Register(
            CompressionType.Snappy,
            data =>
            {
                Interlocked.Increment(ref compressCalls);
                return new ReadOnlySequence<byte>(Snappy.Encode(data.ToArray()));
            },
            (data, _) =>
            {
                Interlocked.Increment(ref decompressCalls);
                return new ReadOnlySequence<byte>(Snappy.Decode(data.ToArray()));
            });
        var compressCallsBefore = compressCalls;
        var decompressCallsBefore = decompressCalls;

        // Act
        var compressorFactories = CompressionFactories.CompressorFactories()
            .Where(f => f.CompressionType == DotPulsar.Internal.PulsarApi.CompressionType.Snappy)
            .ToArray();
        var decompressorFactories = CompressionFactories.DecompressorFactories()
            .Where(f => f.CompressionType == DotPulsar.Internal.PulsarApi.CompressionType.Snappy)
            .ToArray();
        var works = CompressionTester.TestCompression(compressorFactories.Single(), decompressorFactories.Single());

        // Assert
        works.ShouldBeTrue();
        compressCalls.ShouldBeGreaterThan(compressCallsBefore);
        decompressCalls.ShouldBeGreaterThan(decompressCallsBefore);
    }

    [Fact]
    public void Register_GivenCodecThatFailsRoundTrip_ShouldThrowCompressionException()
    {
        // Act
        var exception = Record.Exception(() => CompressionCodecs.Register(
            CompressionType.Lz4,
            data => data,
            (_, _) => new ReadOnlySequence<byte>([1, 2, 3])));

        // Assert
        exception.ShouldBeOfType<CompressionException>();
    }

    [Fact]
    public void Register_GivenCodecThatThrows_ShouldThrowCompressionException()
    {
        // Act
        var exception = Record.Exception(() => CompressionCodecs.Register(
            CompressionType.Lz4,
            _ => throw new InvalidOperationException(),
            (data, _) => data));

        // Assert
        exception.ShouldBeOfType<CompressionException>();
    }

    [Fact]
    public void Register_GivenCompressionTypeNone_ShouldThrowArgumentException()
    {
        // Act
        var exception = Record.Exception(() => CompressionCodecs.Register(CompressionType.None, data => data, (data, _) => data));

        // Assert
        exception.ShouldBeOfType<ArgumentException>();
    }
}
