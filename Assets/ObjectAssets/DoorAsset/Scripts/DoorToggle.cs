using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DoorToggle : Interactible
{
    private SoundManager soundManager;
    [SerializeField] private AudioClip doorSound;

    private Animator Animator;
    [SerializeField] private string doorOpenStr;
    [SerializeField] private string doorCloseStr;
    public bool isOpen = true;
    private FirstPersonControl firstPersonControl;

    private string tutorialSceneName = "Level Tutorial";
    private string mainMenuSceneName = "StartScene";

    private GameObject interactionText;
    public override void OnFocus()
    {
        // Implement on focus tooltip activate
        firstPersonControl.SetInteractText(true);
        Debug.Log("Looking at door");
        return;
    }

    public override void OnInteract()
    {
        print("Door open!");
        if (!isOpen)
        {
            if (!AnimatorIsPlaying())
            {
                Animator.Play(doorOpenStr);
                isOpen = !isOpen;
                soundManager.PlaySoundEffect(doorSound, 1.0f);
            }
        }
        else 
        {
            if (!AnimatorIsPlaying())
            {
                Animator.Play(doorCloseStr);
                isOpen = !isOpen;
                soundManager.PlaySoundEffect(doorSound, 1.0f);
            }
        }

        if (gameObject.tag == "LevelExit") {
            Invoke("LoadNextScene", 2f);
        }
    }

    private void LoadNextScene() {
        print(SceneManager.GetActiveScene().buildIndex);

        Debug.Log("Current scene name is: " + SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == tutorialSceneName) {
            SceneManager.LoadScene(mainMenuSceneName);
        } else {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public override void OnLoseFocus()
    {
        firstPersonControl.SetInteractText(false);
        Debug.Log("Stop looking at door");
        return;
    }

    public override void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void Start()
    {
        firstPersonControl = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonControl>();
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    bool AnimatorIsPlaying()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).length >
               Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
