using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : MonoBehaviour
{
    private SoundManager soundManager;
    [SerializeField] private AudioClip bowFire;

    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float arrowSpeed = 100f;
    private AmmoManager ammoManager;
    void Start()
    {
        GetAmmoManagerRef();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    public void Fire()
    {
        if (arrowPrefab != null)
        {
            if (ammoManager.GetCurrentAmmo() > 0)
            {
                GameObject arrow = Instantiate(arrowPrefab, transform.position, transform.rotation * Quaternion.Euler(0, 90, 90));
                Rigidbody rb = arrow.GetComponent<Rigidbody>();
                rb.velocity = transform.forward * arrowSpeed;
                // decrease ammo by one
                ammoManager.ChangeAmmo(-1);

                soundManager.PlaySoundEffect(bowFire, 0.2f);
            }
        }
        else
        {
            Debug.LogError("Arrow prefab not assigned");
        }
    }

    public void GetAmmoManagerRef()
    {
        // Access the grandparent Transform
        Transform grandparentTransform = transform.parent?.parent; // Using the null-conditional operator
        
        if (grandparentTransform != null)
        {
            // Access the grandparent GameObject
            GameObject grandparentGameObject = grandparentTransform.gameObject;

            // Get the desired component from the grandparent GameObject
            ammoManager = grandparentGameObject.GetComponent<AmmoManager>();

            if (ammoManager != null)
            {
                // Successfully accessed the component, do something with it
                Debug.Log("Component found: " + ammoManager);
            }
            else
            {
                Debug.Log("Component not found on the grandparent GameObject.");
            }
        }
        else
        {
            Debug.Log("This GameObject has no grandparent.");
        }
    }
}
