Shader "Lemegeton/CircleProgressBar"
{
    Properties
    {
        _BackGround ("Background", 2D) = "white" {}
        _BackColor("Background Color",Color) = (0.1,0.1,0.1,0.5)
        _Progress ("Progress", 2D) = "white" {}
        _ProgColor("Progress Color",Color) = (1,1,1,1)
        _FillAmount ("FillAmount", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True"
            "Queue" = "Transparent"
        }
        
        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            
		    Cull Back
            ZWrite Off
            
            Blend SrcAlpha OneMinusSrcAlpha
            
            HLSLPROGRAM
            #pragma vertex vertex
            #pragma fragment fragment
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            TEXTURE2D(_BackGround);      SAMPLER(sampler_BackGround);
            TEXTURE2D(_Progress);        SAMPLER(sampler_Progress);
            
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BackGround_ST)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Progress_ST)
                UNITY_DEFINE_INSTANCED_PROP(float, _FillAmount)
            UNITY_INSTANCING_BUFFER_END(Props)
            
            struct Attributes
            {
                float4 positionOS    : POSITION;
                float2 uv            : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS              : SV_POSITION;
                float2 backgroundUV            :TEXCOORD0;
                float2 progressUV              :TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
            half4 _BackColor;
            half4 _ProgColor;
            CBUFFER_END

            Varyings vertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionCS = TransformObjectToHClip(input.positionOS);
                float4 baseST = UNITY_ACCESS_INSTANCED_PROP(Props, _BackGround_ST);
                output.backgroundUV = input.uv * baseST.xy + baseST.zw;
                baseST = UNITY_ACCESS_INSTANCED_PROP(Props, _Progress_ST);
                output.progressUV = input.uv * baseST.xy + baseST.zw;
                return output;
            }
            
            float4 fragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                half4 backgroundColor = SAMPLE_TEXTURE2D(_BackGround, sampler_BackGround, input.backgroundUV) * _BackColor;
                half4 progressColor = SAMPLE_TEXTURE2D(_Progress, sampler_Progress, input.progressUV) * _ProgColor;
                float fillAmount = UNITY_ACCESS_INSTANCED_PROP(Props, _FillAmount);
                float2 pos = input.progressUV - (0.5, 0.5);
                float ang = degrees(atan2(-pos.x, -pos.y)) + 180;
                int angle = round(fillAmount * 361);
                progressColor.a = progressColor.a * saturate(angle - ang);
                float mask_a = step(0.5, progressColor.a);
                progressColor = mask_a * progressColor + (1 - mask_a) * backgroundColor;
                return progressColor;
            }
            
            ENDHLSL
        }
    }
}
