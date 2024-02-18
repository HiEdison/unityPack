using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WoogiWorld.UI;

public enum ParentType
{
    canvas,
    plane,
}

public enum Method
{
    setButton,
    setButtonInteractable,
    setText,
    setImg,
    setScrollBarEvent,
    setToggleEvent,
    setSliderEvent,
}

public sealed class UIManager
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = new UIManager();
            return instance;
        }
    }

    private Dictionary<string, WoogiPlane> planeDictionary = new Dictionary<string, WoogiPlane>();
    private Dictionary<string, WoogiCanvas> canvasDictionary = new Dictionary<string, WoogiCanvas>();
    private Dictionary<string, List<ArrayList>> registerDic = new Dictionary<string, List<ArrayList>>();
    private Camera mainCamera;

    //RossCam
    //This may not be acceptable as even though we only have a getter, the internals of the dictionary could be modified :(
    public Dictionary<string, WoogiCanvas> CanvasDictionary => canvasDictionary;

    #region region and destory

    ~UIManager()
    {
        instance = null;
        if (OpenPanle != null)
        {
            OpenPanle.Clear();
        }

        if (registerDic != null)
        {
            registerDic.Clear();
        }

        if (planeDictionary != null)
        {
            planeDictionary.Clear();
        }

        if (canvasDictionary != null)
        {
            canvasDictionary.Clear();
        }
    }


    public void DestroyCanvas(string cavasName, bool isAuto = false)
    {
        if (Destory<WoogiCanvas>(ref canvasDictionary, cavasName, isAuto))
        {
        }

        //    WDebug.Log("destory canvas:" + cavasName + " succeed!");
        //else
        //    WDebug.Log("destory canvas:" + cavasName + " failure!");
    }

    internal void RegisterPlane(string name, WoogiPlane plane)
    {
        Register<WoogiPlane>(ref planeDictionary, name, plane);
    }

    internal void RegisterCanvas(string name, WoogiCanvas canvas)
    {
        Register<WoogiCanvas>(ref canvasDictionary, name, canvas);
    }

    private void Register<T>(ref Dictionary<string, T> dictionary, string name, T t) where T : MonoBehaviour
    {
        if (dictionary == null)
            dictionary = new Dictionary<string, T>();
        if (dictionary.ContainsKey(name))
        {
            dictionary[name] = t;
        }
        else
        {
            dictionary.Add(name, t);
        }
    }

    private bool Destory<T>(ref Dictionary<string, T> dictionary, string name, bool isAuto) where T : MonoBehaviour
    {
        if (dictionary == null)
            return false;
        if (dictionary.ContainsKey(name))
        {
            T t = default(T);
            if (dictionary[name] == null)
            {
                dictionary.Remove(name);
                return false;
            }

            if (!isAuto)
            {
                t = dictionary[name];
                GameObject.Destroy(t.gameObject);
            }

            dictionary[name] = null;
            dictionary.Remove(name);
            return true;
        }

        return false;
    }

    #endregion

    #region get canvas and plane

    public WoogiPlane GetPlane(string pName, bool b = false)
    {
        WoogiPlane plane;
        if (GetTPlane(out plane, pName))
        {
        }

        return plane;
    }

    public GameObject GetPlaneGameObject(string planeName)
    {
        WoogiPlane plane = GetPlane(planeName);
        if (plane != null)
            return plane.gameObject;
        return null;
    }

    public Transform GetPlaneTransform(string planeName)
    {
        WoogiPlane plane = GetPlane(planeName);
        if (plane != null)
            return plane.transform;
        return null;
    }

    public RectTransform GetPlaneRectTransform(string planeName)
    {
        WoogiPlane plane = GetPlane(planeName);
        if (plane != null)
            return plane.GetComponent<RectTransform>();
        return null;
    }

    /// <summary>
    /// genericity only both class type : Transfrom and GameObject
    /// </summary>
    /// <typeparam name="T">Transfrom or GameObject</typeparam>
    /// <param name="name">object name</param>
    /// <returns>T</returns>
    public T GetPlane<T>(string name) where T : Component
    {
        return GetObject<T>(GetObject<WoogiPlane>(planeDictionary, name), name);
    }

    public List<string> GetAllCanvasKeys()
    {
        return new List<string>(canvasDictionary.Keys);
    }

    /// <summary>
    /// genericity only both class type : Transfrom and GameObject
    /// </summary>
    /// <typeparam name="T">Transfrom or GameObject</typeparam>
    /// <param name="name">object name</param>
    /// <returns>T</returns>
    public T GetCanvas<T>(string name) where T : Component
    {
        return GetObject<T>(GetObject<WoogiCanvas>(canvasDictionary, name), name);
    }

    public GameObject GetCanvasGo(string name)
    {
        Transform tf = GetCanvas<Transform>(name);
        if (tf != null) return tf.gameObject;
        return null;
    }

    public T GetObject<T>(string pName, string objName) where T : Component
    {
        return GetObject<T, string>(pName, objName);
    }

    public T GetObject<T, TPlane>(TPlane pName, string objName) where T : Component
    {
        GameObject go = this.GetGameObject<TPlane>(pName, objName);
        if (go != null)
        {
            return go.GetComponent<T>();
        }

        return default(T);
    }

    private T GetObject<T>(GameObject go, string name) where T : Component
    {
        if (go != null)
        {
            return go.GetComponent<T>();
        }

        return default(T);
    }

    private GameObject GetObject<T>(Dictionary<string, T> dictionary, string name) where T : Component
    {
        GameObject t = default(GameObject);
        if (dictionary == null)
            return t;
        if (dictionary.ContainsKey(name))
        {
            if (dictionary[name] != null)
                return dictionary[name].gameObject;
            else
                dictionary.Remove(name);
        }

        return t;
    }

    #endregion


    public bool GetTPlane<TPlane>(out WoogiPlane plane, TPlane planeName)
    {
        planeDictionary.TryGetValue(planeName.ToString(), out plane);
        return plane;
    }


    public static void CheckClosePlane(Transform tf, GameObject trigger, Action callback)
    {
        //if (tf && tf.gameObject.activeSelf)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //        RaycastHit hit;
        //        if (Physics.Raycast(ray, out hit))
        //        {
        //            if (trigger == hit.transform.gameObject)
        //                return;
        //        }
        //        GameObject go = EventSystem.current.currentSelectedGameObject;
        //        if (go == null && callback != null)
        //        {
        //            callback();
        //        }
        //        if (go == tf.gameObject || go == trigger) return;
        //        else if (!UIManager.FindGameObject(tf, go) && callback != null)
        //        {
        //            callback();
        //        }
        //    }
        //}
    }

    /// <summary>
    /// check current click object whether to be included
    /// </summary>
    /// <param name="obj">arrayList parent object</param>
    /// <param name="go">current click object</param>
    /// <returns></returns>
    public static bool FindGameObject(Transform obj, GameObject go)
    {
        int length = obj.childCount;
        for (int i = 0; i < length; i++)
        {
            Transform childGo = obj.GetChild(i);
            if (go == childGo.gameObject)
            {
                return true;
            }
            else
            {
                if (FindGameObject(childGo, go))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 抛弃 请使用 SpaceWorldPositionToUIWorldPosition
    /// </summary>
    /// <param name="transf"></param>
    /// <param name="worldGo"></param>
    /// <returns></returns>
    public static Vector3 WorldToUIPoint(Transform transf, Vector3 worldGo)
    {
        if (transf == null)
        {
            WDebug.LogError("WorldToUIPoint value is null!");
            return Vector2.zero;
        }

        Canvas canvas = transf.GetComponentInParent<Canvas>();
        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
        float offect = (Screen.width / canvasScaler.referenceResolution.x) * (1 - canvasScaler.matchWidthOrHeight) +
                       (Screen.height / canvasScaler.referenceResolution.y) * canvasScaler.matchWidthOrHeight;
        Vector2 a = RectTransformUtility.WorldToScreenPoint(Camera.main, worldGo);
        return new Vector2(a.x / offect, a.y / offect);
    }

    /// <summary>
    /// 3d 空间坐标映射到 屏幕的UI坐标.
    /// </summary>
    /// <param name="transf">show obj</param>
    /// <param name="worldPosition">空间坐标</param>
    /// <returns> UI world Position </returns>
    public static Vector3 SpaceWorldPositionToUIWorldPosition(Transform transf, Vector3 worldPosition)
    {
        if (transf == null)
        {
            WDebug.LogError("WorldToUIPoint Transform is null!");
            return Vector2.zero;
        }

        Canvas canvas = transf.GetComponentInParent<Canvas>();
        return SpaceWorldPositionToUIWorldPosition(canvas, worldPosition);
    }

    public static Vector3 SpaceWorldPositionToUIWorldPosition(Canvas canvas, Vector3 worldPosition)
    {
        if (canvas == null)
        {
            //WDebug.LogError("SpaceWorldToUIWorldPosition canvas is null!");
            return Vector2.zero;
        }

        if (instance.mainCamera == null) instance.mainCamera = Camera.main;
        if (instance.mainCamera != null)
        {
            Vector3 pos = instance.mainCamera.WorldToScreenPoint(worldPosition);
            if (canvas.worldCamera != null)
            {
                pos = canvas.worldCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, canvas.planeDistance));
            }

            return pos;
        }

        return Vector3.zero;
    }


    public void OnDestroy()
    {
        planeDictionary.Clear();
    }


    #region asynchronous

    internal Dictionary<string, GameObject> OpenPanle = new Dictionary<string, GameObject>();

    public void LoadAsyncGameObject(string path, Action<GameObject> callback)
    {
        //方便一次性修改
        ResMgr.LoadAsync(path, callback);
    }

    public void LoadAsyncPlane(string path, Transform parent, Action<GameObject> callback)
    {
        string panelName = path;
        if (CheckPlane(path, ref panelName, out var go))
        {
            callback?.Invoke(go);
            return;
        }

        if (parent != null)
        {
            LoadAsyncGameObject(path, (obj) =>
            {
                if (callback != null)
                {
                    if (obj != null)
                    {
                        GameObject go = GameObject.Instantiate(obj, parent) as GameObject;
                        go.transform.SetAsLastSibling();
                        go.name = go.name.Replace("(Clone)", "");
                        RectTransform rectTrans = go.transform.GetComponent<RectTransform>();
                        Vector3 oldPos = rectTrans.anchoredPosition3D;
                        Vector2 oldSize = rectTrans.sizeDelta;
                        rectTrans.sizeDelta = oldSize;
                        rectTrans.anchoredPosition3D = oldPos;
                        go.transform.localScale = Vector3.one;
                        if (!go.activeSelf)
                            go.GetComponent<WoogiPlane>().Regist();
                        OpenPanle[panelName] = go;
                        EventDispatcher.Instance.DispatchEvent(CommonEventType.NEW_LOADING_WINDOW);
                        callback.Invoke(go);
                    }
                }
            });
        }
    }

    bool CheckPlane(string path, ref string panelName, out GameObject go)
    {
        panelName = path;
        if (path.Contains("/"))
        {
            panelName = path.Substring(path.LastIndexOf("/")).Replace("/", "");
        }

        if (OpenPanle.ContainsKey(panelName))
        {
            if (OpenPanle[panelName] != null)
            {
                OpenPanle[panelName].transform.SetAsLastSibling();
                EventDispatcher.Instance.DispatchEvent(CommonEventType.NEW_LOADING_WINDOW);
                go = OpenPanle[panelName];
                return true;
            }
            else
            {
                OpenPanle.Remove(panelName);
            }
        }

        go = null;
        return false;
    }


    public void RemoveAllPanel()
    {
        // instance = null;
        //List<string> allPanleKey = new List<string>();
        //foreach( string key in OpenPanle.Keys )
        //{
        //    allPanleKey.Add(key);
        //}
        //foreach(string key in  allPanleKey)
        //{
        //    DestroyPlane(key);
        //}
    }

    public static void BindButtonEvent(MonoBehaviour recveiObj, WoogiButton btn, ButtonEvent _ButtonEvent)
    {
        if (btn == null)
        {
            WDebug.LogError("btn is null");
            return;
        }

        btn.SetEvent(recveiObj, UIEventType.mouseClick, _ButtonEvent);
    }

    public static void BindTransParentButtonEvent(TransparentButton btn, TransparentButton.ButtonEvent _ButtonEvent)
    {
        if (btn == null)
        {
            WDebug.LogError("btn is null");
            return;
        }

        btn.btEvent = _ButtonEvent;
    }
    #endregion
}