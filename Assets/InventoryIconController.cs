using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryIconController : MonoBehaviour
{
    [SerializeField] private GameObject[] inventorySquares;



    public void SetIcon(int pos, bool setVal)
    {
        if (inventorySquares != null && inventorySquares[pos] != null)
        {
            inventorySquares[pos].SetActive(setVal);
        }
    }


    public void Start()
    {
        foreach (var item in inventorySquares)
        {
            item.SetActive(false);
        }
    }
}
