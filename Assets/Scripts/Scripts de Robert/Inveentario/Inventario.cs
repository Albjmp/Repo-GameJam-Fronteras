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
    //public TextMeshProUGUI textoDescripcionDocumento;

    [Header("Configuración UI - Pestañas")]
    public Button botonPestanaObjetos;
    public Button botonPestanaDocumentos;
    
    [Header("Visuales de Pestañas (Opcional)")]
    public GameObject lineaObjetos;
    public GameObject lineaDocumentos;

    [Header("Sistemas Externos")]
    public GestorDocumentos gestorDocumentos;

    [Header("Configuración para Soltar Ítems")]
    public KeyCode teclaSoltar = KeyCode.Q; 

    [Header("Audio")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoRecoger;

    private List<ItemData> items = new List<ItemData>();
    private TipoItem pestanaActual = TipoItem.Objeto; 
    private ItemData itemSeleccionado; 

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
        // SOLO abrir/cerrar con la tecla I
        if (Input.GetKeyDown(KeyCode.I))
        {
            AlternarInventario();
        }

        // Soltar ítem si el inventario está abierto y presionas la tecla
        if (panelInventario != null && panelInventario.activeSelf && Input.GetKeyDown(teclaSoltar))
        {
            SoltarItemSeleccionado();
        }
    }

    public void AlternarInventario()
    {
        if (panelInventario == null) return;

        bool abrir = !panelInventario.activeSelf;
        panelInventario.SetActive(abrir);
        
        if (abrir) 
        {
            CambiarPestana(TipoItem.Objeto); 
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f; 
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f; 
        }
    }

    public void CambiarPestana(TipoItem nuevaPestana)
    {
        pestanaActual = nuevaPestana;
        itemSeleccionado = null;
        
        if (lineaObjetos != null && lineaDocumentos != null)
        {
            lineaObjetos.SetActive(pestanaActual == TipoItem.Objeto);
            lineaDocumentos.SetActive(pestanaActual == TipoItem.Documento);
        }

        ActualizarUI();
    }

    public bool RecogerObjeto(ItemData item)
    {
        if (items.Contains(item))
        {
            return true;
        }
        
        items.Add(item);
        
        if (fuenteAudio != null && sonidoRecoger != null)
        {
            fuenteAudio.PlayOneShot(sonidoRecoger);
        }
        
        ActualizarUI();

        if (GestorAnomalia.Instance != null)
            GestorAnomalia.Instance.VerificarDocumento(item); 

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

    public void SeleccionarItem(ItemData item)
    {
        itemSeleccionado = item;
    }

    public void SoltarItemSeleccionado()
    {
        if (itemSeleccionado == null) return;

        if (itemSeleccionado.prefabMundo != null)
        {
            Vector3 posSoltar = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
            Instantiate(itemSeleccionado.prefabMundo, posSoltar, Quaternion.identity);
        }

        EliminarObjeto(itemSeleccionado);
        itemSeleccionado = null; 
    }

    private void ActualizarUI()
    {
        foreach (Transform hijo in contenedorSlots) Destroy(hijo.gameObject);

        foreach (ItemData item in items)
        {
            if (item.tipo == pestanaActual) 
            {
                GameObject nuevoSlot = Instantiate(prefabSlot, contenedorSlots);
                
                Transform icono = nuevoSlot.transform.Find("Icono");
                if (icono != null) icono.GetComponent<Image>().sprite = item.icono;

                Transform nombre = nuevoSlot.transform.Find("Nombre");
                if (nombre != null) nombre.GetComponent<TextMeshProUGUI>().text = item.nombre;

                SlotUI scriptSlot = nuevoSlot.GetComponent<SlotUI>();
                if (scriptSlot != null)
                {
                    scriptSlot.itemData = item;
                    scriptSlot.gestorDoc = gestorDocumentos;
                    scriptSlot.inventario = this; 
                }

                //Button botonSlot = nuevoSlot.GetComponent<Button>();
                //if (botonSlot != null && pestanaActual == TipoItem.Documento)
                //{
                    //botonSlot.onClick.AddListener(() =>
                    //{
                        //if (textoDescripcionDocumento != null)
                        //{
                            //textoDescripcionDocumento.text = item.descripcion;
                        //}
                    //});
                //}
            }
        }
    }
}