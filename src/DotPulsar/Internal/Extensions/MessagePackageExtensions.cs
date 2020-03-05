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

using System.Buffers;

namespace DotPulsar.Internal.Extensions
{
    public static class MessagePackageExtensions
    {
        public static uint GetMetadataSize(this MessagePackage package)
            => package.Data.ReadUInt32(6, true);  //TODO RK: 6 should be a constant

        public static ReadOnlySequence<byte> ExtractData(this MessagePackage package, uint metadataSize)
            => package.Data.Slice(10 + metadataSize); //TODO RK: 10 should be a constant

        public static PulsarApi.MessageMetadata ExtractMetadata(this MessagePackage package, uint metadataSize)
            => Serializer.Deserialize<PulsarApi.MessageMetadata>(package.Data.Slice(10, metadataSize)); //TODO RK: 10 should be a constant

        public static bool IsValid(this MessagePackage package)
            => StartsWithMagicNumber(package.Data) && HasValidCheckSum(package.Data);

        private static bool StartsWithMagicNumber(ReadOnlySequence<byte> input)
            => input.StartsWith(Constants.MagicNumber);

        private static bool HasValidCheckSum(ReadOnlySequence<byte> input)
            => input.ReadUInt32(2, true) == Crc32C.Calculate(input.Slice(6));
    }
}