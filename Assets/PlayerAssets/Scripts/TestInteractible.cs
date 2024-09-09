using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractible : Interactible
{
    public override void OnFocus()
    {
        print("Looking at " + gameObject.name);
    }

    public override void OnInteract()
    {
        print("Interacted with " + gameObject.name);
        Destroy(gameObject);
    }

    public override void OnLoseFocus()
    {
        print("Stopped looking at " + gameObject.name);
    }
}
