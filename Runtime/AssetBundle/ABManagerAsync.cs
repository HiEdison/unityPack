using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace WoogiWorld.AssetBundles
{
    public partial class ABManager
    {
        [SerializeField] private int sendbatchAmount = 20;
        [SerializeField] private float retryTimeInterval = 10.0f;

        [SerializeField] private int cachebatchAmount = 20;
        [SerializeField] List<finishedRequest> waitCacheLs = new List<finishedRequest>();
        [SerializeField] List<string> tempdoneLs = new List<string>();
        [SerializeField] List<waitRequest> waitRequestls = new List<waitRequest>();
        [SerializeField] List<string> statusTable = new List<string>();

        public int sendCount = 0;

        #region template variable

        private int count = 0;
        private waitRequest m_waitRequest;


        [Serializable]
        public struct RetryRequest
        {
            public waitRequest waitRequest;
            public float lastSendTime;
        }

        [Serializable]
        public struct waitRequest
        {
            public string url;
            public string abPath;
            public byte reTryCount;
#if !UNITY_WEBGL
            public string localFullPath;
#endif
        }

        [Serializable]
        struct finishedRequest
        {
            public string Key;
            public ABAsyncLoading request;
        }

        #endregion


        void AsyncUpdate()
        {
            if (m_DownloadingRequests.Keys.Count > 0)
            {
                foreach (var item in m_DownloadingRequests)
                {
                    ABAsyncLoading request = item.Value;
                    if (request.isDone)
                    {
                        if (!string.IsNullOrEmpty(request.error) || request.isHttpError || request.isNetworkError)
                        {
                            //TODO增加请求次数
                            tempdoneLs.Add(item.Key);
                            RemoveToRequestQueue(item.Key);
                            string errorMsg = String.Format("{0}:{1}  ", request.error, request.url);
                            if (request.isReTry)
                            {
                                WDebug.Log(String.Format(" add retry {0}:{1}--time:{2}  ",
                                    request.waitRequest.reTryCount, errorMsg, Time.realtimeSinceStartup));
                                if (request.waitRequest.reTryCount > MaxRetryCount - MaxContinuRetryCount)
                                    AddToRequestQueue(request.waitRequest);
                                else
                                {
                                    AddRetryQuesetQueue(request.waitRequest);
                                }
                            }
                            else
                            {
                                m_DownloadingErrors[item.Key] = errorMsg;
                            }


                            continue;
                        }
                        else
                        {
                            tempdoneLs.Add(item.Key);
                            waitCacheLs.Add(new finishedRequest()
                            {
                                Key = item.Key,
                                request = item.Value,
                            });
                        }
                    }
                }
            }

            for (int i = 0; i < m_InProgressOperations.Count;)
            {
                if (!m_InProgressOperations[i].Update())
                {
                    m_InProgressOperations.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            CacheAssetbundle();
            SendRequest();
            SendRetryRequest();
        }


        void CacheAssetbundle()
        {
            if (tempdoneLs.Count > 0)
            {
                foreach (var item in tempdoneLs)
                {
                    m_DownloadingRequests.Remove(item);
                }

                tempdoneLs.Clear();
            }

            count = 0;
            while (waitCacheLs.Count > 0 && count++ < cachebatchAmount)
            {
                finishedRequest item = waitCacheLs[0];
                waitCacheLs.RemoveAt(0);
                RemoveToRequestQueue(item.Key);
                if (item.request != null)
                {
                    //  Profiler.EndSample();
                    AssetBundle bundle = item.request.assetbundle;
                    if (bundle == null)
                    {
                        m_DownloadingErrors[item.Key] = $"{item.Key} is not a valid asset bundle.";
                        if (item.request.isReTry)
                        {
                            AddToRequestQueue(item.request.waitRequest);
                        }
                    }
                    else
                    {
                        m_LoadedAssetBundles[item.Key] = new ABLoaded(item.Key, bundle);
                    }

                    // Profiler.BeginSample("UnityWebRequest-Dispose");
                    item.request.Dispose();
                    //                Profiler.EndSample();
                }
            }
        }


        void SendRequest()
        {
            count = 0;
            while (waitRequestls.Count > 0 && count < sendbatchAmount)
            {
                count++;
                m_waitRequest = waitRequestls[0];
                waitRequestls.RemoveAt(0);
                m_DownloadingRequests.Add(m_waitRequest.abPath, new ABAsyncLoading(m_waitRequest));
                sendCount++;
            }
        }


        void SendRetryRequest()
        {
            if (retryRequestls.Count > 0 &&
                Time.realtimeSinceStartup - retryRequestls[0].lastSendTime >= retryTimeInterval)
            {
                AddToRequestQueue(retryRequestls[0].waitRequest);
                WDebug.Log($"send retry request:{retryRequestls[0].waitRequest.abPath}--{Time.realtimeSinceStartup}");
                retryRequestls.RemoveAt(0);
            }
        }

        /// <summary>
        /// //true: 表示这个assetbundle 正在工作中，等待加载、加载中...
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        bool CheckStatusTable(string assetBundleName)
        {
            foreach (var item in statusTable)
            {
                if (item.Equals(assetBundleName))
                    return true;
            }

            return false;
        }

        List<RetryRequest> retryRequestls = new List<RetryRequest>();


        void AddRetryQuesetQueue(waitRequest retryRequest)
        {
            retryRequestls.Add(new RetryRequest
            {
                waitRequest = retryRequest,
                lastSendTime = Time.realtimeSinceStartup
            });
            statusTable.Add(retryRequest.abPath);
        }

        void AddToRequestQueue(waitRequest waiRequest)
        {
            waitRequestls.Add(waiRequest);
            if (!statusTable.Contains(waiRequest.abPath))
                statusTable.Add(waiRequest.abPath);
        }

        public const byte MaxRetryCount = 15;
        public const byte MaxContinuRetryCount = 3;

        void AddToRequestQueue(string url, string assetBundleName)
        {
            AddToRequestQueue(new waitRequest()
            {
                url = url,
                abPath = assetBundleName,
                reTryCount = MaxRetryCount,
#if !UNITY_WEBGL
#if UNITY_EDITOR
                localFullPath = Application.persistentDataPath + ABConst.ASSET_DIRNAME + "/" + assetBundleName
#elif UNITY_STANDALONE_WIN
              localFullPath = Application.streamingAssetsPath + ABConst.ASSET_DIRNAME + "/" + assetBundleName
#else
                localFullPath = Application.persistentDataPath + ABConst.ASSET_DIRNAME + "/" + assetBundleName
#endif
#endif
            });
        }

        void RemoveToRequestQueue(string assetBundleName)
        {
            statusTable.Remove(assetBundleName);
        }

        #region 异步

        public bool isUsedCustionUrl = false;
        public string UsedCustionUrl = string.Empty;

        /// <summary>
        /// 初始化加载主manifest文件信息，用于获取依赖以及版本校验
        /// </summary>
        /// <returns></returns>
        public AssetBundleLoadManifestOperation Initialize()
        {
#if UNITY2019
            Caching.ClearCache();
#endif
            if (isUsedCustionUrl)
            {
                BaseDownloadingURL = UsedCustionUrl;
            }
            else
            {
                BaseDownloadingURL = ABUtility.DataPath;
            }

            //WDebug.Log($"[AssetBundleManager.Initialize] BaseDownloadingURL={BaseDownloadingURL}");
            return Initialize(ABConst.MANIFEST_NAME);
        }

        // Load AssetBundleManifest.
        AssetBundleLoadManifestOperation Initialize(string manifestAssetBundleName)
        {
            //WDebug.Log($"manifestAssetBundleName:{manifestAssetBundleName}");
            LoadAssetBundle(manifestAssetBundleName, true);
            var operation = new AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest",
                typeof(AssetBundleManifest), this);
            m_InProgressOperations.Add(operation);
            return operation;
        }

        /// <summary>
        /// 当所有依赖都加载成功后调用，返回bundle
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        // Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
        public ABLoaded GetLoadedAssetBundle(string assetBundleName, out string error)
        {
            if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
            {
                return null;
            }

            ABLoaded bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
            {
                return null;
            }

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
            {
                return bundle;
            }

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies)
            {
                if (m_DownloadingErrors.TryGetValue(assetBundleName, out error))
                {
                    return bundle;
                }

                // Wait all the dependent assetBundles being loaded.
                ABLoaded dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null)
                {
                    if (m_DownloadingErrors.TryGetValue(dependency, out error))
                    {
                        return bundle;
                    }

                    return null;
                }
            }

            return bundle;
        }

        // Load AssetBundle and its dependencies.
        protected void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
        {
            if (!isLoadingAssetBundleManifest)
            {
                if (m_AssetBundleManifest == null)
                {
                    WDebug.LogWarning(
                        "Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                    return;
                }
            }

            // Check if the assetBundle has already been processed.
            bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);

            // Load dependencies.
            if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
            {
                //                Log(ABLog.Warning,
                //                    String.Format("[ABManager.LoadingAssetBundle] {0} : ",
                //                        (isLoadingAssetBundleManifest ? "Manifest" : String.Empty, assetBundleName)));
                LoadDependencies(assetBundleName);
            }
        }

        // Remaps the asset bundle name to the best fitting asset bundle variant.
        protected string RemapVariantName(string assetBundleName)
        {
            string[] bundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();

            string[] split = assetBundleName.Split('.');

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                if (curSplit[0] != split[0])
                    continue;

                int found = System.Array.IndexOf(m_ActiveVariants, curSplit[1]);

                // If there is no active variant found. We still want to use the first 
                if (found == -1)
                    found = int.MaxValue - 1;

                if (found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }

            if (bestFit == int.MaxValue - 1)
            {
                WDebug.LogWarning(
                    $"Ambigious asset bundle variant chosen because there was no matching active variant: {bundlesWithVariant[bestFitIndex]}");
            }

            if (bestFitIndex != -1)
            {
                return bundlesWithVariant[bestFitIndex];
            }
            else
            {
                return assetBundleName;
            }
        }

        /// <summary>
        /// 实际使用www或者UnityWebRequest去下载或加载资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="isLoadingAssetBundleManifest"></param>
        /// <returns></returns>
        // Where we actuall call WWW or UnityWebRequest to download the assetBundle.
        protected bool LoadAssetBundleInternal(string assetBundleName, bool isLoadingAssetBundleManifest)
        {
            // Already loaded.
            ABLoaded bundle = null;
            m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle != null)
            {
                bundle.SetReferencedCount(true);
                return true;
            }

            if (CheckStatusTable(assetBundleName))
                return true;

            string url = $"{m_BaseDownloadingURL}/{assetBundleName}";
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (!url.StartsWith("http", System.StringComparison.CurrentCultureIgnoreCase))
            {
                url = url.Replace(@"/", @"\");

                if (Application.platform == RuntimePlatform.Android)
                {
                    if (ABConst.isDebugModel)
                    {
                        /*android 已添加jar:file://*/
                    }
                    else
                    {
                        //  url = "file://" + url;  //使用unityWebRequest方式
                    }
                }
                else
                {
                    //  url = "file://" + url;  //使用unityWebRequest方式
                }
            }
#endif

            AddToRequestQueue(url, assetBundleName);
            return false;
        }

        /// <summary>
        /// 加载依赖
        /// </summary>
        /// <param name="assetBundleName">要加载依赖的assetbundle名称</param>
        // Where we get all the dependencies and load them all.
        protected void LoadDependencies(string assetBundleName)
        {
            if (m_AssetBundleManifest == null)
            {
                WDebug.LogWarning("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }

            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length == 0)
                return;

            for (int i = 0; i < dependencies.Length; i++)
            {
                dependencies[i] = RemapVariantName(dependencies[i]);
            }

            // Record and load all dependencies.
            if (m_Dependencies.ContainsKey(assetBundleName))
            {
                m_Dependencies[assetBundleName] = dependencies;
            }
            else
            {
                m_Dependencies.Add(assetBundleName, dependencies);
            }

            for (int i = 0; i < dependencies.Length; i++)
            {
                LoadAssetBundleInternal(dependencies[i], false);
            }
        }

        /// <summary>
        /// 卸载assetBundle以及它的依赖资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        // Unload assetbundle and its dependencies.
        public void UnloadAssetBundle(string assetBundleName)
        {
            if (!assetBundleName.Contains(ABConst.SUFFIX))
            {
                assetBundleName += ABConst.SUFFIX;
            }

            //            WDebug.Log($"unistall bundle :{assetBundleName}");
            if (assetBundleName != ABConst.MANIFEST_NAME)
                assetBundleName = assetBundleName.ToLower();
            UnloadAssetBundleInternal(assetBundleName);
            UnloadDependencies(assetBundleName);
        }

        protected void UnloadDependencies(string assetBundleName)
        {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies))
            {
                return;
            }

            // Loop dependencies.
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency);
            }

            string error;
            ABLoaded bundle = GetLoadedAssetBundle(assetBundleName, out error);
            if (bundle == null || bundle.GetReferencedCount <= 0)
                m_Dependencies.Remove(assetBundleName);
        }

        protected void UnloadAssetBundleInternal(string assetBundleName)
        {
            string error;
            ABLoaded bundle = GetLoadedAssetBundle(assetBundleName, out error);
            if (bundle == null)
            {
                return;
            }

            UnLoadAssetBundle(assetBundleName,bundle);
        }

        private void UnLoadAssetBundle(string assetBundleName, ABLoaded bundle)
        {
            if (bundle.SetReferencedCount(false) == 0)
            {
                //  WDebug.Log($"UnloadAssetBundleInternal {m_LoadedAssetBundles.Count}");
                bundle.m_AssetBundle.Unload(false);
                m_LoadedAssetBundles.Remove(assetBundleName);
                Log(ABLog.Warning, $"{assetBundleName} has been unloaded successfully");
            }
        }


        /// <summary>
        ///   Load asset from the given assetBundle.
        /// </summary>
        /// <param name="assetBundleName">assetBundle name</param>
        /// <param name="assetName">asset name，Full paths are recommended.
        /// Beacause If you have multiple resources in one ab.
        /// Assets/Sources/AGlobal/Correct0002.png
        /// Assets/Sources/AGlobal/Correct0002.prefab</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ABLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type,
            bool loadAssetOff)
        {
            assetBundleName = assetBundleName.ToLower();
            // Log(ABLog.Log, "[ABManager.LoadAssetAsync] Loading " + assetName + " from " + assetBundleName + " bundle");
            ABLoadAssetOperation operation = null;
            assetBundleName = RemapVariantName(assetBundleName);
            LoadAssetBundle(assetBundleName);
            operation = new ABLoadAssetOperationFull(assetBundleName, assetName, type, this, loadAssetOff);
            m_InProgressOperations.Add(operation);
            return operation;
        }

        // Load level from the given assetBundle.
        public ABLoadOperation LoadLevelAsync(string assetBundleName, string levelName,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            assetBundleName = assetBundleName.ToLower();
            // Log(ABLog.Log, "[ABManager.LoadLevelAsync] Loading " + levelName + " from " + assetBundleName + " bundle");

            ABLoadOperation operation = null;
            assetBundleName = RemapVariantName(assetBundleName);
            LoadAssetBundle(assetBundleName);
            operation = new ABLoadLevelOperation(assetBundleName, levelName, mode, this);
            m_InProgressOperations.Add(operation);
            return operation;
        }

        #endregion

        #region 新加全路径加载ab资源(TOO:Add full path to load ab resources)

        public ABLoadAssetOperation LoadAssetAsyncFullPath(string w_assetBundleName, string w_assetName, System.Type w_type, bool w_loadAssetOff)
        {
            LoadAssetBundleFullPath(w_assetBundleName);
            ABLoadAssetOperation w_operation = new ABLoadAssetOperationFull(w_assetBundleName, w_assetName, w_type, this, w_loadAssetOff);
            m_InProgressOperations.Add(w_operation);
            return w_operation;
        }

        protected void LoadAssetBundleFullPath(string w_assetBundleName, bool w_isLoadingAssetBundleManifest = false)
        {
            if (!w_isLoadingAssetBundleManifest)
            {
                if (m_AssetBundleManifest == null)
                {
                    WDebug.LogWarning("FullPath: Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                    return;
                }
            }

            bool w_isAlreadyProcessed = LoadAssetBundleInternalFullPath(w_assetBundleName, w_isLoadingAssetBundleManifest);
            if (!w_isAlreadyProcessed && !w_isLoadingAssetBundleManifest)
            {
                LoadDependenciesFullPath(w_assetBundleName);
            }
        }

        protected bool LoadAssetBundleInternalFullPath(string w_assetBundleName, bool w_isLoadingAssetBundleManifest)
        {
            ABLoaded w_bundle = null;
            m_LoadedAssetBundles.TryGetValue(w_assetBundleName, out w_bundle);
            if (w_bundle != null)
            {
                w_bundle.SetReferencedCount(true);
                return true;
            }

            if (CheckStatusTable(w_assetBundleName))
            {
                return true;
            }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (!w_assetBundleName.StartsWith("http", System.StringComparison.CurrentCultureIgnoreCase))
            {
                w_assetBundleName = w_assetBundleName.Replace(@"/", @"\");
            }
#endif
            AddToRequestQueue(w_assetBundleName, w_assetBundleName);
            return false;
        }

        protected void LoadDependenciesFullPath(string w_assetBundleName)
        {
            if (m_AssetBundleManifest == null)
            {
                WDebug.LogWarning("FullPath: Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }

            string[] w_dependencies = m_AssetBundleManifest.GetAllDependencies(w_assetBundleName);
            if (w_dependencies.Length == 0)
            {
                return;
            }

            for (int i = 0; i < w_dependencies.Length; i++)
            {
                w_dependencies[i] = RemapVariantName(w_dependencies[i]);
            }

            if (m_Dependencies.ContainsKey(w_assetBundleName))
            {
                m_Dependencies[w_assetBundleName] = w_dependencies;
            }
            else
            {
                m_Dependencies.Add(w_assetBundleName, w_dependencies);
            }

            for (int i = 0; i < w_dependencies.Length; i++)
            {
                LoadAssetBundleInternalFullPath(w_dependencies[i], false);
            }
        }

        #endregion
    }
}