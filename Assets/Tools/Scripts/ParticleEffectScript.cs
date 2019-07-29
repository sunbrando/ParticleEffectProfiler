#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ParticleEffectScript : MonoBehaviour {

    public AnimationCurve 粒子数量 = new AnimationCurve();
    public AnimationCurve DrawCall = new AnimationCurve();
    public AnimationCurve Overdraw = new AnimationCurve();
    //特效是否循环播放
    public bool 循环 = false;
    public float 秒数 = 3;

    EffectEvla m_EffectEvla;
    ParticleSystem[] m_ParticleSystems;
    MethodInfo m_CalculateEffectUIDataMethod;
    int m_ParticleCount = 0;
    int m_MaxParticleCount = 0;

    ParticleEffectCurve m_CurveParticleCount;
    ParticleEffectCurve m_CurveDrawCallCount;
    ParticleEffectCurve m_CurveOverdraw;


    void Awake()
    {
        Debug.Log("开始测试单个粒子系统");
        Application.targetFrameRate = 30;

        m_CurveParticleCount = new ParticleEffectCurve(循环, 秒数);
        m_CurveDrawCallCount = new ParticleEffectCurve(循环, 秒数);
        m_CurveOverdraw = new ParticleEffectCurve(循环, 秒数);

        m_EffectEvla = new EffectEvla();
        m_EffectEvla.Init(Camera.main);
        m_EffectEvla.InitData();
        m_EffectEvla.SetEffectObj(this.gameObject);
    }

    void Start()
    {
        m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();
#if UNITY_2017
        m_CalculateEffectUIDataMethod = typeof(ParticleSystem).GetMethod("CalculateEffectUIData", BindingFlags.Instance | BindingFlags.NonPublic);
#else
        m_CalculateEffectUIDataMethod = typeof(ParticleSystem).GetMethod("CountSubEmitterParticles", BindingFlags.Instance | BindingFlags.NonPublic);
#endif
    }

    private void LateUpdate()
    {
        RecordParticleCoun();
        m_EffectEvla.Update();

        UpdateParticleCountCurve();
        UpdateDrawCallCurve();
        UpdateOverdrawCurve();
    }

    public EffectEvlaData2[] GetEffectEvlaData()
    {
        return m_EffectEvla.GetEffectEvlaData();
    }

    public void RecordParticleCoun()
    {
        m_ParticleCount = 0;
        foreach (var ps in m_ParticleSystems)
        {
            int count = 0;
#if UNITY_2017
            object[] invokeArgs = { count, 0.0f, Mathf.Infinity };
            m_CalculateEffectUIDataMethod.Invoke(ps, invokeArgs);
            count = (int)invokeArgs[0];
#else
            object[] invokeArgs = { count };
            m_CalculateEffectUIDataMethod.Invoke(ps, invokeArgs);
            count = (int)invokeArgs[0];
            count += ps.particleCount;
#endif
            m_ParticleCount += count;
        }
        if (m_MaxParticleCount < m_ParticleCount)
        {
            m_MaxParticleCount = m_ParticleCount;
        }
    }

    public int GetParticleCount()
    {
        return m_ParticleCount;
    }
    public int GetMaxParticleCount()
    {
        return m_MaxParticleCount;
    }

    void UpdateParticleCountCurve()
    {
        粒子数量 = m_CurveParticleCount.UpdateAnimationCurve(m_ParticleCount);
    }
    void UpdateDrawCallCurve()
    {
        DrawCall = m_CurveDrawCallCount.UpdateAnimationCurve(GetParticleEffectData.GetOnlyParticleEffecDrawCall());
    }

    void UpdateOverdrawCurve()
    {
        EffectEvlaData2[] effectEvlaData2 = this.GetEffectEvlaData();
        Overdraw = m_CurveOverdraw.UpdateAnimationCurve((float)effectEvlaData2[0].GetPixRate());
    }

	//监听apply事件
    [InitializeOnLoadMethod]
    static void StartInitializeOnLoadMethod()
    {
        PrefabUtility.prefabInstanceUpdated = delegate(GameObject instance)
        {
            List<ParticleEffectScript> particleEffectScript = GetParticleEffectData.GetComponentByType<ParticleEffectScript>(instance);

            if (particleEffectScript.Count > 0)
            {
                Debug.LogError("保存前请先删除ParticleEffectScript脚本！");
            }
        };
    }
}
#endif
