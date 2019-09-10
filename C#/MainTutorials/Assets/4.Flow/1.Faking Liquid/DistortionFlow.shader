Shader "Custom/DistortionFlow" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffest] _FlowMap("Flow (RG A noise)",2D) = "black" {}
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

		sampler2D _MainTex,_FlowMap;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float2 flowVector = tex2D(_FlowMap,IN.uv_MainTex).rg * 2 - 1;

			float noise = tex2D(_FlowMap,IN.uv_MainTex).a;

			float time = _Time.y + noise;
			//错峰采样两次，A在波峰，B就在波谷。这样可以让A快要消失时，B马上出现(抵消褪色是黑色部分)
			float3 uvwA = FlowUVW(IN.uv_MainTex,flowVector,time,false);
			float3 uvwB = FlowUVW(IN.uv_MainTex,flowVector,time,true);

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
