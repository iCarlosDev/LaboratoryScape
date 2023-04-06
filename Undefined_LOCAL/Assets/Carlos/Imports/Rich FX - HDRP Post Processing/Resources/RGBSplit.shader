
Shader "Hidden/InanEvin/RichFX/RGBSplit"
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


	float2 AdjustUVForStereo(float2 uv, float4 st)
	{
#if defined(UNITY_SINGLE_PASS_STEREO)
		return UnityStereoScreenSpaceUVAdjust(uv, st);
#else
		return uv;
#endif
	}

	float _Intensity;
	float _CosAngle;
	float _SinAngle;
	TEXTURE2D_X(_InputTexture);

	float4 CustomPostProcess(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		uint2 positionSS = input.texcoord * _ScreenSize.xy;
		half2 offset = _Intensity * _ScreenSize.xy / 100.0f * half2(_CosAngle, _SinAngle);
		half cr = LOAD_TEXTURE2D_X(_InputTexture, positionSS + offset).r;
		half2 cga = LOAD_TEXTURE2D_X(_InputTexture, positionSS).ga;
		half cb = LOAD_TEXTURE2D_X(_InputTexture, positionSS - offset).b;
		return half4(cr, cga.x , cb, cga.y);
	}



		ENDHLSL

		SubShader
	{
		Pass
		{
			Name "RGBSplit"
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
