using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WoogiWorld.AssetBundles
{
    public partial class ABManager : MonoBehaviour
    {
        public ABLog log = (ABLog)4;

        string m_BaseDownloadingURL = "";
        string[] m_ActiveVariants = { };
        AssetBundleManifest m_AssetBundleManifest = null;
        Dictionary<string, ABLoaded> m_LoadedAssetBundles = new Dictionary<string, ABLoaded>();

        Dictionary<string, ABAsyncLoading> m_DownloadingRequests =
            new Dictionary<string, ABAsyncLoading>();

        Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string>();
        List<ABLoadOperation> m_InProgressOperations = new List<ABLoadOperation>();
        Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();

        public void ResetBaseDownloadingURL()
        {
            if (m_BaseDownloadingURL != ABUtility.DataPath)
            {
                m_BaseDownloadingURL = ABUtility.DataPath;
                WDebug.LogFormat("Set ABUrl:{0}", m_BaseDownloadingURL);
            }
        }

        // The base downloading url which is used to generate the full downloading url with the assetBundle names.
        public string BaseDownloadingURL
        {
            get { return m_BaseDownloadingURL; }
            set { m_BaseDownloadingURL = value; }
        }

        // Variants which is used to define the active variants.
        public string[] ActiveVariants
        {
            get { return m_ActiveVariants; }
            set { m_ActiveVariants = value; }
        }

        public int CacheABAmount
        {
            get { return m_LoadedAssetBundles.Count; }
        }

        void OnDestroy()
        {
            m_AssetBundleManifest = null;
            if (m_DownloadingRequests != null)
            {
                m_DownloadingRequests.Clear();
            }

            //if (loadedAssetBundles != null) { loadedAssetBundles.Clear(); }
            if (m_LoadedAssetBundles != null)
            {
                m_LoadedAssetBundles.Clear();
            }

            if (m_InProgressOperations != null)
            {
                m_InProgressOperations.Clear();
            }
        }

        // AssetBundleManifest object which can be used to load the dependecies and check suitable assetBundle variants.
        public AssetBundleManifest AssetBundleManifestObject
        {
            set { m_AssetBundleManifest = value; }
        }

        public AssetBundleManifest GetAssetBundleManifest
        {
            get { return m_AssetBundleManifest; }
        }

        public bool HasAssetBundleManifest
        {
            get { return m_AssetBundleManifest; }
        }

        public void Clear(bool isForceRelease = false)
        {
            List<string> keys = new List<string>(m_LoadedAssetBundles.Keys);
            ABLoaded abLoad = null;
            for (int i = 0; i < keys.Count; i++)
            {
                if (m_LoadedAssetBundles.ContainsKey(keys[i]))
                {
                    abLoad = m_LoadedAssetBundles[keys[i]];
                    if (isForceRelease)
                    {
                        abLoad.ForceRelease();
                        UnloadAssetBundle(keys[i]);
                    }
                    else if (abLoad.GetReferencedCount == 0)
                        UnloadAssetBundle(keys[i]);

                    if (m_LoadedAssetBundles.ContainsKey(keys[i]))
                    {
                        UnLoadAssetBundle(keys[i], abLoad);
                    }
                }
            }
        }

        public void SetSourceAssetBundleDirectory(string relativePath)
        {
            BaseDownloadingURL = ABUtility.GetStreamingAssetsPath() + relativePath;
        }

        public void SetSourceAssetBundleURL(string absolutePath)
        {
            BaseDownloadingURL = absolutePath + ABUtility.GetPlatformName() + "/";
        }

        public void SetDevelopmentAssetBundleServer()
        {
            TextAsset urlFile = Resources.Load("AssetBundleServerURL") as TextAsset;
            string url = (urlFile != null) ? urlFile.text.Trim() : null;
            if (url == null || url.Length == 0)
            {
                WDebug.LogWarning("Development Server URL could not be found.");
            }
            else
            {
                SetSourceAssetBundleURL(url);
            }
        }

        public static ABManager CreateABManager()
        {
            var go = new GameObject("ABManager", typeof(ABManager));
            DontDestroyOnLoad(go);
            ABManager abs = go.GetComponent<ABManager>();
            return abs;
        }

        void Update()
        {
            if (logMask != log)
                logMask = log;
            AsyncUpdate();
        }
    }
}