using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestParticleEffe{

    [MenuItem("GameObject/特效/测试", false, 11)]
    static void Test()
    {
        GameObject go = Selection.activeGameObject;
        List<ParticleSystemRenderer> particleSystemRenderer = GetParticleEffectData.GetComponentByType<ParticleSystemRenderer>(go);

        if (particleSystemRenderer.Count == 0)
        {
            Debug.LogError("不是特效无法测试！");
            return;
        }

        List<ParticleEffectScript> particleEffectScript = GetParticleEffectData.GetComponentByType<ParticleEffectScript>(go);

        if (particleEffectScript.Count == 0)
        {
            go.AddComponent<ParticleEffectScript>();
        }

        EditorApplication.isPlaying = true;
    }
}
