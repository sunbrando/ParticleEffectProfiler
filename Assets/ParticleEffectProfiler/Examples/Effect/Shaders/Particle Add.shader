// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ParticleEffectProfiler/Particles Add" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0

	[Space(10)]
    [Header(if you want effect to be displayed on the screen all the time please set Zwrite to off and set ZTest to Never)]
    [Enum(Off, 0, On, 1)] _ZWrite ("想特效不被地面遮挡：Off;否则设置On;(zw)", Float) = 1  //声明外部控制开关
    [Enum(Off, 0, On, 4)] _ZTest2 ("想特效不被地面遮挡：Off;否则On;(zt)", Float) = 4  //声明外部控制开关
    [Space(10)]

    [Enum(add, 1, blend, 10)] _DstBlend2 ("add模式:add;blend模式:blend", Float) = 1  //声明外部控制开关

    [Space(10)]
    //[Toggle] _NOALPHACHANNEL ("勾选以黑色做透明", Float) = 0  

    [KeywordEnum(NO, YES)] _NOALPHACHANNEL2 ("以黑色做透明", Float) = 0

    [Space(10)]

    [KeywordEnum(NO, YES)] _ISAPPLYFOG2 ("受雾效影响", Float) = 0
    //[Toggle] _ISAPPLYFOG ("勾选受雾效影响", Float) = 0 
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

	Blend SrcAlpha [_DstBlend2]
    ZTest [_ZTest2] 
    ZWrite [_ZWrite]

	//AlphaTest Greater .01
	
	//ColorMask RGB
	Cull Off 
	Lighting Off 

	//Fog { Mode Off }
	
	SubShader {

		Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
		Pass {
		
			CGPROGRAM
			#pragma exclude_renderers ps3 xbox360 flash xboxone ps4 psp2
            #pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			
			//#pragma shader_feature _NOALPHACHANNEL_ON
			//#pragma shader_feature _ISAPPLYFOG_ON
			#pragma multi_compile _ISAPPLYFOG2_NO _ISAPPLYFOG2_YES
			#pragma multi_compile _NOALPHACHANNEL2_NO _NOALPHACHANNEL2_YES

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;

				#if _ISAPPLYFOG2_YES
					UNITY_FOG_COORDS(1)
				#endif
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

				#if _ISAPPLYFOG2_YES
					UNITY_TRANSFER_FOG(o,o.vertex);
				#endif

				return o;
			}

			float _InvFade;
			
			fixed4 frag (v2f i) : SV_Target
			{	

				fixed4 color = tex2D(_MainTex, i.texcoord);
				

				#if _NOALPHACHANNEL2_YES
                	// 扣黑底
					color.a =  dot(color.rgb, fixed3(0.33, 0.33, 0.33));
                #endif

                fixed4 finalColor = 2.0f * i.color * _TintColor * color;

				#if _ISAPPLYFOG2_YES
					UNITY_APPLY_FOG(i.fogCoord, finalColor);
				#endif

				return finalColor;
			}
			ENDCG 
		}
	}	
}
}
