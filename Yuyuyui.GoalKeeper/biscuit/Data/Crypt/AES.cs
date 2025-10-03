using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace biscuit.Data.Crypt;

public class AES
{
    public static byte[]? DecryptAes(byte[] data)
    {
        return DecryptAes256Cbc(data, EncryptKey, iv);
    }

    public static byte[]? EncryptAes(byte[] data)
    {
        EncryptAes256Cbc(data, EncryptKey, iv, out var result);
        return result;
    }

    public static void EncryptAes256Cbc(byte[] src, string? stringKey, string? iv, out byte[]? dst)
    {
        dst = null;
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        aes.KeySize = 256;
        aes.BlockSize = 128;
        var bytes = Encoding.UTF8.GetBytes(stringKey!);
        var bytes2 = Encoding.UTF8.GetBytes(iv!);
        using var cryptoTransform = aes.CreateEncryptor(bytes, bytes2);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
        cryptoStream.Write(src, 0, src.Length);
        cryptoStream.FlushFinalBlock();
        dst = memoryStream.ToArray();
    }

    public static void EncryptAes128Cbc(byte[] src, string? stringKey, out string generateIv, out byte[]? dst)
    {
        dst = null;
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        aes.KeySize = 128;
        aes.BlockSize = 128;
        aes.Key = Encoding.UTF8.GetBytes(stringKey!);
        aes.GenerateIV();
        generateIv = Convert.ToBase64String(aes.IV);
        using var cryptoTransform = aes.CreateEncryptor();
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
        cryptoStream.Write(src, 0, src.Length);
        cryptoStream.FlushFinalBlock();
        dst = memoryStream.ToArray();
    }

    public static byte[]? DecryptAes256Cbc(byte[] src, string? stringKey, string? iv)
    {
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        aes.KeySize = 256;
        aes.BlockSize = 128;
        var bytes = Encoding.UTF8.GetBytes(stringKey!);
        var bytes2 = Encoding.UTF8.GetBytes(iv!);
        using var cryptoTransform = aes.CreateDecryptor(bytes, bytes2);
        using var memoryStream = new MemoryStream(src);
        using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
        using var memoryStream2 = new MemoryStream(src.Length);
        byte[]? array;
        try
        {
            var array2 = new byte[4096];
            int count;
            while ((count = cryptoStream.Read(array2, 0, array2.Length)) > 0)
            {
                memoryStream2.Write(array2, 0, count);
            }

            array = memoryStream2.ToArray();
        }
        catch (Exception)
        {
            array = null;
        }
        finally
        {
            memoryStream2.Close();
            cryptoStream.Close();
            memoryStream.Close();
        }

        var result = array;
        return result;
    }

    public static byte[]? DecryptAes128Cbc(byte[] src, string? stringKey, string? iv)
    {
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        aes.KeySize = 128;
        aes.BlockSize = 128;
        aes.Key = Encoding.UTF8.GetBytes(stringKey!);
        aes.IV = Convert.FromBase64String(iv!);
        using var cryptoTransform = aes.CreateDecryptor();
        using var memoryStream = new MemoryStream(src);
        using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
        using var memoryStream2 = new MemoryStream(src.Length);
        byte[]? array;
        try
        {
            var array2 = new byte[4096];
            int count;
            while ((count = cryptoStream.Read(array2, 0, array2.Length)) > 0)
            {
                memoryStream2.Write(array2, 0, count);
            }

            array = memoryStream2.ToArray();
        }
        catch (Exception)
        {
            array = null;
        }
        finally
        {
            memoryStream2.Close();
            cryptoStream.Close();
            memoryStream.Close();
        }

        var result = array;
        return result;
    }

    private static readonly string iv = "1234567890123456";
    private static readonly string EncryptKey = "cve4hbq9sjvawhvdr9kvhpfm5qv393ga";
}