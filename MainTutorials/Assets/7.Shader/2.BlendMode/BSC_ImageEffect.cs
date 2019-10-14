using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BSC_ImageEffect : MonoBehaviour {

    public Shader curShader;
    [Header("亮度调节")]
    public float brightnessAmount = 1.0f;
    [Header("饱和度调节")]
    public float saturationAmount = 1.0f;
    [Header("对比度调节")]
    public float contrastAmount = 1.0f;

    private Material curMaterial;



    //创建材质球
    public Material material
    {
        get
        {
            if (curMaterial == null)
            {
                curMaterial = new Material(curShader);
                curMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return curMaterial;
        }
    }

    private void Start()
    {
        if (SystemInfo.supportsImageEffects == false)
        {
            enabled = false;
            return;
        }

        if (curShader != null && curShader.isSupported == false)
        {
            enabled = false;
        }
    }


    //一旦通过下述的各种检查，我们就需要调用内置的OnRenderImage函数来实现画面特效。这个函数负责从Unity渲染器中抓取当前的render texture，
    //然后使用Graphics.Blit()函数再传递给Shader（通过sourceTexture参数），然后再返回一个处理后的图像再次传递回给Unity渲染器（通过destTexture参数）。
    //这两个行数互相搭配是处理画面特效的很常见的方法。你可以在下面的连接中找到这两个函数更详细的信息：
    //OnRenderImage：该摄像机上的任何脚本都可以收到这个回调（意味着你必须把它附到一个Camera上）。允许你修改最后的Render Texture。
    //Graphics.Blit：sourceTexture会成为material的_MainTex。

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (curShader != null)
        {
            material.SetFloat("_BrightnessAmount", brightnessAmount);
            material.SetFloat("_SaturationAmount", saturationAmount);
            material.SetFloat("_ContrastAmount", contrastAmount);

            Graphics.Blit(sourceTexture, destTexture, material);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }

    private void Update()
    {
        brightnessAmount = Mathf.Clamp(brightnessAmount, 0.0f, 2.0f);
        saturationAmount = Mathf.Clamp(saturationAmount, 0.0f, 2.0f);
        contrastAmount = Mathf.Clamp(contrastAmount, 0.0f, 3.0f);
    }

    private void OnDisable()
    {
        if (curMaterial != null)
        {
            DestroyImmediate(curMaterial);
        }
    }
}
