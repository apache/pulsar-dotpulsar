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
        {
            var offset = Constants.MagicNumber.Length + Constants.ChecksumLengthInBytes;
            return package.Data.ReadUInt32(offset, true);
        }

        public static ReadOnlySequence<byte> ExtractData(this MessagePackage package, uint metadataSize)
        {
            var offset = Constants.MagicNumber.Length + Constants.ChecksumLengthInBytes + Constants.MetadataSizeLengthInBytes + metadataSize;
            return package.Data.Slice(offset);
        }

        public static PulsarApi.MessageMetadata ExtractMetadata(this MessagePackage package, uint metadataSize)
        {
            var offset = Constants.MagicNumber.Length + Constants.ChecksumLengthInBytes + Constants.MetadataSizeLengthInBytes;
            return Serializer.Deserialize<PulsarApi.MessageMetadata>(package.Data.Slice(offset, metadataSize));
        }

        public static bool IsValid(this MessagePackage package)
            => StartsWithMagicNumber(package.Data) && HasValidCheckSum(package.Data);

        private static bool StartsWithMagicNumber(ReadOnlySequence<byte> input)
            => input.StartsWith(Constants.MagicNumber);

        private static bool HasValidCheckSum(ReadOnlySequence<byte> input)
        {
            var providedChecksum = input.ReadUInt32(Constants.MagicNumber.Length, true);
            var calculatedChecksum = Crc32C.Calculate(input.Slice(Constants.MagicNumber.Length + Constants.ChecksumLengthInBytes));
            return providedChecksum == calculatedChecksum;
        }
    }
}
