using UnityEngine;
using System.Collections.Generic;
namespace WoogiWorld.UI
{
    public class UIBase : MonoBehaviour
    {
        public object Data { get; set; }

        protected Dictionary<string, T> GetGameObject<T>() where T : MonoBehaviour
        {
            Dictionary<string, T> list = new Dictionary<string, T>();
            List<T> tlist = new List<T>();
            T wp = transform.GetComponent<T>();
            if (wp != null)
            {
                tlist.Add(wp);
            }
            if (transform.childCount > 0)
                GetAll<T>(transform, ref tlist);
            if (tlist != null)
            {
                for (int i = 0; i < tlist.Count; ++i)
                {
                    if (list.ContainsKey(tlist[i].gameObject.name) && list[tlist[i].gameObject.name] != null)
                    {
#if  UNITY_EDITOR
                        WDebug.LogWarning($"<color>GameObject name  Repeat =>object:{ tlist[i].gameObject.name} , root:{transform.name}");
#endif
                    }
                    else
                        list.Add(tlist[i].gameObject.name, tlist[i]);
                }
            }
            return list;
        }

        private void GetAll<T>(Transform tf, ref List<T> tlist) where T : MonoBehaviour
        {
            foreach (Transform childTf in tf)
            {
                T wp = childTf.GetComponent<T>();
                if (wp != null)
                {
                    tlist.Add(wp);
                }
                if (childTf.childCount > 0)
                    GetAll<T>(childTf, ref tlist);
            }
        }

        protected T Find<T>(Dictionary<string, T> dic, string objectname) where T : MonoBehaviour
        {
            if (dic == null)
                dic = GetGameObject<T>();
            T _t = default(T);
            if (dic.ContainsKey(objectname) && dic[objectname] != null)
            {
                _t = dic[objectname] as T;
            }
            return _t;
        }

        protected bool CheckIsExist<T>(string key, Dictionary<string, T> dic) where T : Component
        {
            if (dic != null)
            {
                if (dic.ContainsKey(key))
                {
                    if (dic[key] != null)
                    {
                        return true;
                    }
                    else
                    {
                        dic.Remove(key);
                    }
                }
            }
            return false;
        }
    }
}


