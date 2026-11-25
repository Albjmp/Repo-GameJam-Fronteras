using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Inventario : MonoBehaviour
{
    public static Inventario Instance { get; private set; }

    [Header("Configuración UI - Panel")]
    public GameObject panelInventario; 
    public Transform contenedorSlots;   
    public GameObject prefabSlot;      

    [Header("Configuración UI - Pestañas")]
    public Button botonPestanaObjetos;
    public Button botonPestanaDocumentos;
    
    [Header("Visuales de Pestañas (Opcional)")]
    public GameObject lineaObjetos;
    public GameObject lineaDocumentos;

    [Header("Audio")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoRecoger;

    private List<ItemData> items = new List<ItemData>();
    private TipoItem pestanaActual = TipoItem.Objeto; 

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        if (panelInventario != null) panelInventario.SetActive(false);

        if (botonPestanaObjetos != null)
            botonPestanaObjetos.onClick.AddListener(() => CambiarPestana(TipoItem.Objeto));
        
        if (botonPestanaDocumentos != null)
            botonPestanaDocumentos.onClick.AddListener(() => CambiarPestana(TipoItem.Documento));
    }

    private void Update()
    {
        // Abrir/Cerrar inventario con la tecla I
        if (Input.GetKeyDown(KeyCode.I) && panelInventario != null)
        {
            bool abrir = !panelInventario.activeSelf;
            panelInventario.SetActive(abrir);
            
            if (abrir) 
            {
                CambiarPestana(TipoItem.Objeto); 
                
                // Liberar ratón y pausar
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f; 
            }
            else
            {
                // Bloquear ratón y reanudar
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f; 
            }
        }
    }

    public void CambiarPestana(TipoItem nuevaPestana)
    {
        pestanaActual = nuevaPestana;
        
        if (lineaObjetos != null && lineaDocumentos != null)
        {
            lineaObjetos.SetActive(pestanaActual == TipoItem.Objeto);
            lineaDocumentos.SetActive(pestanaActual == TipoItem.Documento);
        }

        ActualizarUI();
    }

    public bool RecogerObjeto(ItemData item)
    {
        items.Add(item);
        
        if (fuenteAudio != null && sonidoRecoger != null)
        {
            fuenteAudio.PlayOneShot(sonidoRecoger);
        }
        
        ActualizarUI();
        return true;
    }

    public bool TieneObjeto(ItemData item) => items.Contains(item);

    public void EliminarObjeto(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            ActualizarUI(); 
        }
    }

    private void ActualizarUI()
    {
        // Limpiar slots viejos
        foreach (Transform hijo in contenedorSlots) Destroy(hijo.gameObject);

        // Crear slots nuevos para la pestaña actual
        foreach (ItemData item in items)
        {
            if (item.tipo == pestanaActual) 
            {
                GameObject nuevoSlot = Instantiate(prefabSlot, contenedorSlots);
                
                Transform icono = nuevoSlot.transform.Find("Icono");
                if (icono != null) icono.GetComponent<Image>().sprite = item.icono;

                Transform nombre = nuevoSlot.transform.Find("Nombre");
                if (nombre != null) nombre.GetComponent<TextMeshProUGUI>().text = item.nombre;
            }
        }
    }
}