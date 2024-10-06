using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArrowReplenishToggle : Interactible
{
    [SerializeField] TMP_Text amountText;
    [SerializeField] private int numArrows = 5;
    private string pickUpStr;
    private AmmoManager ammoManager;
    private FirstPersonControl firstPersonControl;
    public void Start()
    {
        firstPersonControl = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonControl>();
        ammoManager = firstPersonControl.GetComponentInChildren<AmmoManager>();
        pickUpStr = "Arrows (" + numArrows + ")";
        amountText.text = pickUpStr;
        amountText.gameObject.SetActive(false);
    }
    public override void OnFocus()
    {
        // Implement on focus tooltip activate
        amountText.text = pickUpStr;
        amountText.gameObject.SetActive(true);
        firstPersonControl.SetInteractText(true);
    }

    public override void OnInteract()
    {
        // Implement interaction protocol
        ammoManager.ChangeAmmo(numArrows);
        amountText.gameObject.SetActive(false);
        Destroy(gameObject);
        // add a pick-up sound here through sound manager
    }

    public override void OnLoseFocus()
    {
        // Implement off focus tooltip
        amountText.gameObject.SetActive(false);
        firstPersonControl.SetInteractText(false); 
    }
}