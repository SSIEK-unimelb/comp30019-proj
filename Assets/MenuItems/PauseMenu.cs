using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private SoundManager soundManager;
    [SerializeField] private AudioClip pauseAudio;
    [SerializeField] private AudioClip unpauseAudio;
    private Button resumeButton;
    private Button restartButton;
    private Button quitButton;
    private GameObject blurPanel;
    public static bool gameIsPaused = false;

    private void Start() {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        resumeButton = transform.Find("ResumeButton").GetComponent<Button>();
        restartButton = transform.Find("RestartButton").GetComponent<Button>();
        quitButton = transform.Find("QuitButton").GetComponent<Button>();
        blurPanel = transform.Find("BlurPanel").gameObject;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            if (gameIsPaused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    private void Pause() {
        Time.timeScale = 0f;
        gameIsPaused = true;
        soundManager.PauseSound();
        soundManager.PlaySoundEffect(pauseAudio, 5f);

        blurPanel.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume() {
        Time.timeScale = 1f;
        gameIsPaused = false;
        soundManager.ResumeSound();
        soundManager.PlaySoundEffect(unpauseAudio, 1f);

        blurPanel.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Restart() {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit() {
        Resume();
        SceneManager.LoadScene("StartScene");
    }

}
