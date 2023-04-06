Shader "Hidden/InanEvin/RichFX/SketchMotion"
{
	HLSLINCLUDE

#pragma target 4.5
#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
#pragma shader_feature  INVERT
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

	// List of properties to control your post process effect
	float _Intensity;
	float _Speed;
	float _MotionAmount;
	float _BaseModifier;
	float _Sketchiness;
	int _Colored;
	TEXTURE2D_X(_InputTexture);
#define RANDSIN 33758.5453


	float move(float x)
	{
		return abs(1.0 - fmod(abs(x), 2.0)) * _MotionAmount - (_MotionAmount);
	}

	float4 CustomPostProcess(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float time = floor(_Time * _Speed) / 16;

		float2 uv = input.texcoord;
		uv.x += move(uv.y * frac(sin(time) * RANDSIN) * 4.0) * frac(sin(time * 2) * RANDSIN) * 0.015 + frac(sin(uv.x * 2 + uv.y * 6.6) * RANDSIN) * _Sketchiness;
		uv.y += move(uv.x * frac(sin(time * 3) * RANDSIN) * 4.0) * frac(sin(time * 2) * RANDSIN) * 0.015 + frac(sin(uv.x * 1.5 + uv.y * 6.7) * RANDSIN) * _Sketchiness;
		uv = clamp(uv, 0, 1);

		float4 finalColor = float4(LOAD_TEXTURE2D_X(_InputTexture, input.texcoord * _ScreenSize.xy).rgb, 1.0);
		float4 edges = 1 - (finalColor * _BaseModifier / float4(LOAD_TEXTURE2D_X(_InputTexture, uv * _ScreenSize.xy).rgb, 1.0));

	#ifdef INVERT
		if (_Colored)
			finalColor.rgb = float3(finalColor.r, finalColor.g, finalColor.b);
		else
			finalColor.rgb = float3(finalColor.r, finalColor.r, finalColor.r);
		return finalColor / float4(length(edges), length(edges), length(edges), length(edges));
	#else
		if(_Colored)
			return finalColor + float4(length(edges), length(edges), length(edges), length(edges));
		else
			return float4(length(edges), length(edges), length(edges), length(edges));
	#endif
	}

	ENDHLSL

		SubShader
	{
		Pass
		{
			Name "SketchMotion"

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
