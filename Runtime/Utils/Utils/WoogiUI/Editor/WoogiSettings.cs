using UnityEngine;
using System.Collections;
using UnityEditor;

public static class WoogiSettings {
    
    static public void SetBool(string name, bool val) { EditorPrefs.SetBool(name, val); }

    static public bool GetBool(string name, bool defaultValue) { return EditorPrefs.GetBool(name, defaultValue); }

    static public bool minimalisticLook
    {
        get { return GetBool("WOOGI Minimalistic", false); }
        set { SetBool("WOOGI Minimalistic", value); }
    }


}
