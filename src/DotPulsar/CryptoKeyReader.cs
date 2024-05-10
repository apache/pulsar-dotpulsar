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

namespace DotPulsar;

public interface CryptoKeyReader
{
    /// <summary>
    /// Return the encryption (public) key corresponding to the key name in the argument.
    /// <p>
    /// This method should be implemented to return the key in byte array. This method will be
    /// called at the time of producer creation as well as consumer receiving messages.
    /// Hence, application should not make any blocking calls within the implementation.
    /// </p>
    /// </summary>
    /// <param name="keyName">Unique name to identify the key</param>
    /// <returns>byte array of the public key value</returns>
    byte[] GetPublicKey(String keyName);

    /// <summary>
    /// Return the decryption (private) key corresponding to the key name in the argument.
    /// </summary>
    /// <param name="keyName">Unique name to identify the key</param>
    /// <returns>byte array of the private key value</returns>
    byte[] GetPrivateKey(String keyName);
}
