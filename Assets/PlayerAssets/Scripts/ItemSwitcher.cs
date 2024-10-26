using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSwitcher : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public GameObject currentItem { get; private set; }
    private int currentItemIndex = -1;
    private int inventorySize = 4;
    private int lastUnlocked;
    private AmmoUI ammoUI;
    private LanternManager lanternManager;
    public bool[] isUnlocked;
    public bool canSwitch = true;

    [SerializeField] private InventoryIconController inventoryIconController;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < itemPrefabs.Length; i++) {
            itemPrefabs[i].SetActive(true);
        }
        isUnlocked = new bool[inventorySize];

        GetAmmoUIRef();
        lanternManager = GetComponent<LanternManager>();
        // testing unlocking
        unlock(0);
        unlock(1);
        unlock(2);
        unlock(3);
        SwitchItem(1);
        SwitchItem(2);
        SwitchItem(3);
        SwitchItem(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!canSwitch) { return; }
        // these can be turned off until needed
        //unlock(2);
        //unlock(3);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {

            SwitchItem(3);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
            SwitchItem(currentItemIndex - 1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            SwitchItem(currentItemIndex + 1);
        }
        UpdateUI();
    }

    public void SwitchItem(int itemIndex) {
        // could also add a condition here to only allow items that the player has in inventory
        //print(lastUnlocked);
        if ((itemIndex >= 0) && (itemIndex <= lastUnlocked) && !isUnlocked[itemIndex]) { return; }
        if (!canSwitch) { return; }
        if (itemIndex == currentItemIndex) {
            return;
        }
        if (itemIndex == -1) {
            // go to last item in inventory
            SwitchItem(lastUnlocked);
            return;
        } else if (itemIndex == lastUnlocked + 1){
            SwitchItem(0);
            return;
        }

        if (currentItem != null) {
            Destroy(currentItem);
        }

        inventoryIconController.Highlight(currentItemIndex, false);
        currentItem = Instantiate(itemPrefabs[itemIndex], transform.position, transform.rotation, transform);
        
        currentItemIndex = itemIndex; // update current item
        inventoryIconController.Highlight(currentItemIndex, true);

    }

    public void SwitchToHoldArms()
    {
        Vector3 offset = new Vector3(-0.018f, -0.411f, 0.253f); 
        Vector3 localOffset = transform.TransformPoint(offset);
        Destroy(currentItem);
        currentItemIndex = 4;
        currentItem = Instantiate(itemPrefabs[currentItemIndex], localOffset, transform.rotation, transform);
    }


    public void unlock(int itemIndex) {
        isUnlocked[itemIndex] = true;
        lastUnlocked = itemIndex;

        inventoryIconController.SetIcon(itemIndex, true);
    }

    public void UpdateUI()
    {
        // Update crossbow ammo UI
        if (currentItemIndex == 2)
        {
            ammoUI.ShowAmmoUI();
        }
        else
        {
            ammoUI.HideAmmoUI();
        }
        // Update lantern burn time UI
        if (currentItemIndex == 1)
        {
            lanternManager.ActivateLantern();
        }
        else
        {
            lanternManager.DeactivateLantern();
        }
    }

    void GetAmmoUIRef()
    {
        // Get the parent of this Object (FPCE)
        Transform parentTransform = transform.parent;

        if (parentTransform != null)
        {
            // Search for AmmoManager in the children of the parent GameObject
            ammoUI = parentTransform.GetComponentInChildren<AmmoUI>();

            if (ammoUI == null)
            {
                Debug.LogError("AmmoUI not found in the children of the parent GameObject.");
            }
        }
        else
        {
            Debug.LogError("This GameObject has no parent.");
        }
    }
}
