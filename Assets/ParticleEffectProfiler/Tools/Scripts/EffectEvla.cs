#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主要用于计算overdraw像素
/// </summary>
public class EffectEvla
{
    public Camera _camera;
    SingleEffectEvla singleEffectEvla;
    public float time = 0;

    public virtual void Init(Camera camera)
    {
        SetCamera(camera);
    }

    public void InitData()
    {
        singleEffectEvla = new SingleEffectEvla(1);
    }

    public void SetEffectObj(GameObject go)
    {
        singleEffectEvla.SetEffectObj(go);
    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;
        camera.SetReplacementShader(Shader.Find("ParticleEffectProfiler/OverDraw"), "");
    }

    public void Update()
    {
        time += Time.deltaTime;
        RecordOverDrawData(singleEffectEvla);
    }

    public EffectEvlaData[] GetEffectEvlaData()
    {
        return singleEffectEvla.GetEffectEvlaDatas();
    }

    #region overdraw
    public void RecordOverDrawData(SingleEffectEvla singleEffectEvla)
    {
        long pixTotal = 0;
        long pixActualDraw = 0;

        GetCameraOverDrawData(out pixTotal, out pixActualDraw);

        // 往数据+1
        singleEffectEvla.UpdateOneData(pixTotal, pixActualDraw);
    }

    public void GetCameraOverDrawData(out long pixTotal, out long pixActualDraw)
    {
        RenderTexture activeTextrue = RenderTexture.active;
        RenderTexture rt = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
        _camera.targetTexture = rt;
        _camera.Render();
        RenderTexture.active = rt;
        Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        GetOverDrawData(texture, out pixTotal, out pixActualDraw);

        RenderTexture.active = activeTextrue;
        Texture2D.DestroyImmediate(texture);
        rt.Release();
        _camera.targetTexture = null;
    }

    public void GetOverDrawData(Texture2D texture, out long pixTotal, out long pixActualDraw)
    {
        var texw = texture.width;
        var texh = texture.height;

        var pixels = texture.GetPixels();

        int index = 0;

        pixTotal = 0;
        pixActualDraw = 0;

        for (var y = 0; y < texh; y++)
        {
            for (var x = 0; x < texw; x++)
            {
                float r = pixels[index].r;
                float g = pixels[index].g;
                float b = pixels[index].b;

                bool isEmptyPix = IsEmptyPix(r, g, b);
                if (!isEmptyPix)
                {

                    pixTotal++;
                }

                int drawThisPixTimes = DrawPixTimes(r, g, b);
                pixActualDraw += drawThisPixTimes;

                index++;
            }
        }
    }


    public int DrawPixTimes(float r, float g, float b)
    {
        return Convert.ToInt32(g / 0.04);
    }

    public bool IsEmptyPix(float r, float g, float b)
    {
        return r == 0 && g == 0 && b == 0;
    }
    #endregion
}
#endif
