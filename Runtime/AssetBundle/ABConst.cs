namespace WoogiWorld.AssetBundles
{
    public class ABConst
    {
        //  public static string VAR_NAME = ABUtility.GetAssetbundleExtension();
        /// <summary>
        /// 素材扩展名 suffix, 如： .unity3d
        /// </summary>
        public static string SUFFIX = "." + ABUtility.GetAssetbundleExtension();
        /// <summary>
        /// asset bundle root note. /AssetBundle
        /// </summary>
        public static string AB_ROOT = "/AssetBundle";
        /// <summary>
        /// 相对素材目录: /AssetBundle/Windows
        /// </summary>
        public static string ASSET_DIRNAME
        {
            get
            {
                return string.Format("{0}/{1}", AB_ROOT, ABUtility.GetPlatformName() + ABUtility.GetAssetbundleQuality());
            }
        }
        
        /// <summary>
        /// 主配置名称:"Windows.at"
        /// </summary>
        public static string MANIFEST_NAME = ABUtility.GetPlatformName() + SUFFIX;
        // + ".at";
        /// <summary>
        /// 是否是测试模式，为true 读的是Asset下StreamingAssets的资源
        /// </summary>
        public static bool isDebugModel = false;
        public static bool isUseAB = false;
        public static ABQuality abQuality = ABQuality.Hight;
        
        public static uint AB_Version = 1233;
        public static bool isAB_Version = false;
    }
}
