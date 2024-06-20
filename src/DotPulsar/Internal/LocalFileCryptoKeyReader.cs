namespace DotPulsar.Internal;

using DotPulsar.Abstractions;
using DotPulsar.Internal.Encryption;
using DotPulsar.Internal.PulsarApi;

public class LocalFileCryptoKeyReader : ICryptoKeyReader
{
    public EncryptionKeyInfo GetPublicKey(string keyName) =>
        throw new NotImplementedException();

    public (byte[], List<KeyValue>) GetPrivateKey(string keyName) =>
        throw new NotImplementedException();
}
