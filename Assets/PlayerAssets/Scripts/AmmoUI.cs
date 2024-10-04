using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] TMP_Text ammoText;
    private AmmoManager ammoManager;
    // Start is called before the first frame update
    void Start()
    {
        // Get the parent of this GameObject (PlayerUI)
        Transform parentTransform = transform.parent;

        if (parentTransform != null)
        {
            Transform grandparentTransform = parentTransform.parent;

            if (grandparentTransform != null)
            {
                // Search for AmmoManager in the children of the parent GameObject
                ammoManager = grandparentTransform.GetComponentInChildren<AmmoManager>();

                if (ammoManager != null)
                {
                    // Subscribe to ammo changes
                    ammoManager.onAmmoChange.AddListener(UpdateAmmoText);
                    UpdateAmmoText(ammoManager.GetCurrentAmmo()); // Initialize the ammo text
                }
                else
                {
                    Debug.LogError("AmmoManager not found in the children of the grandparent GameObject.");
                }
            }
            else
            {
                Debug.LogError("This GameObject has no grandparent.");
            }
        }
        else
        {
            Debug.LogError("This GameObject has no parent.");
        }
    }

    private void UpdateAmmoText(int ammo)
    {
        ammoText.text = "Arrows: " + ammo;
    }

    public void ShowAmmoUI()
    {
        gameObject.SetActive(true);
    }
    public void HideAmmoUI()
    {
        gameObject.SetActive(false);
    }
}
