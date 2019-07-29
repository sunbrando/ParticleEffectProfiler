using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class TestParticleEffect
{
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

    private const string RequestTestKey = "TestParticleEffectRquestTest";
    private static bool _hasPlayed;
}
