
#if !defined(MY_LIGHTING_INCLUDED)
#define MY_LIGHTING_INCLUDED       
    #include "UnityPBSLighting.cginc"
    #include "AutoLight.cginc"

    sampler2D _MainTex;
    fixed4 _Tint;
    float _Smoothness;
    float _Metallic;
    sampler2D _HeightMap;
    float4 _Height_TexelSize;

    
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

            #if defined(VERTEXLIGHT_ON)
                float3 vertexLightColor : TEXCOORD3;
            #endif
        };

        //初始化在片段着色器中的发现
        void InitializeFragmentNormal(inout v2f i)
        {
            float h = tex2D(_HeightMap,i.uv);
            i.normal = float3(0,h,0);
            i.normal = normalize(i.normal);
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
            o.uv = v.uv;
            o.worldPos = mul(unity_ObjectToWorld,v.vertex);
            o.normal = UnityObjectToWorldNormal(v.normal);
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
            float3 albedo = tex2D(_MainTex,i.uv).rgb * _Tint.rgb;

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
