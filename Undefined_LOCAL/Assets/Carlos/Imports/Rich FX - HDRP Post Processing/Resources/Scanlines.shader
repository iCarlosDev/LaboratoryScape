Shader "Hidden/InanEvin/RichFX/Scanlines"
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
	float _Phase;
	float _Count;
	float _Noise;
    TEXTURE2D_X(_InputTexture);
	#define DOTVEC float2(12.9898, 78.233)
	#define FRACMULT 43758.5453

    float4 FragScanlines(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        uint2 positionSS = input.texcoord * _ScreenSize.xy;
		float n = frac(sin(dot(input.texcoord, DOTVEC)) * FRACMULT);
		float4 color = LOAD_TEXTURE2D_X(_InputTexture, positionSS);
		float3 result = color.rgb + color.rgb * saturate(0.1 + (n - 0.01 * floor(n / 0.01) * 100));
		float2 sc = float2(sin(input.texcoord.y * _Count), cos(input.texcoord.y * _Count));
		result += color.rgb * sc.xyx * _Intensity;
		result = color.rgb + saturate(_Noise) * (result - color.rgb);
		return float4(result, color.a);
    }


    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Scanlines"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment FragScanlines
                #pragma vertex Vert
            ENDHLSL
        }

		
    }
    Fallback Off
}
