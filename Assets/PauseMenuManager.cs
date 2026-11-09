using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Paneles de la UI de Pausa")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject fullPauseCanvas;

    [Header("Interfaz Externa a Ocultar")]
    public GameObject barraLinternaUI;

    private bool isPaused = false;

    void Start()
    {
        ResumeGame();
    }

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
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

        if (barraLinternaUI != null)
        {
            barraLinternaUI.SetActive(false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        fullPauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        ControladorJugador jugador = FindAnyObjectByType<ControladorJugador>();
        if (jugador != null && jugador.protagonistaActivo == ControladorJugador.Protagonista.Ivo)
        {
            if (barraLinternaUI != null)
            {
                barraLinternaUI.SetActive(true);
            }
        }

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
