Shader "URP/FullscreenOutline_DepthNormal"
{
    Properties
    {
        _OutlineColor    ("Outline Color", Color)        = (0,0,0,1)
        _Thickness       ("Outline Thickness(px)", Range(1,6)) = 1
        _DepthThreshold  ("Depth Δ",  Range(0,0.02))     = 0.002
        _NormalThreshold ("Normal Δ", Range(0,1))        = 0.15
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" }
        Pass
        {
            Name "FullscreenPass"  Tags{ "LightMode"="UniversalRenderer" }

            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // 핵심 버퍼
            TEXTURE2D_X(_BlitTexture);         SAMPLER(sampler_BlitTexture);
            TEXTURE2D(_CameraDepthTexture);    SAMPLER(sampler_CameraDepthTexture);
            TEXTURE2D(_CameraNormalsTexture);  SAMPLER(sampler_CameraNormalsTexture);

            // 머티리얼 프로퍼티
            float4 _OutlineColor;
            float  _Thickness;
            float  _DepthThreshold;
            float  _NormalThreshold;

            struct Varyings { float4 positionCS : SV_POSITION; };

            Varyings FullscreenVert (uint id : SV_VertexID)
            {
                Varyings o;
                o.positionCS = GetFullScreenTriangleVertexPosition(id);
                return o;
            }

            // depth to linear eye‑space
            float LinearEye (float raw) { return Linear01Depth(raw, _ZBufferParams); }

            float4 frag (Varyings i) : SV_Target
            {
                float2 uv    = i.positionCS.xy / _ScreenParams.xy;
                float2 texel = _Thickness / _ScreenParams.xy;

                // 기준 픽셀 깊이·노멀
                float  depthCenter   = LinearEye(SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r);
                float3 normalCenter  = normalize(SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv).rgb * 2 - 1);

                // 최대 변화량 저장 변수 (ASCII 이름!)
                float  maxDepthDelta   = 0.0;
                float  maxNormalDelta  = 0.0;

                [unroll] for (int y = -1; y <= 1; ++y)
                {
                    [unroll] for (int x = -1; x <= 1; ++x)
                    {
                        if (x == 0 && y == 0) continue;
                        float2 offset = float2(x, y) * texel;

                        float  d = LinearEye(SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + offset).r);
                        float3 n = normalize(SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv + offset).rgb * 2 - 1);

                        maxDepthDelta  = max(maxDepthDelta,  abs(d - depthCenter));
                        maxNormalDelta = max(maxNormalDelta, 1 - dot(n, normalCenter)); // 각도 차
                    }
                }

                bool isEdge =
                    (maxDepthDelta  > _DepthThreshold) ||
                    (maxNormalDelta > _NormalThreshold);

                float4 src = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv);
                return isEdge ? _OutlineColor : src;
            }
            ENDHLSL
        }
    }
}
