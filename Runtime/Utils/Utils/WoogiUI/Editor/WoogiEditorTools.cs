using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;


/// <summary>
/// Create by zhongbing at 2016.1.29 21:26
/// </summary>
public static class WoogiEditorTools
{

    static public bool DrawPrefixButton(string text,float width = 76f)
    {
        return GUILayout.Button(text, "DropDown", GUILayout.Width(width));
    }

    static public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
    {
        return DrawProperty(label, serializedObject, property, false, options);
    }

    static public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, bool padding, params GUILayoutOption[] options)
    {
        SerializedProperty sp = serializedObject.FindProperty(property);

        if (sp != null)
        {
            if (WoogiSettings.minimalisticLook) padding = false;

            if (padding) EditorGUILayout.BeginHorizontal();

            if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
            else EditorGUILayout.PropertyField(sp, options);

            if (padding)
            {
                DrawPadding();
                EditorGUILayout.EndHorizontal();
            }
        }
        return sp;
    }

    static public void DrawPadding()
    {
        if (!WoogiSettings.minimalisticLook)
            GUILayout.Space(18f);
    }

}