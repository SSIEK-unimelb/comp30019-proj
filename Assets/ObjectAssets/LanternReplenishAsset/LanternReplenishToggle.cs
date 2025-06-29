using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanternReplenishToggle : Interactible
{
    private SoundManager soundManager;
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] TMP_Text amountText;
    [SerializeField] private float refillBurnTime = 20f;
    private string pickUpStr;
    private LanternManager lanternManager;
    private FirstPersonControl firstPersonControl;
    public void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        firstPersonControl = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonControl>();
        lanternManager = firstPersonControl.GetComponentInChildren<LanternManager>();
        pickUpStr = "Lantern Oil (" + Mathf.Max(0, Mathf.RoundToInt(refillBurnTime)) + "s)";
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
        amountText.gameObject.SetActive(false);
        lanternManager.AddBurnTime(refillBurnTime);
        Debug.Log("The lantern replenish candle was destroyed!");
        Destroy(gameObject);
        // add a pick-up sound here through sound manager
        soundManager.PlaySoundEffect(pickUpSound, 1f);
    }

    public override void OnLoseFocus()
    {
        // Implement off focus tooltip
        amountText.gameObject.SetActive(false);
        firstPersonControl.SetInteractText(false); 
    }
}