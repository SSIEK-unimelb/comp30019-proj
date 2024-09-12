using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeCircle : MonoBehaviour
{
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

    private AudioSource audioSource;
    [SerializeField] private AudioClip sacrificeSound;
    [SerializeField] private float sacrificeSoundLength = 3f;
    private float sacrificeSoundTime = 0;

    void Start() {
        enemyMask = LayerMask.GetMask(enemyLayer);
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = sacrificeSound;
        door = objectToTrigger.GetComponent<LockedDoor>();
        currentRotationSpeed = initialRotationSpeed;
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
                    audioSource.Play();
                    isSacrificed = true;
                    holdStatus.CanBeHeld = false;
                    door.OnTrigger();
                }
            }
        } else {
            // Increase rotation speed
            currentRotationSpeed += Time.deltaTime * rotationAcceleration;

            sacrificeSoundTime += Time.deltaTime;
            if (sacrificeSoundTime >= sacrificeSoundLength) {
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
