using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using WoogiWorld.AssetBundles;

public class CRCCachePool
{
    static Dictionary<string, string> crcDir = new Dictionary<string, string>();

    public static bool CheckCRC(string key, string crc)
    {
        if (crcDir.TryGetValue(key, out var v))
        {
            return crc.Equals(v);
        }

        return false;
    }

    public static IEnumerator Request()
    {
        UnityWebRequest request =
            UnityWebRequest.Get(string.Concat(ABUtility.DataPath, "/crcmainfest", ABConst.SUFFIX));
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return 0;
        }

        if (!string.IsNullOrEmpty(request.downloadHandler.text))
            crcDir = Json.DeserializeJson<Dictionary<string, string>>(request.downloadHandler.text);

        if (crcDir == null)
            crcDir = new Dictionary<string, string>();
        yield return 0;
    }
}