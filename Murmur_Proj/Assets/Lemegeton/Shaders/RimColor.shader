Shader "Lemegeton/Fresnel"
{
    Properties
    {
        _FresnelColor("Fresnel Color", Color) = (1, 1, 1, 1)
        _Fresnel("Fade(X) Intensity(Y)", vector) = (3, 1, 0, 0)
    }
    
    SubShader
    {
        Tags { 
            "RenderPipeline"="UniversalPipeline" 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
        }
        
        Blend One One
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            struct Attributes
            {
                float4 vertexOS : POSITION;
                half3 normalOS : NORMAL;
            };
            struct Varyings
            {
                float4 vertexCS : SV_POSITION;
                half3 normalWS : TEXCOORD;
                float3 vertexWS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
            half4 _Fresnel;
            half4 _FresnelColor;
            CBUFFER_END

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.vertexCS = TransformObjectToHClip(v.vertexOS);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.vertexWS = TransformObjectToWorld(v.vertexOS);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half3 N = normalize(i.normalWS);
                half3 V = normalize(_WorldSpaceCameraPos - i.vertexWS);
                half dotNV = 1 - saturate(dot(N, V));
                half4 fresnel = pow(dotNV, _Fresnel.x) * _Fresnel.y * _FresnelColor;
                return fresnel;
            }
            ENDHLSL
        }
    }
}
