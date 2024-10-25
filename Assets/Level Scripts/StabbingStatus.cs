using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabbingStatus : MonoBehaviour
{
    public bool isStabbing = false;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();    
    }
    private void Update()
    {
        isStabbing = false;
        AnimatorClipInfo[] animatorinfo;
        animatorinfo = this.animator.GetCurrentAnimatorClipInfo(0);
        if (animatorinfo[0].clip.name.Equals("Stab"))
        {
            isStabbing = true;
        }
    }
}
