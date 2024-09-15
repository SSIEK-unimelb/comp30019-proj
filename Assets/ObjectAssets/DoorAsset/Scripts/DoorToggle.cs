using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorToggle : Interactible
{
    private Animator Animator;
    [SerializeField] private string doorOpenStr;
    [SerializeField] private string doorCloseStr;
    public bool isOpen = true;
    public override void OnFocus()
    {
        return;
        // Implement on focus tooltip activate
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
        return;
        // Implement off focus tooltip 
    }

    public override void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    bool AnimatorIsPlaying()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).length >
               Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
