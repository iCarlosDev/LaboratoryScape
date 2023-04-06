Shader "Hidden/InanEvin/RichFX/SimpleOutline"
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
	float _Threshold;
    TEXTURE2D_X(_InputTexture);

	const float4 outline = float4(0.1, 0.1, 0.1, 1.0);


    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		
		float2 positionSS = input.texcoord * _ScreenSize.xy;
		float d1 = distance(LOAD_TEXTURE2D_X(_InputTexture, (positionSS + float2(0.0, -_Intensity))), LOAD_TEXTURE2D_X(_InputTexture, (positionSS + float2(0.0, _Intensity))));
		float d2 = distance(LOAD_TEXTURE2D_X(_InputTexture, (positionSS + float2(_Intensity, 0.0))), LOAD_TEXTURE2D_X(_InputTexture, (positionSS + float2(-_Intensity, 0.0))));

		if (max(d1,d2) >= _Threshold)
			return outline;
		else 
			return LOAD_TEXTURE2D_X(_InputTexture, positionSS);

    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "SimpleOutline"

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
