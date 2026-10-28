Shader "Custom/LineStencilWriter_URP"
{
    SubShader
    {
        Tags { "Queue"="Transparent-1" "RenderType"="Transparent" "IgnoreProjector"="True" }
        
        ColorMask 0      // Invisible
        ZWrite Off
        Cull Off

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace // Write 1 to buffer
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionHCS : SV_POSITION; };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return half4(0,0,0,0); // ColorMask 0 will discard output
            }
            ENDHLSL
        }
    }
}
