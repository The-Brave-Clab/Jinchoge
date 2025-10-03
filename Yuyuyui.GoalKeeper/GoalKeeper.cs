using System;
using System.Text;

namespace Yuyuyui.GK;

public class GoalKeeper : ILibGK
{
    private Y3GK y3gk;

    public GoalKeeper()
    {
        y3gk = new Y3GK();
        y3gk.Initialize();
    }
    
    public byte[] EncryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false)
    {
        if (string.IsNullOrEmpty(key) || !sessionKey)
            y3gk.ResetKey();
        else
            y3gk.SetKey(key, sessionKey);
        return y3gk.EncodeODF(inputData) ?? [];
    }

    public byte[] DecryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false)
    {
        if (string.IsNullOrEmpty(key) || !sessionKey)
            y3gk.ResetKey();
        else
            y3gk.SetKey(key, sessionKey);
        return y3gk.DecodeODF(inputData) ?? [];
    }

    public byte[] EncryptBin(byte[] inputData, string key = "", byte[]? iv = null)
    {
        return y3gk.EncodeLHB(inputData) ?? [];
    }

    public byte[] DecryptBin(byte[] inputData, string key = "", byte[]? iv = null)
    {
        return y3gk.DecodeLHB(inputData) ?? [];
    }
}