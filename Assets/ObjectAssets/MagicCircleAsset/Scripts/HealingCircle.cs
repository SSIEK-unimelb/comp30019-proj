using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCircle : MonoBehaviour
{
    private SoundManager soundManager;
    [SerializeField] private AudioClip HealingAudio;
    private ParticleSystem particleEffect;

    private HealthManager healthManager;
    private int healAmount = 1;

    [SerializeField] private float healingTime = 5f;
    [SerializeField] private bool isInside = false;
    private bool isHealed = false;
    private float timeInside = 0f;

    [SerializeField] public float initialRotationSpeed = 30f; // Starting rotation speed
    [SerializeField] public float rotationAcceleration = 30f; // How much the rotation speed increases per second
    private float currentRotationSpeed;

    [SerializeField] private float healingDuration = 3f;
    private float currentHealingTime = 0;

    void Start() {
        particleEffect = GetComponentInChildren<ParticleSystem>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        healthManager = GameObject.FindWithTag("Player").GetComponent<HealthManager>();
        currentRotationSpeed = initialRotationSpeed;
        particleEffect.Stop();
    }

    void Update()
    {
        if (!isHealed) {
            if (isInside) {
                // Increment time inside the collider
                timeInside += Time.deltaTime;
                if (timeInside >= healingTime) {
                    Debug.Log("Player has been healed!");
                    isHealed = true;
                    healthManager.Heal(healAmount);
                    soundManager.PlaySoundEffect(HealingAudio, 1.0f);
                    GetComponent<SoundMaker>().MakeSound();
                }
            }
        } else {
            // Increase rotation speed
            currentRotationSpeed += Time.deltaTime * rotationAcceleration;

            currentHealingTime += Time.deltaTime;
            if (currentHealingTime >= healingDuration) {
                Destroy(gameObject);
            }

            // Start particle Effect if it has not been started
            if (!particleEffect.isPlaying) {
                particleEffect.Play();
                Debug.Log("Particle playing");
            }
        }

        transform.Rotate(Vector3.up, currentRotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        // Check if player has entered the circle.
        Debug.Log("This game object has entered the healing circle: " + col.transform.root.gameObject);
        if (col.transform.root.gameObject.CompareTag("Player")) {
            isInside = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        // Check if the player has left the circle.
        Debug.Log("This game object has left the healing circle: " + col.transform.root.gameObject);
        if (col.transform.root.gameObject.CompareTag("Player")) {
            isInside = false;
            timeInside = 0f; // Reset the time when exiting
        }
    }
}
