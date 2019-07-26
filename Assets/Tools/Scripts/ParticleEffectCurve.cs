#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectCurve {

    public AnimationCurve animationCurve = new AnimationCurve();

    //显示有限， 只记录这么多
    static int FPS = 30;
    float m_ValueCount = 3 * FPS;
    bool m_Loop;
    int onlyCount = 0;

    List<float> m_Values = new List<float>();

    public ParticleEffectCurve(bool loop, float time)
    {
        if (!loop)
        {
            m_ValueCount = time * FPS;
        }

        m_Loop = loop;
    }

    public AnimationCurve UpdateAnimationCurve(float value)
    {
        if (m_Loop)
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
