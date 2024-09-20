using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float arrowSpeed = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        if (arrowPrefab != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, transform.position, transform.rotation * Quaternion.Euler(0, 90, 90));
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            rb.velocity = transform.forward * arrowSpeed;
        }
        else
        {
            Debug.LogError("Arrow prefab not assigned");
        }
    }
}
