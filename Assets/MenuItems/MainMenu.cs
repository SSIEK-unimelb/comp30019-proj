using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame() {
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("Level 1");
    }

    public void PlayTutorial() {
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("Level Tutorial");
    }
    
    public void QuitGame() {
        Debug.Log("QUIT");  
        Application.Quit();
    }

}
