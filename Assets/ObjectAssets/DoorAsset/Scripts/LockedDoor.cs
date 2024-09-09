using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    private Animator Animator;
    [SerializeField] private string doorOpenStr;
    public bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    public void OnTrigger() {
        if (!isOpen) {
            Animator.Play(doorOpenStr);
            isOpen = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

