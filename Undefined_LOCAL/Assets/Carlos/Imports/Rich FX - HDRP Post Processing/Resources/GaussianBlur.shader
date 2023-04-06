Shader "Hidden/InanEvin/RichFX/GaussianBlur"
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
	TEXTURE2D_X(_InputTexture);
	#define EXPMULT 0.39894

	float4 CustomPostProcess(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		const int halfSize = 5;
		float kernel[11];
		float3 finalColor = float3(0.0, 0.0, 0.0);
		float z = 0.0;

		for (int j = 0; j <= halfSize; ++j)
			kernel[halfSize + j] = kernel[halfSize - j] = EXPMULT * exp(-0.5 * j * j / (_Intensity*_Intensity)) / _Intensity;

		for (int j = 0; j < 11; ++j)
			z += kernel[j];

		for (int i = -halfSize; i <= halfSize; ++i)
			for (int j = -halfSize; j <= halfSize; ++j)
				finalColor += kernel[halfSize + j] * kernel[halfSize + i] *  LOAD_TEXTURE2D_X(_InputTexture, (input.texcoord * _ScreenSize.xy + float2(float(i), float(j)))).rgb;

		return float4(finalColor / (z*z), 1.0);
	}

		ENDHLSL

		SubShader
		{
		Pass
		{
			Name "GaussianBlur"

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
