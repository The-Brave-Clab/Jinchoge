using System;
using System.Threading;
using System.Threading.Tasks;

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
    Task<byte[]> EncryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false);
    Task<byte[]> DecryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false);
    Task<byte[]> EncryptBin(byte[] inputData, string key = "", byte[]? iv = null);
    Task<byte[]> DecryptBin(byte[] inputData, string key = "", byte[]? iv = null);
}

public static class LibGK<TImpl> where TImpl : ILibGK, new()
{
    private static ThreadLocal<TImpl> impl = new(() => new TImpl());

    public static async Task<byte[]> Execute(CryptType type, CryptDirection direction,
        byte[] inputData,
        string key = "", byte[]? iv = null, bool sessionKey = false)
    {
        switch (type)
        {
            case CryptType.Binary when direction == CryptDirection.Encrypt:
                return await impl.Value!.EncryptBin(inputData, key, iv);
            case CryptType.Binary when direction == CryptDirection.Decrypt:
                return await impl.Value!.DecryptBin(inputData, key, iv);
            case CryptType.API when direction == CryptDirection.Encrypt:
                return await impl.Value!.EncryptApi(inputData, key, iv, sessionKey);
            case CryptType.API when direction == CryptDirection.Decrypt:
                return await impl.Value!.DecryptApi(inputData, key, iv, sessionKey);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}