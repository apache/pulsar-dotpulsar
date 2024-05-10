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

/// <summary>
/// Possible actions that the consumer can take when a message is encrypted and the consumer fails to decrypt the message.
/// </summary>
public enum ConsumerCryptoFailureAction
{
    /// <summary>
    /// Message is not acknowledged and the operation fails.
    /// </summary>
    Fail = 0,

    /// <summary>
    /// Message is silently acknowledged and not delivered to the application.
    /// </summary>
    Discard = 1,

    /// <summary>
    /// Deliver the encrypted message to the application. It's the application's
    /// responsibility to decrypt the message. If message is also compressed,
    /// decompression will fail. If message contain batch messages, client will
    /// not be able to retrieve individual messages in the batch.
    /// </summary>
    Consume = 2
}
