Shader "Unlit/EmissiveShader"
{
    Properties
    {
        _Color ("Glow Color", Color) = (1, 1, 1, 1) // Color of the glow
        _GlowIntensity ("Glow Intensity", Float) = 1.0 // How bright the glow is
        _Radius ("Glow Radius", Float) = 1.0 // Radius for the glow
        _MainTex ("Main Texture", 2D) = "white" {} // Texture for the circle (optional)
        _PulseSpeed ("Pulse Speed", Float) = 1.0 // Speed of the glow pulse
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 _Color;
            float _GlowIntensity;
            float _Radius;
            float _PulseSpeed;

            fixed4 frag (v2f i) : SV_Target {
                float2 center = float2(0.5, 0.5); // Center of the UVs
                float dist = distance(i.uv, center);
                
                // Create a pulse effect based on time
                float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5; // Normalised pulse value [0, 1]
                
                // Calculate the glow based on distance
                float glow = smoothstep(_Radius, _Radius * 0.5, dist) * pulse;

                // Combine the color with glow
                fixed4 finalColor = _Color * glow * _GlowIntensity;
                finalColor.a = glow; // Set alpha for transparency

                return finalColor;
            }
            ENDCG
        }
    }
}