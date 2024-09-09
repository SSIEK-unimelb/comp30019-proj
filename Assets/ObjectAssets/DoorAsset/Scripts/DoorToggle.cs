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
            Animator.Play(doorOpenStr);
            isOpen = !isOpen;
        }
        else 
        {
            Animator.Play(doorCloseStr);
            isOpen = !isOpen;
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

}
