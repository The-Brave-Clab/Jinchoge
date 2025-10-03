using System;
using System.Runtime.InteropServices;

namespace Gk;

public class LHB : IDisposable
{
    public LHB(Igarashi igarashi)
    {
        this.igarashi = new Igarashi(igarashi);
        context = NativeMethods.lhb_new();
    }

    public void SetKey(Igarashi igarashi)
    {
        this.igarashi = new Igarashi(igarashi);
    }

    public Result Encode(ref byte[] _Data, ENCODE _Encode)
    {
        var lhb_RESULT = NativeMethods.lhb_encrypt(context, igarashi.Context, _Encode, _Data, _Data.Length);
        if (!EnumExt.HasFlag(lhb_RESULT, (LHB_RESULT)2147483648U))
        {
            return new Result(this);
        }

        if (lhb_RESULT == (LHB_RESULT)2147483650U)
        {
            throw new Exception(NativeMethods.lhb_get_crypto_result(context).ToString());
        }

        throw new Exception(lhb_RESULT.ToString());
    }

    public Result EncodeEx(ref byte[] _Data, ENCODE _Encode)
    {
        var num = NativeMethods.lhb_calc_encode_buffer_size(_Data.Length);
        if (encodeBuffer == null)
        {
            encodeBuffer = new byte[num];
        }
        else
        {
            Array.Resize(ref encodeBuffer, num);
        }

        var lhb_RESULT = NativeMethods.lhb_encrypt_ex(context, igarashi.Context, _Encode, _Data, _Data.Length,
            encodeBuffer, encodeBuffer.Length);
        if (!EnumExt.HasFlag(lhb_RESULT, (LHB_RESULT)2147483648U))
        {
            return new Result(ref encodeBuffer);
        }

        if (lhb_RESULT == (LHB_RESULT)2147483650U)
        {
            throw new Exception(NativeMethods.lhb_get_crypto_result(context).ToString());
        }

        throw new Exception(lhb_RESULT.ToString());
    }

    public Result Decode(ref byte[] _Data)
    {
        return Decode(ref _Data, true);
    }

    public Result Decode(ref byte[] _Data, bool _CalcSum)
    {
        var lhb_RESULT = NativeMethods.lhb_decrypt(context, igarashi.Context,
            (!_CalcSum) ? LHB_OPTION.NONE : LHB_OPTION.VALIDATE_CHECKSUM, _Data, _Data.Length);
        if (!EnumExt.HasFlag(lhb_RESULT, (LHB_RESULT)2147483648U))
        {
            return new Result(this);
        }

        if (lhb_RESULT == (LHB_RESULT)2147483650U)
        {
            throw new Exception(NativeMethods.lhb_get_crypto_result(context).ToString());
        }

        throw new Exception(lhb_RESULT.ToString());
    }

    public Result DecodeEx(ref byte[] _Data)
    {
        return DecodeEx(ref _Data, true);
    }

    public Result DecodeEx(ref byte[] _Data, bool _CalcSum)
    {
        var lhb_RESULT = NativeMethods.lhb_load_header(context, _Data, _Data.Length);
        if (EnumExt.HasFlag(lhb_RESULT, (LHB_RESULT)2147483648U))
        {
            throw new Exception(lhb_RESULT.ToString());
        }

        var num = NativeMethods.lhb_calc_decode_buffer_size(context);
        if (decodeBuffer == null)
        {
            decodeBuffer = new byte[num];
        }
        else
        {
            Array.Resize(ref decodeBuffer, num);
        }

        var lhb_RESULT2 = NativeMethods.lhb_decrypt_ex(context, igarashi.Context,
            (!_CalcSum) ? LHB_OPTION.NONE : LHB_OPTION.VALIDATE_CHECKSUM, _Data, _Data.Length, decodeBuffer,
            decodeBuffer.Length);
        if (!EnumExt.HasFlag(lhb_RESULT2, (LHB_RESULT)2147483648U))
        {
            Array.Resize(ref decodeBuffer, NativeMethods.lhb_get_length(context));
            return new Result(ref decodeBuffer);
        }

        if (lhb_RESULT2 == (LHB_RESULT)2147483650U)
        {
            throw new Exception(NativeMethods.lhb_get_crypto_result(context).ToString());
        }

        throw new Exception(lhb_RESULT2.ToString());
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            encodeBuffer = null;
            decodeBuffer = null;
            if (context != IntPtr.Zero)
            {
                NativeMethods.lhb_delete(ref context);
                context = IntPtr.Zero;
            }

            igarashi.Dispose();
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private IntPtr context;
    private Igarashi igarashi;
    private byte[]? encodeBuffer;
    private byte[]? decodeBuffer;
    private bool disposedValue;

    private static class NativeMethods
    {
        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = Symbol.lhb_new,
            ThrowOnUnmappableChar = true)]
        internal static extern IntPtr lhb_new();

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = Symbol.lhb_delete,
            ThrowOnUnmappableChar = true)]
        internal static extern void lhb_delete(ref IntPtr ctx);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = Symbol.lhb_encrypt,
            ThrowOnUnmappableChar = true)]
        internal static extern LHB_RESULT lhb_encrypt([In] IntPtr ctx, [In] IntPtr key, ENCODE encode,
            [MarshalAs(UnmanagedType.LPArray)] [In]
            byte[] _Data, int _Len);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Symbol.lhb_encrypt_ex, ThrowOnUnmappableChar = true)]
        internal static extern LHB_RESULT lhb_encrypt_ex([In] IntPtr ctx, [In] IntPtr key, ENCODE encode,
            [MarshalAs(UnmanagedType.LPArray)] [In]
            byte[] _Data, int _Len,
            [MarshalAs(UnmanagedType.LPArray)] [Out]
            byte[] _OutBuffer, int _OutBufferLen);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = Symbol.lhb_decrypt,
            ThrowOnUnmappableChar = true)]
        internal static extern LHB_RESULT lhb_decrypt([In] IntPtr ctx, [In] IntPtr key, [In] LHB_OPTION option,
            [MarshalAs(UnmanagedType.LPArray)] [In]
            byte[] _Data, int _Len);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Symbol.lhb_decrypt_ex, ThrowOnUnmappableChar = true)]
        internal static extern LHB_RESULT lhb_decrypt_ex([In] IntPtr ctx, [In] IntPtr key, [In] LHB_OPTION option,
            [MarshalAs(UnmanagedType.LPArray)] [In]
            byte[] _Data, int _Len,
            [MarshalAs(UnmanagedType.LPArray)] [Out]
            byte[] _OutBuffer, int _OutBufferLen);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Symbol.lhb_get_crypto_result, ThrowOnUnmappableChar = true)]
        internal static extern CryptoResult lhb_get_crypto_result(IntPtr ctx);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Symbol.lhb_get_length, ThrowOnUnmappableChar = true)]
        internal static extern int lhb_get_length(IntPtr ctx);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Symbol.lhb_get_buffer, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr lhb_get_buffer(IntPtr ctx);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Symbol.lhb_calc_encode_buffer_size, ThrowOnUnmappableChar = true)]
        internal static extern int lhb_calc_encode_buffer_size(int _Size);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Symbol.lhb_calc_decode_buffer_size, ThrowOnUnmappableChar = true)]
        internal static extern int lhb_calc_decode_buffer_size(IntPtr ctx);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Symbol.lhb_load_header, ThrowOnUnmappableChar = true)]
        internal static extern LHB_RESULT lhb_load_header([In] IntPtr ctx,
            [MarshalAs(UnmanagedType.LPArray)] [In]
            byte[] _Buffer, int _Size);
    }

    public class Result
    {
        internal Result(LHB lh)
        {
            Length = NativeMethods.lhb_get_length(lh.context);
            var intPtr = NativeMethods.lhb_get_buffer(lh.context);
            if (intPtr == IntPtr.Zero)
            {
                throw new NullReferenceException();
            }

            Data = new byte[Length];
            Marshal.Copy(intPtr, Data, 0, Length);
        }

        internal Result(ref byte[] _Data)
        {
            Data = _Data;
            Length = _Data.Length;
        }

        public byte[] Data { get; internal set; }
        public int Length { get; internal set; }
    }
}