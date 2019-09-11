#if !defined(FLOW_INCLUDED)
#define  FLOW_INCLUDED

float3 FlowUVW(float2 uv,float2 flowVector,float2 jump,float time,bool flowB)
{
	//flowB的起始UV坐标为0.5
	float phaseOffset = flowB ? 0.5 : 0;
	//返回小数部分
	float progress = frac(time + phaseOffset);
	float3 uvw;
	//向着UV的方向,并偏移phaseOffset
	uvw.xy = uv - flowVector * progress + phaseOffset;
	
	uvw.xy += (time - progress) * jump;
	//褪色混合权重  三角波函数：w(p) = 1 - |1 - 2p|,在p = 0 或 p = 1是变为黑色，中间位置颜色最亮
	uvw.z = 1 - abs(1 - 2 * progress);

	return uvw;
}

#endif