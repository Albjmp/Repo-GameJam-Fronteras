using UnityEngine;
using UnityEngine.InputSystem;
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

    public Inventario inventarioJugador;
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
            inventarioJugador = FindAnyObjectByType<Inventario>();
        }
    }

    private void Update()
    {
        Quaternion objetivo = estaAbierta ? rotacionAbierta : rotacionCerrada;
        transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, Time.deltaTime * velocidadApertura);

        if (jugadorCerca && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            IntentarAbrir();
        }
    }

    private void IntentarAbrir()
    {
        if (estaAbierta)
        {
            estaAbierta = false;
            ActualizarTexto("Presiona [E] para abrir");
            return;
        }

        if (inventarioJugador == null) Debug.LogError("ERROR: La puerta no encontró el script Inventario");
        if (llaveRequerida == null) Debug.LogError("ERROR: No asignaste ninguna llave en el Inspector de la puerta");

        if (inventarioJugador != null && inventarioJugador.TieneObjeto(llaveRequerida))
        {
            estaAbierta = true;
            ActualizarTexto("Presiona [E] para cerrar");

            if (consumirLlaveAlUsar)
            {
                inventarioJugador.EliminarObjeto(llaveRequerida);
            }
        } else
        {
            ActualizarTexto("Cerrada.Necesitas: " + llaveRequerida.nombre);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
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
