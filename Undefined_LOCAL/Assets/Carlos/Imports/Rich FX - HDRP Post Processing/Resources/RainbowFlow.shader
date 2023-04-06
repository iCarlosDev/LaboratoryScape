Shader "Hidden/InanEvin/RichFX/RainbowFlow"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
	#pragma shader_feature ISBW

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
	float _Multiplier;
	int _Steps;
	TEXTURE2D_X(_InputTexture);
	#define comp float3(0.2125, 0.7154, 0.0721)	
	#define cons float4(1.05, 0.666, 0.333, 3.0)


	float3 toRGB(float3 c) {
		float3 p = abs(frac(c.xxx + cons.xyz) * 6.0 - cons.www);
		return c.z * lerp(cons.xxx, clamp(p - cons.xxx, 0.0, 1.0), c.y);
	}

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float2 positionSS = input.texcoord * _ScreenSize.xy;
		float4 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS);

		float luma = dot(outColor, comp) * _Multiplier;
		float lumaIndex = floor(luma * _Steps);
		float rem = (luma - (lumaIndex / _Steps)) * _Steps;
		if (fmod(lumaIndex, 2.0) == 0.0) rem = 1.0 - rem;
		float tt = luma + (_Time * _Speed);

#if defined(ISBW)
		float bw = dot((float4(toRGB(float3(fmod(tt, 1.0), 1.0, rem)), 1.)), comp);
		return float4(float3(bw, bw, bw), 1.0);
#endif
		return float4(toRGB(float3(fmod(tt, 1.0), 1.0, rem)), 1.0);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "RainbowFlow"

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
