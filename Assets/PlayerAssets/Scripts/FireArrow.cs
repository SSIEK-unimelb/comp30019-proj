using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float arrowSpeed = 100f;
    [SerializeField] int startingAmmo = 3;
    private GameObject ammoText;
    private int currentAmmo;
    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = startingAmmo;
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAmmoUI();
    }

    public void Fire()
    {
        if (arrowPrefab != null)
        {
            if (currentAmmo > 0)
            {
                GameObject arrow = Instantiate(arrowPrefab, transform.position, transform.rotation * Quaternion.Euler(0, 90, 90));
                Rigidbody rb = arrow.GetComponent<Rigidbody>();
                rb.velocity = transform.forward * arrowSpeed;
                currentAmmo--;
            }
        }
        else
        {
            Debug.LogError("Arrow prefab not assigned");
        }
    }

    public void AddAmmo(int amount) 
    {
        currentAmmo += amount;
    }

    public void UpdateAmmoUI()
    {

    }
}
