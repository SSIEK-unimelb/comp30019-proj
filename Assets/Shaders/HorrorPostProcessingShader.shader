Shader "Hidden/HorrorPostProcessingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        // Pass 1 : Fog and Distortion Effects
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Distortion Effect
            float _WaveX, _WaveY;
            float _WaveStrength;
            float _WaveSpeed;

            // Screen Tearing
            float _TearThreshold;
            float _TearSpeed;
            float _ShiftAmount;

            // Jitter.
            float _JitterSpeed;
            float _JitterThreshold;
            float _JitterX;
            float _JitterY;

            // Vertex shader
            v2f vert (appdata v)
            {
                v2f o;

                // Distortion Effect.
                float4 displacement = float4(sin(v.vertex.x + _Time.y * _WaveSpeed) * _WaveStrength * _WaveX, 
                                            sin(v.vertex.x + _Time.y * _WaveSpeed) * _WaveStrength * _WaveY, 
                                            0.0f, 0.0f);
                v.vertex += displacement;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Create a jitter.
                float jitterTime = frac(_Time.y * _JitterSpeed);
                if (jitterTime < _JitterThreshold) {
                    v.uv += float2(_JitterX, _JitterY);
                }

                // Simulate tearing in the image by shifting parts horizontally.
                float shiftAmount = step(_TearThreshold, frac(sin(_Time.y * _TearSpeed) + v.uv.y)) * _ShiftAmount;
                v.uv.x += shiftAmount;

                o.uv = v.uv;
                return o;
            }

            // RGB Split
            float _ChromaticDistortionAmount;

            // Noise Overlay
            sampler2D _NoiseTex;
            float _NoiseSpeed;
            float _NoiseIntensity;

            // Scanning Bars
            float _BarSpeed;
            float _BarAmount;
            float _BarThickness;
            float _BarIntensity;

            // Fog
            sampler2D _MainTex, _CameraDepthTexture;
            float4 _FogColor;
            float _FogDensity;
            float _FogOffset;

            // Fragment shader
            fixed4 frag (v2f i) : SV_Target
            {
                // Original colour of the pixel before applying any effect.
                fixed4 col = tex2D(_MainTex, i.uv);

                // RGB Split - Shift the color channels ---------------------------------------------------------
                float2 uvDistort = (i.uv - 0.5) * _ChromaticDistortionAmount;
                col.r = tex2D(_MainTex, i.uv + uvDistort).r;
                col.g = tex2D(_MainTex, i.uv).g;
                col.b = tex2D(_MainTex, i.uv - uvDistort).b;

                // Noise Overlay --------------------------------------------------------------------------------
                fixed4 noiseTex = tex2D(_NoiseTex, i.uv + _Time.y * _NoiseSpeed);
                col = lerp(col, noiseTex, _NoiseIntensity);

                // Horizontal Scanning Bars (Can also be used as static) ------------------------------------------
                // Create the moving bar pattern - this creates the oscillating movement of the bars.
                float barPosition = sin(_Time * _BarSpeed + i.uv.y * _BarAmount);

                // Controls the thickness of the bars. Smoothstep gives soft edges to the bars.
                float barMask = smoothstep(0.5 - _BarThickness, 0.5 + _BarThickness, barPosition);

                // Creates a glitch effect on the bars.
                // https://www.reddit.com/r/GraphicsProgramming/comments/3kp644/this_generates_pseudorandom_numbers_fractsindota/
                float randomValue = frac(sin(dot(i.uv + _Time * _BarSpeed, float2(12.9898, 78.233))) * 43758.5453);
                float glitchEffect = randomValue * _BarIntensity;

                // Apply the glitchy bars to the color/pixels.
                col.rgb *= lerp(1.0, glitchEffect, barMask);

                // Fog Effect --------------------------------------------------------------------------------------
                // Get the depth value from the depth texture at the uv coordinates.
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                // Converts the depth value to linear for calculations.
                depth = Linear01Depth(depth);

                // The distance from the camera to a particular point (pixel) in the scene.
                float viewDistance = depth * _ProjectionParams.z;

                // Calculate the fog factor using exponential square formula.
                float fogFactor = (_FogDensity / sqrt(log(2))) * max(0.0f, viewDistance - _FogOffset);
                fogFactor = exp2(-fogFactor * fogFactor);

                // saturate clamps the fogFactor between 0 and 1.
                // If closer to 0, the object is fully hidden by fog.
                // If closer to 1, the object can be seen with minimal fog.
                col = lerp(_FogColor, col, saturate(fogFactor));

                return col;
            }
            ENDCG
        }
    }
}
