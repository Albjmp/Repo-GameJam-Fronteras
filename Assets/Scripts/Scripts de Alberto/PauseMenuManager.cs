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
    public GameObject barraEstaminaUI;

    [Header("Audio de música en pausa")]
    public AudioSource musicaNivel;
    [Range(0f, 1f)] public float multiplicadorVolumenPausa = 0.25f;
    private float volumenOriginalMusica = 1f;

    private bool isPaused = false;

    void Start()
    {
        if (musicaNivel != null)
        {
            volumenOriginalMusica = musicaNivel.volume;
        }

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

        if (barraEstaminaUI != null)
        {
            barraEstaminaUI.SetActive(false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (musicaNivel != null)
        {
            musicaNivel.volume = volumenOriginalMusica * multiplicadorVolumenPausa;
        }
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        fullPauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        ControladorJugador jugador = FindAnyObjectByType<ControladorJugador>();
        //if (jugador != null && jugador.protagonistaActivo == ControladorJugador.Protagonista.Ivo)
        //{
            //if (barraLinternaUI != null)
            //{
                //barraLinternaUI.SetActive(true);
            //}
            //if (barraEstaminaUI != null)
            //{
                //barraEstaminaUI.SetActive(true);
            //}
        //}

        if (jugador != null)
        {
            if (barraLinternaUI != null)
            {
                barraLinternaUI.SetActive(jugador.protagonistaActivo == ControladorJugador.Protagonista.Ivo);
            }

            if (barraEstaminaUI != null)
            {
                barraEstaminaUI.SetActive(jugador.protagonistaActivo == ControladorJugador.Protagonista.Sean);
            }
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (musicaNivel != null)
        {
            musicaNivel.volume = volumenOriginalMusica;
        }
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
