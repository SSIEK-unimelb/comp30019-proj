using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SoundMaker))]
public class LockedDoor : MonoBehaviour
{
    private SoundManager soundManager;
    [SerializeField] private AudioClip openAudio;

    private Animator animator;
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
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    public void OnTrigger() {
        if (!isOpen) {
            animator.Play(doorOpenStr);
            soundManager.PlaySoundEffect(openAudio, 1.0f);

            GetComponent<SoundMaker>().MakeSound();
            isOpen = true;
        }
    }

}

