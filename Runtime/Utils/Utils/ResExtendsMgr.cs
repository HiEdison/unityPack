using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WoogiWorld.AssetBundles;
using Object = UnityEngine.Object;

public class ResExtendsMgr : SingletonBehaviour<ResExtendsMgr>
{
    public static bool isUsedAB = true;
    public static ABManager abMgr;
    public bool ABLoadFinish { get; set; }

    public static bool isReady
    {
        get { return hasInstance & Instance.ABLoadFinish; }
    }

    public bool isCustomUrl = false;

    /// <summary>
    /// assetbundle root path
    /// </summary>
    public string customUrl = string.Empty;

    public static void Setparameter(bool _isCustomUrl, string _customUrl)
    {
        if (hasInstance)
        {
            WDebug.Log($"GMS.Extends Setparameter: isCustom = {_isCustomUrl},{_customUrl}");
            Instance.isCustomUrl = _isCustomUrl;
            Instance.customUrl = _customUrl;
        }
    }

    protected override void OnDestroy()
    {
        abMgr = null;
        base.OnDestroy();
    }

    private IEnumerator Start()
    {
        yield return 0;
        while (!ResMgr.isReady) //等待ab 加载完成.
        {
            yield return 0;
        }

        yield return 0;
 
        abMgr = ABManager.CreateABManager();
        abMgr.isUsedCustionUrl = true;
        if (isCustomUrl && !string.IsNullOrEmpty(customUrl))
        {
            abMgr.UsedCustionUrl = customUrl;
        }
        else
        {
            abMgr.UsedCustionUrl = customUrl + ABConst.ASSET_DIRNAME;
        }

        customUrl = abMgr.UsedCustionUrl;
        if (isUsedAB)
        {
            AssetBundleLoadManifestOperation dd = abMgr.Initialize();
            yield return dd;

            if (!abMgr.HasAssetBundleManifest)
            {
                WDebug.Log("AssetBundleManifest load fail！");
            }
            else
            {
                ABLoadFinish = true;
                //WDebug.Log("AssetBundleManifest load finished！");
            }
        }
        else
        {
            ABLoadFinish = true;
        }

        WDebug.Log($"GMS.Extends url:{abMgr.BaseDownloadingURL}");
        yield return 0;
        if (ABLoadFinish)
        {
            EventDispatcher.Instance.DispatchEvent("gms-woogi-event-Gms.ResExtendsReady");
        }
    }

    /// <summary>
    /// When ab is loaded, then load the asset(ab加载完成的情况下，然后加载asset)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="abName"></param>
    /// <param name="root"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T LoadAssetSync<T>(string path, string abName, MPack root = MPack.aaresourcesextends)
        where T : UnityEngine.Object
    {
        if (isUsedAB)
        {
            if (string.IsNullOrEmpty(abName))
            {
                string[] arr = path.Split('/');
                abName = arr[arr.Length - 1];
            }

            path = string.Format("{0}/{1}{2}", root, path, ABConst.SUFFIX).ToLower();
            ABLoaded ab = abMgr.GetLoadedAssetBundle(path, out var error);
            if (ab != null && ab.m_AssetBundle != null)
            {
                return ab.m_AssetBundle.LoadAsset<T>(abName);
            }

            return default(T);
        }
        else
        {
            return Resources.Load<T>(path + abName);
        }
    }

    public static void LoadSceneAsync(string path, Action<float> progress = null, Action callback = null,
        LoadSceneMode model = LoadSceneMode.Single)
    {
        Instance.SceneAsync(path, progress, callback, model);
    }

    void SceneAsync(string path, Action<float> progress, Action callback,
        LoadSceneMode model)
    {
        if (isUsedAB)
        {
            StartCoroutine(_LoadSceneAsync1(path, progress, callback, model));
        }
        else
        {
            StartCoroutine(_LoadSceneAsync(path, progress, callback, model));
        }
    }

    IEnumerator _LoadSceneAsync1(string path, Action<float> progress, Action callback,
        LoadSceneMode model = LoadSceneMode.Single, bool activateOnload = false)
    {
        yield return 0;
        string levelName = path;
        ABLoadOperation request = abMgr.LoadLevelAsync(path + ABConst.SUFFIX, levelName, model);
        yield return request;
        if (callback != null && callback.Target.ToString() != "null")
        {
            try
            {
                callback();
            }
            catch (Exception e)
            {
                WDebug.LogFormat("<color=#ff0000ff>{0}</color>",
                    levelName + " was loaded Finished,but callback Exception: " + e.Message);
            }
        }
    }

    IEnumerator _LoadSceneAsync(string path, Action<float> progress, Action callback,
        LoadSceneMode model = LoadSceneMode.Single)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(path, model);
        async.allowSceneActivation = false;
        float progressValue = 0;
        progress?.Invoke(progressValue);
        while (!async.isDone)
        {
            if (async.progress < 0.9f)
                progressValue = async.progress;
            else
                progressValue = 1.0f;
            if (progressValue >= 0.9f)
            {
                async.allowSceneActivation = true;
            }

            yield return null;
        }

        progress?.Invoke(progressValue);
        callback?.Invoke();
    }

    /// <summary>
    /// 异步加载gameobject
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    //    public void LoadAsync(string path, Action<GameObject> callback)
    //    {
    //        LoadAsync<GameObject>(path, (o) => { callback?.Invoke(o); }
    //        );
    //    }
    public void LoadAsync(string path, Action<Object> callback)
    {
        LoadAsync<Object>(path, (o) => { callback?.Invoke(o); }
        );
    }

    public void LoadAsyncSprite(string path, Action<Sprite> callback)
    {
        LoadAsync<Sprite>(path, (o) => { callback?.Invoke(o); }
        );
    }

    public static void LoadAsync<T>(string _path, Action<T> _callback, MPack _root = MPack.aaresourcesextends)
        where T : Object
    {
        LoadAsync(_path, null, _callback, _root);
    }

    public static void LoadAsync<T>(string _path, string abName, Action<T> _callback,
        MPack _root = MPack.aaresourcesextends) where T : Object
    {
        // 获取当前时间戳

        if (isUsedAB)
        {
            LoadAsyncRes<T>(_path,abName,_callback,_root);
        }
        else
        {
            if (_path.StartsWith("/")) _path = _path.Remove(0, 1);
            ResourceRequest q = Resources.LoadAsync<T>(_path);
            q.completed += (result) => { _callback?.Invoke(q.asset as T); };
        }
    }

    private static void LoadAsyncRes<T>(string path, string abName, Action<T> callback,MPack root) where T : Object
    {
        Instance.StartCoroutine(Instance.ObjectAsync(path,abName,callback,root));
    }

    IEnumerator ObjectAsync<T>(string path, string abName, Action<T> callback,MPack root) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(abName))
        {
            string[] arr = path.Split('/');
            abName = arr[arr.Length - 1];
        }

        if (path.StartsWith("/"))
        {
            //避免造成双"//",因为开发人员传输的数据不可靠.会导致后面获取依赖失败，必须自查一次.
            path = string.Format("{0}{1}{2}", root, path, ABConst.SUFFIX);
        }
        else
        {
            path = string.Format("{0}/{1}{2}", root, path, ABConst.SUFFIX);
        }

        ABLoadAssetOperation request = abMgr.LoadAssetAsync(path, abName, typeof(T),
            callback == null || (typeof(T) == typeof(UnityEngine.AssetBundle)));
        if (request == null)
        {
            yield break;
        }

        yield return request;

        if (callback != null && callback.Target.ToString() != "null")
        {
            T obj = default(T);
            if (request.isError || !string.IsNullOrEmpty(request.error))
            {
            }
            else
            {
                if (typeof(T) == typeof(AssetBundle))
                {
                    ABLoaded abloaded = abMgr.GetLoadedAssetBundle(request.GetAssetbundlePath(), out var error);
                    if (abloaded != null && abloaded.m_AssetBundle != null)
                    {
                        obj = abloaded.m_AssetBundle as T;
                    }
                }
                else
                {
                    obj = request.GetAsset<T>();
                }
            }

            try
            {
#if UNITY_EDITOR
                //wld 编辑器中更新TextMeshProUGUI的shader，避免紫色的问题
                if (obj is GameObject)
                {
                    var gameobject = obj as GameObject;
                    var tmps = gameobject.GetComponentsInChildren<TextMeshProUGUI>();
                    foreach (var p in tmps)
                    {
                        if (p.material != null)
                        {
                            p.fontSharedMaterial.shader = Shader.Find(p.fontSharedMaterial.shader.name);
                        }
                    }
                    var ps = gameobject.GetComponentsInChildren<ParticleSystem>();
                    foreach (var item in ps)
                    {
                        var rd = item.GetComponent<Renderer>();
                        if (rd != null && rd.sharedMaterial!=null)
                        {
                            rd.sharedMaterial.shader = Shader.Find(rd.sharedMaterial.shader.name);
                        }
                    }
                }
#endif
                callback.Invoke(obj);
            }
            catch (Exception e)
            {
                WDebug.LogError(string.Format("{0} was loaded Finished,but callback Exception: {1}", abName, e));
            }
        }
    }
}