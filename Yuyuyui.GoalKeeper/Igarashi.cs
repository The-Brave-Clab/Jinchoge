using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Yuyuyui.GK;

internal static class Igarashi
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Context
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18, ArraySubType = UnmanagedType.U4)]
        public UInt32[] key; // 72 bytes

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024, ArraySubType = UnmanagedType.I4)]
        public Int32[] sBox;

        public Context()
        {
            key = new UInt32[18];
            sBox = new Int32[1024];
        }

        public static Context InitialContext()
        {
            Context ctx = new Context();
            int offset = 0;

            ctx.key = new UInt32[18];
            Buffer.BlockCopy(Consts.IgarashiCtxInitial, offset, ctx.key, 0, 18 * sizeof(UInt32));
            offset += 18 * sizeof(UInt32);

            ctx.sBox = new Int32[1024];
            Buffer.BlockCopy(Consts.IgarashiCtxInitial, offset, ctx.sBox, 0, 1024 * sizeof(Int32));

            return ctx;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
        public readonly UInt32[] encodingCaps;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 inputHash;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 inputLength;

        public Header()
        {
            encodingCaps = new UInt32[] { 0, 0 };
            inputHash = 0;
            inputLength = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct OdfBufferHeader
    {
        [MarshalAs(UnmanagedType.U8)]
        public UInt64 magic;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U1)]
        public readonly byte[] encodingFlags;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 headerHash;

        public const UInt64 HeaderMagic = 0x7FFF817000088B1F;

        public OdfBufferHeader()
        {
            magic = 0;
            encodingFlags = new byte[] { 0, 0 };
            headerHash = 0;
        }
    }

    private static byte[] ToBytes<T>(T obj) where T : notnull
    {
        int size = Marshal.SizeOf<T>();
        byte[] arr = new byte[size];

        IntPtr ptr = IntPtr.Zero;
        try
        {
            ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return arr;
    }

    private static T FromBytes<T>(byte[] bytes, int startIndex = 0) where T : new()
    {
        T obj;

        int size = Marshal.SizeOf<T>();
        IntPtr ptr = IntPtr.Zero;
        try
        {
            ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(bytes, startIndex, ptr, size);

            obj = (T)Marshal.PtrToStructure(ptr, typeof(T))!;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return obj;
    }
    
    private static T[] SubArray<T>(this T[] array, int offset)
    {
        return array.SubArray(offset, array.Length - offset);
    }
    
    private static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        T[] result = new T[length];
        Array.Copy(array, offset, result, 0, length);
        return result;
    }

    private static UInt32 GkHash(UInt32 hx, byte[] input)
    {
        UInt32 length = (UInt32) input.Length;

        UInt32 h1 = hx >> 0x10;
        UInt32 h2 = hx & 0xffff;
        UInt32 h3 = 0;
        if (length == 1)
        {
            h2 = Consts.HashLookup[input[0]] + h2;
            h3 = h2 - 0xfff1;
            if (h2 < 0xfff1)
            {
                h3 = h2;
            }

            h1 = h3 + h1;
            h2 = h1 + 0xf;
            if (h1 < 0xfff1)
            {
                h2 = h1;
            }

            return h3 | h2 << 0x10;
        }

        int inputOffset = 0;
        if (length < 16)
        {
            while (length != 0)
            {
                length -= 1;
                h2 = input[inputOffset] + h2;
                h1 = h2 + h1;
                ++inputOffset;
            }

            h3 = h2 - 0xfff1;
            if (h2 < 0xfff1)
            {
                h3 = h2;
            }

            h1 %= 0xfff1;
            h2 = h3;
            return h2 | h1 << 0x10;
        }

        bool longLength = length >> 4 >= 0x15b;
        if (longLength)
        {
            while (length >> 4 >= 0x15b)
            {
                var offset = 0;
                for (int p = 0; p != 0x15b; p++)
                {
                    for (int bit = 0; bit < 0x10; ++bit)
                    {
                        h2 += Consts.HashLookup[input[bit + inputOffset + offset]];
                        h1 += h2;
                    }
                    offset += 0x10;
                }

                length -= 5552;
                inputOffset += 5552;
                h2 %= 0xfff1;
                h1 %= 0xfff1;
            }

            if (length == 0)
            {
                return h2 | h1 << 0x10;
            }
        }

        if (!longLength || length >= 0x10)
        {
            var h4 = length - 0x10;
            h3 = h4 & 0xfffffff0;
            int tempInputOffset = inputOffset;
            do
            {
                for (int bit = 0; bit < 0x10; ++bit)
                {
                    h2 += Consts.HashLookup[input[bit + tempInputOffset]];
                    h1 += h2;
                }
                tempInputOffset += 0x10;
                length -= 0x10;
            } while (length >= 0x10);
            length = h4 - h3;
            if (length == 0) goto final;
            inputOffset += (Int32)h3 + 0x10;
        }

        do
        {
            length -= 1;
            h2 = input[inputOffset] + h2;
            h1 = h2 + h1;
            inputOffset += 1;
        } while (length != 0);

        final:
        return h2 % 0xfff1 | (h1 % 0xfff1) * 0x10000;
    }

    public static void ExpandKey(ref Context ctx, byte[] key)
    {
        UInt32 i, j, k, l;

        UInt32 a = 0;
        UInt32 b = 0;
        UInt32 c = 0;
        UInt32 d = 0;
        UInt32 e = 0;
        UInt32 f = 0;
        UInt32 g = 0;

        UInt32 length = (UInt32) key.Length;

        for (i = 0; i < 18; ++i)
        {
            ctx.key[i] ^= (UInt32)(key[(i * 4 + 3) % length] << 0x00) |
                           (UInt32)(key[(i * 4 + 2) % length] << 0x08) |
                           (UInt32)(key[(i * 4 + 1) % length] << 0x10) |
                           (UInt32)(key[(i * 4 + 0) % length] << 0x18);
        }

        i = 0;
        for (j = 0; j < 18; j += 2)
        {
            i = i & 0xff | (a & 0xff) << 0x8 | (b & 0xff) << 0x10 | c << 0x18;
            b = d & 0xff | (e & 0xff) << 0x8 | (f & 0xff) << 0x10 | g << 0x18;
            for (l = 0; l != 15; l++)
            {
                e = ctx.key[l] ^ i;
                i = (UInt32)((ctx.sBox[(e >> 0x10 & 0xff) + 256 * 1] + ctx.sBox[e >> 0x18] ^ ctx.sBox[(e >> 8 & 0xff) + 256 * 2]) +
                    ctx.sBox[(e & 0xff) + 256 * 3] ^ b);
                b = e;
            }

            i ^= ctx.key[15];
            d = (UInt32)((ctx.sBox[(i >> 0x10 & 0xff) + 256 * 1] + ctx.sBox[i >> 0x18] ^ ctx.sBox[(i >> 8 & 0xff) + 256 * 2]) +
                ctx.sBox[(i & 0xff) + 256 * 3] ^ e ^ ctx.key[16]);
            i ^= ctx.key[17];
            c = i >> 0x18;
            g = d >> 0x18;
            ctx.key[j] = i;
            ctx.key[(j | 1)] = d;
            b = i >> 0x10;
            a = i >> 0x8;
            f = d >> 0x10;
            e = d >> 0x8;
        }

        for (k = 0; k < 4; ++k)
        {
            for (j = 0; j < 256; j += 2)
            {
                i = i & 0xff | (a & 0xff) << 8 | (b & 0xff) << 0x10 | c << 0x18;
                b = d & 0xff | (e & 0xff) << 8 | (f & 0xff) << 0x10 | g << 0x18;
                for (l = 0; l != 15; l++)
                {
                    e = ctx.key[l] ^ i;
                    i = (UInt32)((ctx.sBox[(e >> 0x10 & 0xff) + 256 * 1] + ctx.sBox[e >> 0x18] ^ ctx.sBox[
                            (e >> 8 & 0xff) + 256 * 2]) + ctx.sBox[(e & 0xff) + 256 * 3] ^ b);
                    b = e;
                }

                i ^= ctx.key[15];
                d = (UInt32)((ctx.sBox[(i >> 0x10 & 0xff) + 256 * 1] + ctx.sBox[i >> 0x18] ^ ctx.sBox[(i >> 8 & 0xff) + 256 * 2]) +
                    ctx.sBox[(i & 0xff) + 256 * 3] ^ e ^ ctx.key[16]);
                i ^= ctx.key[17];
                c = i >> 0x18;
                g = d >> 0x18;
                ctx.sBox[k * 256 + j] = (Int32)i;
                ctx.sBox[k * 256 + (j | 1)] = (Int32)d;
                b = i >> 0x10;
                a = i >> 0x8;
                f = d >> 0x10;
                e = d >> 0x8;
            }
        }
    }

    private static void DecryptBuffer(in Context ctx, byte[] input, out byte[] output)
    {
        int inputLength = input.Length;
        output = new byte[inputLength];
        UInt32 c = 0;
        UInt32 offset = 0;
        if ((inputLength & 7) == 0)
        {
            for (int i = 0; i < inputLength >> 3; ++i)
            {
                var a = (UInt32)input[0 + offset] << 0x18 |
                        (UInt32)input[1 + offset] << 0x10 |
                        (UInt32)input[2 + offset] << 0x08 |
                        (UInt32)input[3 + offset];
                var b = (UInt32)input[4 + offset] << 0x18 |
                        (UInt32)input[5 + offset] << 0x10 |
                        (UInt32)input[6 + offset] << 0x08 |
                        (UInt32)input[7 + offset];
                for (UInt32 j = 17; j > 2; --j)
                {
                    c = ctx.key[j] ^ a;
                    a = (UInt32)((ctx.sBox[(c >> 0x10 & 0xff) + 256 * 1] + ctx.sBox[c >> 0x18] ^
                                  ctx.sBox[(c >> 0x8 & 0xff) + 256 * 2]) + ctx.sBox[(c & 0xff) + 256 * 3] ^ b);
                    b = c;
                }

                a ^= ctx.key[2];
                b = (UInt32)
                    ((ctx.sBox[(a >> 0x10 & 0xff) + 256 * 1] + ctx.sBox[a >> 0x18] ^ ctx.sBox[(a >> 0x8 & 0xff) + 256 * 2]) +
                        ctx.sBox[(a & 0xff) + 256 * 3] ^ c ^ ctx.key[1]);
                a ^= ctx.key[0];
                output[0 + offset] = (byte)(a >> 0x18);
                output[1 + offset] = (byte)(a >> 0x10);
                output[2 + offset] = (byte)(a >> 0x8);
                output[3 + offset] = (byte)a;
                output[4 + offset] = (byte)(b >> 0x18);
                output[5 + offset] = (byte)(b >> 0x10);
                output[6 + offset] = (byte)(b >> 0x8);
                output[7 + offset] = (byte)b;
                offset += 0x08;
            }
        }
    }

    private static void EncryptBuffer(in Context ctx, byte[] input, out byte[] output)
    {
        int inputLength = input.Length;
        output = new byte[inputLength];
        UInt32 c = 0;
        UInt32 offset = 0;
        if ((inputLength & 7) == 0)
        {
            for (int i = 0; i < inputLength >> 3; ++i)
            {
                var a = (UInt32)input[0 + offset] << 0x18 | 
                            (UInt32)input[1 + offset] << 0x10 |
                            (UInt32)input[2 + offset] << 0x08 | 
                            (UInt32)input[3 + offset];
                var b = (UInt32)input[4 + offset] << 0x18 | 
                            (UInt32)input[5 + offset] << 0x10 |
                            (UInt32)input[6 + offset] << 0x08 | 
                            (UInt32)input[7 + offset];
                for (UInt32 j = 0; j < 15; j++)
                {
                    c = ctx.key[j] ^ a;
                    a = (UInt32)
                        ((ctx.sBox[(c >> 0x10 & 0xff) + 256 * 1] + ctx.sBox[c >> 0x18] ^ ctx.sBox[(c >> 8 & 0xff) + 256 * 2]) +
                            ctx.sBox[(c & 0xff) + 256 * 3] ^ b);
                    b = c;
                }

                a ^= ctx.key[15];
                b = (UInt32)((ctx.sBox[(a >> 0x10 & 0xff) + 256 * 1] + ctx.sBox[a >> 0x18] ^ 
                              ctx.sBox[(a >> 8 & 0xff) + 256 * 2]) + ctx.sBox[(a & 0xff) + 256 * 3] ^ c ^ ctx.key[16]);
                a ^= ctx.key[17];
                output[0 + offset] = (byte)(a >> 0x18);
                output[1 + offset] = (byte)(a >> 0x10);
                output[2 + offset] = (byte)(a >> 0x8);
                output[3 + offset] = (byte)a;
                output[4 + offset] = (byte)(b >> 0x18);
                output[5 + offset] = (byte)(b >> 0x10);
                output[6 + offset] = (byte)(b >> 0x8);
                output[7 + offset] = (byte)b;
                offset += 0x8;
            }
        }
    }

    public static UInt32 OdfEncode(ref Context ctx, byte[] input, out byte[] output, UInt32 encoding)
    {
        UInt32 inputLength = (UInt32) input.Length;

        var padLength = (inputLength + 0xF) & 0xFFFFFF0;
        var bufferLength = padLength + 0x10;
        var bufferHeader = new Header
        {
            encodingCaps =
            {
                [0] = 0x80000004,
                [1] = 0x02008000
            },
            inputHash = GkHash(0, input),
            inputLength = inputLength
        };

        byte[] buffer = new byte[padLength];
        Array.Copy(input, 0, buffer, 0, inputLength);
        byte[] headerBuffer = ToBytes(bufferHeader);

        EncryptBuffer(ctx, headerBuffer, out var headerBufferEncrypted);
        EncryptBuffer(ctx, buffer, out var bufferEncrypted);

        var headerLength = bufferLength + 0xE;
        var odfHeader = new OdfBufferHeader
        {
            magic = OdfBufferHeader.HeaderMagic,
            encodingFlags =
            {
                [0] = 0x4,
                [1] = (byte) (encoding >> 0x18)
            },
            headerHash = GkHash(4, headerBuffer)
        };

        output = new byte[headerLength];
        var rand = new Random();
        rand.NextBytes(output);
        
        Array.Copy(ToBytes(odfHeader), 0, output, 0, Marshal.SizeOf<OdfBufferHeader>());
        Array.Copy(headerBufferEncrypted, 0, output, Marshal.SizeOf<OdfBufferHeader>(), Marshal.SizeOf<Header>());
        Array.Copy(bufferEncrypted, 0, output, Marshal.SizeOf<OdfBufferHeader>() + Marshal.SizeOf<Header>(), padLength);

        return headerLength;
    }

    public static UInt32 OdfDecode(ref Context ctx, byte[] input, out byte[] output)
    {  
        OdfBufferHeader odfHeader = FromBytes<OdfBufferHeader>(input);
        byte[] encryptedDataHeader = input.SubArray(Marshal.SizeOf<OdfBufferHeader>(), Marshal.SizeOf<Header>());
        byte[] encryptedData = input.SubArray(Marshal.SizeOf<OdfBufferHeader>() + Marshal.SizeOf<Header>());

        if (odfHeader.magic != OdfBufferHeader.HeaderMagic) {
            output = Array.Empty<byte>();
            return 0;
        }
        
        DecryptBuffer(ctx, encryptedDataHeader, out var dataHeader);
        Header header = FromBytes<Header>(dataHeader);
        
        Debug.Assert(odfHeader.headerHash == GkHash(4, dataHeader), "ODF Header hash mismatch");

        DecryptBuffer(ctx, encryptedData, out var outputBuffer);
        output = outputBuffer.SubArray(0, (int) header.inputLength);

        Debug.Assert(header.inputHash == GkHash(0, output), "Buffer hash mismatch");
        
        return header.inputLength;
    }
}