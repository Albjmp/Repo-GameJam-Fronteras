using UnityEngine;
using System.Collections;

public class DialogoAlVerObjeto : MonoBehaviour
{
    [Header("Referencias del Cuadro de Diálogo")]
    public GameObject panelDialogo;
    public DIalogueScript scriptDialogo;
    public Camera camaraJugador;

    [Header("Aparición del Enemigo al Terminar")]
    public GameObject enemigoQueAparece;
    public AudioSource fuenteAudioSusto;
    public AudioClip sonidoAparicion;

    [Header("Configuración de Detección Visual")]
    public float distanciaMaximaVision = 15f;
    public LayerMask capaObstaculos;
    public bool activarSoloUnaVez = true;

    [Header("Secuencia de Diálogo")]
    public string[] nombresHablantes;
    [TextArea(2, 5)]
    public string[] lineasDialogo;

    private bool yaActivado = false;
    private Renderer renderizadorObjeto;

    private void Awake()
    {
        renderizadorObjeto = GetComponent<Renderer>();
        if (camaraJugador == null) camaraJugador = Camera.main;
    }

    private void Update()
    {
        if ((activarSoloUnaVez && yaActivado) || Time.timeScale == 0f) return;
        if (panelDialogo != null && panelDialogo.activeSelf) return;
        if (camaraJugador == null || renderizadorObjeto == null) return;

        if (EstaEnCampoDeVision())
        {
            float distancia = Vector3.Distance(camaraJugador.transform.position, transform.position);

            if (distancia <= distanciaMaximaVision)
            {
                Vector3 direccion = (transform.position - camaraJugador.transform.position).normalized;

                if (!Physics.Raycast(camaraJugador.transform.position, direccion, distancia, capaObstaculos))
                {
                    ActivarSecuenciaDialogo();
                }
            }
        }
    }

    private bool EstaEnCampoDeVision()
    {
        Plane[] planosCamara = GeometryUtility.CalculateFrustumPlanes(camaraJugador);
        return GeometryUtility.TestPlanesAABB(planosCamara, renderizadorObjeto.bounds);
    }

    private void ActivarSecuenciaDialogo()
    {
        yaActivado = true;

        if (panelDialogo != null && scriptDialogo != null && lineasDialogo.Length > 0)
        {
            scriptDialogo.SpeakerNames = nombresHablantes;
            scriptDialogo.DialogueLines = lineasDialogo;

            panelDialogo.SetActive(true);
            scriptDialogo.StartDialogue();

            StartCoroutine(EsperarFinDialogoYActivarEnemigo());
        }
    }

    private IEnumerator EsperarFinDialogoYActivarEnemigo()
    {
        yield return null;

        while (panelDialogo != null && panelDialogo.activeInHierarchy)
        {
            yield return null;
        }

        if (enemigoQueAparece != null)
        {
            enemigoQueAparece.SetActive(true);

            if (fuenteAudioSusto != null && sonidoAparicion != null)
            {
                fuenteAudioSusto.PlayOneShot(sonidoAparicion);
            }
        }
    }
}
