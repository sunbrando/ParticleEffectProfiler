using UnityEditor;
using UnityEngine;

/// <summary>
/// 给选中的特效添加脚本的
/// </summary>
[InitializeOnLoad]
public static class TestParticleEffect
{
    private const string RequestTestKey = "TestParticleEffectRquestTest";
    private static bool _hasPlayed;

    [MenuItem("GameObject/特效/测试", false, 11)]
    private static void Test()
    {
        var go = Selection.activeGameObject;
        var particleSystemRenderer = GetParticleEffectData.GetComponentByType<ParticleSystemRenderer>(go);

        if (particleSystemRenderer.Count == 0)
        {
            Debug.LogError("不是特效无法测试！");
            return;
        }

        EditorPrefs.SetBool(RequestTestKey, true);
        EditorApplication.isPlaying = true;
    }

    static TestParticleEffect()
    {
        EditorApplication.update += Update;
        EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
    }

    private static void Update()
    {
        if (EditorPrefs.HasKey(RequestTestKey) && !_hasPlayed &&
            EditorApplication.isPlaying &&
            EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorPrefs.DeleteKey(RequestTestKey);
            _hasPlayed = true;

            var go = Selection.activeGameObject;

            var particleEffectScript = GetParticleEffectData.GetComponentByType<ParticleEffectScript>(go);

            if (particleEffectScript.Count == 0)
            {
                go.AddComponent<ParticleEffectScript>();
            }
        }
    }

    private static void PlaymodeStateChanged()
    {
        if (!EditorApplication.isPlaying)
        {
            _hasPlayed = false;
        }
    }
}
