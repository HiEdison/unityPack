using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Runtime.InteropServices;

static public class WoogiTools
{

    static public void SetDirty(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        if (obj)
        {
            UnityEditor.EditorUtility.SetDirty(obj);
        }
#endif
    }

    static public bool fileAccess
    {
        get
        {
            //return Application.platform != RuntimePlatform.WindowsWebPlayer &&
            //Application.platform != RuntimePlatform.OSXWebPlayer;
            return Application.platform != RuntimePlatform.WebGLPlayer;
        }
    }

    static public byte[] Load(string fileName)
    {
#if UNITY_WEBPLAYER || UNITY_FLASH || UNITY_METRO || UNITY_WP8 || UNITY_WP_8_1
		return null;
#else
        if (!fileAccess)
            return null;

        string path = Application.persistentDataPath + "/" + fileName;

        if (File.Exists(path))
        {
            return File.ReadAllBytes(path);
        }
        return null;
#endif
    }

    static public string LoadText(string fileName)
    {
#if UNITY_WEBPLAYER || UNITY_FLASH || UNITY_METRO || UNITY_WP8 || UNITY_WP_8_1
		return null;
#else
        if (!fileAccess)
            return null;

        string path = Application.persistentDataPath + "/" + fileName;

        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        return null;
#endif
    }

    static public string EncodeColor(Color c)
    {
        int i = 0xFFFFFF & (ColorToInt(c) >> 8);
        return DecimalToHex24(i);
    }

    [System.Diagnostics.DebuggerHidden]
    [System.Diagnostics.DebuggerStepThrough]
    static public int ColorToInt(Color c)
    {
        int retVal = 0;
        retVal |= Mathf.RoundToInt(c.r * 255f) << 24;
        retVal |= Mathf.RoundToInt(c.g * 255f) << 16;
        retVal |= Mathf.RoundToInt(c.b * 255f) << 8;
        retVal |= Mathf.RoundToInt(c.a * 255f);
        return retVal;
    }

    [System.Diagnostics.DebuggerHidden]
    [System.Diagnostics.DebuggerStepThrough]
    static public string DecimalToHex24(int num)
    {
        num &= 0xFFFFFF;
#if UNITY_FLASH
		StringBuilder sb = new StringBuilder();
		sb.Append(DecimalToHexChar((num >> 20) & 0xF));
		sb.Append(DecimalToHexChar((num >> 16) & 0xF));
		sb.Append(DecimalToHexChar((num >> 12) & 0xF));
		sb.Append(DecimalToHexChar((num >> 8) & 0xF));
		sb.Append(DecimalToHexChar((num >> 4) & 0xF));
		sb.Append(DecimalToHexChar(num & 0xF));
		return sb.ToString();
#else
        return num.ToString("X6");
#endif
    }

    public static string EncodeStringColor(string str, string colorHex)
    {
        return "<color=#" + colorHex + ">" + str + "</color>";
    }

    //����㷨�Ǻ����˴�Сд��
    public static string  GetReplaceStrByLower(string srcStr, string findKey)
    {
        Dictionary<string,bool> result = new Dictionary<string, bool>();
        string titleLower = srcStr.ToLower();
        string inputLower = findKey.ToLower();
        int inputLen = findKey.Length;
        while (true)
        {
            int index = titleLower.LastIndexOf(inputLower);
            if (index == -1)
                break;
            string res = srcStr.Substring(index, inputLen);
            result[res] = true;
            titleLower = titleLower.Substring(0, index);
        }
        foreach (string key in result.Keys)
        {
            string newReplaceStr = "<color=#00ff00>" + key + "</color>";
            srcStr = srcStr.Replace(key, newReplaceStr);
        }
        return srcStr;
    }

    public static void AddSomeItem(int curCount, GameObject mItem, List<GameObject> curList)
    {//ֻ���� ������
        int needAdd = curCount - curList.Count;
        if (needAdd > 0)
        {
            Transform p = mItem.transform.parent;
            for (int i = 0; i < needAdd; i++)
            {
                GameObject go = GameObject.Instantiate(mItem) as GameObject;
                go.transform.SetParent(p);
                go.transform.localScale = Vector3.one;
                curList.Add(go);
            }
        }
    }

    static Material grayMaterial = null;

    public static void SetSpriteGray(Image img)
    {//��һ�� gray�� material
        if (grayMaterial == null)
        {
            grayMaterial = Resources.Load<Material>("Export/UI/Material/ClipGray");
        }
        img.material = grayMaterial;
    }

    public static string[] GetStrFromRichText(string Text)
    {
        string[] result = new string[2];
        int btnTextIndedx = Text.IndexOf("[%");
        if (btnTextIndedx < 0)
        {
            result[0] = Text;
            result[1] = "";
        }
        else
        {
            result[0] = Text.Substring(0, btnTextIndedx);
            result[1] = Text.Substring(btnTextIndedx + 2, Text.LastIndexOf(']') - btnTextIndedx - 2);
        }
        return result;
    }

    #if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern string getIPv6( string mHost, string mPort );
#endif

    private static string GetIPv6(string mHost, string mPort)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
		string mIPv6 = getIPv6(mHost, mPort);
		return mIPv6;
#else
        if (mHost.Contains(":"))
            return mHost + "&&ipv6";
        return mHost + "&&ipv4";
#endif
    }

    public static void getIPType(String serverIp, String serverPorts, out String newServerIp, out AddressFamily mIPType)
    {
        mIPType = AddressFamily.InterNetwork;
        newServerIp = serverIp;
        try
        {
            string mIPv6 = GetIPv6(serverIp, serverPorts);
          //  WDebug.Log("[getIPType] " + mIPv6);
            if (!string.IsNullOrEmpty(mIPv6))
            {
                string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
                if (m_StrTemp != null && m_StrTemp.Length >= 2)
                {
                    string IPType = m_StrTemp[1];
                    if (IPType == "ipv6")
                    {
                     //   WDebug.Log("[getIPType] this is ipv6");
                        newServerIp = m_StrTemp[0];
                        mIPType = AddressFamily.InterNetworkV6;
                    }
                }
            }
        }
        catch (Exception e)
        {
            WDebug.LogError(e.Message);
        }
    }

    public static string GetNewIp(String serverIp, int serverPorts)
    {
        if (serverIp.Contains(":"))//ipv6�ĵ�ֱַ�ӷ���
            return serverIp;
        String newServerIp = "";
        AddressFamily newAddressFamily = AddressFamily.InterNetwork;
        getIPType(serverIp, serverPorts.ToString(), out newServerIp, out newAddressFamily);
        if (!string.IsNullOrEmpty(newServerIp))
        {
            serverIp = newServerIp;
        }
        //WDebug.LogError("BBB: " + serverIp);
        if (newAddressFamily == AddressFamily.InterNetworkV6)
        {
            return "[" + serverIp + "]";
        }
        return serverIp;
    }

    public static string GetNewWebV6Url(string url)
    {
        try
        {
            Uri u = new Uri(url);//ͨ��URL��ת���ж�ip
            if (u.HostNameType == UriHostNameType.Dns)
                return url;
            if (u.HostNameType == UriHostNameType.IPv4)
            {
                //���� ios��ip�����õ�һ���µ� url
                string newIp = GetNewIp(u.DnsSafeHost, u.Port);
                return url.Replace(u.DnsSafeHost, newIp);
            }
            if (u.HostNameType == UriHostNameType.IPv6 && !url.Contains("["))
            {
                WDebug.LogWarning("Erro url");   
            }
        }
        catch (Exception e)
        {
            WDebug.LogError(url + " " + e.Message);
        }
        return url;
    }

    public static GameObject InstialObject(GameObject go, Transform parent)
    {
        GameObject result = GameObject.Instantiate(go) as GameObject;
        result.transform.localPosition = Vector3.zero;
        result.transform.localEulerAngles = Vector3.zero;
        result.transform.localScale = Vector3.one;
        result.transform.parent = parent;
        return result;
    }

    public static string GetObjectPath(GameObject go)
    {
        if (go == null)
            return "";
        string result = go.name;
        Transform gg = go.transform;
        while (gg.parent != null)
        {
            result = gg.transform.parent.name + "/" + result;
            gg = gg.transform.parent;
        }
        return result;
    }
}