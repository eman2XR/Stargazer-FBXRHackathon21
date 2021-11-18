using UnityEngine;
using UnityEngine.XR;

[ExecuteInEditMode]
public class MobileBloom : MonoBehaviour
{
    [Range(0, 2)]
    public float BloomDiffusion = 2f;
    public Color BloomColor = Color.white;
    [Range(0, 5)]
    public float BloomAmount = 1f;
    [Range(0, 1)]
    public float BloomThreshold = 0f;
    [Range(0, 1)]
    public float BloomSoftness = 0f;

    static readonly int blurAmountString = Shader.PropertyToID("_BlurAmount");
    static readonly int bloomColorString = Shader.PropertyToID("_BloomColor");
    static readonly int blDataString = Shader.PropertyToID("_BloomData");
    static readonly int bloomTexString = Shader.PropertyToID("_BloomTex");

    public Material material = null;
    private int numberOfPasses;
    private float knee;
    RenderTextureDescriptor half, quarter, eighths, sixths;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (BloomDiffusion == 0 && BloomAmount == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (XRSettings.enabled)
        {
            half = XRSettings.eyeTextureDesc;
            half.height /= 2; half.width /= 2;
            quarter = XRSettings.eyeTextureDesc;
            quarter.height /= 4; quarter.width /= 4;
            eighths = XRSettings.eyeTextureDesc;
            eighths.height /= 8; eighths.width /= 8;
            sixths = XRSettings.eyeTextureDesc;
            sixths.height /= XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.SinglePass ? 8 : 16; sixths.width /= XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.SinglePass ? 8 : 16;
        }
        else
        {
            half = new RenderTextureDescriptor(Screen.width / 2, Screen.height / 2);
            quarter = new RenderTextureDescriptor(Screen.width / 4, Screen.height / 4);
            eighths = new RenderTextureDescriptor(Screen.width / 8, Screen.height / 8);
            sixths = new RenderTextureDescriptor(Screen.width / 16, Screen.height / 16);
        }

        material.SetFloat(blurAmountString, BloomDiffusion);
        material.SetColor(bloomColorString, BloomAmount * BloomColor);
        knee = BloomThreshold * BloomSoftness;
        material.SetVector(blDataString, new Vector4(BloomThreshold, BloomThreshold - knee, 2f * knee, 1f / (4f * knee + 0.00001f)));
        numberOfPasses = Mathf.Clamp(Mathf.CeilToInt(BloomDiffusion * 4), 1, 4);
        material.SetFloat(blurAmountString, numberOfPasses > 1 ? BloomDiffusion > 1 ? BloomDiffusion : (BloomDiffusion * 4 - Mathf.FloorToInt(BloomDiffusion * 4 - 0.001f)) * 0.5f + 0.5f : BloomDiffusion * 4);
        RenderTexture blurTex = null;

        if (numberOfPasses == 1 || BloomDiffusion == 0)
        {
            blurTex = RenderTexture.GetTemporary(half);
            blurTex.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, blurTex, material, 0);
        }
        else if (numberOfPasses == 2)
        {
            blurTex = RenderTexture.GetTemporary(half);
            var temp1 = RenderTexture.GetTemporary(quarter);
            blurTex.filterMode = FilterMode.Bilinear;
            temp1.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, temp1, material, 0);
            Graphics.Blit(temp1, blurTex, material, 1);
            RenderTexture.ReleaseTemporary(temp1);
        }
        else if (numberOfPasses == 3)
        {
            blurTex = RenderTexture.GetTemporary(quarter);
            var temp1 = RenderTexture.GetTemporary(eighths);
            blurTex.filterMode = FilterMode.Bilinear;
            temp1.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, blurTex, material, 0);
            Graphics.Blit(blurTex, temp1, material, 1);
            Graphics.Blit(temp1, blurTex, material, 1);
            RenderTexture.ReleaseTemporary(temp1);
        }
        else if (numberOfPasses == 4)
        {
            blurTex = RenderTexture.GetTemporary(quarter);
            var temp1 = RenderTexture.GetTemporary(eighths);
            var temp2 = RenderTexture.GetTemporary(sixths);
            blurTex.filterMode = FilterMode.Bilinear;
            temp1.filterMode = FilterMode.Bilinear;
            temp2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, blurTex, material, 0);
            Graphics.Blit(blurTex, temp1, material, 1);
            Graphics.Blit(temp1, temp2, material, 1);
            Graphics.Blit(temp2, temp1, material, 1);
            Graphics.Blit(temp1, blurTex, material, 1);
            RenderTexture.ReleaseTemporary(temp1);
            RenderTexture.ReleaseTemporary(temp2);
        }
        material.SetTexture(bloomTexString, blurTex);
        RenderTexture.ReleaseTemporary(blurTex);
        Graphics.Blit(source, destination, material, 2);
    }
}