using System;
using System.Threading;

namespace Yuyuyui.GK;

public enum CryptType
{
    Binary,
    API
}

public enum CryptDirection
{
    Decrypt,
    Encrypt
};

public interface ILibGK
{
    byte[] EncryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false);
    byte[] DecryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false);
    byte[] EncryptBin(byte[] inputData, string key = "", byte[]? iv = null);
    byte[] DecryptBin(byte[] inputData, string key = "", byte[]? iv = null);
}

public static class LibGK<TImpl> where TImpl : class, ILibGK, new()
{
    private static ThreadLocal<TImpl> impl = new(() => new TImpl());

    public static byte[] Execute(CryptType type, CryptDirection direction,
        byte[] inputData,
        string key = "", byte[]? iv = null, bool sessionKey = false)
    {
        switch (type)
        {
            case CryptType.Binary when direction == CryptDirection.Encrypt:
                return impl.Value!.EncryptBin(inputData, key, iv);
            case CryptType.Binary when direction == CryptDirection.Decrypt:
                return impl.Value!.DecryptBin(inputData, key, iv);
            case CryptType.API when direction == CryptDirection.Encrypt:
                return impl.Value!.EncryptApi(inputData, key, iv, sessionKey);
            case CryptType.API when direction == CryptDirection.Decrypt:
                return impl.Value!.DecryptApi(inputData, key, iv, sessionKey);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}