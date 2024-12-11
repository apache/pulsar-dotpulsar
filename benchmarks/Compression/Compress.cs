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
public class Compress
{
    [ParamsAllValues]
    public MessageSize Size { get; set; }

    [ParamsAllValues]
    public MessageType Type { get; set; }

    [GlobalSetup]
    public void Setup() => Data = MessageBytes.GetBytes(Size, Type);

    public ReadOnlySequence<byte> Data { get; private set; }

    [Benchmark]
    public void K4aosCompressionLz4()
    {
        using var compressor = Factories.K4aosCompressionLz4CompressorFactory.Create();
        _ = compressor.Compress(Data);
    }

    [Benchmark]
    public void IronSnappy()
    {
        using var compressor = Factories.IronSnappyCompressorFactory.Create();
        _ = compressor.Compress(Data);
    }

    [Benchmark]
    public void BuiltinZlib()
    {
        using var compressor = Factories.BuiltinZlibCompressionCompressorFactory.Create();
        _ = compressor.Compress(Data);
    }

    [Benchmark]
    public void ZstdNet()
    {
        using var compressor = Factories.ZstdNetCompressorFactory.Create();
        _ = compressor.Compress(Data);
    }

    [Benchmark]
    public void ZstdSharp()
    {
        using var compressor = Factories.ZstdSharpCompressorFactory.Create();
        _ = compressor.Compress(Data);
    }
}
