using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ItemSwitcher : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public GameObject currentItem { get; private set; }
    private int currentItemIndex = -1;

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < itemPrefabs.Length; i++) {
            itemPrefabs[i].SetActive(true);
        }
        SwitchItem(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SwitchItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SwitchItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SwitchItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { SwitchItem(3); }
    }

    void SwitchItem(int itemIndex) {
        // could also add a condition here to only allow items that the player has in inventory
        if (itemIndex < 0 || itemIndex >= itemPrefabs.Length || itemIndex == currentItemIndex) {
            return;
        }

        if (currentItem != null) {
            Destroy(currentItem);
        }

        currentItem = Instantiate(itemPrefabs[itemIndex], transform.position, transform.rotation, transform);
        
        /*
        itemPrefabs[currentItemIndex].SetActive(false); // unequip current item

        itemPrefabs[itemIndex].SetActive(true); // equip selected item
        */
        
        currentItemIndex = itemIndex; // update current item
    }
}
