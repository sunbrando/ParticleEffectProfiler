#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 处理单个特效的像素点
/// </summary>
public class SingleEffectEvla {
    private EffectEvlaData[] _effectEvlaData;
    private int _qualityIndex = 0;
    public GameObject _effectObj;
    public static string[] Qualitys = { "High", "Middle", "Low" };
    
    public SingleEffectEvla(int qualityIndex)
    {
        _effectEvlaData = new EffectEvlaData[Qualitys.Length];
        for (int i = 0; i < _effectEvlaData.Length; i++)
        {
            _effectEvlaData[i] = new EffectEvlaData();
            _effectEvlaData[i].quality = Qualitys[i];
        }

        ChangeQuality(qualityIndex);
    }

    public void UpdateOneData(int pixDraw, int pixActualDraw)
    {
        if (pixDraw <= 0 && pixActualDraw <= 0)
        {
            return;
        }
        EffectEvlaData effectEvlaData = GetEffectEvlaData();
        effectEvlaData.pixTotal = effectEvlaData.pixTotal + pixDraw;
        effectEvlaData.pixActualDrawTotal = effectEvlaData.pixActualDrawTotal + pixActualDraw;
        effectEvlaData.pixDrawTimes = effectEvlaData.pixDrawTimes + 1;
    }

    public EffectEvlaData GetEffectEvlaData()
    {
        return _effectEvlaData[_qualityIndex-1];
    }

    public EffectEvlaData[] GetEffectEvlaDatas()
    {
        return _effectEvlaData;
    }

    public void ChangeQuality(int qualityIndex)
    {
        _qualityIndex = qualityIndex;
    }
}
#endif