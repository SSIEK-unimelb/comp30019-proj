using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorToggle : Interactible
{
    private Animator Animator;
    [SerializeField] private string doorOpenStr;
    [SerializeField] private string doorCloseStr;
    public bool isOpen = true;

    private GameObject interactionText;
    public override void OnFocus()
    {
        // Implement on focus tooltip activate
        interactionText.SetActive(true);
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
            }
        }
        else 
        {
            if (!AnimatorIsPlaying())
            {
                Animator.Play(doorCloseStr);
                isOpen = !isOpen;
            }
        }
    }

    public override void OnLoseFocus()
    {
        interactionText.SetActive(false);
        return;
        // Implement off focus tooltip 
    }

    public override void Awake()
    {
        Animator = GetComponent<Animator>();
        
    }

    public void Start()
    {
        print(GameObject.FindGameObjectWithTag("InteractText"));
        interactionText = GameObject.FindGameObjectWithTag("InteractText");
        print(interactionText);
        interactionText.SetActive(false);
    }

    bool AnimatorIsPlaying()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).length >
               Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
