using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Ionic.Zlib;
using UnityEngine;

public class ResourcesEncryption
{
    private static readonly string Default_AES_Key = "0fe03d52afe6275f1dce272c37bb60cc";
    private static byte[] Keys = { 0x45, 0x1A, 0x98, 0xAB, 0x6F, 0x92, 0xEA, 0xEA,
        0x1C,0xCE, 0x9D, 0x8A, 0x5C, 0xC9, 0x07, 0xCF };

    /// <summary>
    /// 对称加密算法AES RijndaelManaged加密(RijndaelManaged（AES）算法是块式加密算法)
    /// </summary>
    /// <param name="encryptString">待加密字符串</param>
    /// <returns>加密结果字符串</returns>
    public static string AES_Encrypt(string encryptString)
    {
        return AES_Encrypt(encryptString, Default_AES_Key);
    }
    public static string GetMD5HashFromString(string data)
    {
        try
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(Encoding.Default.GetBytes(data));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (System.Exception ex)
        {
            UnityEngine.WDebug.LogWarning("GetMD5HashFromFile() fail,error:" + ex.Message);
            return "";
        }
    }

    public static string AES_PHPEncryptAndCompress(string data, string encryptKey)
    {
        if (data == null || encryptKey == null)
            return "";
        byte[] compressString = ZlibCodecCompress(data);
        string returnString = AES_PHPEncrypt(compressString, encryptKey);
        return returnString;
    }

    public static string AES_PHPEncrypt(byte[] encryptBytes, string encryptKey)
    {
        string md5 = GetMD5HashFromString(encryptKey);

        RijndaelManaged rijndaelProvider = new RijndaelManaged();
        rijndaelProvider.BlockSize = 256;
        rijndaelProvider.KeySize = 256;
        rijndaelProvider.Key = Encoding.UTF8.GetBytes(md5);
        rijndaelProvider.IV = Encoding.UTF8.GetBytes(GetMD5HashFromString(md5));
        rijndaelProvider.Padding = PaddingMode.Zeros;
        rijndaelProvider.Mode = CipherMode.CBC;
        ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

        byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length);

        return Convert.ToBase64String(encryptedData);
    }

    public static string AES_PHPDecryptAndDecompress(string data, string decryptKey)
    {
        if (data == "")
            return "";
        byte[] decryptBytes = AES_PHPDecrypt(data, decryptKey);
        if (decryptBytes == null)
            return "";
        string returnString = ZlibCodecDecompress(decryptBytes);
        return returnString;
    }
    public static byte[] ZlibCodecCompress(string textToCompress)
    {
        int outputSize = 2048;
        byte[] output = new Byte[outputSize];
        byte[] uncompressed = UTF8Encoding.UTF8.GetBytes(textToCompress);
        int lengthToCompress = uncompressed.Length;

        // If you want a ZLIB stream, set this to true.  If you want
        // a bare DEFLATE stream, set this to false.
        bool wantRfc1950Header = true;

        using (MemoryStream ms = new MemoryStream())
        {
            ZlibCodec compressor = new ZlibCodec();
            compressor.InitializeDeflate(Ionic.Zlib.CompressionLevel.BestCompression, wantRfc1950Header);

            compressor.InputBuffer = uncompressed;
            compressor.AvailableBytesIn = lengthToCompress;
            compressor.NextIn = 0;
            compressor.OutputBuffer = output;

            foreach (var f in new FlushType[] { FlushType.None, FlushType.Finish })
            {
                int bytesToWrite = 0;
                do
                {
                    compressor.AvailableBytesOut = outputSize;
                    compressor.NextOut = 0;
                    compressor.Deflate(f);

                    bytesToWrite = outputSize - compressor.AvailableBytesOut;
                    if (bytesToWrite > 0)
                        ms.Write(output, 0, bytesToWrite);
                }
                while ((f == FlushType.None && (compressor.AvailableBytesIn != 0 || compressor.AvailableBytesOut == 0)) ||
                       (f == FlushType.Finish && bytesToWrite != 0));
            }

            compressor.EndDeflate();

            ms.Flush();
            return ms.ToArray();
        }
    }

    public static string ZlibCodecDecompress(byte[] compressed)
    {
        int outputSize = 2048;
        byte[] output = new Byte[outputSize];

        // If you have a ZLIB stream, set this to true.  If you have
        // a bare DEFLATE stream, set this to false.
        bool expectRfc1950Header = true;

        using (MemoryStream ms = new MemoryStream())
        {
            ZlibCodec compressor = new ZlibCodec();
            compressor.InitializeInflate(expectRfc1950Header);

            compressor.InputBuffer = compressed;
            compressor.AvailableBytesIn = compressed.Length;
            compressor.NextIn = 0;
            compressor.OutputBuffer = output;

            foreach (var f in new FlushType[] { FlushType.None, FlushType.Finish })
            {
                int bytesToWrite = 0;
                do
                {
                    compressor.AvailableBytesOut = outputSize;
                    compressor.NextOut = 0;
                    compressor.Inflate(f);

                    bytesToWrite = outputSize - compressor.AvailableBytesOut;
                    if (bytesToWrite > 0)
                        ms.Write(output, 0, bytesToWrite);
                }
                while ((f == FlushType.None && (compressor.AvailableBytesIn != 0 || compressor.AvailableBytesOut == 0)) ||
                       (f == FlushType.Finish && bytesToWrite != 0));
            }

            compressor.EndInflate();

            return UTF8Encoding.UTF8.GetString(ms.ToArray());
        }
    }

    public static byte[] AES_PHPDecrypt(string decryptString, string decryptKey)
    {
        try
        {
            string md5 = GetMD5HashFromString(decryptKey);

            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.BlockSize = 256;
            rijndaelProvider.KeySize = 256;
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(md5);
            rijndaelProvider.IV = Encoding.UTF8.GetBytes(GetMD5HashFromString(md5));
            rijndaelProvider.Padding = PaddingMode.Zeros;
            rijndaelProvider.Mode = CipherMode.CBC;
            ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

            byte[] inputData = Convert.FromBase64String(decryptString);
            byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            //			return (Encoding.UTF8.GetString(decryptedData)).Replace("\0","");
            return TrimEnd(decryptedData);
        }
        catch
        {
            return null;
        }
    }
    private static byte[] TrimEnd(byte[] array)
    {
        int lastIndex = Array.FindLastIndex(array, b => b != 0);

        Array.Resize(ref array, lastIndex + 1);

        return array;
    }
    /// <summary>
    /// 对称加密算法AES RijndaelManaged加密(RijndaelManaged（AES）算法是块式加密算法)
    /// </summary>
    /// <param name="encryptString">待加密字符串</param>
    /// <param name="encryptKey">加密密钥，须半角字符</param>
    /// <returns>加密结果字符串</returns>
    public static string AES_Encrypt(string encryptString, string encryptKey)
    {
        encryptString = encryptString == null ? "" : encryptString;
        encryptKey = GetSubString(encryptKey, 32, "");
        encryptKey = encryptKey.PadRight(32, ' ');

        RijndaelManaged rijndaelProvider = new RijndaelManaged();
        rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey);
        rijndaelProvider.IV = Keys;
        ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

        byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
        byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// 对称加密算法AES RijndaelManaged解密字符串
    /// </summary>
    /// <param name="decryptString">待解密的字符串</param>
    /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
    public static string AES_Decrypt(string decryptString)
    {
        return AES_Decrypt(decryptString, Default_AES_Key);
    }

    /// <summary>
    /// 对称加密算法AES RijndaelManaged解密字符串
    /// </summary>
    /// <param name="decryptString">待解密的字符串</param>
    /// <param name="decryptKey">解密密钥,和加密密钥相同</param>
    /// <returns>解密成功返回解密后的字符串,失败返回空</returns>
    public static string AES_Decrypt(string decryptString, string decryptKey)
    {
        try
        {
            decryptKey = GetSubString(decryptKey, 32, "");
            decryptKey = decryptKey.PadRight(32, ' ');

            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(decryptKey);
            rijndaelProvider.IV = Keys;
            ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

            byte[] inputData = Convert.FromBase64String(decryptString);
            byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);
            return (Encoding.UTF8.GetString(decryptedData)).Replace("\0", "");
        }
        catch (Exception e)
        {
            WDebug.Log(e);
            return string.Empty;
        }
    }

    /// <summary>
    /// 按字节长度(按字节,一个汉字为2个字节)取得某字符串的一部分
    /// </summary>
    /// <param name="sourceString">源字符串</param>
    /// <param name="length">所取字符串字节长度</param>
    /// <param name="tailString">附加字符串(当字符串不够长时，尾部所添加的字符串，一般为"...")</param>
    /// <returns>某字符串的一部分</returns>
    private static string GetSubString(string sourceString, int length, string tailString)
    {
        return GetSubString(sourceString, 0, length, tailString);
    }

    /// <summary>
    /// 按字节长度(按字节,一个汉字为2个字节)取得某字符串的一部分
    /// </summary>
    /// <param name="sourceString">源字符串</param>
    /// <param name="startIndex">索引位置，以0开始</param>
    /// <param name="length">所取字符串字节长度</param>
    /// <param name="tailString">附加字符串(当字符串不够长时，尾部所添加的字符串，一般为"...")</param>
    /// <returns>某字符串的一部分</returns>
    private static string GetSubString(string sourceString, int startIndex, int length, string tailString)
    {
        string myResult = sourceString;

        //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
        if (System.Text.RegularExpressions.Regex.IsMatch(sourceString, "[\u0800-\u4e00]+") ||
            System.Text.RegularExpressions.Regex.IsMatch(sourceString, "[\xAC00-\xD7A3]+"))
        {
            //当截取的起始位置超出字段串长度时
            if (startIndex >= sourceString.Length)
            {
                return string.Empty;
            }
            else
            {
                return sourceString.Substring(startIndex,
                                              ((length + startIndex) > sourceString.Length) ? (sourceString.Length - startIndex) : length);
            }
        }

        //中文字符，如"中国人民abcd123"
        if (length <= 0)
        {
            return string.Empty;
        }
        byte[] bytesSource = Encoding.Default.GetBytes(sourceString);

        //当字符串长度大于起始位置
        if (bytesSource.Length > startIndex)
        {
            int endIndex = bytesSource.Length;

            //当要截取的长度在字符串的有效长度范围内
            if (bytesSource.Length > (startIndex + length))
            {
                endIndex = length + startIndex;
            }
            else
            {   //当不在有效范围内时,只取到字符串的结尾
                length = bytesSource.Length - startIndex;
                tailString = "";
            }

            int[] anResultFlag = new int[length];
            int nFlag = 0;
            //字节大于127为双字节字符
            for (int i = startIndex; i < endIndex; i++)
            {
                if (bytesSource[i] > 127)
                {
                    nFlag++;
                    if (nFlag == 3)
                    {
                        nFlag = 1;
                    }
                }
                else
                {
                    nFlag = 0;
                }
                anResultFlag[i] = nFlag;
            }
            //最后一个字节为双字节字符的一半
            if ((bytesSource[endIndex - 1] > 127) && (anResultFlag[length - 1] == 1))
            {
                length = length + 1;
            }

            byte[] bsResult = new byte[length];
            //WDebug.Log($"ResourcesEncryption.353-->{length}->{bytesSource.Length}->{bsResult.Length}");
            Array.Copy(bytesSource, startIndex, bsResult, 0, length);
            //WDebug.Log($"ResourcesEncryption.355-->{length}->{bytesSource.Length}->{bsResult.Length}");
            myResult = Encoding.Default.GetString(bsResult);
            myResult = myResult + tailString;

            return myResult;
        }

        return string.Empty;

    }



    //这个算法主要是把资源进行简单加密，防止小白直接破解，如果对面拿到了源码的话那就没办法了
    public static void SimpleEncrypte(byte[] data)
    {
        if (data == null) return;
        int len = 100;
        if (data.Length < len)
        {
            len = data.Length;
        }

        for (int i = 0; i < len; i++)
        {
            data[i] += 119;//119是随便想的数字 请不要随便更改，别的函数或者工程也是使用的这个数据
        }
    }
}
