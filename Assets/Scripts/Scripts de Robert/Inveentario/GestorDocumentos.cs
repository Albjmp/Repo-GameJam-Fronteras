using UnityEngine;
using TMPro;

public class GestorDocumentos : MonoBehaviour
{
    [Header("UI de Lectura")]
    public GameObject panelLectura;
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoContenido;
    
    [Header("UI del Inventario")]
    public GameObject contenedorSlots; 

    private void Start()
    {
        if (panelLectura != null) panelLectura.SetActive(false);
    }

    public void AbrirDocumento(ItemData documento)
    {
        if (documento != null)
        {
            textoTitulo.text = documento.nombre;
            textoContenido.text = documento.descripcion; 
            
            panelLectura.SetActive(true);
            
            // Ocultamos los cuadros del inventario para que no se vean de fondo
            if (contenedorSlots != null) contenedorSlots.SetActive(false);
        }
    }

    public void CerrarDocumento()
    {
        panelLectura.SetActive(false);
        
        // Volvemos a mostrar los cuadros
        if (contenedorSlots != null) contenedorSlots.SetActive(true);
    }
}