Shader "Hidden/InanEvin/RichFX/RGBDistortion"
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
	TEXTURE2D_X(_InputTexture);

	float distort(float d, float x) 
	{
		float r = 0.0;
		for (float i = 0.0; i < 15.0; i++)
			r += ((sin((d * 3.0 * sin(i / 2.142)) + (i / 1.41))) / 15.0) * 3.0;
		return pow(r, 3.0) / x;
	}

#define off1 _Intensity
#define off2 _Intensity + _Intensity / 10.0f
#define off3 _Intensity * 0.7f

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float TS = _Time * _Speed;
		float offset1 = distort((input.texcoord.y * off1) + TS, 200.0);
		float offset2 = distort((input.texcoord.y * off2) + TS, 201.0);
		float offset3 = distort((input.texcoord.y * off3) + TS, 202.0);

		offset1 *= clamp(input.texcoord.x * 10, 0, 1) * clamp((1 - input.texcoord.x) * 10, 0, 1);
		offset2 *= clamp(input.texcoord.x * 10, 0, 1) * clamp((1 - input.texcoord.x) * 10, 0, 1);
		offset3 *= clamp(input.texcoord.x * 10, 0, 1) * clamp((1 - input.texcoord.x) * 10, 0, 1);

		offset1 *= clamp(input.texcoord.y * 10, 0, 1) * clamp((1 - input.texcoord.y) * 10, 0, 1);
		offset2 *= clamp(input.texcoord.y * 10, 0, 1) * clamp((1 - input.texcoord.y) * 10, 0, 1);
		offset3 *= clamp(input.texcoord.y * 10, 0, 1) * clamp((1 - input.texcoord.y) * 10, 0, 1);

		float2 positionSSR = (input.texcoord + offset1) * _ScreenSize.xy;
		float2 positionSSG = (input.texcoord + offset2) * _ScreenSize.xy;
		float2 positionSSB = (input.texcoord + offset3) * _ScreenSize.xy;

		return float4(frac(float3(LOAD_TEXTURE2D_X(_InputTexture, positionSSR).r, LOAD_TEXTURE2D_X(_InputTexture, positionSSG).g, LOAD_TEXTURE2D_X(_InputTexture, positionSSB).b)), 1.0);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "RGBDistortion"

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
