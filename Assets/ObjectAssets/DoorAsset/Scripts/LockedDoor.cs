using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SoundMaker))]
public class LockedDoor : MonoBehaviour
{
    [SerializeField] private string levelObjectName = "Level";
    private NavMeshSurfaceBuilder navMeshSurfaceBuilder;
    [SerializeField] private GameObject quadrantToLoad;

    private AudioSource audioSource;

    private Animator animator;
    [SerializeField] private AudioClip openAudio;
    [SerializeField] private string doorOpenStr;
    [SerializeField] private float doorOpenTime = 0.5f;
    public bool isOpen = false;

    // Start is called before the first frame update

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        navMeshSurfaceBuilder = GameObject.Find(levelObjectName).GetComponent<NavMeshSurfaceBuilder>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (quadrantToLoad != null) { 
            quadrantToLoad.SetActive(false);
        }
    }

    public void OnTrigger() {
        if (!isOpen) {
            animator.Play(doorOpenStr);
            audioSource.clip = openAudio;
            audioSource.Play();

            GetComponent<SoundMaker>().MakeSound();
            isOpen = true;
            if (quadrantToLoad != null)
            {
                quadrantToLoad.SetActive(true);
            }
            //StartCoroutine(WaitToOpen());
        }
    }

    private IEnumerator WaitToOpen() {
        yield return new WaitForSeconds(doorOpenTime);
        if (navMeshSurfaceBuilder != null)
        {
            //navMeshSurfaceBuilder.SetBuildToTrue();
        }
    }

}

