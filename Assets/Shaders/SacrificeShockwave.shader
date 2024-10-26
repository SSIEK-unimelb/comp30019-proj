Shader "Unlit/SacrificeShockwave"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}  // Main texture 
        _WaveStrength ("Wave Strength", Float) = 0.1  // Strengh of shockwave distortions
        _WaveFrequency ("Wave Frequency", Float) = 12.0  // Frequency of shockwave
        _TimeOffset ("TimeOffset", Float) = 0.0  // Control time start for animation
        _Center ("Shockwave Center", Vector) = (0.5, 0.5, 0, 0)  // Center of shockwave
        _Radius ("Shockwave Radius", Float) = 1.0  // Radius the shockwave should last for
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Texture and input properties
            sampler2D _MainTex;
            float _WaveStrength;
            float _WaveFrequency;
            float _TimeOffset;
            float2 _Center;
            float _Radius;

            // Input and output structs for vertex and fragment shaders
            struct vertIn {
                float4 vertex: POSITION;
                float2 uv: TEXCOORD0;
            };

            struct vertOut {
                float4 pos: SV_POSITION;
                float2 uv: TEXCOORD0;
            };

            // Vertex shader
            vertOut vert(vertIn v) {
                vertOut o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Fragment shader - applies the shockwave effect
            fixed4 frag(vertOut i) : SV_Target {
                // Distance from current UV pos to center
                float2 uvDist = i.uv - _Center;
                float dist = length(uvDist);
                
                // Calculate shockwave effect from time and other properties
                float wave = sin(dist * _WaveFrequency - _TimeOffset) * _WaveStrength;

                // Fade out the effect as it gets further away
                float attenuation = saturate(1.0 - (dist / _Radius));

                // Apply shockwave as UV offset
                float2 distortedUV = i.uv + normalize(uvDist) * wave * attenuation;

                return tex2D(_MainTex, distortedUV);
            }

            ENDCG
        }
    }
}