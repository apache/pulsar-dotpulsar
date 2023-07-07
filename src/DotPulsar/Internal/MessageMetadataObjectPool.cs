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

namespace DotPulsar.Internal;

using Microsoft.Extensions.ObjectPool;

public static class MessageMetadataObjectPool
{
    private static readonly ObjectPool<MessageMetadata> _messageMetadataPool;

    static MessageMetadataObjectPool()
    {
        var messageMetadataPolicy = new DefaultPooledObjectPolicy<MessageMetadata>();
        _messageMetadataPool = new DefaultObjectPool<MessageMetadata>(messageMetadataPolicy);
    }

    public static MessageMetadata Get()
    {
        return _messageMetadataPool.Get();
    }

    public static void Return(MessageMetadata metadata)
    {
        metadata.SequenceId = 0;
        metadata.Metadata.Properties.Clear();
        _messageMetadataPool.Return(metadata);
    }
}
