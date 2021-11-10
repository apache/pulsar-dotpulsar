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

namespace DotPulsar.Internal.Compression;

using DotPulsar.Internal.Abstractions;
using System;
using System.Buffers;

public sealed class Decompressor : IDecompress
{
    private readonly IDisposable? _disposable;
    private readonly Func<ReadOnlySequence<byte>, int, ReadOnlySequence<byte>> _decompress;

    public Decompressor(Func<ReadOnlySequence<byte>, int, ReadOnlySequence<byte>> decompress, IDisposable? disposable = null)
    {
        _disposable = disposable;
        _decompress = decompress;
    }

    public ReadOnlySequence<byte> Decompress(ReadOnlySequence<byte> data, int decompressedSize)
        => _decompress(data, decompressedSize);

    public void Dispose()
        => _disposable?.Dispose();
}
