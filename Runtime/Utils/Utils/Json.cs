using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System;

public class Json
{
    public static T DeserializeJson<T>(string jsonString, bool needDecrypt = false, bool isLog = false)
    {
        if (jsonString == default(string) || jsonString == "") return default(T);
        if (needDecrypt) jsonString = ResourcesEncryption.AES_Decrypt(jsonString);
        try
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        catch (Exception e) 
        {
            if (!isLog) WDebug.LogError(e.Message + ",  json:" + jsonString); 

            return default(T);
        }
    }

    public static string ReadFile(string path)
    {
        if (!FindFile(path))
        {
            WDebug.Log("Can't Find File :" + path);
            return default(string);
        }

        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception e)
        {
            WDebug.Log(e.Message);
            return default(string);
        }
    }

    public static bool FindFile(string path)
    {
        string directoryPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryPath)) return false;
        return File.Exists(path);
    }

    public static T ReadJsonFile<T>(string path, bool needDecrypt = false)
    {
        return DeserializeJson<T>(ReadFile(path), needDecrypt);
    }


    public static byte[] WriteNetData(System.Object o)
    {
        //对接收的消息：加密+加压
        //double baseKey = 2015032615;
        //string key = (baseKey * 2 - 2) + "";
        string
            sendString =
                JsonConvert.SerializeObject(
                    o); // ResourcesEncryption.AES_PHPEncryptAndCompress (JsonConvert.SerializeObject(o), key);      
        byte[] msg = Encoding.UTF8.GetBytes(sendString);
        return msg;
    }

    public static string SerializerJson<T>(T jsonData, bool needEncrypt = false, bool prettyJson = false,
        bool isLog = false)
    {
        if (jsonData == null) return default(string);
        string jsonString = "";
        try
        {
            jsonString = JsonConvert.SerializeObject(jsonData);
        }
        catch (Exception e)
        {
            if (!isLog) WDebug.Log(e.Message);
            return default(string);
        }

        if (needEncrypt) jsonString = ResourcesEncryption.AES_Encrypt(jsonString);
        return jsonString;
    }

    public static string SerializerJson(object jsonData, string cmdStr, bool needEncrypt = false,
        bool prettyJson = false, bool isLog = false)
    {
        if (jsonData == null) return "{\"cmd\":\"" + cmdStr + "\"}";
        string jsonString = "";
        try
        {
            jsonString = JsonConvert.SerializeObject(jsonData);
            jsonString = "{\"cmd\":\"" + cmdStr + "\"," + jsonString.Substring(1);
        }
        catch (Exception e)
        {
            if (!isLog) WDebug.Log(e.Message);
            return null;
        }

        if (needEncrypt) jsonString = ResourcesEncryption.AES_Encrypt(jsonString);
        return jsonString;
    }

    public class Error
    {
        public int error = 0;
    }

    public static bool CheckErro(string jsonModel)
    {
        Error e = DeserializeJson<Error>(jsonModel);
        if (e.error == 0)
            return false;
        WDebug.Log("error num : " + e.error + ",Please Check this msg");
        return true;
    }

    public static bool WriteJson<T>(T jsonData, string path, bool needEncrypt = false, bool byLine = false,
        bool prettyJson = false)
    {
        string jsonStr = SerializerJson<T>(jsonData, needEncrypt, prettyJson);
        string directoryPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        try
        {
            if (byLine)
            {
                StreamWriter sw = new StreamWriter(path, true);
                sw.WriteLine(jsonStr);
                sw.Close();
            }
            else
            {
                //  if (File.Exists(path)) File.Delete(path);
                File.WriteAllText(path, jsonStr);
            }
        }
        catch (Exception e)
        {
            WDebug.Log(e.Message);
            return false;
        }

        return true;
    }
    public static string SerializerJsonValue(string _json, string _key)
    {
        if (!string.IsNullOrEmpty(_json))
        {
            int _pos = _json.IndexOf(_key + "\":\"");
            if (_pos > -1)
            {
                _json = _json.Substring(_pos + _key.Length + "\":\"".Length);
                _pos = _json.IndexOf("\"");
                if (_pos > -1)
                {
                    _json = _json.Substring(0, _pos);
                    return _json;
                }
            }
            _pos = _json.IndexOf(_key + "\":");
            if (_pos > -1)
            {
                _json = _json.Substring(_pos + _key.Length + "\":".Length);
                _pos = _json.IndexOf(",");
                if (_pos > -1)
                {
                    _json = _json.Substring(0, _pos);
                    return _json;
                }
                _pos = _json.IndexOf("}");
                if (_pos > -1)
                {
                    _json = _json.Substring(0, _pos);
                    return _json;
                }
            }
        }
        return "";
    }
}