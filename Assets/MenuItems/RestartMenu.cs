using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartMenu : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private AudioClip quitGameAudio;

    private void Start() {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void RestartGame() {
        audioSource.PlayOneShot(clickAudio);
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("StartScene");
    }
    
    public void QuitGame() {
        audioSource.PlayOneShot(quitGameAudio);
        Debug.Log("QUIT");  
        Application.Quit();
    }
}
