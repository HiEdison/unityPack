using UnityEngine;
using System.Collections;
using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

public class EncryptUtils
{

    public static PaddingMode Padding = PaddingMode.PKCS7;
    public static CipherMode Mode = CipherMode.CBC;
    public const string _key = "sjdflk()&%#!lsjfeinb@213*(&*453.";
    public const string _iv = "123123123sdfscvx";
    private static byte[] keyArr;
    private static byte[] ivArr;
    public static byte[] KEY
    {
        get
        {
            if (keyArr == null) keyArr = UTF8Encoding.UTF8.GetBytes(_key);
            return keyArr;
        }
    }
    public static byte[] IV
    {
        get
        {
            if (ivArr == null) ivArr = UTF8Encoding.UTF8.GetBytes(_iv);
            return ivArr;
        }
    }

    #region Encrypt + Decrypt
    /// <summary>
    /// 有密码的AES加密 
    /// </summary>
    /// <param name="text">加密字符</param>
    /// <param name="password">加密的密码</param>
    /// <param name="iv">密钥</param>
    /// <returns></returns>
    public static byte[] Encrypt(byte[] content, byte[] Key, byte[] IV)
    {
        try
        {
            RijndaelManaged rDel = new RijndaelManaged();
            //AesManaged rDel = new AesManaged();
            rDel.Key = Key;
            rDel.IV = IV;
            rDel.Mode = Mode;
            rDel.Padding = Padding;
            // WDebug.Log("key keysize=>" + rDel.KeySize + ", blocksize=" + rDel.BlockSize + ",padding=" + rDel.Padding + ",Mode=" + rDel.Mode + ",feedback size=" + rDel.FeedbackSize);
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(content, 0, content.Length);

            return resultArray;
        }
        catch (Exception e)
        {
            string msg = "<EncryptUtils>:==>Encrypt Error len:" + content.Length;
            WDebug.Log(msg);
            return null;
        }

    }

    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="text"></param>
    /// <param name="password"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static byte[] Decrypt(byte[] content, byte[] Key, byte[] IV)
    {
        try
        {
            RijndaelManaged rDel = new RijndaelManaged();
            //AesManaged rDel = new AesManaged();
            rDel.Key = Key;
            rDel.IV = IV;
            rDel.Mode = Mode;
            rDel.Padding = Padding;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(content, 0, content.Length);

            return resultArray;
        }
        catch (Exception e)
        {
            string msg = "<EncryptUtils>:==>Decrypt Error len:" + content.Length;
            WDebug.Log(msg);
            return null;
        }

    }

    /// <summary>
    /// 有密码的AES加密 
    /// </summary>
    /// <param name="text">加密字符</param>
    /// <param name="password">加密的密码</param>
    /// <param name="iv">密钥</param>
    /// <returns></returns>
    public static string EncryptString(string content, byte[] Key, byte[] IV)
    {
        return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(content), Key, IV));
    }

    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="text"></param>
    /// <param name="password"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static string DecryptString(string content, byte[] Key, byte[] IV)
    {
        return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(content), Key, IV));
    }
    #endregion

}
