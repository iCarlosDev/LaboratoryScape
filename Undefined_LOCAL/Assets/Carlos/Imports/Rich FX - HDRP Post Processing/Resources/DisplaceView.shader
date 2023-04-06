Shader "Hidden/InanEvin/RichFX/DisplaceView"
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

	float _AmountX;
	float _AmountY;
    TEXTURE2D_X(_InputTexture);

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
	    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		uint2 positionSS = input.texcoord * _ScreenSize.xy;
		half4 c = LOAD_TEXTURE2D_X(_InputTexture, positionSS);
		half4 n = c.rgba;

		half2 x = positionSS + half2(_AmountX * 8.0, _AmountY * 8.0);
		half2 y = positionSS + half2(_AmountX * 16.0, _AmountY * 16.0);
		half2 z = positionSS + half2(_AmountX * 24.0, _AmountY * 24.0);
	
		n += LOAD_TEXTURE2D_X(_InputTexture, x) * 0.5;
		n += LOAD_TEXTURE2D_X(_InputTexture, y) * 0.3;
		n += LOAD_TEXTURE2D_X(_InputTexture, z) * 0.2;
		n *= 0.5;
		return lerp(c, n, 0.5);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "DisplaceView"

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
