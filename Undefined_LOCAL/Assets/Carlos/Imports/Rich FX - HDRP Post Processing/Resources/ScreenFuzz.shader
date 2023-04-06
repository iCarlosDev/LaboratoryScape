Shader "Hidden/InanEvin/RichFX/ScreenFuzz"
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
    TEXTURE2D_X(_InputTexture);

#define C float4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439)
#define MULTP1 1.79284291400159
#define MULTP2 0.85373472095314

	//  Noise function
	//  Copyright(C) 2011 by Ashima Arts(Simplex noise)
	//	Copyright(C) 2011 - 2016 by Stefan Gustavson(Classic noise and others)

	float3 mod289(float3 x) {
		return x - floor(x * (1.0 / 289.0)) * 289.0;
	}

	float2 mod289(float2 x) {
		return x - floor(x * (1.0 / 289.0)) * 289.0;
	}

	float3 permute(float3 x) {
		return mod289(((x * 34.0) + 1.0) * x);
	}

	float snoise(float2 v)
	{
		float2 i = floor(v + dot(v, C.yy));
		float2 x0 = v - i + dot(i, C.xx);
		float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
		float4 x12 = x0.xyxy + C.xxzz;
		x12.xy -= i1;
		i = mod289(i); 

		float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0)) + i.x + float3(0.0, i1.x, 1.0));
		float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
		m = m * m * m;

		float3 x = 2.0 * frac(p * C.www) - 1.0;
		float3 h = abs(x) - 0.5;
		float3 ox = floor(x + 0.5);
		float3 a0 = x - ox;

		m *= MULTP1 - MULTP2 * (a0 * a0 + h * h);
		float3 g;
		g.x = a0.x * x0.x + h.x * x0.y;
		g.yz = a0.yz * x12.xz + h.yz * x12.yw;
		return 130.0 * dot(m, g);
	}

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float xOffset = ((snoise(float2(_Time.y * 15.0, input.texcoord.y * 80.0)) * 0.003) + (snoise(float2(_Time.y * 1.0, input.texcoord.y * 25.0)) * 0.004)) * _Intensity;
		xOffset *= clamp(input.texcoord.x * 10, 0, 1) * clamp((1 - input.texcoord.x) * 10, 0, 1);
		float2 newUVR = float2(input.texcoord.x + xOffset  , input.texcoord.y);
		float2 newUVG = float2(input.texcoord.x + xOffset, input.texcoord.y);
		float2 newUVB = float2(input.texcoord.x + xOffset , input.texcoord.y);
		float r= LOAD_TEXTURE2D_X(_InputTexture, _ScreenSize.xy * newUVR).r ;
		float g = LOAD_TEXTURE2D_X(_InputTexture, _ScreenSize.xy * newUVG).g ;
		float b = LOAD_TEXTURE2D_X(_InputTexture, _ScreenSize.xy * newUVB).b ;
		float3 outColor = float3(r,g,b);
        return float4(outColor, 1);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "ScreenFuzz"

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
