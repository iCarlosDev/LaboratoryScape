Shader "Hidden/InanEvin/RichFX/Distort"
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

    // List of properties to control your post process effect
	float _Speed;
	float _Amplitude;
	float _FractionX;
	float _FractionY;
    TEXTURE2D_X(_InputTexture);

	float rand(float n) { return frac(sin(n) * 43758.5453123); }
	float noise(float p) 
	{
		float fl = floor(p);
		float fc = frac(p);
		return lerp(rand(fl), rand(fl + 1.0), fc);
	}

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float amp = (1.0 + sin(_Time * _Speed)) * 0.5 * _Amplitude;
		float offsetX = (noise(_Time + input.texcoord.x * _FractionX) * amp ) * clamp(input.texcoord.x * 10, 0.0, 1.0) * clamp((1 - input.texcoord.x) * 10, 0.0, 1.0);
		float offsetY = (noise(_Time + input.texcoord.y * _FractionY) * amp ) * clamp(input.texcoord.y * 10, 0.0, 1.0) * clamp((1 - input.texcoord.y) * 10, 0.0, 1.0);
		return float4(LOAD_TEXTURE2D_X(_InputTexture, (input.texcoord + float2(offsetX, offsetY)) * _ScreenSize.xy).xyz, 1);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Melt"

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
