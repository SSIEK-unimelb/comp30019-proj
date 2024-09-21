using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeCircle : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private GameObject objectToTrigger;
    private LockedDoor door;
    [SerializeField] private string enemyLayer = "Enemy";
    private LayerMask enemyMask;
    private Transform enemySacrifice;

    [SerializeField] private float sacrificeTime = 5f;
    [SerializeField] private bool isInside = false;
    private bool isSacrificed = false;
    private float timeInside = 0f;

    [SerializeField] public float initialRotationSpeed = 30f; // Starting rotation speed
    [SerializeField] public float rotationAcceleration = 30f; // How much the rotation speed increases per second
    private float currentRotationSpeed;

    [SerializeField] private float sacrificeDuration = 3f;
    private float currentSacrificeTime = 0;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        enemyMask = LayerMask.GetMask(enemyLayer);
        door = objectToTrigger.GetComponent<LockedDoor>();
        currentRotationSpeed = initialRotationSpeed;
    }

    void Update()
    {
        if (!isSacrificed) {
            if (isInside) {
                HoldStatus holdStatus = enemySacrifice.GetComponentInParent<HoldStatus>();
                if (holdStatus == null || holdStatus.IsHeld || !holdStatus.CanBeHeld) {
                    timeInside = 0f;
                    if (holdStatus) Debug.Log(holdStatus.IsHeld);
                }

                // Increment time inside the collider
                timeInside += Time.deltaTime;
                if (timeInside >= sacrificeTime) {
                    Debug.Log("Sacrifice");
                    isSacrificed = true;
                    holdStatus.CanBeHeld = false;
                    door.OnTrigger();
                    audioSource.Play();
                    GetComponent<SoundMaker>().MakeSound();
                }
            }
        } else {
            // Increase rotation speed
            currentRotationSpeed += Time.deltaTime * rotationAcceleration;

            currentSacrificeTime += Time.deltaTime;
            if (currentSacrificeTime >= sacrificeDuration) {
                Destroy(GetRootParent(enemySacrifice).gameObject);
                Destroy(gameObject);
            }
        }

        transform.Rotate(Vector3.up, currentRotationSpeed * Time.deltaTime);
    }

    private Transform GetRootParent(Transform child)
    {
        var currentParent = child;
        // Traverse up the hierarchy until the topmost parent is found
        while (currentParent.parent != null) {
            currentParent = currentParent.parent;
        }
        return currentParent;
    }

    private void OnTriggerEnter(Collider col)
    {
        // Check if enemy has entered the circle.
        Debug.Log(LayerMask.LayerToName(col.gameObject.layer));
        if ((enemyMask & (1 << col.gameObject.layer)) != 0) {
            enemySacrifice = col.transform;
            isInside = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        // Check if the enemy has left the circle.
        if ((enemyMask & (1 << col.gameObject.layer)) != 0) {
            isInside = false;
            timeInside = 0f; // Reset the time when exiting
        }
    }
}
