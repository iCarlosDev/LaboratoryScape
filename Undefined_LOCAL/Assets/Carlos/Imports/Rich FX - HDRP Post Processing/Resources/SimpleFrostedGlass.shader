Shader "Hidden/InanEvin/RichFX/SimpleFrostedGlass"
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

	float rand(float2 uv) 
	{
		return frac(sin(dot(uv, float2(92.0, 80.0))) + cos(dot(uv, float2(41.0, 62.0))) * 51.0)* _Intensity;
	}

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);  
		float2 uv = input.texcoord;
		float2 rnd = float2(rand(uv), rand(uv));
		float2 offset = rnd * .01;
		offset.x *= clamp(uv.x * 10, 0, 1) * clamp((1 - uv.x) * 10, 0, 1);
		offset.y *= clamp(uv.y * 10, 0, 1) * clamp((1 - uv.y) * 10, 0, 1);
		uv += offset;
		return float4(LOAD_TEXTURE2D_X(_InputTexture, uv * _ScreenSize.xy));
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "SimpleFrostedGlass"

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
