#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class GetParticleEffectData {

    static int m_MaxDrawCall = 0;

    public static string GetRuntimeMemorySize(GameObject go, out int textureCount)
    {
        var textures = new List<Texture>();
        textureCount = 0;
        long sumSize = 0;

        List<ParticleSystemRenderer> meshRendererlist = GetComponentByType<ParticleSystemRenderer>(go);

        foreach (ParticleSystemRenderer item in meshRendererlist)
        {
            if (item.sharedMaterial)
            {
                Texture texture = item.sharedMaterial.mainTexture;
                if (texture && !textures.Contains(texture))
                {
                    textures.Add(texture);
                    textureCount++;
                    sumSize = sumSize + GetStorageMemorySize(texture);
                }
            }
        }
        return EditorUtility.FormatBytes(sumSize);
    }

    private static int GetStorageMemorySize(Texture texture)
    {
        return (int)InvokeInternalAPI("UnityEditor.TextureUtil", "GetStorageMemorySize", texture);
    }

    private static object InvokeInternalAPI(string type, string method, params object[] parameters)
    {
        var assembly = typeof(AssetDatabase).Assembly;
        var custom = assembly.GetType(type);
        var methodInfo = custom.GetMethod(method, BindingFlags.Public | BindingFlags.Static);
        return methodInfo != null ? methodInfo.Invoke(null, parameters) : 0;
    }

    //获取物体对应的组件，包括子对象
    public static List<T> GetComponentByType<T>(GameObject go) where T : Component
    {
        List<T> list = new List<T>();

        Transform[] grandFa = go.GetComponentsInChildren<Transform>(true); //true代表可以查隐藏的子物体

        foreach (Transform child in grandFa)
        {
            foreach (var component in child.GetComponents<Component>())
            {
                if (component.GetType() == typeof(T))
                {
                    list.Add((T)component);
                }
            }
        }
        return list;
    }

    public static string GetGetRuntimeMemorySizeStr(GameObject go)
    {
        int textureCount;
        string memorySize = GetRuntimeMemorySize(go, out textureCount);
        return string.Format("贴图所占用的内存：{0}   建议：<100 KB\n贴图数量：{1} 建议：<5", memorySize, textureCount);
    }

    public static string GetParticleSystemCount(GameObject go)
    {
        var particleSystems = go.GetComponentsInChildren<ParticleSystem>(true);
        return string.Format("特效中所有粒子系统组件数量：{0}     建议：<5", particleSystems.Length);
    }

    public static int GetOnlyParticleEffecDrawCall()
    {
        //如果场景不只有特效这个值就不一定对
        //批处理的数值比正常值翻倍了？
        int drawCall = UnityEditor.UnityStats.batches / 2;
        if (m_MaxDrawCall<drawCall)
        {
            m_MaxDrawCall = drawCall;
        }
        return drawCall;
    }

    public static string GetOnlyParticleEffecDrawCallStr()
    {
        return string.Format("DrawCall: {0}   最高：{1}   建议：<10", GetOnlyParticleEffecDrawCall(), m_MaxDrawCall);
    }

    public static string GetPixDrawAverageStr(ParticleEffectScript particleEffectGo)
    {
        EffectEvlaData2[] effectEvlaData2 = particleEffectGo.GetEffectEvlaData();
        return effectEvlaData2[0].GetPixDrawAverageStr(); //0 即高品质的 
    }

    public static string GetPixActualDrawAverageStr(ParticleEffectScript particleEffectGo)
    {
        EffectEvlaData2[] effectEvlaData2 = particleEffectGo.GetEffectEvlaData();
        return effectEvlaData2[0].GetPixActualDrawAverageStr();
    }
    public static string GetPixRateStr(ParticleEffectScript particleEffectGo)
    {
        EffectEvlaData2[] effectEvlaData2 = particleEffectGo.GetEffectEvlaData();
        return effectEvlaData2[0].GetPixRateStr() + "   建议：<4";
    }

    public static string GetParticleCountStr(ParticleEffectScript particleEffectGo)
    {
        return string.Format("粒子数量：{0}   最高：{1}   建议：<50", particleEffectGo.GetParticleCount(), particleEffectGo.GetMaxParticleCount());
    }

    public static string GetCullingSupportedString(GameObject go)
    {
        List<ParticleSystem> particleSystems = GetComponentByType<ParticleSystem>(go);
        string text = "";
        foreach (ParticleSystem item in particleSystems)
        {
            string str = CheckCulling(item);
            if (!string.IsNullOrEmpty(str))
            {
                text += item.gameObject.name + ":" + str + "\n\n";
            }
        }
        return text;
    }

    static string CheckCulling(ParticleSystem particleSystem)
    {
        string text = "";
        if (particleSystem.collision.enabled)
        {
            text += "\n勾选了 Collision";
        }

        if (particleSystem.emission.enabled)
        {
            if (particleSystem.emission.rateOverDistance.curveMultiplier != 0)
            {
                text += "\nEmission使用了Current(非线性运算)";
            }
        }

        if (particleSystem.externalForces.enabled)
        {
            text += "\n勾选了 External Forces";
        }

        if (particleSystem.forceOverLifetime.enabled)
        {
            if (GetIsRandomized(particleSystem.forceOverLifetime.x)
                || GetIsRandomized(particleSystem.forceOverLifetime.y)
                || GetIsRandomized(particleSystem.forceOverLifetime.z)
                || particleSystem.forceOverLifetime.randomized)
            {
                text += "\nForce Over Lifetime使用了Current(非线性运算)";
            }
        } 
        if (particleSystem.inheritVelocity.enabled)
        {
            if (GetIsRandomized(particleSystem.inheritVelocity.curve))
            {
                text += "\nInherit Velocity使用了Current(非线性运算)";
            }
        } 
        if (particleSystem.noise.enabled)
        {
            text += "\n勾选了 Noise";
        } 
        if (particleSystem.rotationBySpeed.enabled)
        {
            text += "\n勾选了 Rotation By Speed";
        }
        if (particleSystem.rotationOverLifetime.enabled)
        {
            if (GetIsRandomized(particleSystem.rotationOverLifetime.x)
                || GetIsRandomized(particleSystem.rotationOverLifetime.y)
                || GetIsRandomized(particleSystem.rotationOverLifetime.z))
            {
                text += "\nRotation Over Lifetime使用了Current(非线性运算)";
            }
        } 
        if (particleSystem.shape.enabled)
        {
            ParticleSystemShapeType shapeType = (ParticleSystemShapeType)particleSystem.shape.shapeType;
            switch (shapeType)
            {
                case ParticleSystemShapeType.Cone:
                case ParticleSystemShapeType.ConeVolume:
#if UNITY_2017_1_OR_NEWER
                case ParticleSystemShapeType.Donut:
#endif
                case ParticleSystemShapeType.Circle:
                    if(particleSystem.shape.arcMode != ParticleSystemShapeMultiModeValue.Random)
                    {
                        text += "\nShape的Circle-Arc使用了Random模式";
                    }
                    break;
                case ParticleSystemShapeType.SingleSidedEdge:
                    if (particleSystem.shape.radiusMode != ParticleSystemShapeMultiModeValue.Random)
                    {
                        text += "\nShape的Edge-Radius使用了Random模式";
                    }
                    break;
                default:
                    break;
            }
        } 
        if (particleSystem.subEmitters.enabled)
        {
            text += "\n勾选了 SubEmitters";
        } 
        if (particleSystem.trails.enabled)
        {
            text += "\n勾选了 Trails";
        } 
        if (particleSystem.trigger.enabled)
        {
            text += "\n勾选了 Trigger";
        }
        if (particleSystem.velocityOverLifetime.enabled)
        {
            if (GetIsRandomized(particleSystem.velocityOverLifetime.x)
                || GetIsRandomized(particleSystem.velocityOverLifetime.y)
                || GetIsRandomized(particleSystem.velocityOverLifetime.z))
            {
                text += "\nVelocity Over Lifetime使用了Current(非线性运算)";
            }
        }
        if (particleSystem.limitVelocityOverLifetime.enabled)
        {
            text += "\n勾选了 Limit Velocity Over Lifetime";
        }
        if (particleSystem.main.simulationSpace != ParticleSystemSimulationSpace.Local)
        {
            text += "\nSimulationSpace 不等于 Local";
        }
        if (particleSystem.main.gravityModifierMultiplier != 0)
        {
            text += "\nGravityModifier 不等于0";
        }
        return text;
    }

    static bool GetIsRandomized(ParticleSystem.MinMaxCurve minMaxCurve)
    {
        bool flag = AnimationCurveSupportsProcedural(minMaxCurve.curveMax);

        bool result;
        if (minMaxCurve.mode != ParticleSystemCurveMode.TwoCurves && minMaxCurve.mode != ParticleSystemCurveMode.TwoConstants)
        {
            result = flag;
        }
        else
        {
            bool flag2 = AnimationCurveSupportsProcedural(minMaxCurve.curveMin);
            result = (flag && flag2);
        }

        return result;
    }

    static bool AnimationCurveSupportsProcedural(AnimationCurve curve)
    {
        //switch (AnimationUtility.IsValidPolynomialCurve(curve)) //保护级别，无法访问，靠
        //{
        //    case AnimationUtility.PolynomialValid.Valid:
        //        return true;
        //    case AnimationUtility.PolynomialValid.InvalidPreWrapMode:
        //        break;
        //    case AnimationUtility.PolynomialValid.InvalidPostWrapMode:
        //        break;
        //    case AnimationUtility.PolynomialValid.TooManySegments:
        //        break;
        //}
        return false; //只能默认返回false了
    }
}
#endif
