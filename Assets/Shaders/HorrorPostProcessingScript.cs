using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class HorrorPostProcessingScript : MonoBehaviour
{
    [SerializeField] private Shader shader;
    private Material material;

    [Header("Wave Distortion")]
    [SerializeField] private float waveX = 0.1f;
    [SerializeField] private float waveY = 1.0f;
    [SerializeField] private float waveStrength = 0.01f;
    [SerializeField] private float waveSpeed = 1.0f;

    [Header("Screen Tearing")]
    [SerializeField] private float tearThreshold = 0.999f;
    [SerializeField] private float tearSpeed = 10.0f;
    [SerializeField] private float shiftAmount = 1.0f;

    [Header("Jitter")]
    [SerializeField] private float jitterSpeed = 2.0f;
    [SerializeField] private float jitterThreshold = 0.01f;
    [SerializeField] private float jitterX = 0.005f;
    [SerializeField] private float jitterY = 0f;

    [Header("RGB Split")]
    [SerializeField] private float chromaticDistortionAmount = 0.005f;

    [Header("Noise Overlay")]
    [SerializeField] private Texture noiseTexture;
    [SerializeField] private float noiseSpeed = 0.05f;
    [SerializeField] private float noiseIntensity = 0.01f;

    [Header("Scanning Bars")]
    [SerializeField] private float barSpeed = 1.0f;
    [SerializeField] private float barAmount = 100.0f;
    [SerializeField] private float barThickness = 10.0f;
    [SerializeField] private float barIntensity = 1.0f;

    [Header("Fog")]
    [SerializeField] private Color fogColor = Color.grey;
    [SerializeField] [Range(0.0f, 1.0f)] private float fogDensity = 0.01f;  // The density of the fog.
    [SerializeField] [Range(0.0f, 1.0f)] private float fogOffset = 1.0f;    // This determines how much the fog should affect a pixel.

    void Start() {
        if (material == null) {
            material = new Material(shader);

            // So that the material is not saved with the scene.
            material.hideFlags = HideFlags.HideAndDontSave;
        }

        Camera camera = GetComponent<Camera>();

        // So that the camera includes depth information for calculating fog effect.
        camera.depthTextureMode = camera.depthTextureMode | DepthTextureMode.Depth;
    }

    // OnRenderImage is called after the camera has finished rendering, but before the image is displayed.
    // This is to apply post-processing effects to the image (in this case, fog).
    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        // Vertex Shader
        // Wave/Distortion Effects
        material.SetFloat("_WaveX", waveX);
        material.SetFloat("_WaveY", waveY);
        material.SetFloat("_WaveStrength", waveStrength);
        material.SetFloat("_WaveSpeed", waveSpeed);

        // Screen Tearing
        material.SetFloat("_TearThreshold", tearThreshold);
        material.SetFloat("_TearSpeed", tearSpeed);
        material.SetFloat("_ShiftAmount", shiftAmount);

        // Jitter
        material.SetFloat("_JitterSpeed", jitterSpeed);
        material.SetFloat("_JitterThreshold", jitterThreshold);
        material.SetFloat("_JitterX", jitterX);
        material.SetFloat("_JitterY", jitterY);

        // Fragment Shader
        // RGB Split
        material.SetFloat("_ChromaticDistortionAmount", chromaticDistortionAmount);

        // Noise Overlay
        material.SetTexture("_NoiseTex", noiseTexture);
        material.SetFloat("_NoiseSpeed", noiseSpeed);
        material.SetFloat("_NoiseIntensity", noiseIntensity);

        // Scanning Bars
        material.SetFloat("_BarSpeed", barSpeed);
        material.SetFloat("_BarAmount", barAmount);
        material.SetFloat("_BarThickness", barThickness);
        material.SetFloat("_BarIntensity", barIntensity);

        // Fog Effect
        material.SetVector("_FogColor", fogColor);
        material.SetFloat("_FogDensity", fogDensity);
        material.SetFloat("_FogOffset", fogOffset);

        // Copies image from source, applies the fog shader, writes the result to the destination.
        Graphics.Blit(source, destination, material);
    }
}
