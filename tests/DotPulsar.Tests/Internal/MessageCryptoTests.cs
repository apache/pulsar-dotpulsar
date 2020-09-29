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

namespace DotPulsar.Tests.Internal
{
    using DotPulsar.Abstractions;
    using DotPulsar.Internal;
    using DotPulsar.Internal.Abstractions;
    using Moq;
    using System.Collections.Generic;
    using System.Text;
    using Xunit;

    public class MessageCryptoTests
    {
        static readonly string PublicKeyInfo = @"-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC/xmFrob/LU4xDHpQZR1Yk8PuJ
dZUPZFuZZIsS+IfZD2TrEHIG2ie0Mof05yDor9cJqy8TmfxMggY/KQNaCLW+Acpm
znS+7uQ2FD9AXZ5beyv+wcGAGGFsGFDOluepoe1ljTmtb1rjxY69R+AiNAJdr0KM
mHymC9eqBKD1t1i86wIDAQAB
-----END PUBLIC KEY-----";
        static readonly string PrivateKeyInfo = @"-----BEGIN RSA PRIVATE KEY-----
MIICWwIBAAKBgQC/xmFrob/LU4xDHpQZR1Yk8PuJdZUPZFuZZIsS+IfZD2TrEHIG
2ie0Mof05yDor9cJqy8TmfxMggY/KQNaCLW+AcpmznS+7uQ2FD9AXZ5beyv+wcGA
GGFsGFDOluepoe1ljTmtb1rjxY69R+AiNAJdr0KMmHymC9eqBKD1t1i86wIDAQAB
AoGAIu1ekNvEsqNkyFSpZHE5n0DEjyR7IXKFvEoziiD5nO7Q0n8MRXM2B/usB06R
D8/2uiwTRt6ktMp5mMc/dQZhEwlKxwbFEkg+XUKU+lAEghVWpiOwTTaALveIjgKE
eCY4WDJJjL1lU1WTUqOb8LDHofm2wSfQbyU5RD3/BszEXgECQQDd/kLJ9A3qbhiV
1IKGDCYcqpe45RI3Y0+256t+WBzSrv3HqO6vc3PLgl+SumpFoUe7S1QchGaz/nrm
EsZGUukZAkEA3ScSe6oFJmuzx3Z5yd6wWosbqPrzkRdjP/ZmzgqEWCfKX/OY2kiJ
eu9dNzyeQ6YApeFeQ1BKi9QtP23TOIEiowJAFgVT0L6x5rBXJf23mN55pVxSwpeO
kAn87VLb0yOgcFHFgNnEG4ljUiuzmVV+lzuhZvXY+R81JOO4gzwXiQBOeQJAA7Oo
uosxBOCepMMV7MwedZWIg/6XXyFeFu7/74j7iCI6X/rK3zSBoJ4rGEaae5Vmw2AP
XN8WMFr/2uTyuSpoMwJAHZiK8sLhEExsABriFpvfXG8ktlJ+ix4Y6dy3EojYtgFW
1ozWTYzUUIq5IEcPC7oqR+5FYSC42L8WRrkXaPpHcw==
-----END RSA PRIVATE KEY-----";
        [Fact]
        public void SimpleEncryptionAndDecryption()
        {
            var cryptoReaderMock = new Mock<ICryptoKeyReader>();
            cryptoReaderMock.Setup(m => m.GetPublicKey("key1", null)).Returns(new EncryptionKeyInfo(Encoding.UTF8.GetBytes(PublicKeyInfo), null));
            cryptoReaderMock.Setup(m => m.GetPrivateKey("key1", It.IsAny<Dictionary<string, string>>())).Returns(new EncryptionKeyInfo(Encoding.UTF8.GetBytes(PrivateKeyInfo), null));
            var cryptoReader = cryptoReaderMock.Object;
            IMessageCrypto messageCryptoProducer = new MessageCrypto(cryptoReader);
            IMessageCrypto messageCryptoConsumer = new MessageCrypto(cryptoReader);
            messageCryptoProducer.UpdatePublicKeyCipher(new List<string>(new string[] { "key1" }));

            var testData = "Test data";
            byte[] payload = Encoding.UTF8.GetBytes(testData);
            DotPulsar.Internal.PulsarApi.MessageMetadata messageMetadata = new DotPulsar.Internal.PulsarApi.MessageMetadata();

            var encryptedPayload = messageCryptoProducer.Encrypt(payload, messageMetadata);
            var decryptedPayload = messageCryptoConsumer.Decrypt(encryptedPayload, messageMetadata);

            Assert.NotNull(decryptedPayload);
            var decryptedData = Encoding.UTF8.GetString(decryptedPayload!);

            Assert.Equal(testData, decryptedData);
        }
    }
}
