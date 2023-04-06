Shader "Hidden/InanEvin/RichFX/TextureDistortion"
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
	float _Speed;
	float2 _DistortionTextureSize;
    TEXTURE2D_X(_InputTexture);
	TEXTURE2D(_DistortionTexture);
	SAMPLER(sampler_DistortionTexture);

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float2 uv = input.texcoord;
		float2 uvDist = uv;
		uvDist.x = uvDist.x + _Time * _Speed;
		uvDist.y = uvDist.y + _Time * _Speed;
		float4 distortionColor = SAMPLE_TEXTURE2D(_DistortionTexture, sampler_DistortionTexture, uvDist);
		float offsetX = distortionColor.x / (1 / _Intensity);
		float offsetY = distortionColor.y / (1 / _Intensity);
		offsetX *= clamp(uv.x * 2, 0, 1) * clamp((1 - uv.x) * 2, 0, 1);
		offsetY *= clamp(uv.y * 2, 0, 1) * clamp((1 - uv.y) * 2, 0, 1);
		uv.x = uv.x + offsetX;
		uv.y = uv.y + offsetY;
		return LOAD_TEXTURE2D_X(_InputTexture, uv * _ScreenSize.xy);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "HeatDistortion"

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
