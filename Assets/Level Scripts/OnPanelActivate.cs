using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPanelActivate : MonoBehaviour
{
    [SerializeField] private GameObject highlightPanel;
    // Start is called before the first frame update

    public void Awake()
    {
        highlightPanel.SetActive(false);
    }
    public void SetHighlight(bool setVal)
    {

        // set highlight panel to color
        highlightPanel.SetActive(setVal);
    }
}
