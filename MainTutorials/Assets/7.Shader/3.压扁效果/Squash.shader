Shader "Hidden/Squash"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Top("Top",Float) = 0.5
        _Bottom("Bottom",Float) = -0.5
        _Degree("Squash Degree",Range(0,1)) = 0
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
            float _Top;
            float _Degree;
            float _Bottom;

            float GetNormalizedDist(float y)
            {
                //范围
                float range = _Top - _Bottom;
                //边界
                float _border = _Top;
                //当前顶点与边界的差值
                float dist = abs(y - _border);
                
                float normalizeDist = saturate(dist /range);
                return normalizeDist;
            }

            v2f vert (appdata v)
            {
                v2f o;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //本地Y轴
                float3 localNegativeY = float3(0,-1,0);
				float normalizedDist = GetNormalizedDist(v.vertex.y);
                //获取当前需下移的量
				float val = max(0, _Degree - normalizedDist);
                //顶点坐标向下移
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
