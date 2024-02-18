using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace WoogiWorld.AssetBundles
{
    public class ABAsyncLoading
    {
        UnityWebRequest request = null;
        private AssetBundleCreateRequest _request;
        private bool isMuDone = false;
        public ABManager.waitRequest waitRequest { get; private set; }

        public ABAsyncLoading(ABManager.waitRequest m_waitRequest)
        {
            m_waitRequest.reTryCount--;
            waitRequest = m_waitRequest;

#if UNITY_WEBGL || ALL_IN_ONE_WEBGL
            //Feature #17692 unity datacacheing
            string temp = string.Empty;
#if !UNITY_EDITOR
            if (WPlatform.dataCaching)
            {
                temp = $"{m_waitRequest.url}?v={GMS.WPlatform.version}.{GMS.WPlatform.buildNumber}";
            }
            else
#endif
            {
                temp = m_waitRequest.url;
            }
          //  WDebug.Log(temp);
          //wld  小游戏中从portal进入，有缓存，更新后无法下载到最新资源，临时加个变量
            temp = $"{temp}?t= {System.DateTime.Now.Ticks / 10000}";
            if (ABConst.isAB_Version)
            {
                request = UnityWebRequestAssetBundle.GetAssetBundle(temp, ABConst.AB_Version, 0);
            }
            else
            {
                request = UnityWebRequestAssetBundle.GetAssetBundle(temp);
            }
#else
            if (File.Exists(m_waitRequest.localFullPath))
            {
                if (CRCCachePool.CheckCRC(CRC32(m_waitRequest.abPath),
                    CRC32File(m_waitRequest.localFullPath)))
                {
                    isMuDone = true;
                    SendWebRequest();

                    return;
                }
            }

            request = UnityWebRequest.Get(m_waitRequest.url);
            request.downloadHandler = new ABDownloadHandler(m_waitRequest.localFullPath);
#endif
            SendWebRequest();
        }

        public void SendWebRequest()
        {
            
            if (request != null) request.SendWebRequest();
#if !UNITY_WEBGL
            if (isMuDone)
            {
                // WDebug.LogFormat("[ABAsyncLoading] load local file:{0}", waitRequest.localFullPath);
                _request = AssetBundle.LoadFromFileAsync(waitRequest.localFullPath);
            }
#endif
        }

        public string url
        {
            get { return waitRequest.url; }
        }

        public AssetBundle assetbundle
        {
            get
            {
                try
                {
#if UNITY_WEBGL || ALL_IN_ONE_WEBGL
                    if (request != null && request.isDone)
                        return DownloadHandlerAssetBundle.GetContent(request);
#else
                    if (isMuDone)
                    {
                        if (_request.isDone)
                            return _request.assetBundle;
                    }
                    else
                    {
                        if (request.isDone)
                            return (request.downloadHandler as ABDownloadHandler).assetbundle;
                    }
#endif
                }
                catch (Exception e)
                {
                    WDebug.LogErrorFormat($"[ABAsyncLoading.assetbundle] exc:{e.Message},{waitRequest.abPath}");
                }

                return null;
            }
        }

        public bool isDone
        {
            get { return (_request != null && _request.isDone) || (request != null && request.isDone); }
        }

        public void Dispose()
        {
            _request = null;
            if (request != null) request.Dispose();
            request = null;
        }

        public string error
        {
            get
            {
#if UNITY_EDITOR
//                //模拟多次下载失败          
//                if (waitRequest.reTryCount > 1)
//                {
//                    return "test download error!";
//                }
#endif

                return request != null ? request.error : string.Empty;
            }
        }

        public bool isHttpError
        {
            get { return request != null ? request.isHttpError : false; }
        }

        public bool isNetworkError
        {
            get { return request != null ? request.isNetworkError : false; }
        }

        public bool isReTry
        {
            get { return waitRequest.reTryCount > 0; }
        }

#if !UNITY_WEBGL && !ALL_IN_ONE_WEBGL
        public static string CRC32File(string path)
        {
            String hash = String.Empty;
            if (File.Exists(path))
            {
                DamienG.Security.Cryptography.Crc32 crc32 = new DamienG.Security.Cryptography.Crc32();
                byte[] fileData = crc32.ComputeHash(File.ReadAllBytes(path));
                foreach (byte b in fileData)
                {
                    hash += b.ToString("x2").ToLower();
                }
            }

            return hash;
        }

        public static string CRC32(string data)
        {
            String hash = String.Empty;
            if (!string.IsNullOrEmpty(data))
            {
                DamienG.Security.Cryptography.Crc32 crc32 = new DamienG.Security.Cryptography.Crc32();
                byte[] fileData = Encoding.UTF8.GetBytes(data);
                fileData = crc32.ComputeHash(fileData);
                foreach (byte b in fileData)
                {
                    hash += b.ToString("x2").ToLower();
                }
            }

            return hash;
        }
#endif
    }
}