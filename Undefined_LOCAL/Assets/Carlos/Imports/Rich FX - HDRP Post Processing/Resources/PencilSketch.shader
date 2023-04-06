Shader "Hidden/InanEvin/RichFX/PencilSketch"
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

	int _Steps;
	float _Tolerance;
    TEXTURE2D_X(_InputTexture);

	#define multiplier float3(0.2126,0.7152,  0.0722 )
	#define ceilXMult 0.2126
	#define ceilYMult 0.7152
	#define ceilZMult 0.0722
	#define gDiffD float3(0.051, 0.640, 0.145) - _Tolerance;
	#define gDiffU float3(0.051, 0.640, 0.145) + _Tolerance;

	bool checkPixel(in float3 color)
	{
		float3 c1 = color - gDiffD;
		float3 c2 = color - gDiffU;
		return all(c1 < 0.0) && all(c2 > 0.0);
	}

	float csum(in float3 vec)
	{
		return ceil((vec.x + vec.y + vec.z) * _Steps) / _Steps;
	}

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float2 positionSS = input.texcoord * _ScreenSize;

		float3 outColor  = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;
		float3 east = LOAD_TEXTURE2D_X(_InputTexture, positionSS + float2(1.0, 0.0)).xyz;
		float3 west = LOAD_TEXTURE2D_X(_InputTexture, positionSS + float2(-1.0, 0.0)).xyz;
		float3 north = LOAD_TEXTURE2D_X(_InputTexture, positionSS + float2(0.0, -1.0)).xyz;
		float3 south = LOAD_TEXTURE2D_X(_InputTexture, positionSS + float2(0.0, 1.0)).xyz;
		float finalColor =  csum(multiplier * outColor);
		float lineThreshold = (1.0 / _Steps) - 0.05;


		if (((csum(east * multiplier) - finalColor >= lineThreshold || csum(south * multiplier) - finalColor >= lineThreshold || csum(west * multiplier)- finalColor >= lineThreshold || csum(north * multiplier) - finalColor >= lineThreshold) && fmod(finalColor / (1.0 / _Steps), 2.0))
			|| (checkPixel(east) || checkPixel(south) || checkPixel(north) || checkPixel(west)) ) return float4(0.0, 0.0, 0.0, 1.0);
		else
		{
			if (fmod(finalColor * _Steps, 3.0) == 1.0)
			{
				if (fmod(input.texcoord.x + input.texcoord.y, 2.0) == 0.0) return finalColor + (1.0 / _Steps);
				else
					return finalColor - (1.0 / _Steps);
			}
			else
			{
				if (fmod(input.texcoord.x + 0.5, 2.0) && fmod(input.texcoord.y + 0.5, 2.0)) return finalColor - (1.0 / _Steps);
				else
					return finalColor + (1.0 / _Steps);
			}
		}

    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "PencilSketch"

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
