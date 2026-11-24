using UnityEngine;
using UnityEngine.InputSystem; // Necesario para el nuevo Input System
using TMPro;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Referencias")]
    public Camera camaraJugador;
    public TextMeshProUGUI textoAvisoUI;

    // Desacoplado del Inspector: se obtiene automáticamente del GameObject o por Singleton
    [HideInInspector] public Inventario inventarioJugador;

    [Header("Configuración")]
    public float distanciaRecogida = 3f;
    public Color colorContorno = Color.yellow;
    public float grosorContorno = 5f;

    private ObjetoRecogible objetoApuntado;
    private Outline contornoActual;

    private void Start()
    {
        if (camaraJugador == null) camaraJugador = Camera.main;

        if (inventarioJugador == null)
        {
            inventarioJugador = GetComponent<Inventario>();
            if (inventarioJugador == null)
            {
                inventarioJugador = Inventario.Instance != null ? Inventario.Instance : FindAnyObjectByType<Inventario>();
            }
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        DetectarObjetoEnMira();

        // ARREGLO BUG #3: Comprobación de tecla E compatible con ambos sistemas
        if (objetoApuntado != null && InteractuoPresionado())
        {
            IntentarRecoger();
        }
    }

    // Método auxiliar para detectar la E
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

    private void DetectarObjetoEnMira()
    {
        Ray rayo = camaraJugador.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit impacto;

        if (Physics.Raycast(rayo, out impacto, distanciaRecogida))
        {
            ObjetoRecogible recogible = impacto.collider.GetComponent<ObjetoRecogible>();

            if (recogible != null && recogible.datosObjeto != null)
            {
                if (objetoApuntado != recogible)
                {
                    ApagarContornoAnterior();
                    objetoApuntado = recogible;
                    EncenderContorno(recogible.gameObject);
                }
                
                MostrarAviso("Presiona [E] para recoger: " + recogible.datosObjeto.nombre);
                return;
            }
        }

        if (objetoApuntado != null)
        {
            ApagarContornoAnterior();
            objetoApuntado = null;
            OcultarAviso();
        }
    }

    private void EncenderContorno(GameObject objetivo)
    {
        contornoActual = objetivo.GetComponent<Outline>();
        if (contornoActual == null)
        {
            contornoActual = objetivo.AddComponent<Outline>();
            contornoActual.OutlineColor = colorContorno;
            contornoActual.OutlineWidth = grosorContorno;
        }
        contornoActual.enabled = true;
    }

    private void ApagarContornoAnterior()
    {
        if (contornoActual != null)
        {
            contornoActual.enabled = false;
            contornoActual = null;
        }
    }

    private void IntentarRecoger()
    {
        Inventario inv = inventarioJugador != null ? inventarioJugador : (Inventario.Instance != null ? Inventario.Instance : GetComponent<Inventario>());
        if (inv == null)
        {
            Debug.LogError("[InteraccionJugador] No se encontró el componente Inventario en la escena.");
            return;
        }

        bool guardadoConExito = inv.RecogerObjeto(objetoApuntado.datosObjeto);

        if (guardadoConExito)
        {
            Destroy(objetoApuntado.gameObject);
            objetoApuntado = null;
            contornoActual = null;
            OcultarAviso();
        } else
        {
            MostrarAviso("Inventario lleno. No hay espacio para: " + objetoApuntado.datosObjeto.nombre);
        }
    }

    private void MostrarAviso(string mensaje)
    {
        if (textoAvisoUI != null)
        {
            textoAvisoUI.gameObject.SetActive(true);
            textoAvisoUI.text = mensaje;
        }
    }

    private void OcultarAviso()
    {
        if (textoAvisoUI != null)
        {
            textoAvisoUI.gameObject.SetActive(false);
        }
    }
}