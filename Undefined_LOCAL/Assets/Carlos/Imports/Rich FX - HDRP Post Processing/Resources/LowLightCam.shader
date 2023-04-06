Shader "Hidden/InanEvin/RichFX/LowLightCam"
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
	float _VignetteIntensity;
	float _NoiseIntensity;
	float _Brightness;
	float _Contrast;
    TEXTURE2D_X(_InputTexture);

	#define D float4(0.5, 0.5, 0.0, 1.0)
	#define LIGHTCOLOR float4(0.4, 0.8, 0.6, 1)


    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float colorC = ((1.75 - _VignetteIntensity) - (distance(float4(input.texcoord.x, input.texcoord.y, 0.0, 1.0), D)) + sin(_Time * 2.0) / 16.0 * _Intensity) 
			* (frac(sin((0.1 + _Time) * dot(input.texcoord.xy, float2(12.9898, 78.233))) * 43758.5453) * _NoiseIntensity * _Intensity)
			* LOAD_TEXTURE2D_X(_InputTexture, float2(input.texcoord.x, input.texcoord.y) * _ScreenSize.xy).r;
		return ((colorC - 0.5) * _Contrast + _Brightness) * 2.0 * LIGHTCOLOR;

    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "LowLightCam"

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
