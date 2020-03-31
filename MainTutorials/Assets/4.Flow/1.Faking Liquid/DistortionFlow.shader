﻿// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/DistortionFlow" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffest] _FlowMap("Flow (RG A noise)",2D) = "black" {}
		[NoScaleOffset] _DerivHeightMap ("Deriv (AG) Height(B)",2D) = "black" {}
		_UJump("U jump per phase",Range(-0.25,0.25)) = 0.25
		_VJump("V jump per phase",Range(-0.25,0.25)) = 0.25
		//平铺次数
		_Tiling("Tiling",float) = 1
		//流体速度
		_Speed("Speed",float) = 1
		//流体速度
		_FlowStrength("Flow Strength",float) = 1
		//流体偏移
		_FlowOffset("Flow Offset",float) = 0
		//流体高度
		_HeightScale("Height Scale, Constant",float) = 1
		//流体高度调节
		_HeightScaleModulated("Height Scale, Modulated",float) = 0.75
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#include "Flow.cginc"

		sampler2D _MainTex,_FlowMap,_DerivHeightMap;
		float _UJump,_VJump,_Tiling,_Speed,_FlowStrength,_FlowOffset,_HeightScale,_HeightScaleModulated;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float3 UnpackDerivativeHeight(float4 textureData)
		{
			float3 dh = textureData.agb;
			dh.xy = dh.xy * 2 - 1;
			return dh;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float3 flow = tex2D(_FlowMap,IN.uv_MainTex).rgb;
			flow.xy = flow.xy * 2 -1;
			flow *= _FlowStrength;
			float noise = tex2D(_FlowMap,IN.uv_MainTex).a;

			float time = _Time.y * _Speed + noise;
			float2 jump = float2(_UJump,_VJump);
			//错峰采样两次，A在波峰，B就在波谷。这样可以让A快要消失时，B马上出现(抵消褪色是黑色部分)
			float3 uvwA = FlowUVW(IN.uv_MainTex,flow,jump,_FlowOffset,_Tiling,time,false);
			float3 uvwB = FlowUVW(IN.uv_MainTex,flow,jump,_FlowOffset,_Tiling,time,true);

			float finalHeightScale = flow.z * _HeightScaleModulated + _HeightScale;

			float3 dhA = UnpackDerivativeHeight(tex2D(_DerivHeightMap,uvwA.xy)) * (uvwA.z * finalHeightScale);
			float3 dhB = UnpackDerivativeHeight(tex2D(_DerivHeightMap,uvwB.xy)) * (uvwB.z * finalHeightScale);

			o.Normal = normalize(float3(-(dhA.xy + dhB.xy),1));
			
			// Albedo comes from a texture tinted by color
			fixed4 texA = tex2D (_MainTex, uvwA.xy) * uvwA.z * _Color;
			fixed4 texB = tex2D (_MainTex, uvwB.xy) * uvwB.z * _Color;

			fixed4 c = (texA + texB) * _Color;

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
