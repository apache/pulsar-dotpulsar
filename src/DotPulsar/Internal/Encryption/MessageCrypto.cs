namespace DotPulsar.Internal.Encryption;

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System.Buffers;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

/// <summary>
/// To learn more about  the underlying crypto details, you can read:
/// AWS KMS cryptographic details: https://docs.aws.amazon.com/kms/latest/cryptographic-details/intro.html
/// Practical Challenges with AES-GCM and the need for a new cipher: https://csrc.nist.gov/csrc/media/Events/2023/third-workshop-on-block-cipher-modes-of-operation/documents/accepted-papers/Practical%20Challenges%20with%20AES-GCM.pdf
/// Key Management Systems at the Cloud Scale: https://www.mdpi.com/2410-387X/3/3/23
/// Why NIST recommends avoiding more than 2^32 encryptions (https://nvlpubs.nist.gov/nistpubs/Legacy/SP/nistspecialpublication800-38d.pdf)
/// </summary>
public class MessageCrypto : IMessageCrypto
{
    private const int AesGcmNonceByteMaxSize = 12;
    private const int AesGcmTagByteMaxSize = 16;
    private const int AesGcmKeyByteMaxSize = 32;

    private readonly ConcurrentDictionary<string, EncryptionKeyInfo> _encryptedSessionKeys = new();

    private readonly byte[] _dataKey = new byte[AesGcmKeyByteMaxSize];

    private readonly byte[] _iv = new byte[AesGcmNonceByteMaxSize];

    public List<EncryptionKeys> EncryptionKeys { get; } = new();

    public MessageCrypto()
    {
        var randomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(_dataKey);
    }

    public ReadOnlySequence<byte> Decrypt(ReadOnlySequence<byte> data) =>
        throw new NotImplementedException();

    public (ReadOnlySequence<byte>, byte[], List<EncryptionKeys>) Encrypt(List<string> encryptionKeyNames, ICryptoKeyReader cryptoKeyReader, ReadOnlySequence<byte> data)
    {
        if (!encryptionKeyNames.Any())
        {
            return (data, _iv, EncryptionKeys);
        }

        List<EncryptionKeys> encryptionKeys = new();
        // Update message metadata with encrypted data key
        foreach (var encryptionKeyName in encryptionKeyNames)
        {
            if (!_encryptedSessionKeys.ContainsKey(encryptionKeyName))
            {
                // Attempt to load the key. This will allow us to load keys as soon as
                // a new key is added to producer config
                AddPublicKeyCipher(encryptionKeyName, cryptoKeyReader);
            }

            if (_encryptedSessionKeys.TryGetValue(encryptionKeyName, out var encryptedSessionKey))
            {
                var key = new EncryptionKeys
                {
                    Key = encryptionKeyName,
                    Value = encryptedSessionKey.Key,
                };

                if (encryptedSessionKey.Metadata != null && encryptedSessionKey.Metadata.Any())
                {
                    key.Metadatas.AddRange(encryptedSessionKey.Metadata.Select(keyValue => new KeyValue { Key = keyValue.Key, Value = keyValue.Value }));
                }

                encryptionKeys.Add(key);
            }
        }

        // Create GCM param - IV or Nonce
        // TODO: Replace random with counter and periodic refreshing based on timer/counter value
        var randomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(_iv);

        // Initialize the byte arrays for the ciphertext and tag returned by the encryption operation.
        var cipherText = new byte[data.Length];
        var tag = new byte[AesGcmTagByteMaxSize];

        try
        {
            // Encrypt the data.
            using var aes = new AesGcm(_dataKey, tag.Length);
            aes.Encrypt(_iv, data.ToArray(), cipherText, tag);
        }
        catch (Exception e)
        {
            throw new CryptoException(e.Message, e);
        }

        return (new ReadOnlySequence<byte>(cipherText), _iv, encryptionKeys);
    }

    private void AddPublicKeyCipher(List<string> encryptionKeyNames, ICryptoKeyReader cryptoKeyReader)
    {
        // Generate data key
        var randomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(_dataKey);

        foreach (var encryptionKeyName in encryptionKeyNames)
        {
            AddPublicKeyCipher(encryptionKeyName, cryptoKeyReader);
        }
    }

    private void AddPublicKeyCipher(string encryptionKeyName, ICryptoKeyReader cryptoKeyReader)
    {
        if (encryptionKeyName == null || cryptoKeyReader == null)
        {
            throw new CryptoException("KeyName or KeyReader is null");
        }

        // Read the public key and its info using callback
        var keyInfo = cryptoKeyReader.GetPublicKey(encryptionKeyName);

        PublicKey pubKey;
        try
        {
            pubKey = LoadPublicKey(keyInfo.Key);
        }
        catch (Exception e)
        {
            throw new CryptoException($"Failed to load public key {encryptionKeyName}", e);
        }

        byte[] encryptedKey;
        try
        {
            // Encrypt data key using public key
            var rsa = RSA.Create();
            rsa.ImportParameters(pubKey.Key);
            encryptedKey = rsa.Encrypt(_dataKey, RSAEncryptionPadding.OaepSHA256);
        }
        catch (Exception e)
        {
            throw new CryptoException(e.Message, e);
        }

        var eki = new EncryptionKeyInfo(encryptedKey, keyInfo.Metadata);
        _encryptedSessionKeys.AddOrUpdate(encryptionKeyName, eki);
    }

    private PublicKey LoadPublicKey(byte[] key)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(Encoding.UTF8.GetString(key));
        // rsa.ImportParameters(new RSAParameters());
        return rsa;
    }
}

public class EncryptionKeyInfo(byte[] encryptedKey, Dictionary<string, string>? keyInfoMetadata)
{
    public byte[] Key { get; } = encryptedKey;

    public Dictionary<string, string>? Metadata { get; } = keyInfoMetadata;
}
