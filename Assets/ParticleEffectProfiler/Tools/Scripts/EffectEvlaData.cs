#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理像素计算
/// </summary>
public class EffectEvlaData
{

    public long pixTotal = 0;                  // n次后的理论上总渲染数
    public long pixActualDrawTotal = 0;        // n次后的实际上渲染次数
    // 总调用渲染次数
    public int pixDrawTimes = 0;

    public string effectPath = "";
    // 带宽
    public int tape = 0;

    public string quality;

    public double GetPixDrawAverage()
    {
        if (pixDrawTimes == 0)
        {
            return 0;
        }
        return pixTotal * 1.0f / pixDrawTimes;
    }

    public double GetPixActualDrawAverage()
    {
        if (pixDrawTimes == 0)
        {
            return 0;
        }
        return pixActualDrawTotal * 1.0f / pixDrawTimes;
    }

    public double GetPixRate()
    {
        double pixDrawAverage = GetPixDrawAverage();
        if (pixDrawAverage == 0)
        {
            return 0;
        }
        return GetPixActualDrawAverage() / GetPixDrawAverage();
    }

    public string GetPixDrawAverageStr()
    {
        return "特效原填充像素点：" + Math.Round(this.GetPixDrawAverage(), 2);
    }
    public string GetPixActualDrawAverageStr()
    {
        return "特效实际填充像素点：" + Math.Round(this.GetPixActualDrawAverage(), 2);
    }
    public string GetPixRateStr()
    {
        return "平均每像素overdraw率：" + Math.Round(this.GetPixRate(), 2);
    }
    public string GetTapeStr()
    {
        return "带宽：" + this.tape;
    }
}
#endif