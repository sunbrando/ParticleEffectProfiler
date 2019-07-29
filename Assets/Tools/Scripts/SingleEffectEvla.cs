#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SingleEffectEvla {
    private ParticleSystem[] _particleSystems;
    private EffectEvlaData2[] _effectEvlaData;
    private int _qualityIndex = 0;
    public GameObject _effectObj;
    
    public SingleEffectEvla(int qualityIndex)
    {
        _effectEvlaData = new EffectEvlaData2[EffectEvla2.Qualitys.Length];
        for (int i = 0; i < _effectEvlaData.Length; i++)
        {
            _effectEvlaData[i] = new EffectEvlaData2();
            _effectEvlaData[i].quality = EffectEvla2.Qualitys[i];
        }

        ChangeQuality(qualityIndex);
    }

    // 参数1：特效路径
    // 参数2：特效的位置
    public SingleEffectEvla(string effectPath, Vector3 position, int qualityIndex)
    {
        _effectEvlaData = new EffectEvlaData2[EffectEvla2.Qualitys.Length];
        for (int i = 0; i < _effectEvlaData.Length; i++)
        {
            _effectEvlaData[i].quality = EffectEvla2.Qualitys[i];
        }
        ChangeQuality(qualityIndex);
        this.LoadEffect(effectPath, position);
    }

    // 参数1：特效路径
    // 参数2：特效的位置
    public void LoadEffect(string effectPath, Vector3 position)
    {
        // 路径一样，忽略
        if (_effectEvlaData[_qualityIndex-1].effectPath.Equals(effectPath))
        {
            return;
        }

        // 清理之前的特效
        this.UnLoadEffect();

        // 设置特效路径
        _effectEvlaData[_qualityIndex-1].effectPath = effectPath;

        // 创建特效
        GameObject pbj = AssetDatabase.LoadAssetAtPath<GameObject>(effectPath);

        SetEffectObj(GameObject.Instantiate<GameObject>(pbj));
        // 位置
        _effectObj.transform.position = position;
        
        // 隐藏
        _effectObj.SetActive(false);

        // 记录数据
        RecordTape();
    }

    public void SetEffectObj(GameObject go)
    {
        _effectObj = go;
        // pslist
        _particleSystems = _effectObj.GetComponentsInChildren<ParticleSystem>(true);
    }

    // 清理特效
    public void UnLoadEffect()
    {
        _particleSystems = null;
        if (_effectObj != null)
        {
            GameObject.DestroyImmediate(_effectObj);
            _effectObj = null;
        }  
    }

    public void Simsimulate(float time)
    {
        if (!_effectObj.activeSelf)
        {
            _effectObj.SetActive(true);
            return;
        }
        
        if (_particleSystems != null)
        {
            foreach (ParticleSystem particle in _particleSystems)
            {
                particle.Simulate(time, true);
            }
        }
    }

    public void UpdateOneData(long pixDraw, long pixActualDraw)
    {
        if (pixDraw <= 0 && pixActualDraw <= 0)
        {
            //Debug.LogError("update " + pixDraw + "pixActualDraw" + pixActualDraw+"name"+ _effectEvlaData.effectPath);
            return;
        }
        EffectEvlaData2 effectEvlaData2 = GetEffectEvlaData();
        effectEvlaData2.pixTotal = effectEvlaData2.pixTotal + pixDraw;
        effectEvlaData2.pixActualDrawTotal = effectEvlaData2.pixActualDrawTotal + pixActualDraw;
        effectEvlaData2.pixDrawTimes = effectEvlaData2.pixDrawTimes + 1;

        //Debug.Log("update one" + pixDraw + "pixActualDraw" + pixActualDraw + _effectEvlaData.effectPath);
    }

    public EffectEvlaData2 GetEffectEvlaData()
    {
        return _effectEvlaData[_qualityIndex-1];
    }

    public EffectEvlaData2[] GetEffectEvlaDatas()
    {
        return _effectEvlaData;
    }

    public void Start()
    {
        _effectObj.SetActive(true);
    }
    public void Stop()
    {
        _effectObj.SetActive(false);
    }

    #region tape
    public void RecordTape()
    {
        int tapeHigh, tapeMiddle, tapeLow;
        GetEffectTape(out tapeHigh, out tapeMiddle, out tapeLow);
        for(int i = 0; i < _effectEvlaData.Length; i++)
        {
            EffectEvlaData2 effectEvlaData2 = _effectEvlaData[i];
            //"High", "Middle", "Low"
            if (effectEvlaData2.quality == "High")
            {
                effectEvlaData2.tape = tapeHigh;
            }
            else if (effectEvlaData2.quality == "Middle")
            {
                effectEvlaData2.tape = tapeMiddle;
            }
            else
            {
                effectEvlaData2.tape = tapeLow;
            }
        }
    }

    private void GetEffectTape(out int tapeHigh, out int tapeMiddle, out int tapeLow)
    {
        // 获取粒子使用的所有贴图
        tapeLow = 0;
        tapeMiddle = 0;
        tapeHigh = 0;
        foreach (ParticleSystem particle in _particleSystems)
        {
            string particleName = particle.name;
            Material material = particle.GetComponent<Renderer>().sharedMaterial;
            if (material != null)
            {
                Shader shader = material.shader;
                if (shader.name.IndexOf("Particles Add") != -1 || shader.name.IndexOf("add_follow_circle") != -1 || shader.name.IndexOf("Particles Additive_Ground") != -1)
                {
                    string[] name = new string[] { "_MainTex" };
                    int tH, tM, tL;
                    GetTape(particleName, material, name, out tH, out tM, out tL);
                    tapeHigh += tH;
                    tapeMiddle += tM;
                    tapeLow += tL;
                }

                if (shader.name.Equals("Biwu2/Effect/Dissolution"))
                {
                    string[] name = new string[] { "_MainTex", "_MaskTexture" };
                    int tH, tM, tL;
                    GetTape(particleName, material, name, out tH, out tM, out tL);
                    tapeHigh += tH;
                    tapeMiddle += tM;
                    tapeLow += tL;
                }

                if (shader.name.IndexOf("daoguang_rongjie") != -1)
                {
                    string[] name = new string[] { "_Diffuse", "_Noise" };
                    int tH, tM, tL;
                    GetTape(particleName, material, name, out tH, out tM, out tL);
                    tapeHigh += tH;
                    tapeMiddle += tM;
                    tapeLow += tL;
                }

                if (shader.name.IndexOf("Dissolution2") != -1)
                {
                    string[] name = new string[] { "_MainTex", "_AlphaMaskTex1", "_AlphaMaskTex2" };
                    int tH, tM, tL;
                    GetTape(particleName, material, name, out tH, out tM, out tL);
                    tapeHigh += tH;
                    tapeMiddle += tM;
                    tapeLow += tL;
                }

                if (shader.name.IndexOf("Energy Ball") != -1)
                {
                    string[] name = new string[] { "_MainTex", "_Blend_Texture", "_Blend_Texture01" };

                    int tH, tM, tL;
                    GetTape(particleName, material, name, out tH, out tM, out tL);
                    tapeHigh += tH;
                    tapeMiddle += tM;
                    tapeLow += tL;
                }
            }
        }

        //Debug.Log(GetEffectEvlaData().effectPath + "带宽" + "高" + tapeHigh + "中" + tapeMiddle + "低" + tapeLow);
    }

    private void GetTape(string particleName, Material material, string[] textureName, out int tapeHigh, out int tapeMiddle, out int tapeLow)
    {
        tapeHigh = 0;
        tapeMiddle = 0;
        tapeLow = 0;
        if (particleName.IndexOf("高") != -1)
        {
            tapeHigh = GetTapeFromTexture(material, textureName);
        }
        else if (particleName.IndexOf("中") != -1)
        {
            tapeHigh = GetTapeFromTexture(material, textureName);
            tapeMiddle = GetTapeFromTexture(material, textureName);
        }
        else
        {
            tapeHigh = GetTapeFromTexture(material, textureName);
            tapeMiddle = GetTapeFromTexture(material, textureName);
            tapeLow = GetTapeFromTexture(material, textureName);
        }
    }

    private int GetTapeFromTexture(Material material, string[] textureName)
    {
        int tape = 0;
        foreach (string name in textureName)
        {
            Texture texture = material.GetTexture(name);
            if (texture != null)
                tape += texture.width * texture.height;
        }
        return tape;
    }

    #endregion

    public void Print()
    {
        Debug.Log("---------------------");
        Debug.Log("特效路径：" + _effectEvlaData[_effectEvlaData.Length - 1].effectPath);
        for(int i = 0; i < _effectEvlaData.Length; i++)
        {
            _effectEvlaData[i].Print();
        }
        Debug.Log("---------------------");
    }

    public void ChangeQuality(int qualityIndex)
    {
        _qualityIndex = qualityIndex;
    }
}
#endif
