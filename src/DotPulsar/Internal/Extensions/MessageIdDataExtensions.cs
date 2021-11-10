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

namespace DotPulsar.Internal.Extensions;

using DotPulsar.Internal.PulsarApi;

public static class MessageIdDataExtensions
{
    public static MessageId ToMessageId(this MessageIdData messageIdData)
        => new(messageIdData.LedgerId, messageIdData.EntryId, messageIdData.Partition, messageIdData.BatchIndex);

    public static void MapFrom(this MessageIdData destination, MessageId source)
    {
        destination.LedgerId = source.LedgerId;
        destination.EntryId = source.EntryId;
        destination.Partition = source.Partition;
        destination.BatchIndex = source.BatchIndex;
    }
}
