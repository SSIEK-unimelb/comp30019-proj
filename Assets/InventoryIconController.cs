using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    public void Awake()
    {
        foreach (var item in inventorySquares)
        {
            //item.SetActive(false);
        }
    }

    public void Highlight(int pos, bool setVal)
    {
        foreach (var item in inventorySquares)
        {
            item.GetComponent<OnPanelActivate>().SetHighlight(false);
        }
        print(pos);
        if(pos < 0 || pos >= inventorySquares.Length) { return; }
        var panel = inventorySquares[pos].GetComponent<OnPanelActivate>();
        if (panel != null) { 
            panel.SetHighlight(setVal);
        }
    }
}
