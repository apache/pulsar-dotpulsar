namespace DotPulsar.Internal;

using DotPulsar.Abstractions;

public class LocalFileCryptoKeyReader : ICryptoKeyReader
{
    public byte[] GetPublicKey(string keyName) =>
        throw new NotImplementedException();

    public byte[] GetPrivateKey(string keyName) =>
        throw new NotImplementedException();
}
