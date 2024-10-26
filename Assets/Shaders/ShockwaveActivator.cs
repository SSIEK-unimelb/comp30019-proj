using UnityEngine;

public class ShockwaveActivator : MonoBehaviour
{
    [SerializeField] private Material shockwaveMaterial; // Reference to the material using the shockwave shader
    [SerializeField] private float waveStrength = 0.1f;  
    [SerializeField] private float radius = 1.0f;         
    [SerializeField] private float waveFrequency = 12.0f; 
    [SerializeField] private float timeOffset;            // Time offset for the wave animation

    void Start()
    {
        // Ensure the material is assigned
        if (shockwaveMaterial == null)
        {
            Debug.LogError("Shockwave material is not assigned!");
        }
    }

    void Update()
    {
        // Update the time offset for animation
        timeOffset = Time.time;

        // Logic for activating shockwave goes here
    }

    public void ActivateShockwave()
    {
        if (shockwaveMaterial != null)
        {
            // Convert the GameObject's position to UV coordinates for the shader
            Vector2 center = new Vector2(0.5f, 0.5f); // Center of texture (UV coords)
            shockwaveMaterial.SetVector("_Center", new Vector4(center.x, center.y, 0, 0));
            shockwaveMaterial.SetFloat("_WaveStrength", waveStrength);
            shockwaveMaterial.SetFloat("_Radius", radius);
            shockwaveMaterial.SetFloat("_TimeOffset", timeOffset);
            shockwaveMaterial.SetFloat("_WaveFrequency", waveFrequency);
        }
    }
}