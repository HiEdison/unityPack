#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace WoogiWorld.AssetBundles
{
    public enum ResType
    {
        Atlas,
        res,
    }

    public class ABLocalConfig: ScriptableObject
    {
        public void ReSet()
        {
            List<string> oldLs = new List<string>();
            oldLs.AddRange(AbNameEditorAssembleLs);
            foreach (string value in  oldLs)
            {
                for (int h = 0; h < level1SubdirectoryLs.Count; h++)
                {
                    if (value.StartsWith(level1SubdirectoryLs[h]))
                    {
                        AbNameEditorAssembleLs.Remove(value);
                        break;
                    }
                }
            }
            //string str1 = Json.SerializerJson<List<string>>(AbNameEditorAssembleLs);
            //File.WriteAllText("c:/Test1.txt", str1);
            string fullPath = "";
            for (int h = 0; h < level1SubdirectoryLs.Count; h++)
            {
                fullPath = ABBuildConfig.APPLICATION_PATH + level1SubdirectoryLs[h];
                Debug.Log(fullPath);
                if (Directory.Exists(fullPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(fullPath);
                    DirectoryInfo[] dirArray = dir.GetDirectories();
                    for (int i = 0; i < dirArray.Length; i++)
                    {
                        string s = dirArray[i].FullName;
                        s = s.Replace(@"\", "/");
                        s = s.Replace(ABBuildConfig.APPLICATION_PATH, "");
                        if (!AbNameEditorAssembleLs.Contains(s))
                        {
                            AbNameEditorAssembleLs.Add(s);
                        }
                    }
                }
            }
            //string str = Json.SerializerJson<List<string>>(AbNameEditorAssembleLs);
            //File.WriteAllText("c:/Test.txt", str);
        }

        /// <summary>
        /// 此处添加需要清除的资源后缀名,注意大小写。
        /// </summary>
        public  List<string> filtersuffix;
       
        /// <summary>
        /// Atlas root note. Sources/UI/
        /// </summary>
        public  List<string> atlas_root_note;
        /// <summary>
        /// resources root note. Resources/（除开图集之外的资源，预制、散图、shader、场景等等）.
        /// </summary>
        public  List<string> res_Root_note;

        /// <summary>
        /// 多个资源构建成1个bundle的全路径.
        /// </summary>
        public  List<string> AbNameEditorAssembleLs;

        #region 限制.

        /// <summary>
        ///  是否编辑指定的部分对象的assetbundle name.
        /// </summary>
        public  bool isBuildPartObjectABName = false;
        /// <summary>
        /// 编辑指定的部分对象的assetbundle name. path ls
        /// </summary>
        public List<string> AbNameEditorPart;

        /// <summary>
        ///  是否忽略指定的部分对象的assetbundle name.
        /// </summary>
        public  bool isIgnorePartObjectABName = false;
        public  List<string> Ignorefolder;

        /// <summary>
        ///  是否编辑指定的部分场景的assetbundle name.
        /// </summary>
        public  bool isBuildPartSceneABName = false;
        /// <summary>
        ///build Level 1 subdirectory list（文件夹列表）": 文件夹A将1级子目录文件构建成单个assetbundle,如：A目录下有10个文件夹和2张图片，那么每个子文件夹和文件各自成单独的AB，合计12个ab.
        /// </summary>
        public List<string> level1SubdirectoryLs;
        /// <summary>
        /// 编辑指定的部分场景的assetbundle name. path ls
        /// </summary>
        public  List<string> AbNameEditorScenePart;

        #endregion

        /// <summary>
        /// The is del dir ls.
        /// </summary>
        public bool isDelDirLs = false;
        public List<string> delDirLs;
        public List<string> delIgnoreDirLs;

        public bool isUseCustomABOutputPath;
        public string customABOutputPath;
    }

    public class ABBuildConfig
    {
        #region AssetBundleConfig

        /// <summary>
        /// AssetBundle打包路径 .../StreamingAssets/AssetBundle or other path.
        /// </summary>
        public static string AB_PATH = string.Format("{0}{1}", Application.dataPath.Replace("/Assets", "") /*Application.streamingAssetsPath*/, ABConst.AB_ROOT);
        /// <summary>
        /// 资源地址 .../Asset/
        /// </summary>
        public static string APPLICATION_PATH = Application.dataPath + "/";
        /// <summary>
        /// 工程地址
        /// </summary>
        public static string PROJECT_PATH = APPLICATION_PATH.Substring(0, APPLICATION_PATH.Length - 7);

        public static ABLocalConfig Config
        {
            get
            {
                TextAsset txt = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Scripts/AssetBundle/Editor/ABGloablConfig.json");
                if (txt != null)
                {
                    ABLocalConfig config = Json.DeserializeJson<ABLocalConfig>(txt.text);
                    if (config != null)
                    {
                        config.ReSet();
                        return config;
                    }
                }
                return null;
            }
        }

        #endregion
    }
}
#endif