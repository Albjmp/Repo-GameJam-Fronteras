using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class TriggerCinematica : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject panelCinematica; // Un panel negro que cubre la pantalla
    public VideoPlayer reproductorVideo; // Opcional, para cuando tengas el video
    public UnityEvent onCinematicaTerminada;

    private bool yaActivada = false;

    public void IniciarCinematica()
    {
        if (yaActivada) return;
        yaActivada = true;

        // 1. Preparar entorno
        if (panelCinematica != null) panelCinematica.SetActive(true);
        Time.timeScale = 0f; // Pausar juego
        Cursor.visible = false;

        // 2. Reproducir (Si tienes el video, descomenta y asigna el VideoPlayer)
        if (reproductorVideo != null)
        {
            reproductorVideo.Play();
            // Escuchar el evento de que el video terminó
            reproductorVideo.loopPointReached += AlTerminarVideo;
        }
        else
        {
            // Si no tienes video, simula una espera de 3 segundos y termina
            Invoke(nameof(AlTerminarVideo), 3f); 
        }
    }

    private void AlTerminarVideo(VideoPlayer vp)
    {
        // Restaurar entorno
        if (panelCinematica != null) panelCinematica.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = true;
        
        onCinematicaTerminada.Invoke(); // Avisa a otros scripts que ya terminó
    }
}