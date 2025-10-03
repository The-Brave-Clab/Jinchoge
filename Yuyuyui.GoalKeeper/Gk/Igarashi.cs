using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Gk;

public class Igarashi : IDisposable
{
    public Igarashi(string key)
    {
        Sinatra(Encoding.ASCII.GetBytes(key), (KEY_REQUEST)4026531840U);
    }

    public Igarashi(byte[] key)
    {
        Sinatra(key, (KEY_REQUEST)4026531840U);
    }

    public Igarashi(string key, KEY_REQUEST request)
    {
        Sinatra(Encoding.ASCII.GetBytes(key), request);
    }

    public Igarashi(byte[] key, KEY_REQUEST request)
    {
        Sinatra(key, request);
    }

    public Igarashi(Igarashi igr)
    {
        var cryptoResult = NativeMethods.gk_crypto_duplicate_context(igr.Context, ref keyPtr);
        if (EnumExt.HasFlag(cryptoResult, (CryptoResult)2147483648U))
        {
            throw new Exception(cryptoResult.ToString());
        }
    }

    public static uint GkLibraryVersion => NativeMethods.gk_version();

    public static bool CheckGkLibraryVersion()
    {
        return GkLibraryVersion == GkVersion.VERSION;
    }

    public static void WakeUp()
    {
        if (!CheckGkLibraryVersion())
        {
            throw new InvalidProgramException("Version Mismatch Native:" + GkLibraryVersion.ToString("x8") +
                                              " != Managed:" + GkVersion.VERSION.ToString("x8"));
        }

        NativeMethods.gk_startup();
    }

    public static string? WakeUpEx()
    {
        if (!CheckGkLibraryVersion())
        {
            throw new InvalidProgramException("Version Mismatch Native:" + GkLibraryVersion.ToString("x8") +
                                              " != Managed:" + GkVersion.VERSION.ToString("x8"));
        }

        return Marshal.PtrToStringAnsi(NativeMethods.gk_startup());
    }

    public IntPtr Context => keyPtr;

    public byte[] IV
    {
        get
        {
            var num = NativeMethods.gk_crypto_get_iv_length(keyPtr);
            var intPtr = NativeMethods.gk_crypto_get_iv(keyPtr);
            if (intPtr == IntPtr.Zero)
            {
                throw new NullReferenceException();
            }

            var array = new byte[num];
            Marshal.Copy(intPtr, array, 0, num);
            return array;
        }
        set
        {
            var cryptoResult = NativeMethods.gk_crypto_set_iv(keyPtr, value, value.Length);
            if (EnumExt.HasFlag(cryptoResult, (CryptoResult)2147483648U))
            {
                throw new Exception(cryptoResult.ToString());
            }
        }
    }

    public CAPS Caps => NativeMethods.gk_crypto_get_caps(keyPtr);

    public byte[] Encrypt(byte[] _input, ENCODE _Encode)
    {
        var array = new byte[_input.Length];
        var cryptoResult =
            NativeMethods.gk_crypto_encrypt(keyPtr, _Encode, _input, array, _input.Length, IntPtr.Zero, IntPtr.Zero);
        if (EnumExt.HasFlag(cryptoResult, (CryptoResult)2147483648U))
        {
            throw new Exception(cryptoResult.ToString());
        }

        return array;
    }

    public byte[] Decrypt(byte[] _input, ENCODE _Encode)
    {
        var array = new byte[_input.Length];
        var cryptoResult =
            NativeMethods.gk_crypto_decrypt(keyPtr, _Encode, _input, array, _input.Length, IntPtr.Zero, IntPtr.Zero);
        if (EnumExt.HasFlag(cryptoResult, (CryptoResult)2147483648U))
        {
            throw new Exception(cryptoResult.ToString());
        }

        return array;
    }

    private void Sinatra(byte[] key, KEY_REQUEST request)
    {
        var cryptoResult = NativeMethods.gk_crypto_new(ref keyPtr, key, key.Length, request);
        if (EnumExt.HasFlag(cryptoResult, (CryptoResult)2147483648U))
        {
            throw new Exception(cryptoResult.ToString());
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue)
        {
            return;
        }

        if (IntPtr.Zero != keyPtr)
        {
            NativeMethods.gk_crypto_delete(ref keyPtr);
            keyPtr = IntPtr.Zero;
        }

        disposedValue = true;
    }

    ~Igarashi()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private IntPtr keyPtr = IntPtr.Zero;
    private bool disposedValue;

    private static class NativeMethods
    {
        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Gk.Symbol.gk_crypto_new, ThrowOnUnmappableChar = true)]
        internal static extern CryptoResult gk_crypto_new(ref IntPtr ret,
            [MarshalAs(UnmanagedType.LPArray)] [In]
            byte[] _Key, int _Len, KEY_REQUEST _Request);

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto,
            EntryPoint = Gk.Symbol.gk_crypto_duplicate_context, ThrowOnUnmappableChar = true)]
        internal static extern CryptoResult gk_crypto_duplicate_context([In] IntPtr src, ref IntPtr dest);

        [DllImport(Symbol.libgk_so, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.gk_crypto_delete)]
        internal static extern void gk_crypto_delete(ref IntPtr key);

        [DllImport(Symbol.libgk_so, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.gk_crypto_set_iv)]
        internal static extern CryptoResult gk_crypto_set_iv([In] IntPtr _Ctx,
            [MarshalAs(UnmanagedType.LPArray)] [In]
            byte[] _IV, int _Len);

        [DllImport(Symbol.libgk_so, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.gk_crypto_get_iv)]
        internal static extern IntPtr gk_crypto_get_iv([In] IntPtr _Ctx);

        [DllImport(Symbol.libgk_so, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.gk_crypto_get_iv_length)]
        internal static extern int gk_crypto_get_iv_length([In] IntPtr _Ctx);

        [DllImport(Symbol.libgk_so, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.gk_crypto_get_caps)]
        internal static extern CAPS gk_crypto_get_caps([In] IntPtr _Ctx);

        [DllImport(Symbol.libgk_so, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.gk_crypto_encrypt)]
        internal static extern CryptoResult gk_crypto_encrypt([In] IntPtr _Ctx, ENCODE _Encode,
            [MarshalAs(UnmanagedType.LPArray)] [In]
            byte[] _Input,
            [MarshalAs(UnmanagedType.LPArray)] [Out]
            byte[] _Output, int _Length, IntPtr _H1, IntPtr _H2);

        [DllImport(Symbol.libgk_so, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.gk_crypto_decrypt)]
        internal static extern CryptoResult gk_crypto_decrypt([In] IntPtr _Ctx, ENCODE _Encode,
            [MarshalAs(UnmanagedType.LPArray)] [In]
            byte[] _Input,
            [MarshalAs(UnmanagedType.LPArray)] [Out]
            byte[] _Output, int _Length, IntPtr _H1, IntPtr _H2);

        [DllImport(Symbol.libgk_so, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.gk_version)]
        internal static extern uint gk_version();

        [DllImport(Symbol.libgk_so, BestFitMapping = false, CharSet = CharSet.Auto, EntryPoint = Gk.Symbol.gk_startup,
            ThrowOnUnmappableChar = true)]
        internal static extern IntPtr gk_startup();
    }
}