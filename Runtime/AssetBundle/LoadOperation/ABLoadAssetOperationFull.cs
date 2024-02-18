using UnityEngine;
using System.Collections;

namespace WoogiWorld.AssetBundles
{
    public class ABLoadAssetOperationFull : ABLoadAssetOperation
    {
        protected string m_AssetBundleName;
        protected string m_AssetName;
        protected string m_DownloadingError;
        protected System.Type m_Type;
        protected AssetBundleRequest m_Request = null;
        protected ABManager m_AbMgr;
        protected bool m_loadAssetAsyncOff = false;
        protected bool manualDone = false; //只加载ab，不加载asset的时候，使用它.
        ~ABLoadAssetOperationFull()
        {
            m_Type = null;
            m_Request = null;
        }
        
        public override bool IsRequestNull()
        {
            return m_Request != null && m_Request.asset != null;
        }

        public override string GetAssetbundlePath()
        {
            return m_AssetBundleName;
        }

        public ABLoadAssetOperationFull(string bundleName, string assetName, System.Type type, ABManager abMgr, bool _loadAssetAsyncOff)
        {
            m_AssetBundleName = bundleName;
            m_AssetName = assetName;
            m_Type = type;
            m_AbMgr = abMgr;
            m_loadAssetAsyncOff = _loadAssetAsyncOff;
        }

        public override T GetAsset<T>()
        {
            if (m_Request != null && m_Request.isDone)
            {
                //WDebug.Log(m_Request.asset);
                return m_Request.asset as T;
            }
            else
            {
                return null;
            }
        }

        public override bool Update()
        {
            if (m_Request != null)
                return false;
            ABLoaded bundle = m_AbMgr.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
            if (bundle != null)
            {
                if (!m_loadAssetAsyncOff)
                {
                    m_Request = bundle.m_AssetBundle.LoadAssetAsync(m_AssetName, m_Type);
                    if (!string.IsNullOrEmpty(m_DownloadingError))
                    {
                        WDebug.LogError(m_DownloadingError);
                    }
                }
                else
                {
                    manualDone = true;
                }

                return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(m_DownloadingError))
                {
                    isError = true;
                    error = m_DownloadingError;
                    return false;
                }

                return true;
            }
        }

        public override bool IsDone()
        {
            if (manualDone)
            {
                return true;
            }

            if (m_Request == null && m_DownloadingError != null)
            {
//                WDebug.LogError(m_DownloadingError);
                return true;
            }

            return m_Request != null && m_Request.isDone;
        }
    }

    public class AssetBundleLoadManifestOperation : ABLoadAssetOperationFull
    {
        public AssetBundleLoadManifestOperation(string bundleName, string assetName, System.Type type, ABManager abMgr)
            : base(bundleName, assetName, type, abMgr,false)
        {
        }

        public override bool Update()
        {
            base.Update();

            if (m_Request != null && m_Request.isDone)
            {
                m_AbMgr.AssetBundleManifestObject = GetAsset<AssetBundleManifest>();
             //   WDebug.Log($"UnloadAssetBundle:{m_AssetBundleName}");
                m_AbMgr.UnloadAssetBundle(m_AssetBundleName);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}