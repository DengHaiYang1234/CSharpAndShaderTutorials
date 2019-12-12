Shader "Hidden/BSC_Effect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//亮度
		_BrightnessAmount("BrightnessAmount",Range(0.0,2.0)) = 1.0
		//饱和度
		_SaturationAmount("SaturationAmount",Range(0.0,1.0)) = 1.0
		//对比度
		_ContrastAmount("Contrast Amount",Range(0.0,1.0)) = 1.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			fixed _BrightnessAmount;
			fixed _SaturationAmount;
			fixed _ContrastAmount;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			//color:基色
			float3 ContrastSaturationBrightness(float3 color,float brt,float sat,float con)
			{
				float avgLumR = 0.5;
				float avgLumG = 0.5;
				float avgLumB = 0.5;

				//从图片中获取亮度的亮度系数
				float3 LuminanceCoeff = float3(0.2125,0.7154,0.0721);

				//亮度的强度计算
				float3 avgLumin = float3(avgLumR,avgLumG,avgLumB);
				float3 brtColor = color * brt;
				float intensityf = dot(brtColor,LuminanceCoeff);
				float3 intensity = float3(intensityf,intensityf,intensityf);
				//饱和度计算
				float3 satColor = lerp(intensity,brtColor,sat);
				//对比度
				float3 conColor = lerp(avgLumin,satColor,con);

				return conColor;

			}
			

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				col.rgb = ContrastSaturationBrightness(col.rgb,_BrightnessAmount,_SaturationAmount,_ContrastAmount);

				return col;
			}
			ENDCG
		}
	}
}
