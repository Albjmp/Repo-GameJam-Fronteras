using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;
using TMPro;

public class InteraccionCuadro : MonoBehaviour
{
    [Header("Configuración de Cinemática")]
    public ControladorCinematica controladorCinematica;
    public VideoClip clipCinematica;
    public float retrasoSegundos = 3f;
    public bool activarSoloUnaVez = true;

    [Header("Interfaz de usuario (Opcional)")]
    public TextMeshProUGUI textoInteractuarUI;
    public string mensajeAviso = "Presiona [E] para observar";

    [Header("Eventos al pulsar E (Opcional)")]
    public UnityEvent alInteractuar;

    private bool jugadorCerca = false;
    private bool yaActivado = false;
    private bool esperandoCinematica = false;

    private void Start()
    {
        if (controladorCinematica == null)
        {
            controladorCinematica = FindAnyObjectByType<ControladorCinematica>();
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f || ControladorCinematica.EnCinematica) return;
        if (!jugadorCerca || esperandoCinematica) return;
        if (activarSoloUnaVez && yaActivado) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(IniciarCinematicaConRetraso());
        }
    }

    private IEnumerator IniciarCinematicaConRetraso()
    {
        yaActivado = true;
        esperandoCinematica = true;

        if (textoInteractuarUI != null)
        {
            textoInteractuarUI.gameObject.SetActive(false);
        }

        alInteractuar?.Invoke();

        yield return new WaitForSeconds(retrasoSegundos);

        esperandoCinematica = false;

        if (controladorCinematica != null && clipCinematica != null)
        {
            controladorCinematica.ReproducirCinematica(clipCinematica);
        }
        else
        {
            Debug.LogWarning("Falta asignar ControladorCinematica o ClipCinematica en " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (activarSoloUnaVez && yaActivado) return;

        jugadorCerca = true;

        if (textoInteractuarUI != null)
        {
            textoInteractuarUI.text = mensajeAviso;
            textoInteractuarUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorCerca = false;

        if (textoInteractuarUI != null)
        {
            textoInteractuarUI.gameObject.SetActive(false);
        }
    }
}