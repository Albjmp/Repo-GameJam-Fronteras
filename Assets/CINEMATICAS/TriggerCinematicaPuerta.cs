using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class TriggerCinematicaPuerta : MonoBehaviour
{
    [Header("Configuración de Cinemática y Menú")]
    public ControladorCinematica controladorCinematica;
    public VideoClip clipCinematica;
    public string escenaMenuPrincipal = "MainMenu";

    private bool yaActivado = false;

    private void Start()
    {
        if (controladorCinematica == null)
        {
            controladorCinematica = FindAnyObjectByType<ControladorCinematica>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || yaActivado) return;
        if (Time.timeScale == 0f || ControladorCinematica.EnCinematica) return;

        yaActivado = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (controladorCinematica != null && clipCinematica != null)
        {
            controladorCinematica.escenaSiguienteAlFinal = escenaMenuPrincipal;
            controladorCinematica.ReproducirCinematicaFinal(clipCinematica);
        }
        else
        {
            AudioListener.pause = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(escenaMenuPrincipal);
        }
    }
}