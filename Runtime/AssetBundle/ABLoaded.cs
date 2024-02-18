using UnityEngine;

namespace WoogiWorld.AssetBundles
{
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class ABLoaded
    {
        public AssetBundle m_AssetBundle;
        private int m_ReferencedCount;
        private string key;
        ~ABLoaded()
        {
            m_AssetBundle = null;
        }
        public ABLoaded(string key,AssetBundle assetBundle)
        {
            m_AssetBundle = assetBundle;
            m_ReferencedCount = 1;
            this.key = key;
        }

        public int SetReferencedCount(bool isAdd)
        {
            if (isAdd)
                m_ReferencedCount++;
            else
                m_ReferencedCount--;
            return m_ReferencedCount;
        }

        public int GetReferencedCount
        {
            get { return m_ReferencedCount; }
        }

        public void ForceRelease()
        {
            if (m_ReferencedCount > 1)
                m_ReferencedCount = 1;
        }
    }
}