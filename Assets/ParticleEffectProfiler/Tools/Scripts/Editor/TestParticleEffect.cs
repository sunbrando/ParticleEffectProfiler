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
    static bool isRestart = false;

    [MenuItem("GameObject/特效/测试", false, 11)]
    private static void Test()
    {
        var go = Selection.activeGameObject;
        var particleSystemRenderer = go.GetComponentsInChildren<ParticleSystemRenderer>(true);

        if (particleSystemRenderer.Length == 0)
        {
            Debug.LogError("不是特效无法测试！");
            return;
        }

        EditorPrefs.SetBool(RequestTestKey, true);

        //已经在播放状态，使其重新开始
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            isRestart = true;
        }
        else
        {
            EditorApplication.isPlaying = true;
        }

        var particleEffectScript = go.GetComponentsInChildren<ParticleEffectScript>(true);
        if (particleEffectScript.Length == 0)
        {
            go.AddComponent<ParticleEffectScript>();
        }
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
        }
    }

    private static void PlaymodeStateChanged()
    {
        if (!EditorApplication.isPlaying)
        {
            _hasPlayed = false;
        }

        if (isRestart)
        {
            EditorApplication.isPlaying = true;
            isRestart = false;
        }
    }
}
