using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 将特效的性能数据显示到Scene
/// </summary>
[CustomEditor(typeof(ParticleEffectScript))] 
public class MyParticleEffectUI : Editor {

    string[] m_Label = new string[20];

    void OnSceneGUI()
    {
        ParticleEffectScript particleEffectScript = (ParticleEffectScript)target;

        int index = 0;
        m_Label[index] = GetParticleEffectData.GetGetRuntimeMemorySizeStr(particleEffectScript.gameObject);
        m_Label[++index] = GetParticleEffectData.GetParticleSystemCount(particleEffectScript.gameObject);

        if (EditorApplication.isPlaying)
        {
            m_Label[++index] = GetParticleEffectData.GetOnlyParticleEffecDrawCallStr();
            m_Label[++index] = GetParticleEffectData.GetParticleCountStr(particleEffectScript);
            m_Label[++index] = GetParticleEffectData.GetPixDrawAverageStr(particleEffectScript);
            m_Label[++index] = GetParticleEffectData.GetPixActualDrawAverageStr(particleEffectScript);
            m_Label[++index] = GetParticleEffectData.GetPixRateStr(particleEffectScript);
        }

        ShowUI(); 
    }

    void ShowUI()
    {
        //开始绘制GUI
        Handles.BeginGUI();

        //规定GUI显示区域
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUIStyle style = new GUIStyle();
        style.richText = true;
        style.fontStyle = FontStyle.Bold;

        for (int i = 0; i < m_Label.Length; i++)
		{
            if (!string.IsNullOrEmpty(m_Label[i]))
	        {
		        GUILayout.Label(m_Label[i], style);
	        }
		}

        GUILayout.EndArea();

        Handles.EndGUI();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ParticleEffectScript particleEffectScript = (ParticleEffectScript)target;

        string autoCullingTips = GetParticleEffectData.GetCullingSupportedString(particleEffectScript.gameObject);
        if (!string.IsNullOrEmpty(autoCullingTips))
        {
            GUILayout.Label("ParticleSystem以下选项会导致无法自动剔除：", EditorStyles.whiteLargeLabel);
            GUILayout.Label(autoCullingTips);
        }
    }
}
