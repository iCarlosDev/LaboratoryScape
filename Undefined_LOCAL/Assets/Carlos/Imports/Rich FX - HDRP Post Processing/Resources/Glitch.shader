Shader "Hidden/InanEvin/RichFX/Glitch"
{
	SubShader
	{
		Pass
		{
			Cull Off ZWrite Off ZTest Always
			HLSLPROGRAM
			#pragma vertex Vertex
			#pragma fragment Fragment
			#include "Glitch1.hlsl"
			ENDHLSL
		}
		Pass
		{
			Cull Off ZWrite Off ZTest Always
			HLSLPROGRAM
			#define GLITCH_BASIC
			#pragma vertex Vertex
			#pragma fragment Fragment
			#include "Glitch1.hlsl"
			ENDHLSL
		}
		Pass
		{
			Cull Off ZWrite Off ZTest Always
			HLSLPROGRAM
			#define GLITCH_BLOCK
			#pragma vertex Vertex
			#pragma fragment Fragment
			#include "Glitch1.hlsl"
			ENDHLSL
		}
		Pass
		{
			Cull Off ZWrite Off ZTest Always
			HLSLPROGRAM
			#define GLITCH_BASIC
			#define GLITCH_BLOCK
			#pragma vertex Vertex
			#pragma fragment Fragment
			#include "Glitch1.hlsl"
			ENDHLSL
		}
	}
		Fallback Off
}
