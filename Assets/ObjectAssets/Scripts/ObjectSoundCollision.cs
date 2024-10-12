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
    private float ignoreCollisionsDuration = 2f;

    private void Start() {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (!canCollide) {
            ignoreCollisionsDuration -=  Time.deltaTime;
            if (ignoreCollisionsDuration <= 0) {
                canCollide = true;
            }
        }
    }

    public void AssignRigidbodyOfGoblins() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (!canCollide) {
            return;
        }

        if (_rigidbody.velocity.magnitude < velocityThreshold) {
            return;
        }

        soundManager.PlaySoundEffect(collideSound, 1.0f);

        Debug.Log("This object made sound: " + gameObject);
        Debug.Log("This object's velocity is: " + _rigidbody.velocity.magnitude);
    }
}
