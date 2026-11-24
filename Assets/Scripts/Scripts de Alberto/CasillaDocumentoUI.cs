using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CasillaDocumentoUI : MonoBehaviour
{
    [Header("Referencias Visuales")]
    public Image imagenCasilla;
    public Image imagenIcono;
    public TextMeshProUGUI textoTitulo;
    public Button botonCasilla;

    [Header("Colores de Selección")]
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = new Color(1f, 0.9f, 0.4f, 1f);

    private ItemData datosDocumento;
    private System.Action<ItemData> callbackSeleccion;

    /// <summary>
    /// Expone el ItemData asignado para comparaciones por referencia.
    /// </summary>
    public ItemData DatosDocumento => datosDocumento;

    private void Awake()
    {
        if (botonCasilla == null)
        {
            botonCasilla = GetComponent<Button>();
        }

        if (botonCasilla != null)
        {
            botonCasilla.onClick.AddListener(OnClickCasilla);
        }
    }

    public void Configurar(ItemData doc, Sprite spriteCuadro, Sprite spriteCarpetaDefecto, System.Action<ItemData> alSeleccionar)
    {
        datosDocumento = doc;
        callbackSeleccion = alSeleccionar;

        if (imagenCasilla != null && spriteCuadro != null)
        {
            imagenCasilla.sprite = spriteCuadro;
            imagenCasilla.color = colorNormal;
        }

        if (imagenIcono != null)
        {
            if (doc != null && doc.icono != null)
            {
                imagenIcono.sprite = doc.icono;
                imagenIcono.color = Color.white;
            }
            else if (spriteCarpetaDefecto != null)
            {
                imagenIcono.sprite = spriteCarpetaDefecto;
                imagenIcono.color = Color.white;
            }
            else
            {
                imagenIcono.color = new Color(1f, 1f, 1f, 0.6f);
            }
        }

        if (textoTitulo != null && doc != null)
        {
            textoTitulo.text = string.IsNullOrEmpty(doc.nombre) ? "Documento" : doc.nombre;
        }
    }

    public void EstablecerSeleccionado(bool seleccionado)
    {
        if (imagenCasilla != null)
        {
            imagenCasilla.color = seleccionado ? colorSeleccionado : colorNormal;
        }
    }

    private void OnClickCasilla()
    {
        if (datosDocumento != null)
        {
            callbackSeleccion?.Invoke(datosDocumento);
        }
    }

}
