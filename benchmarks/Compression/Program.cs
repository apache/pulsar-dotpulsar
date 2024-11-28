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

using BenchmarkDotNet.Running;
using Compression;

OutputCompressionInfo(MessageSize.Small, MessageType.Protobuf);
OutputCompressionInfo(MessageSize.Large, MessageType.Protobuf);
OutputCompressionInfo(MessageSize.Small, MessageType.Json);
OutputCompressionInfo(MessageSize.Large, MessageType.Json);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

static void OutputCompressionInfo(MessageSize size, MessageType type)
{
    var data = MessageBytes.GetBytes(size, type);
    Console.WriteLine($"{size} {type} message. Uncompressed size: {data.Length}");

    using (var compressor = Factories.K4aosCompressionLz4CompressorFactory.Create())
        Console.WriteLine($"\tCompressed with K4os.Compression.LZ4: {compressor.Compress(data).Length}");
    using (var compressor = Factories.IronSnappyCompressorFactory.Create())
        Console.WriteLine($"\tCompressed with IronSnappy: {compressor.Compress(data).Length}");
    using (var compressor = Factories.DotNetZipCompressorFactory.Create())
        Console.WriteLine($"\tCompressed with DotNetZip: {compressor.Compress(data).Length}");
    using (var compressor = Factories.BuiltinZlibCompressionCompressorFactory.Create())
        Console.WriteLine($"\tCompressed with System.IO.Compression: {compressor.Compress(data).Length}");
    using (var compressor = Factories.ZstdNetCompressorFactory.Create())
        Console.WriteLine($"\tCompressed with ZstdNet: {compressor.Compress(data).Length}");
    using (var compressor = Factories.ZstdSharpCompressorFactory.Create())
        Console.WriteLine($"\tCompressed with ZstdSharp.Port: {compressor.Compress(data).Length}");
}
