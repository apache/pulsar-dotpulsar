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

/// <summary>
/// Null encryptor that does not encrypt or decrypt data.
/// </summary>
public class NullEncryptor
{
    public static bool TryLoading(out IEncryptorFactory? encryptorFactory, out IDecryptorFactory? decryptorFactory)
    {
        decryptorFactory = new DecryptorFactory(string.Empty, () => new NullEncryption());
        encryptorFactory = new EncryptorFactory(string.Empty, () => new NullEncryption());

        return EncryptionTester.TestEncryption(encryptorFactory, decryptorFactory);
    }
}
