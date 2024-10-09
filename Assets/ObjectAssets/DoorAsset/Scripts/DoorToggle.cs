using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DoorToggle : Interactible
{
    private Animator Animator;
    [SerializeField] private string doorOpenStr;
    [SerializeField] private string doorCloseStr;
    public bool isOpen = true;
    // wasOpen should be the same as isOpen.
    // isOpen is false in the editor, so wasOpen is set to false.
    private bool wasOpen = false;
    private FirstPersonControl firstPersonControl;

    [SerializeField] private string levelObjectName = "Level";
    [SerializeField] private float doorOpenTime = 0.5f;
    private NavMeshSurfaceBuilder navMeshSurfaceBuilder;

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

        // Rebuilds the navmesh once, otherwise the navmesh will try building too often.
        if (wasOpen != isOpen) {
            StartCoroutine(WaitToOpenOrClose());
            wasOpen = isOpen;
        }

        if (gameObject.tag == "LevelExit") {
            Invoke("LoadNextScene", 2f);
        }
    }

    private void LoadNextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public override void OnLoseFocus()
    {
        firstPersonControl.SetInteractText(false);
        Debug.Log("Stop looking at door");
        return;
        // Implement off focus tooltip 
    }

    public override void Awake()
    {
        Animator = GetComponent<Animator>();
        navMeshSurfaceBuilder = GameObject.Find(levelObjectName).GetComponent<NavMeshSurfaceBuilder>();
    }

    public void Start()
    {
        firstPersonControl = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonControl>();
    }

    bool AnimatorIsPlaying()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).length >
               Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    private IEnumerator WaitToOpenOrClose() {
        yield return new WaitForSeconds(doorOpenTime);
        if (navMeshSurfaceBuilder != null) {
            navMeshSurfaceBuilder.SetBuildToTrue();
        }
    }
}
