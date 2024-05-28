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

namespace DotPulsar.Internal.Encryption;

using DotPulsar.Internal.Abstractions;
using System.Buffers;

/// <summary>
/// Null encryptor that does not encrypt or decrypt data.
/// </summary>
public class NullEncryptor(IDisposable? disposable = null) : IEncryptor, IDecryptor
{
    public void Dispose()
        => disposable?.Dispose();

    public ReadOnlySequence<byte> Decrypt(ReadOnlySequence<byte> data) =>
        data;

    public ReadOnlySequence<byte> Encrypt(ReadOnlySequence<byte> data) =>
        data;
}
