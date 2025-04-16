Shader "URP/FullscreenOutlineByDepthAndNormalsThickness"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineThickness ("Outline Thickness", Range(1, 10)) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            Name "FullscreenPass"
            Tags { "LightMode" = "UniversalRenderer" }

            HLSLPROGRAM
            #pragma vertex FullscreenVert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            // 카메라 노말 텍스처. URP 설정에서 노말 텍스처 생성 활성화 필요.
            TEXTURE2D(_CameraNormalsTexture);
            SAMPLER(sampler_CameraNormalsTexture);

            // 외곽선 두께 조절 프로퍼티 (_OutlineThickness는 1일 때 기본 3×3 샘플링)
            float _OutlineThickness;

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings FullscreenVert(uint vertexID : SV_VertexID)
            {
                Varyings o;
                o.positionCS = GetFullScreenTriangleVertexPosition(vertexID);
                return o;
            }

            float LinearEyeDepth(float rawDepth)
            {
                return Linear01Depth(rawDepth, _ZBufferParams);
            }

            float4 frag(Varyings i) : SV_Target
            {
                float2 screenUV = i.positionCS.xy / _ScreenParams.xy;
                float2 texelSize = 1.0 / _ScreenParams.xy;

                // 깊이(edgeDepth) 검출
                float depth[9];
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        int index = (y + 1) * 3 + (x + 1);
                        // _OutlineThickness 값을 곱해 샘플링 오프셋을 조절
                        float2 offset = float2(x, y) * texelSize * _OutlineThickness;
                        float d = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV + offset).r;
                        depth[index] = LinearEyeDepth(d);
                    }
                }
                float sobelX = depth[0] + 2.0 * depth[3] + depth[6] - depth[2] - 2.0 * depth[5] - depth[8];
                float sobelY = depth[0] + 2.0 * depth[1] + depth[2] - depth[6] - 2.0 * depth[7] - depth[8];
                float edgeDepth = sqrt(sobelX * sobelX + sobelY * sobelY);

                // 노말(edgeNormal) 검출
                float3 normals[9];
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        int index = (y + 1) * 3 + (x + 1);
                        float2 offset = float2(x, y) * texelSize * _OutlineThickness;
                        // 카메라 노말 텍스처는 [0,1] 범위이므로 [-1,1]로 복원
                        float3 normSample = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, screenUV + offset).rgb;
                        normSample = normSample * 2.0 - 1.0;
                        normals[index] = normalize(normSample);
                    }
                }
                float3 sobelN_x = normals[0] + 2.0 * normals[3] + normals[6] - normals[2] - 2.0 * normals[5] - normals[8];
                float3 sobelN_y = normals[0] + 2.0 * normals[1] + normals[2] - normals[6] - 2.0 * normals[7] - normals[8];
                float edgeNormal = sqrt(dot(sobelN_x, sobelN_x) + dot(sobelN_y, sobelN_y));

                // 깊이 또는 노말의 변화가 임계값을 초과하면 테두리 출력
                if (edgeDepth > 0.005 || edgeNormal > 1)
                    return float4(0, 0, 0, 1); // 테두리: 검정색
                else
                    return float4(1, 1, 1, 1); // 배경: 흰색
            }
            ENDHLSL
        }
    }
}