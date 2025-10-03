using System;
using System.Runtime.InteropServices;

namespace Gk;

public class ODF : IDisposable
{
    public ODF(Igarashi igarashi)
    {
        this.igarashi = new Igarashi(igarashi);
        var odf_RESULT = NativeMethods.odf_new(ref context, igarashi.Context);
        if (EnumExt.HasFlag(odf_RESULT, (ODF_RESULT)2147483648U))
        {
            throw new Exception(odf_RESULT.ToString());
        }
    }

    public Result Encode(byte[] _Data, ENCODE _Encode)
    {
        var odf_RESULT = NativeMethods.odf_encode(context, _Encode, _Data, _Data.Length);
        if (!EnumExt.HasFlag(odf_RESULT, (ODF_RESULT)2147483648U))
        {
            return new Result(this);
        }

        if (odf_RESULT == (ODF_RESULT)3221225472U)
        {
            throw new Exception(NativeMethods.odf_get_crypto_result(context).ToString());
        }

        throw new Exception(odf_RESULT.ToString());
    }

    public Result Decode(byte[] _Data)
    {
        var odf_RESULT = NativeMethods.odf_decode(context, _Data, _Data.Length);
        if (!EnumExt.HasFlag(odf_RESULT, (ODF_RESULT)2147483648U))
        {
            return new Result(this);
        }

        if (odf_RESULT == (ODF_RESULT)2684354560U)
        {
            throw new Exception(NativeMethods.odf_get_crypto_result(context).ToString());
        }

        throw new Exception(odf_RESULT.ToString());
    }

    public void SetKey(Igarashi igarashi)
    {
        var odf_RESULT = NativeMethods.odf_set_key(context, igarashi.Context);
        if (EnumExt.HasFlag(odf_RESULT, (ODF_RESULT)2147483648U))
        {
            throw new Exception(odf_RESULT.ToString());
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue) return;
        if (context != IntPtr.Zero)
        {
            NativeMethods.odf_delete(ref context);
            context = IntPtr.Zero;
        }

        igarashi.Dispose();
        disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private IntPtr context = IntPtr.Zero;
    private Igarashi igarashi;
    private bool disposedValue;

    private static class NativeMethods
    {
        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.odf_new,
            ThrowOnUnmappableChar = true)]
        internal static extern ODF_RESULT odf_new(ref IntPtr ret, [In] IntPtr ctx);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.odf_delete,
            ThrowOnUnmappableChar = true)]
        internal static extern void odf_delete(ref IntPtr ctx);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.odf_encode,
            ThrowOnUnmappableChar = true)]
        internal static extern ODF_RESULT odf_encode([In] IntPtr ctx, ENCODE enc,
            [MarshalAs(UnmanagedType.LPArray)] [In]
            byte[] _Data, int _Len);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.odf_decode,
            ThrowOnUnmappableChar = true)]
        internal static extern ODF_RESULT odf_decode([In] IntPtr ctx,
            [MarshalAs(UnmanagedType.LPArray)] [In]
            byte[] _Data, int _Len);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Gk.Symbol.odf_get_length, ThrowOnUnmappableChar = true)]
        internal static extern int odf_get_length(IntPtr ctx);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.odf_get_data,
            ThrowOnUnmappableChar = true)]
        internal static extern IntPtr odf_get_data(IntPtr ctx);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.odf_set_key,
            ThrowOnUnmappableChar = true)]
        internal static extern ODF_RESULT odf_set_key(IntPtr ctx, [In] IntPtr key);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Gk.Symbol.odf_get_crypto_result, ThrowOnUnmappableChar = true)]
        internal static extern CryptoResult odf_get_crypto_result(IntPtr ctx);
    }

    public class Result
    {
        internal Result(ODF odf)
        {
            Length = NativeMethods.odf_get_length(odf.context);
            var intPtr = NativeMethods.odf_get_data(odf.context);
            if (intPtr == IntPtr.Zero)
            {
                throw new NullReferenceException();
            }

            Data = new byte[Length];
            Marshal.Copy(intPtr, Data, 0, Length);
        }

        public byte[] Data { get; internal set; }
        public int Length { get; internal set; }
    }
}