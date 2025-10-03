using System;
using System.Text;
using Gk;

namespace biscuit.Data.Crypt;

public class GK : ICrypt
{
    public GK(byte[] key, byte[]? iv)
    {
        SetKey(key, iv);
    }

    public GK(string key, byte[]? iv)
    {
        SetKey(key, iv);
    }

    ~GK()
    {
        Dispose();
    }

    public void SetKey(string key, byte[]? iv)
    {
        SetKey(Encoding.UTF8.GetBytes(key), iv);
    }

    public void SetKey(byte[] key, byte[]? iv)
    {
        if (_igarashi != null)
        {
            _igarashi.Dispose();
            _igarashi = null;
        }

        _igarashi = new Igarashi(key, KEY_REQUEST.IGARASHI);
        if (iv != null)
        {
            _igarashi.IV = iv;
        }

        if (_odf != null)
        {
            _odf.SetKey(_igarashi);
        }
        else
        {
            _odf = new ODF(_igarashi);
        }

        if (_lhb != null)
        {
            var igarashi = new Igarashi(LHBParam.YUYUYU.key, (KEY_REQUEST)3221225472U);
            igarashi.IV = LHBParam.YUYUYU.iv;
            _lhb.SetKey(igarashi);
        }
        else
        {
            _lhb = new LHB(new Igarashi(LHBParam.YUYUYU.key, (KEY_REQUEST)3221225472U) { IV = LHBParam.YUYUYU.iv });
        }
    }

    public byte[] Encode(byte[] target)
    {
        if (_odf == null)
        {
            throw new OyoyoException();
        }

        var result = _odf.Encode(target, ENCODE.IGARASHI);
        return result.Data;
    }

    public byte[] Decode(byte[] target)
    {
        if (_odf == null)
        {
            throw new OyoyoException();
        }

        var result = _odf.Decode(target);
        return result.Data;
    }

    public byte[]? ToLHB(byte[] target)
    {
        var result = _lhb?.Encode(ref target, (ENCODE)2164260864U);
        return result?.Data;
    }

    public byte[]? FromLFB(byte[] target)
    {
        var result = _lhb?.Decode(ref target, false);
        return result?.Data;
    }

    public void Dispose()
    {
        _igarashi?.Dispose();
        _igarashi = null;
        _odf?.Dispose();
        _odf = null;
        _lhb?.Dispose();
        _lhb = null;
    }

    private Igarashi? _igarashi;
    private ODF? _odf;
    private LHB? _lhb;

    public class OyoyoException() : Exception("ODF not found.");
}