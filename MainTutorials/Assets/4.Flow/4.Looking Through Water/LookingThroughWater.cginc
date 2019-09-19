#if !defined(LOOKING_THROUGH_WATER_INCLUDED)
#define LOOKING_THROUGH_WATER_INCLUDED

//https://www.jianshu.com/p/80a932d1f11e

//https://www.jianshu.com/p/4e8162ed0c8d
sampler2D _CameraDepthTexture,_WaterBackground;
float4 _CameraDepthTexture_TexelSize;
float3 _WaterFogColor;
float _WaterFogDestity;
float _RefractionStrength;

float2 AlignWithGrabTexel(float2 uv)
{
	#if UNITY_UV_STARTS_AT_TOP
		if(_CameraDepthTexture_TexelSize.y < 0)
			uv.y = 1- uv.y;
	#endif

	return (floor(uv * _CameraDepthTexture_TexelSize.zw) + 0.5) * abs(_CameraDepthTexture_TexelSize.xy);
}

float3 ColorBelowWater(float4 screenPos,float3 tangentSpaceNormal)
{
	float2 uvOffset = tangentSpaceNormal.xy * _RefractionStrength;
	//垂直方向的偏移
	uvOffset.y *= _CameraDepthTexture_TexelSize.z * abs(_CameraDepthTexture_TexelSize.y);
	//投影在屏幕空间上的屏幕坐标
	float2 uv = AlignWithGrabTexel((screenPos.xy + uvOffset) / screenPos.w);
	#if UNITY_UV_STARTS_AT_TOP
		if(_CameraDepthTexture_TexelSize.y < 0)
			uv.y = 1- uv.y;
	#endif
	//https://www.jianshu.com/p/4e8162ed0c8d
	//获取相机渲染的背景深度（接近水底）
	float backgroundDepth =
		LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));////转换到摄像机空间
	//获取当前水与相机的深度值（也就是水表面与相机的在裁剪空间的z值）
	float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);
	//差值近似为水底与水面的深度距离
	float depthDifference = backgroundDepth - surfaceDepth;

	uvOffset *= saturate(depthDifference);
	uv = AlignWithGrabTexel((screenPos.xy + uvOffset) / screenPos.w);
	backgroundDepth =
		LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
	depthDifference = backgroundDepth - surfaceDepth;

	float3 backgroundColor = tex2D(_WaterBackground,uv).rgb;

	float fogFactor = exp2(-_WaterFogDestity * depthDifference);
	//越深越为蓝色
	return lerp(_WaterFogColor,backgroundColor,fogFactor);
}

#endif