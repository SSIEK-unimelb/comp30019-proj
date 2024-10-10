using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SoundMaker))]
public class LockedDoor : MonoBehaviour
{
    private AudioSource audioSource;

    private Animator animator;
    [SerializeField] private AudioClip openAudio;
    [SerializeField] private string doorOpenStr;
    public bool isOpen = false;

    // Start is called before the first frame update

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnTrigger() {
        if (!isOpen) {
            animator.Play(doorOpenStr);
            audioSource.clip = openAudio;
            audioSource.Play();

            GetComponent<SoundMaker>().MakeSound();
            isOpen = true;
        }
    }

}

