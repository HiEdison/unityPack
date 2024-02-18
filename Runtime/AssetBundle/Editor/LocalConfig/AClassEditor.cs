using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AClass))]
public class AClassEditor : Editor
{
    SerializedProperty filtersuffix;
    SerializedProperty atlas_root_note;
    SerializedProperty res_Root_note;

    SerializedProperty AbNameEditorAssembleLs;
    SerializedProperty isBuildPartObjectABName;
    SerializedProperty AbNameEditorPart;
    SerializedProperty isIgnorePartObjectABName;
    SerializedProperty Ignorefolder;
    SerializedProperty isBuildPartSceneABName;
    SerializedProperty level1SubdirectoryLs;
    SerializedProperty AbNameEditorScenePart;
    SerializedProperty isDelDirLs;
    SerializedProperty delDirLs;

    SerializedProperty isUseCustomABOutputPath;
    SerializedProperty customABOutputPath;

    SerializedProperty timestamped;

    string title = "This assetbundles local config. Timestamp:{0}\n<color=#ffff00ff>Please read it carefully and edit it!</color>";
    string animationTip = "请你看仔细了再编辑,不要***乱改!";
    float animationTipIndex = 0f;
    string jsonPath;
    string jsonFullPath;
    bool isRefresh = false;
    private Dictionary<SerializedProperty, List<string>> helps = new Dictionary<SerializedProperty, List<string>>();
    private void OnDisable()
    {
        if (!isRefresh)
        {
            object aclass = AssetDatabase.LoadAssetAtPath(GetAssetPath(), typeof(AClass));
            var str = JsonConvert.SerializeObject(aclass);
            try
            {
                File.WriteAllText(jsonFullPath, str);
            }
            catch
            {
                Debug.Log("error!");
            }
            AssetDatabase.Refresh();
        }
    }
    private AClass localConfig;
    private void OnEnable()
    {
        jsonFullPath = GetAssetFullPath().Replace(".asset", ".json");
        jsonPath = GetAssetPath().Replace(".asset", ".json");
        TextAsset txt = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
        if (txt != null)
        {
            localConfig = JsonConvert.DeserializeObject<AClass>(txt.text);
        }
        filtersuffix = serializedObject.FindProperty("filtersuffix");
        atlas_root_note = serializedObject.FindProperty("atlas_root_note");
        res_Root_note = serializedObject.FindProperty("res_Root_note");

        AbNameEditorAssembleLs = serializedObject.FindProperty("AbNameEditorAssembleLs");
        isBuildPartObjectABName = serializedObject.FindProperty("isBuildPartObjectABName");
        AbNameEditorPart = serializedObject.FindProperty("AbNameEditorPart");

        isIgnorePartObjectABName = serializedObject.FindProperty("isIgnorePartObjectABName");
        Ignorefolder = serializedObject.FindProperty("Ignorefolder");
        isBuildPartSceneABName = serializedObject.FindProperty("isBuildPartSceneABName");

        level1SubdirectoryLs = serializedObject.FindProperty("level1SubdirectoryLs");
        AbNameEditorScenePart = serializedObject.FindProperty("AbNameEditorScenePart");
        isDelDirLs = serializedObject.FindProperty("isDelDirLs");
        delDirLs = serializedObject.FindProperty("delDirLs");

        isUseCustomABOutputPath = serializedObject.FindProperty("isUseCustomABOutputPath");
        customABOutputPath = serializedObject.FindProperty("customABOutputPath");
        timestamped = serializedObject.FindProperty("timestamped");

        isRefresh = false;
        if (localConfig != null)
        {
            if (string.IsNullOrEmpty(localConfig.timestamped)) localConfig.timestamped = "";
            if (!localConfig.timestamped.Equals(timestamped.stringValue))
            {
                string text = File.ReadAllText(jsonFullPath);
                var asset = JsonConvert.DeserializeObject<AClass>(text);
                AssetDatabase.CreateAsset(asset, GetAssetPath());
                isRefresh = true;
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        else
        {
            localConfig = ScriptableObject.CreateInstance<AClass>();
        }
        #region add helps.
        helps.Clear();
        helps.Add(filtersuffix, new List<string>()
        {
            "All lower case.",
            "Value Format 0: .shader",
            "Value Format 1: .prefab"
        });
        helps.Add(atlas_root_note, new List<string>()
        {
            "类型：文件夹",
            "Root path:"+Application.dataPath+"/",
            "Value Format 0： Sources/Export/UI",
            "Value Format 1： Sources/BuiltIn/UI"
        });
        helps.Add(res_Root_note, new List<string>()
        {
            "类型：文件夹",
            "Root path:"+Application.dataPath+"/",
            "Value Format 0： Scenes",
            "Value Format 1： Resources/Export", "除开图集之外的资源，预制、散图、shader、场景等等"
        });
        helps.Add(AbNameEditorAssembleLs, new List<string>()
        {
            "类型：文件夹",
            "Root path:"+Application.dataPath+"/",
            "一个文件夹构建成1个bundle，全路径"
        });
        helps.Add(isBuildPartObjectABName, new List<string>()
        {
            "类型：文件",
            "编辑指定的对象可为assetbundle"
        });
        helps.Add(isIgnorePartObjectABName, new List<string>()
        {
            "类型：文件",
            "Root path:"+System.Environment.CurrentDirectory+"/",
            "For example：是否忽略指定的部分对象的assetbundle name"
        });
        helps.Add(isBuildPartSceneABName, new List<string>()
        {
            "For example：build Level 1 subdirectory list（文件夹列表）, 文件夹A将1级子目录文件构建成单个assetbundle, ",
            "如：A目录下有10个文件夹和2张图片，那么每个子文件夹和文件各自成单独的AB，合计12个ab." });
        helps.Add(AbNameEditorScenePart, new List<string>()
        {
            "类型：文件",
            "Root path:"+System.Environment.CurrentDirectory+"/",
            "编辑指定的部分场景的assetbundle name. path ls"
        });
        helps.Add(isDelDirLs, new List<string>()
        {
            "类型：文件(Scenes/Login.unity)",
            "类型：文件夹(Resources/Export)",
            "Root path:"+Application.dataPath+"/"
        });
        #endregion
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (!isRefresh)
        {
            index = 0;
            #region title
            GUI.skin.label.fontSize = 18;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.label.richText = true;
            GUILayout.Label(string.Format(title, string.IsNullOrEmpty(localConfig.timestamped) ? "0" : localConfig.timestamped));
            #endregion
            EditorGUILayout.BeginHorizontal();
            #region 左
            EditorGUILayout.BeginVertical("box");

            LayoutArray(filtersuffix, "Filter files for editable assetbundles.", helps[filtersuffix]);
            LayoutArray(atlas_root_note, "Atlas root node", helps[atlas_root_note]);
            LayoutArray(res_Root_note, "Resources root node", helps[res_Root_note]);
            LayoutArray(AbNameEditorAssembleLs, "Build an assetbundle with a folder", helps[AbNameEditorAssembleLs]);

            LayoutBoolArray(isBuildPartObjectABName, AbNameEditorPart, "Specify some resources to edit assetbundle", helps[isBuildPartObjectABName]);
            LayoutBoolArray(isIgnorePartObjectABName, Ignorefolder, "Ignore", helps[isIgnorePartObjectABName]);
            LayoutBoolArray(isBuildPartSceneABName, AbNameEditorScenePart, "Set part of the scene to build an assetbundle", helps[AbNameEditorScenePart]);

            LayoutArray(level1SubdirectoryLs, "Traversing subdirectories, each asset subdirectory generates 1 assetbundle", helps[isBuildPartSceneABName]);

            LayoutBoolArray(isDelDirLs, delDirLs, "Publish the AB type installation package, and set the AB resource list to be deleted.", helps[isDelDirLs]);

            FoldoutArea("Assetbundles save path", (bool isOpen) =>
            {
                if (isOpen)
                {
                    EditorGUI.indentLevel = 2;
                    GUI.contentColor = Color.white;
                    EditorGUILayout.PropertyField(isUseCustomABOutputPath, true);
                    if (isUseCustomABOutputPath.boolValue)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.indentLevel = 4;
                        EditorGUILayout.PropertyField(customABOutputPath);
                        if (GUILayout.Button("Select"))
                        {
                            customABOutputPath.stringValue = EditorUtility.OpenFolderPanel("Please select assetbundles save path.", Application.dataPath, "D:/Project/WoogiCreate/Resources");
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            });
            EditorGUILayout.EndVertical();
            #endregion
            #region 右
            EditorGUILayout.BeginVertical("box");
            if (GUILayout.Button(isOpenAllFoldout == true ? "Open All Foldout" : "Close All Foldout"))
            {
                for (int i = 0; i < foldoutLs.Count; i++)
                    foldoutLs[i] = isOpenAllFoldout;
                isOpenAllFoldout = !isOpenAllFoldout;
            }
            Rect rect = EditorGUILayout.BeginVertical();
            float cc = rect.width / animationTip.Length;
            float x = rect.x;
            float limitx = rect.x + rect.width;
            for (int i = 0; i < animationTip.Length; i++)
            {
                x = rect.x + cc * (i + (int)animationTipIndex);
                if (x > limitx)
                {
                    x -= rect.width;
                }
                GUI.Label(new Rect(x, (rect.y + 10) + 30 * i, 100, 40), animationTip[i].ToString(), SetStyle(new Color(1, 0, 0)));
            }
            animationTipIndex += 0.5f;
            if (animationTipIndex > animationTip.Length) animationTipIndex = 0;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            #endregion
            EditorGUILayout.EndHorizontal();
            timestamped.stringValue = System.DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }
        serializedObject.ApplyModifiedProperties();
    }
    private static List<bool> foldoutLs = new List<bool>();
    private static bool isOpenAllFoldout = true;
    private int index = 0;
    private void LayoutArray(SerializedProperty array, string msg, List<string> helps = null)
    {
        FoldoutArea(msg, (bool isOpen) =>
        {
            if (isOpen)
            {
                EditorGUI.indentLevel = 2;
                if (helps != null || helps.Count > 0)
                {
                    GUI.contentColor = Color.cyan;
                    string help = helps[0];
                    if (helps.Count > 1)
                    {
                        for (int i = 1; i < helps.Count; i++)
                        {
                            help += "\n" + helps[i];
                        }
                    }
                    EditorGUILayout.HelpBox(help, MessageType.Info);
                }
                CustomLayoutArray(array, 2);
            }
        });
    }

    private void LayoutBoolArray(SerializedProperty itemParent, SerializedProperty itemChild, string msg, List<string> helps = null)
    {
        FoldoutArea(msg, (bool isOpen) =>
        {
            if (isOpen)
            {
                EditorGUI.indentLevel = 2;
                GUI.contentColor = Color.white;
                EditorGUILayout.PropertyField(itemParent);
                if (itemParent.boolValue)
                {
                    if (helps != null || helps.Count > 0)
                    {
                        GUI.contentColor = Color.cyan;
                        string help = helps[0];
                        if (helps.Count > 1)
                        {
                            for (int i = 1; i < helps.Count; i++)
                            {
                                help += "\n" + helps[i];
                            }
                        }
                        EditorGUILayout.HelpBox(help, MessageType.Info);
                    }
                    CustomLayoutArray(itemChild, 2);
                }
            }
        });
    }

    private void CustomLayoutArray(SerializedProperty array, int currentNode = 2)
    {
        EditorGUI.indentLevel = currentNode * 2;
        GUI.contentColor = Color.white;
        if (EditorGUILayout.PropertyField(array/*, true*/))
        {
            EditorGUI.indentLevel = currentNode * 3;
            for (int i = 0; i < array.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i));
                if (GUILayout.Button("Del"))
                {
                    array.DeleteArrayElementAtIndex(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear All")) array.ClearArray();
            if (GUILayout.Button("Add")) array.arraySize += 1;
            EditorGUILayout.EndHorizontal();
        }
    }

    private void FoldoutArea(string lable, Action<bool> onExecute)
    {
        if (index != 0) EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("box"); //"Button" style
        EditorGUI.indentLevel = 0;
        if (foldoutLs.Count <= index) foldoutLs.Add(false);
        foldoutLs[index] = EditorGUILayout.Foldout(foldoutLs[index], lable);
        onExecute(foldoutLs[index]); //execute content.
        EditorGUILayout.EndVertical();
        index++;
        EditorGUI.indentLevel = 0;
    }

    private GUIStyle SetStyle(Color color, int fontSize = 30, Texture2D texture2D = null)
    {
        GUIStyle bb = new GUIStyle();
        bb.normal.background = texture2D;
        bb.normal.textColor = color;
        bb.fontSize = fontSize;
        return bb;
    }

    private string GetAssetFullPath()
    {
        return Application.dataPath.Replace("/Assets", "/") + AssetDatabase.GetAssetPath(target);
    }

    private string GetAssetPath()
    {
        return AssetDatabase.GetAssetPath(target);
    }

}
