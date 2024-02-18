using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace WoogiWorld.AssetBundles
{
    public enum ABQuality
    {
        Low,
        Hight,
    }

    public class ABUtility
    {
        static string AndroidTarget
        {
            get
            {
#if GOOGLE_PLAY
                            return "Android_gp";
#elif HUA_WEI
                            return "Android_huawei";
#elif XIAO_MI
                            return "Android_xiaomi";
#else
                return "Android";
#endif
            }
        }

        public static string GetAssetbundleExtension()
        {

            switch (GetPlatformName())
            {
                case "Android_gp":
                case "Android_huawei":
                case "Android_xiaomi":
                case "Android":
                    return "and";
                case "IOS":
                    return "ios";
                case "Windows":
                    return "win";
                case "OSX":
                    return "mac";
                case "WebGL":
                    return "webgl";
                default:
                    return "win";
            }
        }

        public static string GetAssetbundleQuality(string separator = "_")
        {
            ABConst.abQuality = ABQuality.Hight; //Ŀǰ����δ���ţ�����Ĭ��ʹ��hight.
            switch (ABConst.abQuality)
            {
                case ABQuality.Low:
                    return separator + ABQuality.Low.ToString();
                case ABQuality.Hight:
                    return "";
            }

            return "";
        }

        #region Get Current  Platform Name

        public static string GetPlatformName()
        {
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			return GetPlatformForAssetBundles(Application.platform);
#endif
        }

#if UNITY_EDITOR
        public static string GetPlatformForAssetBundles(BuildTarget target)
        {
            
#if ALL_IN_ONE_WEBGL
            return "WebGL";
#endif
            switch (target)
            {
                case BuildTarget.Android:
                    return AndroidTarget;
                case BuildTarget.iOS:
                    return "IOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSX:
                    return "OSX";
                default:
                    return null;
            }
        }
#else
        public static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
#if ALL_IN_ONE_WEBGL
            return "WebGL";
#endif
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return AndroidTarget;
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                default:
                    return null;
            }
        }
#endif

        #endregion

        #region data handle.

        public static string GetStreamingAssetsPath()
        {
            string path = string.Empty;
            if (Application.isEditor)
                path = Application.streamingAssetsPath;
            else
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        path = "jar:file://" + Application.dataPath + "!/assets";
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        path = Application.dataPath + "/Raw";
                        break;
                    case RuntimePlatform.OSXPlayer:
                        path = Application.streamingAssetsPath;
                        break;
                    default:
                        path = Application.dataPath + "/StreamingAssets";
                        break;
                }
            }

            return path;
        }

        /// <summary>
        /// ab root path
        /// </summary>
        public static string DataPath
        {
            get
            {
                if (ABConst.isDebugModel)
                {
                    return GetStreamingAssetsPath() + ABConst.ASSET_DIRNAME;
                }
                else
                {
#if UNITY_EDITOR
                    return Application.persistentDataPath + "/Res/en" + ABConst.ASSET_DIRNAME;
#else
                    if (Application.platform == RuntimePlatform.WindowsPlayer||Application.platform == RuntimePlatform.WebGLPlayer)
                        return Application.streamingAssetsPath + ABConst.ASSET_DIRNAME;//Path.Combine(Application.streamingAssetsPath, "WINDOWS" + ABConst.ASSET_DIRNAME);
                    return Application.persistentDataPath + "/Res/en" + ABConst.ASSET_DIRNAME;
#endif
                }
            }
        }

        public static string DataPathRoot
        {
            get
            {
                if (ABConst.isDebugModel)
                {
                    return GetStreamingAssetsPath();
                }
                else
                {
#if UNITY_EDITOR
                    return Application.persistentDataPath + "/Res/en";
#else
                    if (Application.platform == RuntimePlatform.WindowsPlayer)
                        return Application.streamingAssetsPath;//Path.Combine(Application.streamingAssetsPath, "WINDOWS" + ABConst.ASSET_DIRNAME);
                    else
                        return Application.persistentDataPath + "/Res/en";
#endif
                }
            }
        }

        #endregion
    }
}