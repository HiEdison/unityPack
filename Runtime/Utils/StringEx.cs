using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// 字符串优化类
/// </summary>
public static class StringEx
{
    private static StringBuilder stringBuilder = new StringBuilder();
    private static StringBuilder shareStringBuilder = new StringBuilder();

    /// <summary>
    /// 获取共享StringBuilder
    /// </summary>
    /// <returns></returns>
    public static StringBuilder GetShareStringBuilder()
    {
        if (shareStringBuilder.Length > 0)
            shareStringBuilder.Remove(0, stringBuilder.Length);
        return shareStringBuilder;
    }

    /// <summary>
    /// 合并字符串
    /// </summary>
    /// <param name="src"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string Format(string src, params object[] args)
    {
        if (stringBuilder.Length > 0)
            stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.AppendFormat(src, args);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 合并字符串
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <returns></returns>
    public static string Concat(string s1, string s2)
    {
        if (stringBuilder.Length > 0)
            stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(s1);
        stringBuilder.Append(s2);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 合并字符串
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <param name="s3"></param>
    /// <returns></returns>
    public static string Concat(string s1, string s2, string s3)
    {
        if (stringBuilder.Length > 0)
            stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append(s1);
        stringBuilder.Append(s2);
        stringBuilder.Append(s3);
        return stringBuilder.ToString();
    }
    
    /// <summary>
    /// 合并字符串
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <param name="s3"></param>
    /// <returns></returns>
    public static string Concat(params object[] args)
    {
        if (stringBuilder.Length > 0)
            stringBuilder.Remove(0, stringBuilder.Length);
        foreach (var VARIABLE in args)
        {
              stringBuilder.Append(VARIABLE);
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 移除前缀字符串
    /// </summary>
    /// <param name="self"></param>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string RemovePrefixString(this string self, string str)
    {
        string strRegex = @"^(" + str + ")";
        return Regex.Replace(self, strRegex, "");
    }

    /// <summary>
    /// 移除后缀字符串
    /// </summary>
    /// <param name="self"></param>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string RemoveSuffixString(this string self, string str)
    {
        string strRegex = @"(" + str + ")" + "$";
        return Regex.Replace(self, strRegex, "");
    }

    /// <summary>
    /// 匹配正则表达式
    /// </summary>
    /// <param name="slef"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static bool RegexMatch(this string slef, string pattern)
    {
        Regex reg = new Regex(pattern);
        return reg.Match(slef).Success;
    }

    /// <summary>
    /// 是否为Email
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsEmail(this string self)
    {
        return self.RegexMatch(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
    }

    /// <summary>
    /// 是否为域名
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsDomain(this string self)
    {
        return self.RegexMatch(@"[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(/.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+/.?");
    }

    /// <summary>
    /// 是否为域名
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsIP(this string self)
    {
        return self.RegexMatch(@"((?:(?:25[0-5]|2[0-4]\\d|[01]?\\d?\\d)\\.){3}(?:25[0-5]|2[0-4]\\d|[01]?\\d?\\d))");
    }

    /// <summary>
    /// 是否为手机号码
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsMobilePhone(this string self)
    {
        return self.RegexMatch(@"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$");
    }

    /// <summary>
    /// 转换为MD5, 加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
    /// </summary>
    /// <param name="self"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static string ConvertToMD5(this string self, string flag = "x2")
    {
        byte[] sor = Encoding.UTF8.GetBytes(self);
        MD5 md5 = MD5.Create();
        byte[] result = md5.ComputeHash(sor);
        StringBuilder strbul = new StringBuilder(40);
        for (int i = 0; i < result.Length; i++)
        {
            strbul.Append(result[i].ToString(flag));
        }
        return strbul.ToString();
    }

    /// <summary>
    /// 转换为32位MD5
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string ConvertToMD5_32(this string self)
    {
        return ConvertToMD5(self, "x2");
    }

    /// <summary>
    /// 转换为48位MD5
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string ConvertToMD5_48(this string self)
    {
        return ConvertToMD5(self, "x3");
    }

    /// <summary>
    /// 转换为64位MD5
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string ConvertToMD5_64(this string self)
    {
        return ConvertToMD5(self, "x4");
    }

}