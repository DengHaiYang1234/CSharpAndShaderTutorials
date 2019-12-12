Shader "Hidden/BlendMode_Effect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlendTex("Blend Texture",2D) = "white" {}
		_Opacity("Blend Opacity",Range(0.0,1.0)) = 1.0
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
			uniform sampler2D _BlendTex;
			fixed _Opacity;

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
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 renderTex = tex2D(_MainTex,i.uv);
				fixed4 blendTex = tex2D(_BlendTex,i.uv);

				//正片叠低
				//fixed4 blendMultiply = renderTex * blendTex;

				//ADD,与正片叠底正好相反
				//fixed4 blendAdd = renderTex + blendTex;

				fixed4 blendScreen = 1.0 - ((1.0 - renderTex) * (1.0 - blendTex));

				renderTex = lerp(renderTex,blendScreen,_Opacity);

				return renderTex;
			}
			ENDCG
		}
	}
}
