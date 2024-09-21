using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnDeath : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip gameOver;
    private bool deathAnimationFinished = false;
    [SerializeField] private Transform pointToRotateAbout;
    private Vector3 axisToRotateAbout = Vector3.right;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float angleToStop = 90f;
    private float currentAngle = 0f;
    [SerializeField] private float deathAnimationTime = 1f;

    private void Start() {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void StartDeathAnimation() {
        // Destroy PlayerController and set the view angle to horizontal
        Destroy(GetComponent<FirstPersonControl>());
        GameObject camera = GameObject.Find("Camera");
        if (camera != null) {
            Vector3 rotation = camera.transform.eulerAngles;  // Get the current rotation in Euler angles
            rotation.x = 0;  // Set the X-axis rotation to 0 degrees
            camera.transform.eulerAngles = rotation;  // Apply the modified rotation
        }

        audioSource.clip = gameOver;
        audioSource.Play();

        StartCoroutine(PlayerDeathAnimation());
    }

    // Fall backwards
    private IEnumerator PlayerDeathAnimation() {
        while (!deathAnimationFinished) {
            float rotationStep = rotationSpeed * Time.deltaTime;
            
            if (currentAngle + rotationStep > angleToStop) rotationStep = angleToStop - currentAngle;

            if (currentAngle < angleToStop) {
                transform.RotateAround(pointToRotateAbout.position, transform.TransformDirection(axisToRotateAbout), -rotationStep);
                currentAngle += rotationStep;
            } else {
                deathAnimationFinished = true;
            }

            yield return new();
        }

        // Wait a few seconds after death animation has finished.
        yield return new WaitForSeconds(deathAnimationTime);

        ReloadScene();
    }

    private void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
