Shader "Hidden/TestSquash"
{
    Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TopY("Top Y", Float) = 0 //The top Y of the GameObject in world coord
		_BottomY("Bottom Y", Float) = 0 
		_Control("Control Squash", Range(0, 1)) = 0 //control the level of squash
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _TopY;
			float _BottomY;
			float _Control;

			float GetNormalizedDist(float worldPosY)
			{
				float range = _TopY - _BottomY;
				float border = _TopY;

				float dist = abs(worldPosY - border);
				float normalizedDist = saturate(dist / range);
				return normalizedDist;
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //获取
                float3 localNegativeY = float3(0,-1,0);
				float normalizedDist = GetNormalizedDist(v.vertex.y);
				float val = max(0, _Control - normalizedDist);
				v.vertex.xyz += (localNegativeY * val);
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
