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
using System.Buffers;

public sealed class Compressor : ICompress
{
    private readonly IDisposable? _disposable;
    private readonly Func<ReadOnlySequence<byte>, ReadOnlySequence<byte>> _compress;

    public Compressor(Func<ReadOnlySequence<byte>, ReadOnlySequence<byte>> compress, IDisposable? disposable = null)
    {
        _disposable = disposable;
        _compress = compress;
    }

    public Compressor(Func<ReadOnlySequence<byte>, ReadOnlySequence<byte>> compress)
        => _compress = compress;

    public ReadOnlySequence<byte> Compress(ReadOnlySequence<byte> data)
        => _compress(data);

    public void Dispose()
        => _disposable?.Dispose();
}
