namespace DotPulsar.Internal.Abstractions;

using DotPulsar.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System.Buffers;

public interface IMessageCrypto
{
    ReadOnlySequence<byte> Decrypt(ReadOnlySequence<byte> data);

    (ReadOnlySequence<byte>, byte[], List<EncryptionKeys>) Encrypt(List<string> encryptionKeyNames, ICryptoKeyReader cryptoKeyReader, ReadOnlySequence<byte> data);
}
