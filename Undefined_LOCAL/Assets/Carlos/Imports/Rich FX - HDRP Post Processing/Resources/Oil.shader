Shader "Hidden/InanEvin/RichFx/Oil"
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

    // List of properties to control your post process effect
	float _Radius;
    TEXTURE2D_X(_InputTexture);

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		uint2 positionSS = input.texcoord * _ScreenSize.xy;

		const int radius = _Radius;

		float3 m[4] = { float3(0.0, 0.0, 0.0) ,float3(0.0, 0.0, 0.0) ,float3(0.0, 0.0, 0.0) , float3(0.0, 0.0, 0.0) };
		float3 s[4] = { float3(0.0, 0.0, 0.0) ,float3(0.0, 0.0, 0.0) ,float3(0.0, 0.0, 0.0) , float3(0.0, 0.0, 0.0) };


		for (int j = -radius; j <= 0; ++j) 
		{
			for (int i = -radius; i <= 0; ++i)
			{
				float3 c = LOAD_TEXTURE2D_X(_InputTexture, positionSS + float2(i, j)).rgb;
				m[0] += c;
				s[0] += c * c;
			}
		}

		for (int j = -radius; j <= 0; ++j) 
		{
			for (int i = 0; i <= radius; ++i) 
			{
				float3 c = LOAD_TEXTURE2D_X(_InputTexture, positionSS + float2(i, j)).rgb;
				m[1] += c;
				s[1] += c * c;
			}
		}

		for (int j = 0; j <= radius; ++j) 
		{
			for (int i = 0; i <= radius; ++i) 
			{
				float3 c = LOAD_TEXTURE2D_X(_InputTexture, positionSS + float2(i, j) ).rgb;
				m[2] += c;
				s[2] += c * c;
			}
		}

		for (int j = 0; j <= radius; ++j) 
		{
			for (int i = -radius; i <= 0; ++i) 
			{
				float3 c = LOAD_TEXTURE2D_X(_InputTexture, positionSS + float2(i, j)).rgb;
				m[3] += c;
				s[3] += c * c;
			}
		}

		float n = float((radius + 1) * (radius + 1));
		float minSigma = 1e+2;
		float4 outColor;

		for (int k = 0; k < 4; ++k) 
		{
			m[k] /= n;
			s[k] = abs(s[k] / n - m[k] * m[k]);

			float sigma2 = s[k].r + s[k].g + s[k].b;
			if (sigma2 < minSigma) 
			{
				minSigma = sigma2;
				outColor = float4(m[k], 1.0);
			}
		}

		return outColor;
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Oil"

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
