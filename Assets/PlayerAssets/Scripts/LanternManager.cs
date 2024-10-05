using System.Collections;
using TMPro;
using UnityEngine;

public class LanternManager : MonoBehaviour
{
    [SerializeField] private float initialBurnTime = 60f; // 1 minute of burn time
    [SerializeField] TMP_Text lanternText; // UI text for lantern burn time
    private float currentBurnTime;
    private Light lanternLight;
    private GameObject lanternParticle;
    private bool isLanternActive = false;

    private void Start()
    {
        currentBurnTime = initialBurnTime;
        UpdateLanternUI();
        HideLanternUI();
    }

    private void Update()
    {
        if (isLanternActive && currentBurnTime > 0)
        {
            // Decrease burn time
            ActivateLantern();
            TurnOnLantern();
            currentBurnTime -= Time.deltaTime;
            UpdateLanternUI();

            // If time runs out, turn off the lantern
            if (currentBurnTime <= 0)
            {
                TurnOffLantern();
            }
        }
    }

    public void ActivateLantern()
    {
        isLanternActive = true;
        // Find the lantern game objects when lantern is equipped
        lanternLight = GameObject.Find("Lantern Light").GetComponent<Light>();
        lanternParticle = GameObject.Find("Lantern_Particle");
        if (lanternLight == null || lanternParticle == null)
        {
            return;
        }
        ShowLanternUI();
        if (currentBurnTime > 0)
        {
            lanternLight.enabled = true;
            lanternParticle.SetActive(true);
        }
        else
        {
            lanternLight.enabled = false;
            lanternParticle.SetActive(false);
        }
    }

    public void DeactivateLantern()
    {
        isLanternActive = false;
        HideLanternUI();
    }

    private void TurnOffLantern()
    {
        isLanternActive = false;
        lanternLight.enabled = false;
        lanternParticle.SetActive(false);
        currentBurnTime = 0;
        UpdateLanternUI();
    }

    private void TurnOnLantern()
    {
        lanternLight = GameObject.Find("Lantern Light").GetComponent<Light>();
        lanternParticle = GameObject.Find("Lantern_Particle");
        isLanternActive = true;
        lanternLight.enabled = true;
        lanternParticle.SetActive(true);
        UpdateLanternUI();
    }

    private void UpdateLanternUI()
    {
        lanternText.text = "Burn Time: " + Mathf.Max(0, Mathf.RoundToInt(currentBurnTime)) + "s";
    }

    public void ShowLanternUI()
    {
        lanternText.gameObject.SetActive(true);
    }

    public void HideLanternUI()
    {
        lanternText.gameObject.SetActive(false);
    }

    public void AddBurnTime(float burnTime)
    {
        currentBurnTime += burnTime;
        UpdateLanternUI();
    }
}