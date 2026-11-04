using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Drag your panels into these slots in the Inspector
    public GameObject mainMenuContent; // A GameObject containing your Play/Settings/Quit buttons
    public GameObject settingsPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenSettings()
    {
        mainMenuContent.SetActive(false); // Hide main buttons
        settingsPanel.SetActive(true);    // Show settings
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);   // Hide settings
        mainMenuContent.SetActive(true);  // Show main buttons
    }

    public void QuitGame()
    {
        Debug.Log("Game Exited");
        Application.Quit();
    }
}
