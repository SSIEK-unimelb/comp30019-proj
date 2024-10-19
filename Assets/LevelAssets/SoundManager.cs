using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource soundEffects;

    private AudioSource backgroundMusicAudioSource;
    [SerializeField] private AudioClip backgroundMusic;
    private AudioSource heartBeatAudioSource;
    [SerializeField] private AudioClip heartBeat;
    private AudioSource personBreathingAudioSource;
    [SerializeField] private AudioClip breathingNoChase;
    [SerializeField] private AudioClip breathingChase;
    private float breathChangeTime = 2;
    private float currentBreathTime = 0;

    private enum Health {
        LOW_HEALTH,
        MID_HEALTH,
        HIGH_HEALTH
    }
    private Health healthState;
    private bool isDead = false;

    private void Awake() {
        soundEffects = gameObject.AddComponent<AudioSource>();
        backgroundMusicAudioSource = gameObject.AddComponent<AudioSource>();
        heartBeatAudioSource = gameObject.AddComponent<AudioSource>();
        personBreathingAudioSource = gameObject.AddComponent<AudioSource>();
        healthState = Health.HIGH_HEALTH;
    }

    private void Start() {
        backgroundMusicAudioSource.loop = true;
        heartBeatAudioSource.loop = true;
        personBreathingAudioSource.loop = true;

        backgroundMusicAudioSource.clip = backgroundMusic;
        heartBeatAudioSource.clip = heartBeat;
        personBreathingAudioSource.clip = breathingNoChase;

        personBreathingAudioSource.volume = 0.4f;
        personBreathingAudioSource.pitch = 0.8f;

        backgroundMusicAudioSource.Play();
        heartBeatAudioSource.Play();
        personBreathingAudioSource.Play();
    }

    private void Update() {
        // If not dead, vary breathing.
        if (!isDead) {
            currentBreathTime -= Time.deltaTime;
            if (currentBreathTime <= 0) {
                switch(healthState) {
                    case Health.HIGH_HEALTH:
                        personBreathingAudioSource.pitch = UnityEngine.Random.Range(0.75f, 0.85f);
                        break;
                    case Health.MID_HEALTH:
                        personBreathingAudioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                        break;
                    case Health.LOW_HEALTH:
                        personBreathingAudioSource.pitch = UnityEngine.Random.Range(0.85f, 0.95f);
                        break;
                }
                
                currentBreathTime = breathChangeTime;
            }
        }
    }

    public void HighHealth() {
        healthState = Health.HIGH_HEALTH;
        personBreathingAudioSource.clip = breathingNoChase;
        personBreathingAudioSource.Play();
        heartBeatAudioSource.pitch = 0.8f;
    }

    public void MidHealth() {
        healthState = Health.MID_HEALTH;
        personBreathingAudioSource.clip = breathingNoChase;
        personBreathingAudioSource.Play();
        heartBeatAudioSource.pitch = 1.0f;
    }

    public void LowHealth() {
        healthState = Health.LOW_HEALTH;
        personBreathingAudioSource.clip = breathingChase;
        personBreathingAudioSource.Play();
        heartBeatAudioSource.pitch = 0.9f;
    }
    
    public void DecreaseHeartBeatAndBreathing() {
        isDead = true;
        heartBeatAudioSource.volume = 0.1f;
        heartBeatAudioSource.pitch = 0.8f;
        heartBeatAudioSource.Play();
        personBreathingAudioSource.volume = 0.1f;
        personBreathingAudioSource.pitch = 0.6f;
        personBreathingAudioSource.Play();
    }

    public void PlaySoundEffect(AudioClip clip, float volume) {
        soundEffects.PlayOneShot(clip, volume);
    }
}
