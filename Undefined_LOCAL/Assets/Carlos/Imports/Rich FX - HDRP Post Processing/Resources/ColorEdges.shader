Shader "Hidden/InanEvin/RichFX/ColorEdges"
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
	float4 _EdgeColor;
	float _EdgeWidth;
	float4 total;
	TEXTURE2D_X(_InputTexture);

	float getMultipCol(float2 uv, float mult)
	{
		float4 outColor = LOAD_TEXTURE2D_X(_InputTexture, uv) * total * mult;
		return (outColor.x + outColor.y + outColor.z);
	}

	float4 CustomPostProcess(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float2 positionSS = input.texcoord * _ScreenSize.xy;
		total = float4(0.2125, 0.7152, 0.0722, 0.0) * _Intensity;
		float left = 0.0;
		float up = 0.0;
		float2 ssMP = positionSS + float2(-_EdgeWidth, _EdgeWidth);
		float2 ssPP = positionSS + float2(_EdgeWidth, _EdgeWidth);
		float2 ssPM = positionSS + float2(_EdgeWidth, -_EdgeWidth);
		float2 ssP0 = positionSS + float2(_EdgeWidth, 0.0);
		float2 ssM0 = positionSS + float2(-_EdgeWidth, 0.0);
		float2 ssMM = positionSS + float2(-_EdgeWidth, -_EdgeWidth);
		float2 ss0M = positionSS + float2(0.0, -_EdgeWidth);
		float2 ss0P = positionSS + float2(0.0, _EdgeWidth);

		left += getMultipCol(ssPP, 1);
		left += getMultipCol(ssPM, 1);
		up += getMultipCol(ssMP, 1);
		up += getMultipCol(ssPP, 1);

		up += getMultipCol(ss0P, 2);
		left += getMultipCol(ssP0, 2);

		left += getMultipCol(ssM0, -2);
		left += getMultipCol(ssMP, -2);
		up += getMultipCol(ssMM, -2);
		up += getMultipCol(ss0M, -2);

		left *= left;
		up *= up;


		return float4(LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz + _EdgeColor * (left+up),1);
	}

		ENDHLSL

		SubShader
	{
		Pass
		{
			Name "ColorEdges"

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
