using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactible : MonoBehaviour
{
    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();

    public virtual void Awake() {
        gameObject.layer = 6; // this is the number representing the "Interactible" layer
    }
}
