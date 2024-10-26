using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeCircle : MonoBehaviour
{
    private SoundManager soundManager;
    [SerializeField] private AudioClip sacrificeAudio;
    [SerializeField] private GameObject objectToTrigger;
    private LockedDoor door;
    [SerializeField] private string enemyLayer = "Enemy";
    private Transform enemySacrifice;
    private ParticleSystem particleEffect;

    [SerializeField] private float sacrificeTime = 5f;
    [SerializeField] private bool isInside = false;
    private bool isSacrificed = false;
    private float timeInside = 0f;

    [SerializeField] public float initialRotationSpeed = 30f; // Starting rotation speed
    [SerializeField] public float rotationAcceleration = 30f; // How much the rotation speed increases per second
    private float currentRotationSpeed;

    [SerializeField] private float sacrificeDuration = 3f;
    private float currentSacrificeTime = 0;
    private ShockwaveActivator shockwaveActivator;

    void Start() {
        particleEffect = GetComponentInChildren<ParticleSystem>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        door = objectToTrigger.GetComponent<LockedDoor>();
        shockwaveActivator = GetComponent<ShockwaveActivator>();
        Debug.Log("The door is currently: " + door);
        currentRotationSpeed = initialRotationSpeed;
        particleEffect.Stop();
    }

    void Update()
    {
        if (!isSacrificed) {
            if (isInside) {
                HoldStatus holdStatus = enemySacrifice.GetComponentInParent<HoldStatus>();
                if (holdStatus == null || holdStatus.IsHeld) {
                    timeInside = 0f;
                    if (holdStatus) Debug.Log(holdStatus.IsHeld);
                }

                // Increment time inside the collider
                timeInside += Time.deltaTime;
                if (timeInside >= sacrificeTime) {
                    Debug.Log("Sacrifice");
                    isSacrificed = true;
                    shockwaveActivator.ActivateShockwave();
                    holdStatus.CanBeHeld = false;
                    Debug.Log("The door is currently: " + door);
                    door.OnTrigger();
                    soundManager.PlaySoundEffect(sacrificeAudio, 1.0f);
                    GetComponent<SoundMaker>().MakeSound();
                }
            }
        } else {
            // Increase rotation speed
            currentRotationSpeed += Time.deltaTime * rotationAcceleration;

            currentSacrificeTime += Time.deltaTime;
            if (currentSacrificeTime >= sacrificeDuration) {
                ParentReference parentReference = enemySacrifice.GetComponentInParent<ParentReference>();
                Debug.Log("The object to be destroyed is: " + parentReference.getTransform().parent.gameObject);
                Destroy(parentReference.getTransform().parent.gameObject);
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

    
    private void OnTriggerStay(Collider col)
    {
        // Check if enemy has entered the circle.
        // Debug.Log(LayerMask.LayerToName(col.gameObject.layer));
        if (col.gameObject.layer == LayerMask.NameToLayer(enemyLayer)) {
            // If goblin can be held (is dead) and is not currently being held.
            HoldStatus holdStatus = col.transform.GetComponentInParent<HoldStatus>();
            if (holdStatus) Debug.Log("Can goblin " + col.transform.parent.name + " be held? " + holdStatus.CanBeHeld);
            if (holdStatus && holdStatus.CanBeHeld && !holdStatus.IsHeld) {
                enemySacrifice = col.transform;
                isInside = true;
                holdStatus.CanBeHeld = false;
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        // Check if an enemy has left the circle.
        if (col.gameObject.layer == LayerMask.NameToLayer(enemyLayer)) {
            // If the enemy was not sacrificed
            if (enemySacrifice == null) return;
            isInside = false;
            timeInside = 0f; // Reset the time when exiting
            HoldStatus holdStatus = col.transform.GetComponentInParent<HoldStatus>();
            holdStatus.CanBeHeld = true;
        }
    }
}
