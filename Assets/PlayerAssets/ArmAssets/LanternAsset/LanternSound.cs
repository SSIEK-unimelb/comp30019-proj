using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternSound : MonoBehaviour
{
    private LanternManager lanternManager;
    private AudioSource audioSource;

    void Start() {
        lanternManager = GetComponentInParent<LanternManager>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if (PauseMenu.gameIsPaused) {
            if (audioSource.isPlaying) {
                audioSource.Pause();
            }
        } else {
            if (lanternManager.IsLanternActive()) {
                if (!audioSource.isPlaying) {
                    audioSource.Play();
                }
            } else {
                if (audioSource.isPlaying) {
                    audioSource.Stop();
                }
            }
        }
    }
}
