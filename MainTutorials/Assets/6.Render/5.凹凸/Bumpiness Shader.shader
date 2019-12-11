Shader "Hidden/Lighting Shader"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        [NoScaleOffset] _NormalMap("Normals",2D) = "bump" {}
        _BumpScale("Bump Scale",Float) = 1
        _Tint("Ting",Color) = (1,1,1,1)
        _Smoothness("Smoothness",Range(0,1)) = 0.5
        [Gamma]_Metallic("Metallic",Range(0,1)) = 0
        _DetailTex("Detail Tex",2D) = "gray" {}
        [NoScaleOffset] _DetailNormalMap("Detail Normals",2D) = "bump" {}
        _DetailBumpScale("Detail Bump Scale",Float) = 1
        [NoScaleOffset] _HeightMap("Custom Compute Normal Tex",2D) = "Gray" {}
    }
    
    SubShader
    {
        // No culling or depth
        //Cull Off ZWrite Off ZTest Always

        Pass
        {
            Tags{"LightMode" = "ForwardBase"}

            CGPROGRAM
            #pragma target 3.0

            #pragma multi_compile _ VERTEXLIGHT_ON

            #define FORWARD_BASE_PASS

            #pragma vertex vert
            #pragma fragment frag
            #include "MyLighting&Bump.cginc"
            ENDCG
        }

        Pass
        {
            Tags{"LightMode" = "ForwardAdd"}
            Blend One One
            Zwrite Off
            CGPROGRAM
            #pragma target 3.0
            //POINT  DIRECTIONAL  SPOT POINT_COOKIE     DIRECTIONAL_COOKIE
            #pragma multi_compile_fwdadd
            //#pragma multi_compile DIRECTIONAL DIRECTIONAL_COOKIE POINT SPOT

            #pragma vertex vert
            #pragma fragment frag
             #include "MyLighting&Bump.cginc"
            ENDCG
        }
    }
}
