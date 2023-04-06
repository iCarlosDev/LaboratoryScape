Shader "Hidden/InanEvin/RichFX/Pixelate"
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

    TEXTURE2D_X(_InputTexture);
	int _Pixelate;

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _ScreenSize.xy;
		if (_Pixelate > 1.0) 
		{
			float2 bs = 1.0 / float2(_ScreenSize.x, _ScreenSize.y) * _Pixelate;
			float2 cb = float2(floor(positionSS.x / bs.x) * bs.x, (floor(positionSS.y / bs.y) * bs.y));
			float4 outColor = float4(0.0, 0.0, 0.0, 0.0);
			float x4 = bs.x / 4.0;
			float x2 = bs.x / 2.0;
			float x075 = bs.x * 0.75;
			float y4 = bs.y / 4.0;
			float y2 = bs.y / 2.0;
			float y075 = bs.y * 0.75;

			outColor =  LOAD_TEXTURE2D_X(_InputTexture, cb + bs / 2.0);
			outColor += LOAD_TEXTURE2D_X(_InputTexture, cb + float2(x4, y4));
			outColor += LOAD_TEXTURE2D_X(_InputTexture, cb + float2(x2, y4));
			outColor += LOAD_TEXTURE2D_X(_InputTexture, cb + float2(x075, y4));
			outColor += LOAD_TEXTURE2D_X(_InputTexture, cb + float2(x4, y2));
			outColor += LOAD_TEXTURE2D_X(_InputTexture, cb + float2(x075, y2));
			outColor += LOAD_TEXTURE2D_X(_InputTexture, cb + float2(x4, y075));
			outColor += LOAD_TEXTURE2D_X(_InputTexture, cb + float2(x2, y075));
			outColor += LOAD_TEXTURE2D_X(_InputTexture, cb + float2(x075, y075));
			outColor /= 9.0;
			return outColor;
		}
		else 
			return LOAD_TEXTURE2D_X(_InputTexture, positionSS);

    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Pixelate"

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
