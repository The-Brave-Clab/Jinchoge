using System;
using System.Text;

namespace biscuit.Data.Crypt;

public class AESCrypt : ICrypt
{
    public AESCrypt(byte[] key, byte[] iv)
    {
        SetKey(key, iv);
    }

    public AESCrypt(string key, byte[] iv)
    {
        SetKey(key, iv);
    }

    ~AESCrypt()
    {
        Dispose();
    }

    public void SetKey(string key, byte[]? iv)
    {
        optionalKey = key;
        if (iv != null)
        {
            optionalIV = Encoding.UTF8.GetString(iv);
        }
    }

    public void SetKey(byte[] key, byte[]? iv)
    {
        throw new NotImplementedException("到達想定外コード");
    }

    public byte[]? Encode(byte[] target)
    {
        byte[]? result;
        if (optionalKeySize == KEY_SIZE_256)
        {
            AES.EncryptAes256Cbc(target, optionalKey, optionalIV, out result);
        }
        else
        {
            AES.EncryptAes128Cbc(target, optionalKey, out optionalIV, out result);
        }

        return result;
    }

    public byte[]? Decode(byte[] target)
    {
        return optionalKeySize == KEY_SIZE_256
            ? AES.DecryptAes256Cbc(target, optionalKey, optionalIV)
            : AES.DecryptAes128Cbc(target, optionalKey, optionalIV);
    }

    public byte[] ToLHB(byte[] target)
    {
        throw new NotImplementedException("到達想定外コード");
    }

    public byte[] FromLFB(byte[] target)
    {
        throw new NotImplementedException("到達想定外コード");
    }

    public void Dispose()
    {
    }

    public static int KEY_SIZE_128 = 128;
    public static int KEY_SIZE_256 = 256;
    public string? optionalIV;
    public string? optionalKey;
    public int optionalKeySize = KEY_SIZE_256;
}