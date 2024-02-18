using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using WoogiWorld.AssetBundles;

public enum MPack
{
    none,
    Resources,
    DrageEditor_Resources,
    aaresourcesextends,
}

public class ResMgr : SingletonBehaviour<ResMgr>
{
    [SerializeField] private bool isUseAb;

    /// <summary>
    /// 等待ResMgr 初始化完成后，在初始化ResExtendsMgr
    /// </summary>
    public static bool isReady = false;

    public static bool isUsedAB = false;
    public bool isCustomUrl = false;
    public string customUrl = string.Empty;
    public static ABManager abMgr;

    public static void Setparameter(bool _usedAB, bool _isCustomUrl, string _customUrl)
    {
        if (hasInstance)
        {
            WDebug.Log($"GMS.Setparameter:{_usedAB}, isCustom = {_isCustomUrl},{_customUrl}");
            isUsedAB = _usedAB;
            Instance.isUseAb = isUsedAB;
            Instance.isCustomUrl = _isCustomUrl;
            Instance.customUrl = _customUrl;
        }
    }

    private void Awake()
    {
        Instance = this;
        abMgr = ABManager.CreateABManager();
        isUseAb = isUsedAB;
        isReady = false;
    }

    private IEnumerator Start()
    {
        yield return 0;
        isUseAb = isUsedAB;
        if (isUseAb)
        {
            if (isCustomUrl && !string.IsNullOrEmpty(customUrl))
            {
                abMgr.isUsedCustionUrl = isCustomUrl;
                abMgr.UsedCustionUrl = customUrl;
            }
            AssetBundleLoadManifestOperation dd = abMgr.Initialize();
            yield return dd;
            if (!abMgr.HasAssetBundleManifest)
            {
                WDebug.Log("AssetBundleManifest load fail！");
            }
            else
            {
                WDebug.Log("AssetBundleManifest load finished！");
            }
            WDebug.Log($"GMS parameter:{isUseAb},{abMgr.isUsedCustionUrl},{abMgr.BaseDownloadingURL}");
        }
        yield return new WaitForSeconds(0.2f);
        isReady = true;
        EventDispatcher.Instance.DispatchEvent("gms-woogi-event-Gms.ResReady");
    }


    public static string GetMPackPath(MPack t)
    {
        switch (t)
        {
            case MPack.DrageEditor_Resources:
                return "DrageEditor/Resources";
            case MPack.aaresourcesextends:
                return "aaresourcesextends";
            case MPack.Resources:
                return "Resources";
            default:
                return "Resources";
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
    public static T LoadAssetSync<T>(string path, string abName, MPack root = MPack.Resources)
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

    public static void LoadAsync(string path, Action<Object> callback)
    {
        LoadAsync<Object>(path, (o) => { callback?.Invoke(o); });
    }

    public static void LoadAsync<T>(string path, Action<T> callback, MPack root = MPack.Resources)
        where T : Object
    {
        LoadAsync<T>(path, null, callback, root);
    }

    public static void LoadAsync<T>(string path, string abName, Action<T> callback, MPack root = MPack.Resources)
        where T : Object
    {
        if (isUsedAB)
        {
            Instance.StartCoroutine(Instance.ObjectAsync<T>(path, abName, callback, GetMPackPath(root)));
        }
        else
        {
            ResourceRequest q = Resources.LoadAsync<T>(path);
            q.completed += (result) => { callback?.Invoke(q.asset as T); };
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
        ABLoadOperation request =
            abMgr.LoadLevelAsync("scenes/" + path + ABConst.SUFFIX, levelName, model);
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

    IEnumerator ObjectAsync<T>(string path, string abName, Action<T> callback, string root = "Resources")
        where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(abName))
        {
            string[] arr = path.Split('/');
            abName = arr[arr.Length - 1];
        }

        path = string.Format("{0}/{1}{2}", root, path, ABConst.SUFFIX);
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
                callback.Invoke(obj);
            }
            catch (Exception e)
            {
                WDebug.LogError(string.Format("{0} was loaded Finished,but callback Exception: {1}", abName, e));
            }
        }
    }
}