Shader "Hidden/InanEvin/RichFX/LetterBox"
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

	half _Offset;
	half _OffsetInv;
	half4 _Color;
    TEXTURE2D_X(_InputTexture);

    float4 Letter(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _ScreenSize.xy;
        float4 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS);
		half cond = saturate(step(input.texcoord.y, _Offset) + step(_OffsetInv, input.texcoord.y));
		outColor.rgb = lerp(outColor.rgb, _Color.rgb, cond * _Color.a);
		return outColor;
    }


	float4 Pillar(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		uint2 positionSS = input.texcoord * _ScreenSize.xy;
		float4 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS);
		half cond = saturate(step(input.texcoord.x, _Offset) + step(_OffsetInv, input.texcoord.x));
		outColor.rgb = lerp(outColor.rgb, _Color.rgb, cond * _Color.a);
		return outColor;
	}

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "LetterBox"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off
			Fog { Mode off }

            HLSLPROGRAM
                #pragma fragment Letter
                #pragma vertex Vert
            ENDHLSL
        }

			Pass
		{
			Name "LetterBox"

			ZWrite Off
			ZTest Always
			Blend Off
			Cull Off
			Fog { Mode off }

			HLSLPROGRAM
				#pragma fragment Pillar
				#pragma vertex Vert
			ENDHLSL
		}
    }
    Fallback Off
}
