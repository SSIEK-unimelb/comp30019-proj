using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUnloader : MonoBehaviour
{
    [SerializeField] private GameObject[] ScenesToUnload;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            // unload scenes
            foreach (var scene in ScenesToUnload) {
                scene.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            foreach (var scene in ScenesToUnload) {
                scene.SetActive(true);
            }
        }
    }
}
