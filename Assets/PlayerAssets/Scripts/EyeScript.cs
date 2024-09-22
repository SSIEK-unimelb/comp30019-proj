using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeScript : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Looking")]
    [SerializeField] private float lookSpeedX = 2.0f;
    [SerializeField] private float lookSpeedY = 2.0f;
    [SerializeField] private float aboveLookClamp = 90.0f;
    [SerializeField] private float belowLookClamp = 90.0f;
    private float rotationX = 0;
    [SerializeField] private Camera cam;
    private AudioListener audioListener;

    private bool eyeForm;
    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        audioListener = GetComponentInChildren<AudioListener>();
        eyeForm = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (eyeForm)
        {
            RegisterMouseMovement();
        }
    }

    private void RegisterMouseMovement()
    {
        // look vertically
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -aboveLookClamp, belowLookClamp);
        cam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // horizontal looking moves the character
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
        // print(rotationX);
    }

    public void ActivateEye() {
        if (!eyeForm)
        {
            cam.enabled = !cam.enabled;
            audioListener.enabled = !audioListener.enabled;
            transform.GetComponent<Rigidbody>().isKinematic = true;
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

            eyeForm = true;
        }
        else {
            cam.enabled = !cam.enabled;
            audioListener.enabled = !audioListener.enabled;
            Destruct();
            eyeForm = false;
        }
    }

    public bool isEyeActive() { 
        return eyeForm;
    }

    public void Destruct() {
        Destroy(gameObject);
    }
}
 