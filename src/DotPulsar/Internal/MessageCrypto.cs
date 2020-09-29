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

namespace DotPulsar.Internal
{
    using DotPulsar.Abstractions;
    using DotPulsar.Exceptions;
    using DotPulsar.Internal.Abstractions;
    using DotPulsar.Internal.PulsarApi;
    using Microsoft.Extensions.Caching.Memory;
    using NSec.Cryptography;
    using PemUtils;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    public sealed class MessageCrypto : IMessageCrypto
    {
        static readonly int IV_LEN = 12;
        private readonly ICryptoKeyReader _cryptoKeyReader;
        private Dictionary<string, EncryptionKeys>? _encryptedDataKeyMap = null;
        private MemoryCache _symmetricKeysCache = new MemoryCache(new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromHours(4) });
        private Nonce _nonce;
        private Key? _symmetricKey = null;
        public MessageCrypto(ICryptoKeyReader cryptoKeyReader)
        {
            _cryptoKeyReader = cryptoKeyReader;
        }

        private Key CreateSymmetricKey()
        {
            var createParam = new KeyCreationParameters()
            {
                ExportPolicy = KeyExportPolicies.AllowPlaintextExport
            };
            return new Key(AeadAlgorithm.Aes256Gcm, createParam);
        }

        private Nonce CreateNonce()
        {
            var fixPart = new ReadOnlySpan<byte>(RandomGenerator.Default.GenerateBytes(4));
            return new Nonce(fixPart, IV_LEN - fixPart.Length);
        }

        private RSAParameters LoadKey(byte[] keyBytes)
        {
            var rawStream = new MemoryStream(keyBytes);
            using var pemReader = new PemReader(rawStream);
            return pemReader.ReadRsaKey();
        }

        private void AddPublicKeyCipher(string keyName)
        {
            EncryptionKeyInfo keyInfo = _cryptoKeyReader.GetPublicKey(keyName);
            RSAParameters rsaPublicKey;
            try
            {
                rsaPublicKey = LoadKey(keyInfo.Key);
            }
            catch (Exception e)
            {
                throw new CryptoException($"Failed to get key {keyName}:{e.Message}");
            }
            try
            {
                using var rsa = RSA.Create();
                rsa.ImportParameters(rsaPublicKey);
                var encKey = rsa.Encrypt(_symmetricKey!.Export(KeyBlobFormat.RawSymmetricKey), RSAEncryptionPadding.OaepSHA1);
                EncryptionKeys key = new EncryptionKeys()
                {
                    Key = keyName,
                    Value = encKey
                };
                if (keyInfo.Metadata != null)
                {
                    foreach (var kv in keyInfo.Metadata)
                    {
                        key.Metadatas.Add(new KeyValue()
                        {
                            Key = kv.Key,
                            Value = kv.Value
                        });
                    }
                }
                _encryptedDataKeyMap![keyName] = key;
            }
            catch (Exception e)
            {
                throw new CryptoException($"Failed to encrypt data key {keyName}:{e.Message}");
            }
        }

        public void UpdatePublicKeyCipher(List<string> names)
        {
            _nonce = CreateNonce();
            _symmetricKey = CreateSymmetricKey();
            _encryptedDataKeyMap = new Dictionary<string, EncryptionKeys>();

            foreach (var keyName in names)
            {
                AddPublicKeyCipher(keyName);
            }
        }

        public byte[] Encrypt(byte[] payload, MessageMetadata messageMetadata)
        {
            if (_encryptedDataKeyMap!.Count == 0) return payload;
            foreach (var encKey in _encryptedDataKeyMap)
            {
                messageMetadata.EncryptionKeys.Add(encKey.Value);
            }
            try
            {
                _nonce++;
                var symmetricAlgo = AeadAlgorithm.Aes256Gcm;
                var encryptedPayload = symmetricAlgo.Encrypt(_symmetricKey!, _nonce, ReadOnlySpan<byte>.Empty, payload);
                messageMetadata.EncryptionParam = _nonce.ToArray();
                return encryptedPayload;
            }
            catch (Exception e)
            {
                throw new CryptoException($"Failed to encrypt message payload:{e.Message}");
            }

        }

        private byte[]? DecryptData(Key dataKey, MessageMetadata messageMetadata, byte[] payload)
        {
            try
            {
                var nonceBytes = messageMetadata.EncryptionParam;
                var nonce = new Nonce(nonceBytes, 0);
                var symmetricAlgo = AeadAlgorithm.Aes256Gcm;
                var decryptedPayload = symmetricAlgo.Decrypt(dataKey, nonce, ReadOnlySpan<byte>.Empty, payload, out byte[]? output);
                return output;
            }
            catch (Exception e)
            {
                throw new CryptoException($"Failed to decrypt message payload:{e.Message}");
            }
        }

        private byte[]? GetKeyFromCacheAndTryDecryptData(byte[] payload, MessageMetadata messageMetadata)
        {
            var encKeys = messageMetadata.EncryptionKeys;
            foreach (var encKey in encKeys)
            {
                bool result = _symmetricKeysCache.TryGetValue(Convert.ToBase64String(encKey.Value), out Key storedSymmetricKey);
                if (result && storedSymmetricKey != null)
                {
                    var decryptedData = DecryptData(storedSymmetricKey, messageMetadata, payload);
                    if (decryptedData != null) return decryptedData;
                }
            }
            return null;
        }

        private bool DecryptDataKey(string keyName, byte[] encryptedDataKey, List<KeyValue>? encKeyMetadata)
        {
            Dictionary<string, string>? keyMetadata = null;
            if (encKeyMetadata != null)
            {
                keyMetadata = new Dictionary<string, string>();
                foreach (var kv in encKeyMetadata)
                {
                    keyMetadata[kv.Key] = kv.Value;
                }
            }
            var keyInfo = _cryptoKeyReader.GetPrivateKey(keyName, keyMetadata);

            RSAParameters rsaPrivateKey;
            try
            {
                rsaPrivateKey = LoadKey(keyInfo.Key);
            }
            catch (Exception e)
            {
                throw new CryptoException($"Failed to load private key {keyName}:{e.Message}");
            }
            try
            {
                using var rsa = RSA.Create();
                rsa.ImportParameters(rsaPrivateKey);
                var decryptedKey = rsa.Decrypt(encryptedDataKey, RSAEncryptionPadding.OaepSHA1);
                _symmetricKey = Key.Import(AeadAlgorithm.Aes256Gcm, decryptedKey, KeyBlobFormat.RawSymmetricKey);
                _symmetricKeysCache.Set(Convert.ToBase64String(encryptedDataKey), _symmetricKey);
                return true;
            }
            catch (Exception e)
            {
                throw new CryptoException($"Failed to decrypt data key {keyName}:{e.Message}");
            }
        }

        public byte[]? Decrypt(byte[] payload, MessageMetadata messageMetadata)
        {
            if (_symmetricKey != null)
            {
                var decryptedData = GetKeyFromCacheAndTryDecryptData(payload, messageMetadata);
                if (decryptedData != null) return decryptedData;
            }

            // dataKey is null or decryption failed. Attempt to regenerate data key
            foreach (var encKey in messageMetadata.EncryptionKeys)
            {
                var encKeyBytes = encKey.Value;
                var encKeyMetadata = encKey.Metadatas;
                if (DecryptDataKey(encKey.Key, encKeyBytes, encKeyMetadata)) break;
            }
            var encKeyInfo = messageMetadata.EncryptionKeys.Where(encKey =>
            {
                var encKeyBytes = encKey.Value;
                var encKeyMetadata = encKey.Metadatas;
                return DecryptDataKey(encKey.Key, encKeyBytes, encKeyMetadata);
            }).FirstOrDefault();

            if (encKeyInfo == null || _symmetricKey == null)
            {
                // Fail to decrypt symmetric key.
                return null;
            }

            return GetKeyFromCacheAndTryDecryptData(payload, messageMetadata);
        }
    }
}
