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

namespace DotPulsar.Internal.Requests;

using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System.Diagnostics.CodeAnalysis;

public readonly struct StandardRequest : IRequest
{
    private readonly ulong _requestId;
    private readonly ulong? _consumerId;
    private readonly ulong? _producerId;
    private readonly BaseCommand.Type? _commandType;

    private StandardRequest(ulong requestId, ulong? consumerId, ulong? producerId, BaseCommand.Type? commandType)
    {
        _requestId = requestId;
        _consumerId = consumerId;
        _producerId = producerId;
        _commandType = commandType;
    }

    public static StandardRequest WithRequestId(ulong requestId)
        => new(requestId, null, null, null);

    public static StandardRequest WithConsumerId(ulong requestId, ulong consumerId, BaseCommand.Type? commandType = null)
        => new(requestId, consumerId, null, commandType);

    public static StandardRequest WithProducerId(ulong requestId, ulong producerId, BaseCommand.Type? commandType = null)
        => new(requestId, null, producerId, commandType);

    public bool SenderIsConsumer(ulong consumerId)
        => _consumerId.HasValue && _consumerId.Value == consumerId;

    public bool SenderIsProducer(ulong producerId)
        => _producerId.HasValue && _producerId.Value == producerId;

    public bool IsCommandType(BaseCommand.Type commandType)
        => _commandType.HasValue && _commandType.Value == commandType;

#if NETSTANDARD2_0
    public bool Equals(IRequest other)
#else
    public bool Equals([AllowNull] IRequest other)
#endif
    {
        if (other is StandardRequest request)
            return _requestId.Equals(request._requestId);

        return false;
    }

    public override int GetHashCode()
        => HashCode.Combine(_requestId);
}
