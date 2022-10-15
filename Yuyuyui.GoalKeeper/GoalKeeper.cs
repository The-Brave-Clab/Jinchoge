using System;
using System.Text;

namespace Yuyuyui.GK;

public class GoalKeeper : ILibGK
{
    private void NormalizeKey(CryptType type, ref string key, ref byte[]? iv)
    {
        if (type == CryptType.Binary)
        {
            if (string.IsNullOrEmpty(key)) key = Consts.GK_BIN_DEFAULT_KEY;
            if (iv == null || iv.Length == 0) iv = Consts.GK_BIN_DEFAULT_IV;
        }
        else
        {
            if (string.IsNullOrEmpty(key)) key = Consts.GK_API_DEFAULT_KEY;
        }
    }
    
    public byte[] EncryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false)
    {
        NormalizeKey(CryptType.API, ref key, ref iv);
        Igarashi.Context igarashiCtx = Igarashi.Context.InitialContext();
        Igarashi.ExpandKey(ref igarashiCtx, Encoding.UTF8.GetBytes(key));
        Igarashi.OdfEncode(ref igarashiCtx, inputData, out var encryptedBytes, 0x2000000);
        return encryptedBytes;
    }

    public byte[] DecryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false)
    {
        NormalizeKey(CryptType.API, ref key, ref iv);
        Igarashi.Context igarashiCtx = Igarashi.Context.InitialContext();
        Igarashi.ExpandKey(ref igarashiCtx, Encoding.UTF8.GetBytes(key));
        Igarashi.OdfDecode(ref igarashiCtx, inputData, out var decryptedBytes);
        return decryptedBytes;
    }

    public byte[] EncryptBin(byte[] inputData, string key = "", byte[]? iv = null)
    {
        throw new NotImplementedException();
    }

    public byte[] DecryptBin(byte[] inputData, string key = "", byte[]? iv = null)
    {
        throw new NotImplementedException();
    }
}