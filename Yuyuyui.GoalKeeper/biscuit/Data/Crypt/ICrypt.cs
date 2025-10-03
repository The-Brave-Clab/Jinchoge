using System;

namespace biscuit.Data.Crypt;

public interface ICrypt : IDisposable
{
    void SetKey(string key, byte[]? iv);
    void SetKey(byte[] key, byte[]? iv);
    byte[]? Encode(byte[] target);
    byte[]? Decode(byte[] target);
    byte[]? ToLHB(byte[] target);
    byte[]? FromLFB(byte[] target);
}