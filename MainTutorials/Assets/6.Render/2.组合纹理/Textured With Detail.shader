Shader "Hidden/Textured With Detail"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset]_Texture1("Texture 1",2D) = "white" {}
        [NoScaleOffset]_Texture2("Texture 2",2D) = "white" {}
        [NoScaleOffset]_Texture3("Texture 3",2D) = "white" {}
        [NoScaleOffset]_Texture4("Texture 4",2D) = "white" {}
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
                float2 uvSplat : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _Texture1;
            sampler2D _Texture2;
            sampler2D _Texture3;
            sampler2D _Texture4;
            float4 _MainTex_ST,_DetailTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv,_MainTex);
                o.uvSplat = v.uv;
                return o;
            }



            fixed4 frag (v2f i) : SV_Target
            {
                float4 splat = tex2D(_MainTex,i.uvSplat);
                return tex2D(_Texture1,i.uv) * splat.r  
                        + tex2D(_Texture2,i.uv) * splat.g
                        + tex2D(_Texture2,i.uv) * splat.b
                        + tex2D(_Texture2,i.uv) * (1 - splat.r - splat.g - splat.b);
                ;
            }
            ENDCG
        }
    }
}
