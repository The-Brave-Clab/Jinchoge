using System;
using biscuit.Data.Crypt;
using Gk;

public class Y3GK : IDisposable
{
    static Y3GK()
    {
        NativeResolver.EnsureRegistered();
    }

    public ICrypt? GetApiCrypter()
    {
        return api_crypter;
    }

    public ICrypt? GetBinaryCrypter()
    {
        return binary_crypter;
    }

    public void Initialize()
    {
        Igarashi.WakeUp();
        api_crypter ??= CryptFactory.Create("db9a0d951de48825", null);
        if (binary_crypter != null) return;
        var iv = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        var text = "8d49d9db4439e344";
        binary_crypter = CryptFactory.Create(text, iv);
    }

    public void ResetKey()
    {
        sessionEnable = false;
        SetKey("db9a0d951de48825");
    }

    public void SetKey(string new_key, bool session_key = false)
    {
        if (session_key)
        {
            sessionEnable = true;
            api_crypter!.SetKey(new_key, null);
            key = new_key;
        }
        else if (!sessionEnable)
        {
            api_crypter!.SetKey(new_key, null);
            key = new_key;
        }
    }

    public byte[]? DecodeODF(byte[] target)
    {
        return api_crypter == null ? target : api_crypter.Decode(target);
    }

    public byte[]? EncodeODF(byte[] target)
    {
        return api_crypter == null ? target : api_crypter.Encode(target);
    }

    public byte[]? DecodeLHB(byte[] target)
    {
        return binary_crypter == null ? target : binary_crypter.FromLFB(target);
    }

    public byte[]? EncodeLHB(byte[] target)
    {
        return binary_crypter == null ? target : binary_crypter.ToLHB(target);
    }

    public const string gk_public_key = "db9a0d951de48825";
    private bool sessionEnable;
    private string key = "db9a0d951de48825";
    private ICrypt? api_crypter;
    private ICrypt? binary_crypter;

    public void Dispose()
    {
        api_crypter?.Dispose();
        binary_crypter?.Dispose();
    }
}