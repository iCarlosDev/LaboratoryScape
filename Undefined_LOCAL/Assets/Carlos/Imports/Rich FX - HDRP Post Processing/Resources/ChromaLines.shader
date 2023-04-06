Shader "Hidden/InanEvin/RichFX/ChromaLines"
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
	int _ScanlinesCount;
	float _ScanlinesIntensity;
	float _Speed;
    TEXTURE2D_X(_InputTexture);

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float2 uv = input.texcoord;
		float d = length(uv - float2(0.5, 0.5));

		float blur = 0.0;
		blur = (1.0 + sin(_Time * _Speed)) * 0.5;
		blur *= 1.0 + sin(_Time * _Speed * 2.0f) * 0.5;
		blur = pow(blur, 3.0);
		blur *= _Intensity / 10;
		blur *= d;

		float3 col;
		col.r = LOAD_TEXTURE2D_X(_InputTexture, float2(uv.x + blur, uv.y) * _ScreenSize.xy).r;
		col.g = LOAD_TEXTURE2D_X(_InputTexture, uv * _ScreenSize.xy).g;
		col.b = LOAD_TEXTURE2D_X(_InputTexture, float2(uv.x - blur, uv.y) * _ScreenSize.xy).b;
		col -= sin(uv.y * _ScanlinesCount) * _ScanlinesIntensity;
		col *= 1.0 - d * 0.5;
		return float4(col, 1);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "ChromaLines"

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
