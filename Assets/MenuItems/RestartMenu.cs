using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartMenu : MonoBehaviour
{
    public void RestartGame() {
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitGame() {
        Debug.Log("QUIT");  
        Application.Quit();
    }
}
