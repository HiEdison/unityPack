using System.Collections.Generic;
using UnityEngine;

namespace WoogiWorld.UI
{
    /// <summary>
    /// name : xxx_obsolete,  Ignore retrieving containing strings '_obsolete'
    /// </summary>
    public sealed class WoogiPlane : UIBase
    {
        private bool isRegist;
        #region 
        public void Regist()
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
            isRegist = true;
            UIManager.Instance.RegisterPlane(gameObject.name, this);

            if (isInitComplete) return;
            isInitComplete = true;
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(
            //delegate(object o)
            {
                woogiGameObject = GetGameObject<WoogiGameObject>();
            }
            // ));       
        }

        internal void OnDestroy()
        {
            //PanelManager.Instance.DestroyPanel(gameObject.name);
            //UIManager.Instance.DestroyPlane(gameObject.name, true);
            //WLD tips: 改了目录后这个不生效，暂时注释了
            //ResourceManager.UnloadAsset("export/panel/" + gameObject.name);
            if (woogiGameObject != null) { woogiGameObject.Clear(); }
        }
        #endregion
        private Dictionary<string, WoogiGameObject> woogiGameObject = new Dictionary<string, WoogiGameObject>();
        private bool isInitComplete = false;
        public void Awake()
        {
            if (!isRegist)
                Regist();
        }

        #region 设置激活状态
        /// <summary>
        /// set active
        /// </summary>
        /// <param name="objectname">gameobject name</param>
        /// <param name="value">bool value</param>
        public void SetActive(string objectname, bool value)
        {
            if (SetActive<WoogiGameObject>(woogiGameObject, objectname, value)) return;
            else
            {
                Transform tf = transform.Find(objectname);
                if (tf != null && tf.gameObject.activeSelf != value)
                    tf.gameObject.SetActive(value);
            }
        }

        /// <summary>
        /// set active
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="t">dictionary </param>
        /// <param name="objectname">gameobject name</param>
        /// <param name="value">bool value</param>
        /// <returns></returns>
        private bool SetActive<T>(Dictionary<string, T> t, string objectname, bool value) where T : MonoBehaviour
        {
            if (CheckIsExist<T>(objectname, t))
            {
                if (t[objectname].gameObject.activeSelf != value)
                    t[objectname].gameObject.SetActive(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// get active state 
        /// </summary>
        /// <param name="objectname">gameobject name</param>
        /// <returns>bool</returns>
        public bool ActiveSelf(string objectname)
        {
            if (ActiveSelf<WoogiGameObject>(woogiGameObject, objectname)) return true;
            else
            {
                Transform tf = transform.Find(objectname);
                if (tf != null)
                    return tf.gameObject.activeSelf;
            }
            return false;
        }
        /// <summary>
        /// get active state 
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="t">dictionary</param>
        /// <param name="objectname">gameobject name</param>
        /// <returns>bool</returns>
        private bool ActiveSelf<T>(Dictionary<string, T> t, string objectname) where T : MonoBehaviour
        {
            if (CheckIsExist<T>(objectname, t))
            {
                return t[objectname].gameObject.activeSelf;
            }
            return false;
        }
        #endregion

        #region get GameObject \ RectTramsform
        public GameObject GetGameObject(string objectname)
        {
            if (Find<WoogiGameObject>(woogiGameObject, objectname) != null)
            {
                return woogiGameObject[objectname].gameObject;
            }
            else
            {
                Transform tf = transform.Find(objectname);
                return tf != null ? tf.gameObject : null;
            }
        }

        public RectTransform GetRectTransform(string objectname)
        {
            GameObject go = GetGameObject(objectname);
            if (go != null)
                return go.GetComponent<RectTransform>();
            return null;
        }

        public Transform GetTransform(string objectname)
        {
            GameObject go = GetGameObject(objectname);
            if (go != null)
                return go.transform;
            return null;
        }

        #endregion
    }
}


