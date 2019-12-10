
#if !defined(MY_LIGHTING_INCLUDED)
#define MY_LIGHTING_INCLUDED       
    #include "UnityPBSLighting.cginc"
    #include "AutoLight.cginc"
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

        void ComputeVertexLightColor(inout v2f i)
        {
            #if defined(VERTEXLIGHT_ON)
                //pixel light count = 1
                float3 lightPos = float3(unity_4LightPosX0.x,unity_4LightPosY0.x,unity_4LightPosZ0.x);
                float3 lightVec = lightPos - i.worldPos;
                float3 lightDir = normalize(lightVec);
                float ndotl = DotClamped(i.normal,lightDir);
                float attenuation = 1 / (1 + dot(lightVec,lightVec) * unity_4LightAtten0.x);
                i.vertexLightColor = unity_LightColor[0].rgb * ndotl * attenuation;

                //pixel light count = 4
                // i.vertexLightColor = Shade4PointLights(
                //     unity_4LightPosX0,unity_4LightPosY0,unity_4LightPosZ0,
                //     unity_LightColor[0].rgb,unity_LightColor[1].rgb,
                //     unity_LightColor[2].rgb,unity_LightColor[3].rgb,
                //     unity_4LightAtten0,i.worldPos,i.normal
                // );
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

        sampler2D _MainTex;
        fixed4 _Tint;
        float _Smoothness;
        float _Metallic;



        UnityLight CreateLight(v2f i)
        {
            UnityLight light;
             #if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
                light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
            #else
                light.dir = _WorldSpaceLightPos0.xyz;
            #endif
            // //顶点与光照的方向
            // float3 lightVec = _WorldSpaceLightPos0.xyz - i.worldPos;
            // //衰减系数    确保光照系数在0时达到最大值
            // float attenuation = 1 / (1 + dot(lightVec,lightVec));

            UNITY_LIGHT_ATTENUATION(attenuation, 0, i.worldPos);
            light.color = _LightColor0.rgb * attenuation;
            light.ndotl = DotClamped(i.normal,light.dir);
            return light;
        }



        fixed4 frag (v2f i) : SV_Target
        {
            //https://blog.csdn.net/qq_38275140/article/details/85297474
            i.normal = normalize(i.normal);
            //光照方向
            //float3 lightDir = _WorldSpaceLightPos0.xyz;
            float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            //光照颜色
            //float3 lightColor = _LightColor0.rgb;
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

            // UnityLight light;
			// light.color = lightColor;
			// light.dir = lightDir;
			// light.ndotl = DotClamped(i.normal, lightDir);
			// UnityIndirect indirectLight;
			// indirectLight.diffuse = 0;
			// indirectLight.specular = 0; 

            // float3 shColor = ShadeSH9(float4(i.normal,1));
            // return float4(shColor,1);

            return UNITY_BRDF_PBS(
				albedo, specularTint,
				oneMinusReflectivity, _Smoothness,
				i.normal, viewDir,CreateLight(i),CreatIndirectLight(i)
			);
        }
    #endif
