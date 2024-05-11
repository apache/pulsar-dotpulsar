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

using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;

public static class EncryptionFactories
{
    private static readonly List<IEncryptorFactory> _encryptorFactories;
    private static readonly List<IDecryptorFactory> _decryptorFactories;

    static EncryptionFactories()
    {
        _encryptorFactories = [];
        _decryptorFactories = [];

        LoadSupportForNull();
        LoadSupportForAes();
    }

    private static void LoadSupportForNull()
    {
        if (NullEncryption.TryLoading(out var encryptionFactory, out var decryptionFactory))
            Add(encryptionFactory, decryptionFactory);
    }

    private static void LoadSupportForAes()
    {
        if (AesEncryption.TryLoading(out var encryptorFactory, out var decryptorFactory))
            Add(encryptorFactory, decryptorFactory);
    }

    private static void Add(IEncryptorFactory? encryptorFactory, IDecryptorFactory? decryptorFactory)
    {
        _encryptorFactories.Add(encryptorFactory!);
        _decryptorFactories.Add(decryptorFactory!);
    }

    public static IEnumerable<IDecryptorFactory> DecryptorFactories()
        => _decryptorFactories;

    public static IEncryptorFactory GetEncryptorFactory(string encryptionAlgorith)
    {
        var encryptorFactory = _encryptorFactories.SingleOrDefault(f => f.EncryptionAlgo == encryptionAlgorith);
        if (encryptorFactory is null)
            throw new CryptoException($"Support for '{encryptionAlgorith}' encryption was not found");

        return encryptorFactory;
    }
}
