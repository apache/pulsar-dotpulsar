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

namespace Compression;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Buffers;

[RankColumn, Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Decompress
{
    [ParamsAllValues]
    public MessageSize Size { get; set; }

    [ParamsAllValues]
    public MessageType Type { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var data = MessageBytes.GetBytes(Size, Type);
        DecompressedSize = (int) data.Length;

        using (var compressor = Factories.K4aosCompressionLz4CompressorFactory.Create())
            K4aosCompressionLz4Data = compressor.Compress(data);
        using (var compressor = Factories.IronSnappyCompressorFactory.Create())
            IronSnappyData = compressor.Compress(data);
        using (var compressor = Factories.BuiltinZlibCompressionCompressorFactory.Create())
            BuiltinZlibData = compressor.Compress(data);
        using (var compressor = Factories.ZstdNetCompressorFactory.Create())
            ZstdNetData = compressor.Compress(data);
        using (var compressor = Factories.ZstdSharpCompressorFactory.Create())
            ZstdSharpData = compressor.Compress(data);
    }

    public int DecompressedSize { get; private set; }
    public ReadOnlySequence<byte> K4aosCompressionLz4Data { get; private set; }
    public ReadOnlySequence<byte> IronSnappyData { get; private set; }
    public ReadOnlySequence<byte> BuiltinZlibData { get; private set; }
    public ReadOnlySequence<byte> ZstdNetData { get; private set; }
    public ReadOnlySequence<byte> ZstdSharpData { get; private set; }

    [Benchmark]
    public void K4aosCompressionLz4()
    {
        using var decompressor = Factories.K4aosCompressionLz4DecompressorFactory.Create();
        _ = decompressor.Decompress(K4aosCompressionLz4Data, DecompressedSize);
    }

    [Benchmark]
    public void IronSnappy()
    {
        using var decompressor = Factories.IronSnappyDecompressorFactory.Create();
        _ = decompressor.Decompress(IronSnappyData, DecompressedSize);
    }

    [Benchmark]
    public void BuiltinZlib()
    {
        using var decompressor = Factories.BuiltinZlibCompressionDecompressorFactory.Create();
        _ = decompressor.Decompress(BuiltinZlibData, DecompressedSize);
    }

    [Benchmark]
    public void ZstdNet()
    {
        using var decompressor = Factories.ZstdNetDecompressorFactory.Create();
        _ = decompressor.Decompress(ZstdNetData, DecompressedSize);
    }

    [Benchmark]
    public void ZstdSharp()
    {
        using var decompressor = Factories.ZstdSharpDecompressorFactory.Create();
        _ = decompressor.Decompress(ZstdSharpData, DecompressedSize);
    }
}
