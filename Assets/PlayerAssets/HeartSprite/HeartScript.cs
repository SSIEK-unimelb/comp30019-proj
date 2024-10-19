using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartScript : MonoBehaviour
{
    private SoundManager soundManager;

    [SerializeField] private Image heart1;
    [SerializeField] private Image heart2;
    [SerializeField] private Image heart3;

    private void Start() {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    public void UpdateHearts(int currentHealth) {
    if (currentHealth == 3) {
        heart1.enabled = true;
        heart2.enabled = true;
        heart3.enabled = true;
        soundManager.HighHealth();
    } else if (currentHealth == 2) {
        heart1.enabled = true;
        heart2.enabled = true;
        heart3.enabled = false;
        soundManager.MidHealth();
    } else if (currentHealth == 1) {
        heart1.enabled = true;
        heart2.enabled = false;
        heart3.enabled = false;
        soundManager.LowHealth();
    } else {
        heart1.enabled = false;
        heart2.enabled = false;
        heart3.enabled = false;
        soundManager.DecreaseHeartBeatAndBreathing();
    }
}
}
