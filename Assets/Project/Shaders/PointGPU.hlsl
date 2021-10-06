#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<float3> _Positions;
	StructuredBuffer<float2> _uvs;
	StructuredBuffer<float3> _Normals;
#endif

float _Step;
float4 _color;
float1 _intensity;
float3 _worldPos;
float1 _scale;
float2 _uvPosition;
float3 _normalDirection;
float3 _objectRotation;

void ConfigureProcedural()
{ //run per vertex
#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	
	float3 position = _Positions[unity_InstanceID];
	_uvPosition = _uvs[unity_InstanceID];
	_normalDirection = _Normals[unity_InstanceID];
	
		float4x4 x_rotation = { 1.0,  0.0,  0.0, 0.0f,
								0.0,   cos(_objectRotation.x), -sin(_objectRotation.x), 0.0f,
								0.0, sin(_objectRotation.x),  cos(_objectRotation.x), 0.0f,
								0.0f, 0.0f,  0.0f, 1.0f};
	
		float4x4 y_rotation = { cos(_objectRotation.y),  0.0,  sin(_objectRotation.y), 0.0f,
							   0.0,   1.0,  0.0f, 0.0f,
					          -sin(_objectRotation.y), 0.0f,  cos(_objectRotation.y), 0.0f,
					           0.0f, 0.0f,  0.0f, 1.0f};
	
	
		float4x4 z_rotation = { cos(_objectRotation.z),  -sin(_objectRotation.z),  0.0, 0.0f,
								sin(_objectRotation.z),   cos(_objectRotation.z), 0.0, 0.0f,
								0.0, 0.0,  1.0, 0.0f,
								0.0f, 0.0f,  0.0f, 1.0f};
  
	
		float4x4 scale = {_Step,0.0,0.0,0.0,
							0.0,_Step,0.0,0.0,
							0.0,0.0,_Step,0.0,
							0.0,0.0,0.0,1.0 };
	
		float4x4 worldPos = {1.0,0.0,0.0,(position.x * _scale)+_worldPos.x,
							0.0,1.0,0.0,(position.y * _scale)+_worldPos.y,
							0.0,0.0,1.0,(position.z * _scale)+ _worldPos.z,
							0.0,0.0,0.0,1.0 };
	
		float4x4 localPos = {1.0,0.0,0.0,position.x * _scale,
							0.0,1.0,0.0,position.y * _scale,
							0.0,0.0,1.0, position.z * _scale,
							0.0,0.0,0.0,1.0 };
	
		float4x4 localPosNegative = {1.0,0.0,0.0,- position.x * _scale,
									0.0,1.0,0.0,- position.y * _scale,
									0.0,0.0,1.0,- position.z * _scale,
									0.0,0.0,0.0,1.0 };
	
	float4x4 rotationMatrix = mul(mul(x_rotation,y_rotation),z_rotation);
	unity_ObjectToWorld = 0;
    unity_ObjectToWorld =mul(mul(mul(mul(worldPos,localPosNegative),rotationMatrix),localPos),scale);
#endif
}

void ShaderGraphFunction_float(float3 In, out float3 Out, out float4 Color, out float1 Intensity, out float2 UVPosition, out float3 NormalDirection)
{
    Out = In;
    Color = _color;
    Intensity = _intensity;
    UVPosition = _uvPosition;
    NormalDirection = _normalDirection;

}

void ShaderGraphFunction_half(half3 In, out half3 Out, out float4 Color, out float1 Intensity, out float2 UVPosition, out float3 NormalDirection)
{
    Out = In;
    Color = _color;
    Intensity = _intensity;
    UVPosition = _uvPosition;
    NormalDirection = _normalDirection;
}