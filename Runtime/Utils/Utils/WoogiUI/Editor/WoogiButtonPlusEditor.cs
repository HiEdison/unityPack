using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using WoogiWorld.UI;

[CustomEditor(typeof(WoogiButtonPlus), true)]
public class WoogiButtonPlusEditor : Editor
{
    SerializedProperty monob;
    SerializedProperty funcName;
    SerializedProperty buttonPath;
    SerializedProperty isPlayClickSound;

    protected virtual void OnEnable()
    {
        isPlayClickSound = serializedObject.FindProperty("isPlayClickSound");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isPlayClickSound"));
        monob = serializedObject.FindProperty("mReceiveObj");
        if (monob.objectReferenceValue == null)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }
        monob = serializedObject.FindProperty("mReceiveObj");
        GUILayout.Space(6f);
        GUILayout.BeginHorizontal();
        WoogiEditorTools.DrawPrefixButton("Target");
        monob = WoogiEditorTools.DrawProperty("", serializedObject, "mReceiveObj", GUILayout.MinWidth(20f));
        GUILayout.EndHorizontal();

        //====================================
        GUILayout.BeginHorizontal();
        WoogiEditorTools.DrawPrefixButton("Func");
        funcName = serializedObject.FindProperty("mFunction");
        GUILayout.Label(funcName.stringValue);
        if (GUILayout.Button("Edit", GUILayout.Width(60f)))
        {
            if (monob.objectReferenceValue != null)
            {
                string type = monob.objectReferenceValue.GetType().ToString();
                string ScripteName = type;
                int lastIndex = type.LastIndexOf('.');//防止带有域名空间的
                if (lastIndex != 0)
                {
                    ScripteName = type.Substring(lastIndex + 1, type.Length - lastIndex - 1);
                }
                string[] reuslt = AssetDatabase.FindAssets(ScripteName);
                string path = "";
                for (int i = 0; i < reuslt.Length; i++)
                {
                    string tempPath = AssetDatabase.GUIDToAssetPath(reuslt[i]);
                    string tempPathFileName = tempPath.Substring(tempPath.LastIndexOf('/') + 1);
                    if ( tempPathFileName == (ScripteName + ".cs"))
                    {
                        path = tempPath;
                    }
                }
                if (path != "")
                {
                    //打开这个文件
                    TextAsset o = AssetDatabase.LoadAssetAtPath(path,typeof(TextAsset)) as TextAsset;
                    string[] content = o.text.Split('\n');
                    int line = -1;
                    string comStr = "void" + funcName.stringValue.Trim() + "(GameObject";
                    for (int i = 0; i < content.Length; i++)
                    {
                        string temstr = content[i].Replace(" ", "");
                        if (temstr.Contains(comStr))
                        {
                            line = i + 1;
                            break;
                        }
                    }
                    AssetDatabase.OpenAsset(o.GetInstanceID(), line);
                }
                else
                {
                    Debug.Log("No Script : " + reuslt[0]);
                }
                //Debug.LogError(ScripteName);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        buttonPath = serializedObject.FindProperty("mButtonPath");
        if( GUILayout.Button("CopyPath", GUILayout.Width(120f)) )
        {
            TextEditor  te = new TextEditor();
            te.text = buttonPath.stringValue;
            te.SelectAll();
            te.Copy();
            Debug.LogWarning("Copy Success : " + buttonPath.stringValue);
        }

        GUILayout.Label(buttonPath.stringValue);
        GUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
        WoogiTools.SetDirty(serializedObject.targetObject);
    }
}