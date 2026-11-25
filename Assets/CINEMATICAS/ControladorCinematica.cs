using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[RequireComponent(typeof(VideoPlayer))]
public class ControladorCinematica : MonoBehaviour
{
    public static bool EnCinematica { get; private set; } = false;

    [Header("1. Cinemática al iniciar nivel (Opcional)")]
    [Tooltip("Asigna aquí la cinemática inicial si debe reproducirse apenas cargue este nivel.")]
    public VideoClip clipAlIniciar;

    [Header("3. Escena tras cinemática final (Opcional)")]
    [Tooltip("Nombre exacto de la escena a cargar al terminar la cinemática del puzzle (ej. MainMenu).")]
    public string escenaSiguienteAlFinal;

    [Header("Referencias de UI y Jugador")]
    public GameObject hudJuego;
    [Tooltip("Arrastra aquí scripts de cámara o movimiento del jugador para bloquearlos durante el vídeo.")]
    public MonoBehaviour[] scriptsABloquear;

    [Header("Opciones de reproducción")]
    public bool permitirSaltarConEspacio = true;

    [Header("Eventos al terminar cualquier cinemática")]
    public UnityEvent onCinematicaTerminada;

    private VideoPlayer reproductor;
    private AudioSource fuenteAudioCinematica;
    private bool reproduciendo = false;
    private bool esCinematicaFinal = false;

    private void Awake()
    {
        reproductor = GetComponent<VideoPlayer>();
        reproductor.playOnAwake = false;
        reproductor.renderMode = VideoRenderMode.CameraNearPlane;
        reproductor.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;

        // Configura un AudioSource exclusivo que ignora la pausa general de audio
        fuenteAudioCinematica = GetComponent<AudioSource>();
        if (fuenteAudioCinematica == null)
        {
            fuenteAudioCinematica = gameObject.AddComponent<AudioSource>();
        }
        fuenteAudioCinematica.playOnAwake = false;
        fuenteAudioCinematica.ignoreListenerPause = true;

        reproductor.audioOutputMode = VideoAudioOutputMode.AudioSource;
        reproductor.EnableAudioTrack(0, true);
        reproductor.SetTargetAudioSource(0, fuenteAudioCinematica);

        if (reproductor.targetCamera == null)
        {
            reproductor.targetCamera = Camera.main;
        }

        reproductor.loopPointReached += AlTerminarVideo;
        reproductor.errorReceived += AlOcurrirErrorVideo;

        if (clipAlIniciar != null)
        {
            EnCinematica = true;
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
    }

    private void Start()
    {
        if (clipAlIniciar != null)
        {
            ReproducirCinematica(clipAlIniciar);
        }
    }

    private void Update()
    {
        if (!reproduciendo || !permitirSaltarConEspacio) return;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            FinalizarCinematica();
        }
    }

    public void ReproducirCinematica(VideoClip clip)
    {
        if (clip == null) return;
        esCinematicaFinal = false;
        IniciarReproduccion(clip);
    }

    public void ReproducirCinematicaFinal(VideoClip clip)
    {
        if (clip == null) return;
        esCinematicaFinal = true;
        IniciarReproduccion(clip);
    }

    private void IniciarReproduccion(VideoClip clip)
    {
        EnCinematica = true;
        reproduciendo = true;

        if (hudJuego != null) hudJuego.SetActive(false);
        CambiarEstadoScripts(false);

        Time.timeScale = 0f;
        AudioListener.pause = true;

        reproductor.clip = clip;
        reproductor.Play();
    }

    private void AlTerminarVideo(VideoPlayer vp)
    {
        FinalizarCinematica();
    }

    private void AlOcurrirErrorVideo(VideoPlayer vp, string mensaje)
    {
        Debug.LogWarning("Aviso de reproducción de vídeo: " + mensaje);
        FinalizarCinematica();
    }

    private void FinalizarCinematica()
    {
        if (!reproduciendo) return;
        reproduciendo = false;
        EnCinematica = false;

        reproductor.Stop();
        AudioListener.pause = false;
        Time.timeScale = 1f;

        if (esCinematicaFinal && !string.IsNullOrEmpty(escenaSiguienteAlFinal))
        {
            SceneManager.LoadScene(escenaSiguienteAlFinal);
            return;
        }

        if (hudJuego != null) hudJuego.SetActive(true);
        CambiarEstadoScripts(true);

        onCinematicaTerminada?.Invoke();
    }

    private void CambiarEstadoScripts(bool estado)
    {
        if (scriptsABloquear == null) return;

        foreach (MonoBehaviour script in scriptsABloquear)
        {
            if (script != null) script.enabled = estado;
        }
    }

    private void OnDestroy()
    {
        if (reproduciendo)
        {
            AudioListener.pause = false;
            Time.timeScale = 1f;
            EnCinematica = false;
        }

        if (reproductor != null)
        {
            reproductor.loopPointReached -= AlTerminarVideo;
            reproductor.errorReceived -= AlOcurrirErrorVideo;
        }
    }
}