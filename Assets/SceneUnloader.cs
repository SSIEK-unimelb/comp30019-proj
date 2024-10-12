using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUnloader : MonoBehaviour
{
    [SerializeField] private GameObject[] ScenesToUnload;
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            // unload scenes
            foreach (var scene in ScenesToUnload)
            {
                scene.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag.Equals("Player")) {
            foreach (var scene in ScenesToUnload)
            {
                scene.SetActive(true);
            }
        }
    }
}
