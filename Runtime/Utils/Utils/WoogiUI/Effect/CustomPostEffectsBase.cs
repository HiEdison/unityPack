using UnityEngine;

/** Copyright(C) 2019 by 	WoogiWorld
 *All rights reserved.
 *ProductName:  		WoogiCreate
 *FileName:     		Learn12PostEffectsBase.cs
 *Author:       		None
 *Version:      		2.5.4
 *UnityVersion：		2018.3.0f2
 *Date:         		2019-10-15
 *Description:          No description
 *History:
*/

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CustomPostEffectsBase : MonoBehaviour
{

    protected void CheckResource()
    {
        if (!CheckSuppert())
        {
            NoSupperted();
        }
    }
    protected bool CheckSuppert()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            WDebug.LogError("no suppert image effect");
            return false;
        }
        return true;
    }
    protected void NoSupperted()
    {
        enabled = false;
    }
    // Start is called before the first frame update
    protected void Start()
    {
        CheckResource();
    }

    protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
    {
        if (shader == null) return null;
        if (shader.isSupported && material != null && material.shader == shader)
        {
            return material;
        }
        if (!shader.isSupported)
        {
            return null;
        }
        else
        {
            material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if (material)
                return material;
            else
                return null;
        }
    }

}
