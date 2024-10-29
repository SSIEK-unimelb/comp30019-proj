using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private AudioClip quitGameAudio;

    private void Start() {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayGame() {
        audioSource.PlayOneShot(clickAudio);
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("Level 1");
    }

    public void PlayTutorial() {
        audioSource.PlayOneShot(clickAudio);
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("Level Tutorial");
    }

    public void PlayLevel2() {
        audioSource.PlayOneShot(clickAudio);
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene("Level 2");
    }
    
    public void QuitGame() {
        audioSource.PlayOneShot(quitGameAudio);
        Debug.Log("QUIT");  
        Application.Quit();
    }

}
