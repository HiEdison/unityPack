using UnityEngine;

/** Copyright(C) 2019 by 	WoogiWorld
 *All rights reserved.
 *ProductName:  		WoogiCreate
 *FileName:     		Learn12_BrightnessSaturationAndContrast.cs
 *Author:       		None
 *Version:      		2.5.4
 *UnityVersion：		2018.3.0f2
 *Date:         		2019-10-15
 *Description:          No description
 *History:
*/

public class CustomBrightnessSaturationAndContrast : CustomPostEffectsBase
{
    public Shader briSatConShader;
    private Material briSatConMaterial;

    private void Awake()
    {
        if (briSatConShader == null)
            briSatConShader = Shader.Find("Custom/learn12_BrightnessSaturationAndContrast");
    }
    public Material material
    {
        get
        {
            briSatConMaterial = CheckShaderAndCreateMaterial(briSatConShader, briSatConMaterial);
            return briSatConMaterial;
        }
    }

    [Range(0.0f, 3.0f)]
    public float brightness = 1.0f;

    [Range(0.0f, 3.0f)]
    public float saturation = 1.0f;

    [Range(0.0f, 3.0f)]
    public float contrast = 1.0f;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            material.SetFloat("_Brightness", brightness);
            material.SetFloat("_Saturation", saturation);
            material.SetFloat("_Contrast", contrast);

            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
