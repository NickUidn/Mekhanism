Shader "Custom/StencilReader_URP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Stencil
        {
            Ref 1
            Comp Equal
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes 
            { 
                float4 positionOS : POSITION; 
                float2 uv : TEXCOORD0; 
                half4 color : COLOR;          // <-- receives Sprite Renderer vertex color
            };

            struct Varyings 
            { 
                float2 uv : TEXCOORD0; 
                float4 positionHCS : SV_POSITION; 
                half4 color : COLOR;          // <-- passes color to fragment
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _Color;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * unity_SpriteColor;   // apply Sprite Renderer Color in URP 2D
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, IN.uv);
                return texColor * _Color * IN.color;        // texture × material tint × sprite color
            }
            ENDHLSL
        }
    }
}