using UnityEngine;
using UnityEngine.SceneManagement;

namespace WoogiWorld.AssetBundles
{
    public class ABLoadLevelOperation : ABLoadOperation
    {
        protected string m_AssetBundleName;
        protected string m_LevelName;
        protected LoadSceneMode m_IsAdditive;
        protected string m_DownloadingError;
        protected AsyncOperation m_Request;
        protected ABManager m_abMgr;

        ~ABLoadLevelOperation()
        {
            m_Request = null;
        }

        public ABLoadLevelOperation(string assetbundleName, string levelName, LoadSceneMode mode, ABManager abMgr)
        {
            m_AssetBundleName = assetbundleName;
            m_LevelName = levelName;
            m_IsAdditive = mode;
            m_abMgr = abMgr;
        }

        public override bool Update()
        {
            ABLoaded bundle = m_abMgr.GetLoadedAssetBundle(m_AssetBundleName, out m_DownloadingError);
            if (bundle != null)
            {
                m_Request = SceneManager.LoadSceneAsync(m_LevelName, m_IsAdditive);
                return false;
            }
            else
            {
                isError = true;
                error = m_DownloadingError;
            }

            return true;
        }

        public override bool IsDone()
        {
            if (m_Request == null && m_DownloadingError != null)
            {
                return true;
            }

            return m_Request != null && m_Request.isDone;
        }

        public override bool IsRequestNull()
        {
            return false;
        }
    }
}