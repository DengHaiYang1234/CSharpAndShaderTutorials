// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Waves" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0


		//其实A和B，只在不同的一个方向上移动。可以形成类似海绵的的效果
		_WaveA("Wave A (dir(float2),steepness,wavelength)",Vector) = (1,0,0.5,10)
		_WaveB("Wave B",Vector) = (0,1,0.25,20)
		_WaveC("Wave C",Vector) = (1,1,0.15,10)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float4 _WaveA,_WaveB,_WaveC;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float3 GerstnerWave(float4 wave,float3 p,inout float3 tangent,inout float3 binormal)
		{
			float steepness = wave.z;
			float waveLength = wave.w;
			//一个_Wavelength长度的波在2PI周期类的波数
			float k = 2 * UNITY_PI / waveLength;
			//注：方向与波的陡峭都相同，修改波长的值为可以被开根号，且之后的值依然是2的幂次方。那么就可以见到长江后浪推前浪的效果。例如：64与16
			float c = sqrt(9.8 / k);
			float2 d = normalize(wave.xy);
			//每个波移动的速度是由时间决定的
			//float c = k * _Speed * _Time.y;
			//dot(d,p.xz)。在两个方向上移动。d为单位向量
			float f = k * (dot(d,p.xz) - c * _Time.y);
			float a = steepness / k;

			//导数	   求点的导数，得到的就是切线的点
			tangent += normalize(float3(
				1 - d.x * d.x * (steepness * sin(f)),
				d.x * (steepness * cos(f)),
				-d.x * d.y * (steepness * sin(f))
			));

			binormal += float3(
				-d.x * d.y * (steepness * sin(f)),
				d.y * (steepness * cos(f)),
				1 - d.y * d.y * (steepness * sin(f))
			);

			//某点的斜率的坐标
			return float3(
				d.x * (a * cos(f)),
				a * sin(f),
				d.y * (a * cos(f))
			);
		}

		void vert(inout appdata_full vertexData)
		{
			float3 gridPoint = vertexData.vertex.xyz;
			float3 tangent = float3(1,0,0);
			float3 binormal = float3(0,0,1);
			float3 p = gridPoint;
			p+=GerstnerWave(_WaveA,gridPoint,tangent,binormal);
			p+=GerstnerWave(_WaveB,gridPoint,tangent,binormal);
			p+=GerstnerWave(_WaveC,gridPoint,tangent,binormal);
			//法线解决光照与阴影的问题
			float3 normal = normalize(cross(binormal,tangent));
			vertexData.vertex.xyz = p;
			vertexData.normal = normal;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
