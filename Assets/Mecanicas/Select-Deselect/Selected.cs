using UnityEngine;

public class Selected : MonoBehaviour
{
    LayerMask mask;

    public Texture2D puntero;
    public GameObject TextDetect;

    GameObject ultimoReconocido = null;
    public float distancia = 1.5f;

    void Start()
    {
        mask = LayerMask.GetMask("Raycast Detect");
      
        if (TextDetect != null)
        {
            TextDetect.SetActive(false);
        }
    }

    void Update()
    {
        RaycastHit hit;
        bool detectoInteractivo = false;

        // Lanzamos el raycast
        if (Physics.Raycast(transform.position, transform.forward, out hit, distancia, mask))
        {
            // Verificamos si es un objeto interactivo
            if (hit.collider.CompareTag("ObjetoInteractivo"))
            {
                detectoInteractivo = true;
                
                // Gestionamos la selección visual del objeto
                if (ultimoReconocido != hit.transform.gameObject)
                {
                    Deselect();
                    SelectedObject(hit.transform);
                }

                // Detectar tecla E para interactuar
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ObjetoInteractivo objInteractivo = hit.collider.GetComponent<ObjetoInteractivo>();
                    if (objInteractivo != null)
                    {
                        objInteractivo.ActivarObjeto();
                    }
                }
            }
        }

        // Si no estamos mirando ningún objeto interactivo válido, limpiamos la selección
        if (!detectoInteractivo)
        {
            Deselect();
        }

        // Actualizamos el estado del texto de la UI en base a si hay un objeto reconocido
        if (TextDetect != null)
        {
            TextDetect.SetActive(ultimoReconocido != null);
        }

        Debug.DrawRay(transform.position, transform.forward * distancia, Color.yellow);
    }

    void SelectedObject(Transform objTransform)
    {
        Renderer rend = objTransform.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.red;
        }
        ultimoReconocido = objTransform.gameObject;
    }

    void Deselect()
    {
        if (ultimoReconocido != null)
        {
            Renderer rend = ultimoReconocido.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Color.white;
            }
            ultimoReconocido = null;
        }
    }

    void OnGUI()
    {
        // Verificamos que la variable puntero esté asignada para evitar errores en consola
        if (puntero != null)
        {
            Rect rect = new Rect(Screen.width / 2 - puntero.width / 2, Screen.height / 2 - puntero.height / 2, puntero.width, puntero.height);
            GUI.DrawTexture(rect, puntero);
        }
    }
}