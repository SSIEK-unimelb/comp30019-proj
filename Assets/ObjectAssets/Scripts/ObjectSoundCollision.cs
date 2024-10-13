using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSoundCollision : MonoBehaviour
{
    private SoundManager soundManager;
    [SerializeField] private AudioClip collideSound;

    private Rigidbody _rigidbody;
    private float velocityThreshold = 0.5f;

    private bool canCollide = false;
    private float ignoreCollisionsDurationAtStart = 2f;
    private float cooldownDuration = 0.5f;
    private float currentCooldownTime = 0f;

    private void Start() {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (!canCollide) {
            ignoreCollisionsDurationAtStart -=  Time.deltaTime;
            if (ignoreCollisionsDurationAtStart <= 0) {
                canCollide = true;
            }
        }

        currentCooldownTime -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision) {
        if (!canCollide) {
            return;
        }

        if (currentCooldownTime > 0) {
            return;
        }

        // So that the goblin does not collide with itself.
        if (gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                // Debug.Log("Cannot collide with oneself");
                return;
            }

            // If alive, goblin makes no sound.
            HoldStatus holdStatus = gameObject.GetComponentInParent<HoldStatus>();
            if (holdStatus != null) {
                if (!holdStatus.CanBeHeld) {
                    return;
                }
            }
        }

        if (_rigidbody.velocity.magnitude < velocityThreshold) {
            return;
        }

        soundManager.PlaySoundEffect(collideSound, 0.6f);

        // Now that a collision sound has played, cooldown.
        currentCooldownTime = cooldownDuration;

        Debug.Log("This object made sound: " + gameObject + " due to colliding with: " + collision.transform.gameObject);
        // Debug.Log("This object's velocity is: " + _rigidbody.velocity.magnitude);
    }
}
