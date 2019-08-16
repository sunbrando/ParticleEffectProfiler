#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 折线图
/// </summary>
public class ParticleEffectCurve {

    public AnimationCurve animationCurve = new AnimationCurve();

    static float second = 3;
    static int FPS = 30;

    //打点的数量：默认90个
    float m_ValueCount = second * FPS;
    int onlyCount = 0;

    List<float> m_Values = new List<float>();

    public ParticleEffectCurve(float s)
    {
        second = s;
    }

    public AnimationCurve UpdateAnimationCurve(float value, bool loop)
    {
        m_ValueCount = second * FPS;

        if (loop)
        {
            if (m_Values.Count >= m_ValueCount)
            {
                m_Values.RemoveAt(0);
            }

            m_Values.Add(value);
            for (int i = 0; i < m_Values.Count - 1; i++)
            {
                if (animationCurve.length > i)
                {
                    animationCurve.RemoveKey(i);
                }
                animationCurve.AddKey((float)i, m_Values[i]);
            }
        }
        else
        {
            if (onlyCount < m_ValueCount)
            {
                onlyCount += 1;
                animationCurve.AddKey((float)onlyCount, value);
            }
        }

        return animationCurve;
    }

}
#endif