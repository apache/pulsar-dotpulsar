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

using Compression.Messages;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Buffers;
using System.Text;

public static class MessageBytes
{
    static MessageBytes()
    {
        var smallMessage = CreateMessage(1024); //1KB
        SmallProtobufMessageBytes = new ReadOnlySequence<byte>(smallMessage.ToByteArray());
        SmallJsonMessageBytes = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(JsonFormatter.Default.Format(smallMessage)));

        var largeMessage = CreateMessage(1024*1024); //1MB
        LargeProtobufMessageBytes = new ReadOnlySequence<byte>(largeMessage.ToByteArray());
        LargeJsonMessageBytes = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(JsonFormatter.Default.Format(largeMessage)));
    }

    public static ReadOnlySequence<byte> SmallProtobufMessageBytes { get; }
    public static ReadOnlySequence<byte> LargeProtobufMessageBytes { get; }
    public static ReadOnlySequence<byte> SmallJsonMessageBytes { get; }
    public static ReadOnlySequence<byte> LargeJsonMessageBytes { get; }

    public static ReadOnlySequence<byte> GetBytes(MessageSize messageSize, MessageType messageType)
    {
        if (messageSize == MessageSize.Small && messageType == MessageType.Protobuf)
            return SmallProtobufMessageBytes;
        if (messageSize == MessageSize.Large && messageType == MessageType.Protobuf)
            return LargeProtobufMessageBytes;
        if (messageSize == MessageSize.Small && messageType == MessageType.Json)
            return SmallJsonMessageBytes;
        return LargeJsonMessageBytes;
    }

    private static Message CreateMessage(int minimumByteSize)
    {
        var message = new Message()
        {
            Pi = 3.14159265359,
            IsTrue = true,
            OneHour = TimeSpan.FromHours(1).ToDuration(),
            Now = DateTime.UtcNow.ToTimestamp(),
        };

        while (message.CalculateSize() < minimumByteSize)
        {
            var innerMessage = new InnerMessage
            {
                MyEnum = InnerMessage.Types.Enum.Five,
                NewGuid = Guid.NewGuid().ToString("N"),
                RandomNumber = Random.Shared.Next()
            };

            message.InnerMessages.Add(innerMessage);
        }

        return message;
    }
}
