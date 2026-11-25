using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

public class PuertaConLlave : MonoBehaviour
{
    [Header("Configuración de llave")]
    public ItemData llaveRequerida;
    public bool consumirLlaveAlUsar = false;

    [Header("Rotación de la puerta")]
    public float anguloApertura = 90f;
    public float velocidadApertura = 3f;

    [Header("Interfaz de usuario")]
    public TextMeshProUGUI textoAvisoUI;

    [Header("Audio de la puerta")]
    public AudioSource fuenteAudioPuerta;
    public AudioClip sonidoDesbloquearLlave;
    public AudioClip sonidoAbrirPuerta;

    [Header("Eventos al Abrir")]
    public UnityEvent onPuertaAbierta;

    [HideInInspector] public Inventario inventarioJugador;
    private bool jugadorCerca = false;
    private bool estaAbierta = false;
    private Quaternion rotacionCerrada;
    private Quaternion rotacionAbierta;

    private void Start()
    {
        rotacionCerrada = transform.rotation;
        rotacionAbierta = Quaternion.Euler(transform.eulerAngles + new Vector3(0, anguloApertura, 0));

        if (inventarioJugador == null)
        {
            inventarioJugador = ObtenerInventario();
        }
    }

    public Inventario ObtenerInventario()
    {
        if (inventarioJugador != null) return inventarioJugador;
        if (Inventario.Instance != null) return Inventario.Instance;
        inventarioJugador = FindAnyObjectByType<Inventario>();
        return inventarioJugador;
    }

    private void Update()
    {
        Quaternion objetivo = estaAbierta ? rotacionAbierta : rotacionCerrada;
        transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, Time.deltaTime * velocidadApertura);

        if (jugadorCerca && InteractuoPresionado())
        {
            IntentarAbrir();
        }
    }

    private bool InteractuoPresionado()
    {
        bool presionado = false;
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) presionado = true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.E)) presionado = true;
#endif
        return presionado;
    }

    private void IntentarAbrir()
    {
        if (estaAbierta)
        {
            estaAbierta = false;
            ActualizarTexto("Presiona [E] para abrir");
            return;
        }

        Inventario inv = ObtenerInventario();
        if (inv == null)
        {
            Debug.LogError("ERROR: La puerta no encontró el script Inventario en la escena");
            return;
        }

        if (llaveRequerida == null)
        {
            Debug.LogError("ERROR: No asignaste ninguna llave en el Inspector de la puerta");
            return;
        }

        if (inv.TieneObjeto(llaveRequerida))
        {
            estaAbierta = true;
            ActualizarTexto("Presiona [E] para cerrar");

            if (fuenteAudioPuerta != null)
            {
                if (sonidoDesbloquearLlave != null) fuenteAudioPuerta.PlayOneShot(sonidoDesbloquearLlave);
                if (sonidoAbrirPuerta != null) fuenteAudioPuerta.PlayOneShot(sonidoAbrirPuerta);
            }

            onPuertaAbierta?.Invoke();

            if (consumirLlaveAlUsar)
            {
                inv.EliminarObjeto(llaveRequerida);
            }
        }
        else
        {
            ActualizarTexto("Cerrada. Necesitas: " + llaveRequerida.nombre);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (inventarioJugador == null)
            {
                inventarioJugador = other.GetComponent<Inventario>();
                if (inventarioJugador == null)
                {
                    inventarioJugador = ObtenerInventario();
                }
            }
            if (textoAvisoUI != null)
            {
                textoAvisoUI.gameObject.SetActive(true);
                ActualizarTexto(estaAbierta ? "Presiona [E] para cerrar" : "Presiona [E] para abrir");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (textoAvisoUI != null)
            {
                textoAvisoUI.gameObject.SetActive(false);
            }
        }
    }

    private void ActualizarTexto(string mensaje)
    {
        if (textoAvisoUI != null)
        {
            textoAvisoUI.text = mensaje;
        }
    }
}
