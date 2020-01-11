#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 折线图
/// </summary>
public class ParticleEffectCurve {

    public AnimationCurve animationCurve = new AnimationCurve();

    public static int FPS = 30;

    //打点的数量：默认90个
    int m_ValueCount = 3 * FPS;

    List<int> m_Values = new List<int>();

    public AnimationCurve UpdateAnimationCurve(int value, bool loop, int second)
    {
        m_ValueCount = second * FPS;

        if (animationCurve.length > m_ValueCount)
        {
            for (int i = animationCurve.length-1; i >= m_ValueCount; i--)
            {
                Debug.Log(i);
                animationCurve.RemoveKey(i);
                if (i <= m_Values.Count)
                {
                    m_Values.RemoveAt(i);
                }
            }
        }

        if (loop)
        {
            if (m_Values.Count >= m_ValueCount)
            {
                m_Values.RemoveAt(0);
            }

            m_Values.Add(value);
            for (int i = 0; i < m_Values.Count; i++)
            {
                if (animationCurve.length > i)
                {
                    animationCurve.RemoveKey(i);
                }
                animationCurve.AddKey(i, m_Values[i]);
            }
        }
        else
        {
            if (animationCurve.length < m_ValueCount)
            {
                animationCurve.AddKey(animationCurve.length, value);
            }
        }

        return animationCurve;
    }

}
#endif