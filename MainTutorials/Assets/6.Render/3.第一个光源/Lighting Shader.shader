Shader "Hidden/Lighting Shader"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _Tint("Ting",Color) = (1,1,1,1)
        _Smoothness("Smoothness",Range(0,1)) = 0.5
        [Gamma]_Metallic("Metallic",Range(0,1)) = 0
       //_SpecularTint("Specular",Color) = (0.5,0.5,0.5)
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
            #pragma vertex vert
            #pragma fragment frag

            //#include "UnityStandardBRDF.cginc"
            //#include "UnityStandardUtils.cginc"
            #include "UnityPBSLighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            sampler2D _MainTex;
            fixed4 _Tint;
            float _Smoothness;
            float _Metallic;

            fixed4 frag (v2f i) : SV_Target
            {
                i.normal = normalize(i.normal);
                //光照方向
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                //光照颜色
                float3 lightColor = _LightColor0.rgb;
                //材质的颜色被称为albedo。材质的漫反射率的颜色被称为材质的反射率。反射率是一个来自拉丁语的单词。因此，它描述了有多少红色、绿色和蓝色被漫反射。其余的部分被吸收。 我们可以使用材质的纹理和色调来定义材质的反射率
                float3 albedo = tex2D(_MainTex,i.uv).rgb * _Tint.rgb;

                //albedo *= 1 - (max(_SpecularTint.r,max(_SpecularTint.g,_SpecularTint.b)));
                float3 specularTint;

                float oneMinusReflectivity;
                //albedo = EnergyConservationBetweenDiffuseAndSpecular(albedo,_SpecularTint.rgb,oneMinusReflectivity);    
                albedo  = DiffuseAndSpecularFromMetallic(albedo,_Metallic,specularTint,oneMinusReflectivity);
                //albedo *= 1 - _SpecularTint;
                //漫反射
                //DotClamped: 限制点乘的结构为（0，1）
                // float3 diffuse = albedo * lightColor * DotClamped(lightDir,i.normal);
                // //漫反射
                // //return float4(diffuse,1);

                // //镜面反射
                // // float3 reflectionDir = reflect(-lightDir,i.normal);
                // // return pow(DotClamped(viewDir,reflectionDir),_Smoothness * 100);

                // //Blinn-Phong 
                // float3 halfVector  = normalize(lightDir + viewDir);
                // float3 specular = specularTint * lightColor * pow(DotClamped(halfVector,i.normal),_Smoothness * 100);
                // return fixed4(diffuse + specular,1);


                //return UNITY_BRDF_PBS(albedo,specularTint,oneMinusReflectivity,_Smoothness,i.normal,viewDir);

                UnityLight light;
				light.color = lightColor;
				light.dir = lightDir;
				light.ndotl = DotClamped(i.normal, lightDir);
				UnityIndirect indirectLight;
				indirectLight.diffuse = 0;
				indirectLight.specular = 0;

                return UNITY_BRDF_PBS(
					albedo, specularTint,
					oneMinusReflectivity, _Smoothness,
					i.normal, viewDir,light,indirectLight
				);


            }
            ENDCG
        }
    }
}
