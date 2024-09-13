using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    private NavMeshSurfaceBuilder navMeshSurfaceBuilder;

    private Animator animator;
    [SerializeField] private string doorOpenStr;
    [SerializeField] private float doorOpenTime = 0.5f;
    public bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        navMeshSurfaceBuilder = GameObject.Find("Floor").GetComponent<NavMeshSurfaceBuilder>();
        animator = GetComponent<Animator>();
    }

    public void OnTrigger() {
        if (!isOpen) {
            animator.Play(doorOpenStr);
            GetComponent<SoundMaker>().MakeSound();
            isOpen = true;
            StartCoroutine(WaitToOpen());
        }
    }

    private IEnumerator WaitToOpen() {
        yield return new WaitForSeconds(doorOpenTime);
        navMeshSurfaceBuilder.BuildNavMeshSurface();
    }

}

