using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// It seems this script is unused?
// The script to change scenes is the DoorToggle script.

public class SceneExit : MonoBehaviour
{
    private void OnInteract(Collider other) {
        if (other.tag == "LevelExit") {
            Debug.Log("Hello");

            Debug.Log("Current scene name is: " + SceneManager.GetActiveScene().name);
            if (SceneManager.GetActiveScene().name == "Level Tutorial") {
                SceneManager.LoadScene("MainMenu");
            } else {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
