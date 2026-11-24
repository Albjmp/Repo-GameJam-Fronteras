using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Selected : MonoBehaviour
{
    [Header("Configuración de Raycast")]
    [Tooltip("Distancia máxima de interacción en metros.")]
    [SerializeField] private float distancia = 3.5f;

    [Tooltip("Capas con las que colisiona el raycast. Si se deja en 'Nothing' o 'Default', detectará automáticamente.")]
    [SerializeField] private LayerMask mask;

    [Header("Interfaz de Usuario")]
    public Texture2D puntero;
    public GameObject TextDetect;

    private TextMeshProUGUI tmpText;
    private Text uiText;
    private Camera camara;

    private GameObject ultimoReconocido = null;
    private ObjetoInteractivo objetoActual = null;
    private Renderer rendererActual = null;
    private Color colorOriginal = Color.white;
    private bool tieneColorGuardado = false;
    private string propiedadColor = "_Color";

    void Awake()
    {
        camara = GetComponent<Camera>();
        if (camara == null) camara = Camera.main;

        int capaRaycastDetect = LayerMask.GetMask("Raycast Detect");
        int capaDefault = 1 << LayerMask.NameToLayer("Default");

        if (mask.value == 0)
        {
            mask = (capaRaycastDetect != 0) ? (capaRaycastDetect | capaDefault) : Physics.DefaultRaycastLayers;
        }
        else
        {
            if (capaRaycastDetect != 0) mask |= capaRaycastDetect;
        }

        if (distancia < 2.5f) distancia = 3.5f;
    }

    void Start()
    {
        if (TextDetect != null)
        {
            tmpText = TextDetect.GetComponent<TextMeshProUGUI>();
            if (tmpText == null) tmpText = TextDetect.GetComponentInChildren<TextMeshProUGUI>();

            if (tmpText == null)
            {
                uiText = TextDetect.GetComponent<Text>();
                if (uiText == null) uiText = TextDetect.GetComponentInChildren<Text>();
            }

            TextDetect.SetActive(false);
        }
    }

    void Update()
    {
        RaycastHit hit;
        bool detectoInteractivo = false;

        Transform origen = (camara != null) ? camara.transform : transform;
        Vector3 puntoInicio = origen.position;
        Vector3 direccion = origen.forward;

        // Lanzamos el raycast
        if (Physics.Raycast(puntoInicio, direccion, out hit, distancia, mask))
        {
            // AQUÍ ESTÁ EL DEBUG LOG CORREGIDO
            Debug.Log("El rayo choca con: " + hit.collider.gameObject.name);

            ObjetoInteractivo interactivo = hit.collider.GetComponentInParent<ObjetoInteractivo>();

            if (interactivo != null || hit.collider.CompareTag("ObjetoInteractivo"))
            {
                detectoInteractivo = true;

                if (ultimoReconocido != hit.transform.gameObject)
                {
                    Deselect();
                    SelectedObject(hit.transform, interactivo);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (interactivo != null)
                    {
                        Debug.Log($"[Selected] Interactuando con '{interactivo.gameObject.name}'");
                        interactivo.ActivarObjeto();
                        ActualizarTextoUI(interactivo);
                    }
                }
            }
        }
        else
        {
            Debug.Log("El rayo no choca con ningún objeto.");
        }

        if (!detectoInteractivo)
        {
            Deselect();
        }

        if (TextDetect != null)
        {
            TextDetect.SetActive(ultimoReconocido != null);
        }

        Debug.DrawRay(puntoInicio, direccion * distancia, Color.yellow);
    }

    void SelectedObject(Transform objTransform, ObjetoInteractivo interactivo)
    {
        ultimoReconocido = objTransform.gameObject;
        objetoActual = interactivo;

        rendererActual = objTransform.GetComponent<Renderer>();
        if (rendererActual != null && rendererActual.material != null)
        {
            if (rendererActual.material.HasProperty("_BaseColor"))
            {
                propiedadColor = "_BaseColor";
                colorOriginal = rendererActual.material.GetColor("_BaseColor");
                tieneColorGuardado = true;
                rendererActual.material.SetColor("_BaseColor", Color.red);
            }
            else if (rendererActual.material.HasProperty("_Color"))
            {
                propiedadColor = "_Color";
                colorOriginal = rendererActual.material.GetColor("_Color");
                tieneColorGuardado = true;
                rendererActual.material.SetColor("_Color", Color.red);
            }
        }

        ActualizarTextoUI(interactivo);
    }

    void Deselect()
    {
        if (rendererActual != null && tieneColorGuardado && rendererActual.material != null)
        {
            if (rendererActual.material.HasProperty(propiedadColor))
            {
                rendererActual.material.SetColor(propiedadColor, colorOriginal);
            }
        }

        rendererActual = null;
        tieneColorGuardado = false;
        ultimoReconocido = null;
        objetoActual = null;
    }

    private void ActualizarTextoUI(ObjetoInteractivo interactivo)
    {
        if (interactivo == null) return;

        string mensaje = interactivo.ObtenerTextoInteraccion();

        if (tmpText != null) tmpText.text = mensaje;
        else if (uiText != null) uiText.text = mensaje;
    }

    void OnGUI()
    {
        if (puntero != null)
        {
            Rect rect = new Rect(Screen.width / 2 - puntero.width / 2, Screen.height / 2 - puntero.height / 2, puntero.width, puntero.height);
            GUI.DrawTexture(rect, puntero);
        }
    }
}