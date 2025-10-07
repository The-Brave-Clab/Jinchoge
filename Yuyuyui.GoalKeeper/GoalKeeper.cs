using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yuyuyui.GK;

public class GoalKeeper : ILibGK
{
    private ThreadLocal<Y3GK> y3gk;

    public GoalKeeper()
    {
        y3gk = new(() =>
        {
            var result = new Y3GK();
            result.Initialize();
            return result;
        });
    }
    
    public Task<byte[]> EncryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false)
    {
        return Task.Run(() =>
        {
            if (string.IsNullOrEmpty(key) || !sessionKey)
                y3gk.Value!.ResetKey();
            else
                y3gk.Value!.SetKey(key, sessionKey);
            return y3gk.Value.EncodeODF(inputData) ?? [];
        });
    }

    public Task<byte[]> DecryptApi(byte[] inputData, string key = "", byte[]? iv = null, bool sessionKey = false)
    {
        return Task.Run(() =>
        {
            if (string.IsNullOrEmpty(key) || !sessionKey)
                y3gk.Value!.ResetKey();
            else
                y3gk.Value!.SetKey(key, sessionKey);
            return y3gk.Value.DecodeODF(inputData) ?? [];
        });
    }

    public Task<byte[]> EncryptBin(byte[] inputData, string key = "", byte[]? iv = null)
    {
        return Task.Run(() => y3gk.Value!.EncodeLHB(inputData) ?? []);
    }

    public Task<byte[]> DecryptBin(byte[] inputData, string key = "", byte[]? iv = null)
    {
        return Task.Run(() => y3gk.Value!.DecodeLHB(inputData) ?? []);
    }
}