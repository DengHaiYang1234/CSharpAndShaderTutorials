
#if !defined(MY_LIGHTING_INCLUDED)
#define MY_LIGHTING_INCLUDED       
    #include "UnityPBSLighting.cginc"
    #include "AutoLight.cginc"

    sampler2D _MainTex;
    float4 _MainTex_ST;
    fixed4 _Tint;
    float _Smoothness;
    float _Metallic;
    sampler2D _HeightMap;
    float4 _HeightMap_TexelSize;

    sampler2D _DetailTex;
    float4 _DetailTex_TexelSize;
    float4 _DetailTex_ST;

    sampler2D _NormalMap;
    float _BumpScale;

    sampler2D _DetailNormalMap;
    float _DetailBumpScale;

    
    struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
            float4 tangent : TANGENT;
        };

        struct v2f
        {
            float4 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            float3 normal : TEXCOORD1;

            #if defined(BINORMAL_PER_FRAGMENT)
                float4 tangent : TEXCOORD2;
            #else
                float3 tangent : TEXCOORD2;  
                float3 binormal : TEXCOORD3;
            #endif     

            float3 worldPos : TEXCOORD4;

            #if defined(VERTEXLIGHT_ON)
                float3 vertexLightColor : TEXCOORD5;
            #endif
        };

        float3 CreatBinormal(float3 normal,float3 tangent,float binormalSign)
        {
            return cross(normal,tangent.xyz) * (binormalSign * unity_WorldTransformParams.w);
        }

        // (勿删)初始化在片段着色器中的法线(手动计算凹凸法线)
        // void InitializeFragmentNormal(inout v2f i)
        // {
        //     float2 du = float2(_HeightMap_TexelSize.x * 0.5,0);
        //     float u1 = tex2D(_HeightMap,i.uv - du);
        //     float u2 = tex2D(_HeightMap,i.uv + du);
        //     //float3 tu = float3(1,u2-u1,0);


        //     float2 dv = float2(0,_HeightMap_TexelSize.y * 0.5);
        //     float v1 = tex2D(_HeightMap,i.uv - dv);
        //     float v2 = tex2D(_HeightMap,i.uv + dv);
        //     //float3 tv = float3(0,v2-v1,1);

        //     //i.normal = cross(tv,tu);
        //     i.normal = float3(u1 - u2,1,v1 - v2);
        //     i.normal = normalize(i.normal);
        // }

        

        void InitializeFragmentNormal(inout v2f i)
        {
            // i.normal.xy = tex2D(_NormalMap,i.uv).wy * 2 - 1;
            // i.normal.xy *= _BumpScale;
            // i.normal.z = sqrt(1 - saturate(dot(i.normal.xy,i.normal.xy)));
            float3 mainNormal = UnpackScaleNormal(tex2D(_NormalMap,i.uv.xy),_BumpScale);
            float3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap,i.uv.zw),_DetailBumpScale);
            //i.normal = float3(mainNormal.xy + detailNormal.xy,mainNormal.z * detailNormal.z);
            float3 tangentSpaceNormal = BlendNormals(mainNormal,detailNormal);
            tangentSpaceNormal = tangentSpaceNormal.xzy;

             #if defined(BINORMAL_PER_FRAGMENT)
                float3 binormal = CreatBinormal(i.normal,i.tangent.xyz,i.tangent.w);
            #else
                float3 binormal = i.binormal;
            #endif    


            //float3 binormal = cross(i.normal,i.tangent.xyz) * (i.tangent.w * unity_WorldTransformParams.w);

            i.normal = normalize(
                tangentSpaceNormal.x * i.tangent + 
                tangentSpaceNormal.y * binormal + 
                tangentSpaceNormal.z * i.normal
            );
        }

        void ComputeVertexLightColor(inout v2f i)
        {
            #if defined(VERTEXLIGHT_ON)
                i.vertexLightColor = Shade4PointLights(
                    unity_4LightPosX0,unity_4LightPosY0,unity_4LightPosZ0,
                    unity_LightColor[0].rgb,unity_LightColor[1].rgb,
                    unity_LightColor[2].rgb,unity_LightColor[3].rgb,
                    unity_4LightAtten0,i.worldPos,i.normal
                );
            #endif    
        }

        UnityIndirect CreatIndirectLight(v2f i)
        {
            UnityIndirect indirectLight;
            indirectLight.diffuse = 0;
            indirectLight.specular = 0;

            #if defined(VERTEXLIGHT_ON)
                indirectLight.diffuse = i.vertexLightColor;
            #endif

            #if defined(FORWARD_BASE_PASS)
                indirectLight.diffuse += max(0,ShadeSH9(float4(i.normal,1)));
            #endif
            
            return indirectLight;
        }

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv.xy = TRANSFORM_TEX(v.uv,_MainTex);
            o.uv.zw = TRANSFORM_TEX(v.uv,_DetailTex);
            o.worldPos = mul(unity_ObjectToWorld,v.vertex);
            o.normal = UnityObjectToWorldNormal(v.normal);
            
            #if defined(BINORMAL_PER_FRAGMENT)
                o.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz),v.tangent.w);
            #else
                o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.binormal = CreatBinormal(v.normal,v.tangent,v.tangent.w);
            #endif    

            
            ComputeVertexLightColor(o);
            return o;
        }

        UnityLight CreateLight(v2f i)
        {
            UnityLight light;
             #if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
                light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
            #else
                light.dir = _WorldSpaceLightPos0.xyz;
            #endif
            UNITY_LIGHT_ATTENUATION(attenuation, 0, i.worldPos);
            light.color = _LightColor0.rgb * attenuation;
            light.ndotl = DotClamped(i.normal,light.dir);
            return light;
        }



        fixed4 frag (v2f i) : SV_Target
        {
            //https://blog.csdn.net/qq_38275140/article/details/85297474
            InitializeFragmentNormal(i);
            //光照方向
            float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            //光照颜色
            //材质的颜色被称为albedo。材质的漫反射率的颜色被称为材质的反射率。反射率是一个来自拉丁语的单词。因此，它描述了有多少红色、绿色和蓝色被漫反射。其余的部分被吸收。 我们可以使用材质的纹理和色调来定义材质的反射率
            float3 albedo = tex2D(_MainTex,i.uv.xy).rgb * _Tint.rgb;
            albedo *= tex2D(_DetailTex,i.uv.zw) * unity_ColorSpaceDouble;

            float3 specularTint;

            float oneMinusReflectivity;

            albedo  = DiffuseAndSpecularFromMetallic(albedo,_Metallic,specularTint,oneMinusReflectivity);
            
            return UNITY_BRDF_PBS(
				albedo, specularTint,
				oneMinusReflectivity, _Smoothness,
				i.normal, viewDir,CreateLight(i),CreatIndirectLight(i)
			);
        }
    #endif
