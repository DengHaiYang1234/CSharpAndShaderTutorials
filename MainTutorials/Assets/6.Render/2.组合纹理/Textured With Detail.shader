Shader "Hidden/Textured With Detail"
{
    Properties
    {
        _Tint("Tint",Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _DetailTex("Detail Texture",2D) = "gray" {}
    }
    SubShader
    {
        // No culling or depth
        //Cull Off ZWrite Off ZTest Always

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
                float2 uvDetail : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _DetailTex;
            float4 _MainTex_ST,_DetailTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv,_MainTex);
                o.uvDetail = TRANSFORM_TEX(v.uv,_DetailTex);
                return o;
            }


            fixed4 _Tint;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Tint;

                col *= tex2D(_DetailTex,i.uvDetail) * 2;

                return col;
            }
            ENDCG
        }
    }
}
