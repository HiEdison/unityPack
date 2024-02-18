using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WoogiWorld.AssetBundles
{
    public class ABBuildInfo
    {
        public string name;
        public List<string> ls = new List<string>();

        public ABBuildInfo(string _n, string _p)
        {
            name = _n;
            ls.Add(_p);
        }

        public void Add(string _p)
        {
            if (!ls.Contains(_p))
                ls.Add(_p);
        }

        public string[] Getpaths()
        {
            return ls.ToArray();
        }
    }

    public class ABBuildTool
    {
        [MenuItem("Tools/ABName/test Config")]
        private static void test()
        {
            config = ABBuildConfig.Config;
        }

        private static void ClearConsole()
        {
            Type log = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntries");
            MethodInfo clearMethod = log.GetMethod("Clear");
            clearMethod.Invoke(null, null);
        }

        [MenuItem("Tools/ABName/Open Config")]
        public static void OpenAssetBundleConfig()
        {
            string path = Application.dataPath + "/Scripts/AssetBundle/Editor/ABGloablConfig.json";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            //用记事本.
            System.Diagnostics.Process.Start("notepad.exe", path);
        }

        public static void OpenAssetBundleDirectory()
        {
            //清空一下缓存        
            Caching.ClearCache();
            config = ABBuildConfig.Config;
            string rootPath = "D:/Project/WoogiCreate/Resources/AssetBundle";
            if (config != null && !string.IsNullOrEmpty(config.customABOutputPath))
            {
                rootPath = config.customABOutputPath + ABConst.AB_ROOT;
            }

            EditorUtility.OpenWithDefaultApp(rootPath);
        }

        [MenuItem("Tools/ABName/Del resource 0")]
        public static void DelFileFromABConfig()
        {
            config = ABBuildConfig.Config;
            if (config == null)
            {
                Debug.LogError("loacl config miss!");
                return;
            }

            if (config.isDelDirLs)
            {
                bool isignoreItem = false;
                List<string> ignoreItemLs = new List<string>();
                string resPath;
                for (int i = 0; i < config.delDirLs.Count; i++)
                {
                    ignoreItemLs.Clear();
                    isignoreItem = false;
                    resPath = Path.Combine(ABBuildConfig.APPLICATION_PATH, config.delDirLs[i]);
                    //Debug.Log("Del file from AssetBundle Config :" + resPath);
                    foreach (var item in config.delIgnoreDirLs)
                    {
                        if (item.StartsWith(config.delDirLs[i]))
                        {
                            ignoreItemLs.Add(Path.Combine(ABBuildConfig.APPLICATION_PATH, item));
                            isignoreItem = true;
                        }
                    }

                    if (Directory.Exists(resPath))
                    {
                        if (isignoreItem)
                        {
                            DelFile(resPath, ignoreItemLs);
                        }
                        else
                        {
                            Directory.Delete(resPath, true);
                        }
                    }
                    else if (File.Exists(resPath))
                    {
                        if (!isignoreItem)
                        {
                            File.Delete(resPath);
                        }
                    }
                }
            }

            Debug.Log("Del file from AssetBundle Config , complete! " + config.isDelDirLs);
            AssetDatabase.Refresh();
        }

        static void DelFile(string resPath, List<string> ignorels)
        {
            string[] filesstr = Directory.GetFiles(resPath, "*", SearchOption.AllDirectories);
            bool isDel = true;
            string temp;
            for (int i = 0; i < filesstr.Length; i++)
            {
                isDel = true;

                temp = filesstr[i].Replace(@"\", "/");
                if (temp.EndsWith(".meta"))
                {
                    continue;
                }

                foreach (var item in ignorels)
                {
                    if (temp.StartsWith(item))
                    {
                        isDel = false;
                        break;
                    }
                }

                if (isDel)
                {
                    File.Delete(temp);
                }
            }

//todo later 删除空文件夹.
        }


        #region 懒人

        static List<AssetBundleBuild> abAssetBuildmaps = new List<AssetBundleBuild>();
        static Dictionary<string, ABBuildInfo> abAssetBuildmapsPool = new Dictionary<string, ABBuildInfo>();
        private static bool isFromCmd;

        public static void AKeyToRelease_AB_gp()
        {
            AssetDatabase.Refresh();
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
                if (!defineSymbols.Contains("GOOGLE_PLAY"))
                {
                    defineSymbols += ";GOOGLE_PLAY";
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineSymbols);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("[ABBuildTool.AKeyToRelease_AB_gp]:" +
                          PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
#if GOOGLE_PLAY
                Debug.Log("[ABBuildTool.AKeyToRelease_AB_gp]: GOOGLE_PLAY");
#else
                Debug.Log("[ABBuildTool.AKeyToRelease_AB_gp]: unkown");
#endif
                AKeyToRelease_AB();
            }
            else
            {
                Debug.Log("[ABBuildTool.AKeyToRelease_AB_gp]:" +
                          (EditorUserBuildSettings.activeBuildTarget.ToString()));
            }
        }

        public static void AKeyToRelease_AB()
        {
#if UNITY_2022
            //Bug #18198
            string lib = Application.dataPath.Replace("Assets", "Library");
            FileTool.DelDirectory($"{lib}/BuildPlayerData");
#endif
            AssetDatabase.Refresh();
            Debug.Log("[AKeyToRelease_AB] activeBuildTarget:" + EditorUserBuildSettings.activeBuildTarget);
            // if (!ProjectToolWindow.IsOpenAutoRefresh)
            // {Main
            //     Debug.Log("current autoRefresh is false.");
            //     ProjectToolWindow.IsOpenAutoRefresh = true;
            // }

            AssetDatabase.Refresh();
            //AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            double timeStart = EditorApplication.timeSinceStartup;
            ClearAllResourcesABName();
            BuildALLABName();
            BuildAssetCurrentBuildTargetResource();
            Debug.Log("Release Time:" + (int)(EditorApplication.timeSinceStartup - timeStart) + " s");
        }

        #endregion


        #region create Build target.

        // [MenuItem("Tools/BuildAB/Windows")]
        public static void BuildAssetWinResource()
        {
            BuildAssetResource(BuildTarget.StandaloneWindows);
        }

        //  [MenuItem("Tools/BuildAB/Mac")]
        public static void BuildAssetMacResource()
        {
            BuildAssetResource(BuildTarget.StandaloneOSX);
        }

        // [MenuItem("Tools/BuildAB/Android")]
        public static void BuildAssetAndResource()
        {
            BuildAssetResource(BuildTarget.Android);
        }

        //  [MenuItem("Tools/BuildAB/IOS")]
        public static void BuildAssetIOSResource()
        {
            BuildAssetResource(BuildTarget.iOS);
        }

        // [MenuItem("Tools/BuildAB/CurrentBuildTarget")]
        public static void BuildAssetCurrentBuildTargetResource()
        {
            BuildAssetResource(EditorUserBuildSettings.activeBuildTarget);
        }

        //  [MenuItem("Tools/BuildAB/Win-And-Ios-Mac")]
        public static void BuildAssetAllBuildTargetResource()
        {
            ClearConsole();
            BuildAssetWinResource();
            BuildAssetMacResource();
            BuildAssetAndResource();
            BuildAssetIOSResource();
        }

        private static void BuildAssetResource(BuildTarget target)
        {
            abAssetBuildmaps.Clear();
            foreach (var item in abAssetBuildmapsPool)
            {
                abAssetBuildmaps.Add(new AssetBundleBuild()
                {
                    assetBundleName = item.Key,
                    assetNames = item.Value.Getpaths()
                });
            }

            abAssetBuildmapsPool.Clear();
            if (abAssetBuildmaps.Count == 0) Debug.LogError("please create ab name.");

            config = ABBuildConfig.Config;
            //清空一下缓存        
            Caching.ClearCache();
            string platform = ABUtility.GetPlatformForAssetBundles(target);
            Debug.Log("outputPath:" + platform);
            string rootPath = ABBuildConfig.AB_PATH;
            bool isFromConfigPath = true;
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
            {
                if (isFromCmd)
                {
                    isFromConfigPath = false;
                }
            }

            if (isFromConfigPath)
            {
                if (config != null)
                {
                    if (config.isUseCustomABOutputPath && !string.IsNullOrEmpty(config.customABOutputPath))
                        rootPath = config.customABOutputPath + ABConst.AB_ROOT;
                    Debug.Log("config.isUseCustomABOutputPath:" + config.isUseCustomABOutputPath);
                }
                else
                {
                    Debug.Log("config is null.");
                }
            }

            string outputPath = rootPath + "/" + platform + ABUtility.GetAssetbundleQuality();
            Debug.Log("outputPath:" + outputPath);
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            //BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.DeterministicAssetBundle, target);

            // BuildPipeline.BuildAssetBundles(outputPath, abAssetBuildmaps.ToArray(),
            //     BuildAssetBundleOptions.ChunkBasedCompression, target);

            BuildPipeline.BuildAssetBundles(outputPath, abAssetBuildmaps.ToArray(),
                BuildAssetBundleOptions.UncompressedAssetBundle, target);

            AssetDatabase.Refresh();
            Debug.Log("BuildAB count：" + abAssetBuildmaps.Count);
            abAssetBuildmaps.Clear();
            //modify 
            string oldPath = outputPath + "/" + platform + ABUtility.GetAssetbundleQuality();
            string newPath = outputPath + "/" + platform + ABConst.SUFFIX;
            if (File.Exists(oldPath))
            {
                if (File.Exists(newPath) && !string.IsNullOrEmpty(ABConst.SUFFIX))
                    File.Delete(newPath);
                File.Move(oldPath, newPath);

                oldPath = outputPath + "/" + platform + ABUtility.GetAssetbundleQuality() + ".manifest";
                newPath = outputPath + "/" + platform + ABConst.SUFFIX + ".manifest";
                if (File.Exists(newPath) && !string.IsNullOrEmpty(ABConst.SUFFIX))
                    File.Delete(newPath);
                File.Move(oldPath, newPath);

                Debug.Log("BuildAB complete! resource path：" + outputPath);
                RemoveInvalidResource(outputPath, target, platform);
            }

            AssetDatabase.Refresh();
        }

        private static void RemoveInvalidResource(string outputPath, BuildTarget target, string platform)
        {
            string mainAbPath = outputPath + "/" + ABUtility.GetPlatformForAssetBundles(target) + ABConst.SUFFIX;
            AssetBundle ab = AssetBundle.LoadFromFile(mainAbPath);
            AssetBundleManifest manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] abs = manifest.GetAllAssetBundles();
            ab.Unload(true);
            mainAbPath = mainAbPath.Replace(@"/", @"\");

            List<string> absList = new List<string>();
            for (int i = 0; i < abs.Length; i++)
            {
                absList.Add((outputPath + "/" + abs[i]).Replace(@"/", @"\"));
            }

            absList.Add(mainAbPath);
            List<FileInfo> fls = FindFileInfos(outputPath);
            int length = fls.Count;
            FileInfo finfo;
            for (int i = 0; i < length; i++)
            {
                finfo = fls[i];
                if (!absList.Contains(finfo.FullName))
                {
                    if (finfo.Extension == ".manifest")
                    {
                        string f = finfo.DirectoryName + @"\" + finfo.Name.Replace(".manifest", "");
                        if (absList.Contains(f))
                        {
                            continue;
                        }
                    }

                    File.Delete(finfo.FullName);
                    if (File.Exists(finfo.FullName + ".maifest"))
                        File.Delete(finfo.FullName + ".maifest");
                }

                EditorUtility.DisplayProgressBar("[" + length + "]Remove Invalid Resource", "[" + i + "]" /*+ path*/,
                    1f * i / length);
            }

            EditorUtility.ClearProgressBar();
            //del empty Directory
            List<DirectoryInfo> dls = FindDirectoryInfos(outputPath);
            for (int i = 0; i < dls.Count; i++)
            {
                if (Directory.Exists(dls[i].FullName))
                {
                    if (FindFileInfos(dls[i].FullName).Count == 0)
                    {
                        Directory.Delete(dls[i].FullName, true);
                    }
                }
            }

            Debug.Log("Remove Invalid Resource complete! " + outputPath);
            AssetDatabase.Refresh();
        }

        #endregion

        #region create and clear AB name.

        private static ABLocalConfig config;

        //  [MenuItem("Tools/ABName/BuildALLABName")]
        public static void BuildALLABName()
        {
            config = ABBuildConfig.Config;
            if (config == null)
            {
                Debug.LogError("loacl config miss!");
                return;
            }

            for (int i = 0; i < config.atlas_root_note.Count; i++)
            {
                string atlasPath = Path.Combine(ABBuildConfig.APPLICATION_PATH, config.atlas_root_note[i]);
                GenerateABName(atlasPath, GenerateResABName, ResType.Atlas); //图集.
            }

            for (int i = 0; i < config.res_Root_note.Count; i++)
            {
                string resPath = Path.Combine(ABBuildConfig.APPLICATION_PATH, config.res_Root_note[i]);
                GenerateABName(resPath, GenerateResABName, ResType.res); //资源.
            }

            Debug.Log("Build all ab name complete! " + ABConst.SUFFIX);
        }

        private static bool GenerateResABName(string path)
        {
            bool isContain = false;
            bool isLimit = false;

            if (config.isIgnorePartObjectABName)
            {
                for (int i = 0; i < config.Ignorefolder.Count; i++)
                {
                    if (path.StartsWith(config.Ignorefolder[i]))
                    {
                        isLimit = true;
                        break;
                    }
                }
            }

            if (!isLimit)
            {
                if (config.isBuildPartObjectABName)
                {
                    for (int k = 0; k < config.AbNameEditorPart.Count; k++)
                    {
                        if (path.StartsWith(config.AbNameEditorPart[k]))
                        {
                            isContain = true;
                            break;
                        }
                    }

                    if (isContain)
                        isLimit = false;
                    else
                        isLimit = true;
                }
            }

            return isLimit;
        }

        private static bool GenerateSceneABName(string path)
        {
            bool isContain = false;
            bool isLimit = false;
            if (config.isBuildPartSceneABName)
            {
                for (int k = 0; k < config.AbNameEditorScenePart.Count; k++)
                {
                    if (path.StartsWith(config.AbNameEditorScenePart[k]))
                    {
                        isContain = true;
                        break;
                    }
                }

                if (isContain)
                    isLimit = false;
                else
                    isLimit = true;
            }

            return isLimit;
        }

        private static void GenerateABName(string respath, System.Func<string, bool> execute, ResType resType)
        {
            ClearConsole();
            Caching.ClearCache();
            List<FileInfo> files = new List<FileInfo>();
            files.AddRange(FindFileInfos(respath));
            bool isClearName = false;
            List<string> ignoreLs = new List<string>() { ".meta", ".cs", ".js" };
            for (int i = 0; i < files.Count; i++)
            {
                if (ignoreLs.Contains(files[i].Extension))
                    continue;
                if (config.filtersuffix.Contains(files[i].Extension.ToLower()))
                {
                    isClearName = false;
                    string path = files[i].FullName.Replace('\\', '/').Substring(ABBuildConfig.PROJECT_PATH.Length);
                    if (execute != null && execute(path))
                    {
                        isClearName = true;
                    }

                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    string findFristchar;
                    if (importer)
                    {
                        if (isClearName)
                        {
                            SetAssetbundleName(importer);
                        }
                        else
                        {
                            string abName = path.Remove(0, 7); // path.Replace("Assets/", "");
                            abName = abName.Replace(files[i].Extension, "");
                            //   Debug.Log(abName);
                            switch (resType)
                            {
                                case ResType.Atlas: //图集打包方式不一样,单独区分
                                    TextureImporter import = importer as TextureImporter;
                                    if (import != null)
                                    {
                                        if (!string.IsNullOrEmpty(import.spritePackingTag))
                                        {
                                            abName = "atlas/" + import.spritePackingTag;
                                        }
                                    }

                                    break;
                                case ResType.res:
                                    string singleName = "";
                                    for (int h = 0; h < config.AbNameEditorAssembleLs.Count; h++)
                                    {
                                        singleName = config.AbNameEditorAssembleLs[h];
                                        if (abName.StartsWith(singleName))
                                        {
                                            findFristchar = abName.Replace(singleName, "");
                                            if (findFristchar.Length == 0 || findFristchar[0] == '/'
                                               ) //检查是否是文件夹，s/a11/a包含s/a1，但它们不属于一个bundle.
                                            {
                                                abName = singleName;
                                                break;
                                            }
                                        }
                                    }

                                    break;
                            }

                            EditorUtility.DisplayProgressBar(
                                "Set AssetName " + files.Count, /*string.Format("[{0}]:{1}", i, abName)*/i + "",
                                1f * i / files.Count);
                            SetAssetbundleName(importer, abName);
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }


        private static void SetAssetbundleName(AssetImporter importer, string name = null)
        {
            if (importer != null && !string.IsNullOrEmpty(name))
            {
                name += ABConst.SUFFIX;
                if (abAssetBuildmapsPool.TryGetValue(name, out var temp))
                {
                    temp.ls.Add(importer.assetPath);
                }
                else
                {
                    abAssetBuildmapsPool[name] = new ABBuildInfo(name, importer.assetPath);
                }
            }
        }

        /// <summary>
        /// 每个资源做单独assetbundle.
        /// </summary>
        // [MenuItem("Tools/ABName/BuildABFileName(Selection)")]
        public static void CreateABName()
        {
            Caching.ClearCache();
            UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object),
                SelectionMode.Assets | SelectionMode.ExcludePrefab);
            if (!(SelectedAsset.Length == 1))
                return;
            string fullPath = ABBuildConfig.PROJECT_PATH + AssetDatabase.GetAssetPath(SelectedAsset[0]);
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo dir = new DirectoryInfo(fullPath);
                var files = dir.GetFiles("*", SearchOption.AllDirectories);
                for (var i = 0; i < files.Length; ++i)
                {
                    var fileInfo = files[i];
                    EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 1f * i / files.Length);
                    string path = fileInfo.FullName.Replace('\\', '/').Substring(ABBuildConfig.PROJECT_PATH.Length);
                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    if (importer)
                    {
                        string name = SelectedAsset[0].name + "/" +
                                      path.Substring(fullPath.Substring(ABBuildConfig.PROJECT_PATH.Length).Length + 1);
                        SetAssetbundleName(importer, name.Substring(0, name.LastIndexOf('.')));
                    }
                }

                AssetDatabase.RemoveUnusedAssetBundleNames();
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            Debug.Log("Finish");
        }

        // [MenuItem("Tools/ABName/BuildABAtlasName(Selection)")]
        public static void CreateABAtlasName()
        {
            Caching.ClearCache();
            UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object),
                SelectionMode.Assets | SelectionMode.ExcludePrefab);
            if (!(SelectedAsset.Length == 1))
                return;
            string fullPath = ABBuildConfig.PROJECT_PATH + AssetDatabase.GetAssetPath(SelectedAsset[0]);
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo dir = new DirectoryInfo(fullPath);
                DirectoryInfo[] dirArray = dir.GetDirectories();
                for (int j = 0; j < dirArray.Length; j++)
                {
                    var files = dirArray[j].GetFiles("*", SearchOption.AllDirectories);
                    string name = SelectedAsset[0].name + "/" + dirArray[j].Name;
                    for (var i = 0; i < files.Length; ++i)
                    {
                        var fileInfo = files[i];
                        EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 1f * i / files.Length);
                        string path = fileInfo.FullName.Replace('\\', '/').Substring(ABBuildConfig.PROJECT_PATH.Length);
                        AssetImporter importer = AssetImporter.GetAtPath(path);
                        SetAssetbundleName(importer, name);
                    }
                }
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 整个文件所有资源打包成一个assetbundle.
        /// </summary>
        //  [MenuItem("Tools/ABName/BuildABFolderName(Selection)")]
        public static void CreateABFolderName()
        {
            Caching.ClearCache();
            UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object),
                SelectionMode.Assets | SelectionMode.ExcludePrefab);
            if (!(SelectedAsset.Length == 1))
                return;
            string fullPath = ABBuildConfig.PROJECT_PATH + AssetDatabase.GetAssetPath(SelectedAsset[0]);
            if (Directory.Exists(fullPath))
            {
                string name = SelectedAsset[0].name;
                DirectoryInfo dir = new DirectoryInfo(fullPath);
                var files = dir.GetFiles("*", SearchOption.AllDirectories);
                for (var i = 0; i < files.Length; ++i)
                {
                    var fileInfo = files[i];
                    EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 1f * i / files.Length);
                    string path = fileInfo.FullName.Replace('\\', '/').Substring(ABBuildConfig.PROJECT_PATH.Length);
                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    SetAssetbundleName(importer, name);
                }

                AssetDatabase.RemoveUnusedAssetBundleNames();
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        //  [MenuItem("Tools/ABName/GetAllABName")]
        public static void GetAllAssetBundleName()
        {
            string[] names = AssetDatabase.GetAllAssetBundleNames();

            foreach (var name in names)
            {
                Debug.Log(name);
            }
        }

        //  [MenuItem("Tools/ABName/ClearABName(Selection)")]
        public static void ClearResourcesABName()
        {
            Caching.ClearCache();
            UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object),
                SelectionMode.Assets | SelectionMode.ExcludePrefab);
            if (!(SelectedAsset.Length == 1))
                return;
            string fullPath = ABBuildConfig.PROJECT_PATH + AssetDatabase.GetAssetPath(SelectedAsset[0]);
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo dir = new DirectoryInfo(fullPath);
                var files = dir.GetFiles("*", SearchOption.AllDirectories);
                for (var i = 0; i < files.Length; ++i)
                {
                    var fileInfo = files[i];
                    EditorUtility.DisplayProgressBar("清除AssetName名称", "正在清除AssetName名称中...", 1f * i / files.Length);
                    string path = fileInfo.FullName.Replace('\\', '/').Substring(ABBuildConfig.PROJECT_PATH.Length);
                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    SetAssetbundleName(importer);
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }

        //  [MenuItem("Tools/ABName/ClearAllABName")]
        public static void ClearAllResourcesABName()
        {
            Caching.ClearCache();
            abAssetBuildmaps.Clear();
            abAssetBuildmapsPool.Clear();
            string[] abNameLs = AssetDatabase.GetAllAssetBundleNames();
            int length = abNameLs.Length;
            for (int i = 0; i < length; i++)
            {
                AssetDatabase.RemoveAssetBundleName(abNameLs[i], true);
                EditorUtility.DisplayProgressBar("[" + length + "]清除AssetName名称", "[" + i + "]" /*+ path*/,
                    1f * i / length);
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();

            Debug.Log("ClearAllABName complete!");
        }

        #endregion

        #region get fileInfo list and DirectoryInfo list

        public static List<FileInfo> FindFileInfos(string fullPath, string searchPattern = "*")
        {
            List<FileInfo> ls = new List<FileInfo>();
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo dir = new DirectoryInfo(fullPath);
                /*  for (int i = 0; i < ABBuildConfig.Filtersuffix.Length; i++)
                {
                    ls.AddRange(dir.GetFiles("*" + ABBuildConfig.Filtersuffix[i], SearchOption.AllDirectories));
                }*/
                ls.AddRange(dir.GetFiles(searchPattern, SearchOption.AllDirectories));
            }

            return ls;
        }

        public static List<DirectoryInfo> FindDirectoryInfos(string fullPath)
        {
            List<DirectoryInfo> ls = new List<DirectoryInfo>();
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo dir = new DirectoryInfo(fullPath);
                ls.AddRange(dir.GetDirectories("*", SearchOption.AllDirectories));
            }

            return ls;
        }

        #endregion

        public static void CheckAllFileDepends()
        {
            //string[] dir = Directory.GetFiles("Assets/", "*.*", SearchOption.AllDirectories);
            //Dictionary<string, int> dic = new Dictionary<string, int>();
            //for (int i = 0; i < dir.Length; ++i)
            //{
            //    if (dir[i].EndsWith(".meta", System.StringComparison.OrdinalIgnoreCase))
            //    {
            //        continue;
            //    }
            //    if (dir[i].EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
            //    {
            //        string[] array = AssetDatabase.GetDependencies(dir[i], true);
            //        for (int j = 0; j < array.Length; j++)
            //        {
            //            if (dic.ContainsKey(array[j]))
            //            {

            //            }
            //        }
            //        Debug.Log(dir[i] + "______" + string.Join(",", array));
            //    }
            //}
        }

        public static void CheckSigleFileDepends(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets/Resources/Export/Models/Restaurant/DiningCar.Prefab"; // test
                Debug.Log("test----------:" + path);
            }

            string[] array = AssetDatabase.GetDependencies(path, false);
            for (int i = 0; i < array.Length; i++)
            {
                Debug.Log(array[i]);
            }
        }
    }
}