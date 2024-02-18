using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ResourceRename
{
    private static readonly Regex NameRegex = CreateCompiled(@"^[a-zA-Z][a-zA-Z0-9_]*$");

    public static Regex CreateCompiled(string pattern, RegexOptions options = RegexOptions.None)
    {
#if !NET_2_0_SUBSET
        options |= RegexOptions.Compiled;
#endif
        return new Regex(pattern, options);
    }

//    AssetDatabase
//    [MenuItem("Tools/ABName/Rename")]
    static void ResRename()
    {
        string[] assetName = AssetDatabase.GetAllAssetPaths();
        string[] igonels = new[]
        {
            "Assets/Plugins/",
            ".cs",
            ".meta",
            "Assets/Resources/BuiltIn/Shader/"
        };
        string[] checkLs = new[]
        {
            "Assets/Resources/",
            "Assets/Sources",
        };
        string strName;
        string strName1;
        bool isIgone;
        bool isCheck;
        string rootPath = Application.dataPath;
        rootPath = rootPath.Replace("Assets", "");
        for (int i = 0; i < assetName.Length; i++)
        {
            EditorUtility.DisplayProgressBar("progress", "", i / (assetName.Length * 1f));
            strName = assetName[i];
            isCheck = isIgone = false;
            if (!File.Exists(rootPath + strName))
            {
                continue;
            }

            if (strName.Contains("Assets/"))
            {
                foreach (var item in igonels)
                {
                    if (strName.Contains(item))
                    {
                        isIgone = true;
                        break;
                    }
                }


                if (!isIgone)
                {
                    foreach (var item in checkLs)
                    {
                        if (strName.Contains(item))
                        {
                            isCheck = true;
                            break;
                        }
                    }

                    if (isCheck)
                    {
                        if ((strName.Contains("-") ||
                             strName.Contains(" ")))
                        {
                            AssetImporter import = AssetImporter.GetAtPath(strName);
                            if (import != null)
                            {
                                Object obj = AssetDatabase.LoadAssetAtPath<Object>(import.assetPath);
                                if (obj != null)
                                {
                                    strName1 = strName.Replace("-", "__");
                                    strName1 = strName1.Replace(" ", "___");
                                    Debug.Log(strName);
                                    Debug.Log(strName1);
//                                    EditorUtility.SetDirty(obj);
//                                    AssetDatabase.SaveAssets();

                                    File.Move(rootPath + strName, rootPath + strName1);
                                }
                            }
                        }
                    }
                }
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
}