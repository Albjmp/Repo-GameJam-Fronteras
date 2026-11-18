using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteraccionJugador : MonoBehaviour
{
    [Header("Referencias")]
    public Camera camaraJugador;
    public Inventario inventarioJugador;
    public TextMeshProUGUI textoAvisoUI;

    [Header("Configuración")]
    public float distanciaRecogida = 3f;

    private ObjetoRecogible objetoApuntado;

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        DetectarObjetoEnMira();

        if (objetoApuntado != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            IntentarRecoger();
        }
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
                objetoApuntado = recogible;
                MostrarAviso("Presiona [E] para recoger: " + recogible.datosObjeto.nombre);
                return;
            }
        }

        if (objetoApuntado != null)
        {
            objetoApuntado = null;
            OcultarAviso();
        }
    }

    private void IntentarRecoger()
    {
        bool guardadoConExito = inventarioJugador.RecogerObjeto(objetoApuntado.datosObjeto);

        if (guardadoConExito)
        {
            Destroy(objetoApuntado.gameObject);
            objetoApuntado = null;
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
