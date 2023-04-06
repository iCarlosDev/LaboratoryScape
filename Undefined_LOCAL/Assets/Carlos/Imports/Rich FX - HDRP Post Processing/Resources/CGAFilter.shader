Shader "Hidden/InanEvin/RichFX/CGAFilter"
{
	HLSLINCLUDE

#pragma target 4.5
#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

		struct Attributes
	{
		uint vertexID : SV_VertexID;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		float2 texcoord   : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	Varyings Vert(Attributes input)
	{
		Varyings output;
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
		output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
		return output;
	}

	float _Intensity;
	float _Gamma;
	TEXTURE2D_X(_InputTexture);

	float4 _Colors[4];

	float4 findCol(float3 color)
	{
		float3 minCol = _Colors[0];
		float minDist = length(color - _Colors[0]);
		float magnitude = 0.0f;
		for (int i = 0; i < 3; i++)
		{
			if (i == 0)
			{
				magnitude = length(color - _Colors[1]);
				if (magnitude < minDist)
				{
					minCol = _Colors[1];
					minDist = magnitude;
				}
			}
			else if (i == 1)
			{
				magnitude = length(color - _Colors[2]);
				if (magnitude < minDist)
				{
					minCol = _Colors[2];
					minDist = magnitude;
				}
			}
			else if (i == 2)
			{
				magnitude = length(color - _Colors[3]);
				if (magnitude < minDist)
				{
					minCol = _Colors[3];
					minDist = magnitude;
				}
			}
		}

		return float4(minCol, 1);
	}

	float4 CustomPostProcess(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float2 positionSS = input.texcoord * _ScreenSize.xy;
		float3 c = float3(0.0, 0.0, 0.0);
		c = LOAD_TEXTURE2D_X(_InputTexture, positionSS).rgb;
		c.r = pow(abs(c.r), _Gamma);
		c.g = pow(abs(c.g), _Gamma);
		c.b = pow(abs(c.b), _Gamma);
		return findCol(c);
	}



		ENDHLSL

		SubShader
	{
		Pass
		{
			Name "CGAFilter"

			ZWrite Off
			ZTest Always
			Blend Off
			Cull Off

			HLSLPROGRAM
				#pragma fragment CustomPostProcess
				#pragma vertex Vert
			ENDHLSL
		}
	}
	Fallback Off
}
