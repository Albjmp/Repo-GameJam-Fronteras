using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject fullPauseCanvas;
    private bool isPaused = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            } else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        fullPauseCanvas.SetActive(true);
        pauseMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        //Por si se necesita el mouse para el gameplay
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Game Exited");
        Application.Quit();
    }

    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
}
