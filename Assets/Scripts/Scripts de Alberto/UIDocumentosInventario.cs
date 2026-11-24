using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDocumentosInventario : MonoBehaviour
{
    [Header("Referencias Generales")]
    // Desacoplado del Inspector: resuelto automáticamente por Singleton o búsqueda de escena
    [HideInInspector] public Inventario inventario;
    public RectTransform contenedorDocumentos;
    public TextMeshProUGUI textoLecturaDocumento;
    public TextMeshProUGUI textoTituloDocumento;
    public Image imagenPreviewDocumento;

    [Header("Prefabs y Recursos Visuales")]
    public GameObject prefabCasillaDocumento;
    public Sprite spriteCuadro;
    public Sprite spriteCarpeta;

    private List<CasillaDocumentoUI> casillasInstanciadas = new List<CasillaDocumentoUI>();
    private ItemData documentoSeleccionado;

    private void Awake()
    {
        if (inventario == null)
        {
            inventario = Inventario.Instance != null ? Inventario.Instance : FindAnyObjectByType<Inventario>();
        }

        ConfigurarViewport();
    }

    private void OnEnable()
    {
        if (inventario == null)
        {
            inventario = Inventario.Instance != null ? Inventario.Instance : FindAnyObjectByType<Inventario>();
        }

        ConfigurarViewport();

        if (inventario != null)
        {
            inventario.alActualizarInventario += ActualizarDocumentos;
        }

        ActualizarDocumentos();
    }

    /// <summary>
    /// Asegura que el Viewport del ScrollRect ocupe todo el espacio disponible.
    /// Si el Viewport queda con tamaño cero, las casillas de documentos no se ven.
    /// </summary>
    private void ConfigurarViewport()
    {
        if (contenedorDocumentos == null) return;

        RectTransform viewport = contenedorDocumentos.parent as RectTransform;
        if (viewport == null) return;

        viewport.anchorMin = Vector2.zero;
        viewport.anchorMax = Vector2.one;
        viewport.pivot = new Vector2(0f, 1f);
        viewport.anchoredPosition = Vector2.zero;
        viewport.sizeDelta = Vector2.zero;
    }

    private void OnDisable()
    {
        if (inventario != null)
        {
            inventario.alActualizarInventario -= ActualizarDocumentos;
        }
    }

    public void ActualizarDocumentos()
    {
        // Limpiar casillas anteriores
        foreach (CasillaDocumentoUI casilla in casillasInstanciadas)
        {
            if (casilla != null)
            {
                Destroy(casilla.gameObject);
            }
        }
        casillasInstanciadas.Clear();

        if (inventario == null || inventario.listaDocumentos == null || inventario.listaDocumentos.Count == 0)
        {
            if (textoLecturaDocumento != null)
            {
                textoLecturaDocumento.text = "<b>DOCUMENTOS</b>\n\nNo tienes documentos recolectados en este momento.";
            }
            if (textoTituloDocumento != null)
            {
                textoTituloDocumento.text = "";
            }
            if (imagenPreviewDocumento != null)
            {
                if (spriteCarpeta != null)
                {
                    imagenPreviewDocumento.sprite = spriteCarpeta;
                    imagenPreviewDocumento.color = new Color(1f, 1f, 1f, 0.2f);
                }
                else
                {
                    imagenPreviewDocumento.color = Color.clear;
                }
            }
            documentoSeleccionado = null;
            return;
        }

        ItemData primerDoc = null;
        bool docSeleccionadoSigueExistiendo = false;

        foreach (ItemData doc in inventario.listaDocumentos)
        {
            if (doc == null) continue;
            if (primerDoc == null) primerDoc = doc;
            if (doc == documentoSeleccionado) docSeleccionadoSigueExistiendo = true;

            GameObject objCasilla = CrearOInstanciarCasilla();
            CasillaDocumentoUI compCasilla = objCasilla.GetComponent<CasillaDocumentoUI>();
            if (compCasilla == null)
            {
                compCasilla = objCasilla.AddComponent<CasillaDocumentoUI>();
            }

            compCasilla.Configurar(doc, spriteCuadro, spriteCarpeta, SeleccionarDocumento);
            casillasInstanciadas.Add(compCasilla);
        }

        if (docSeleccionadoSigueExistiendo && documentoSeleccionado != null)
        {
            SeleccionarDocumento(documentoSeleccionado);
        }
        else if (primerDoc != null)
        {
            SeleccionarDocumento(primerDoc);
        }
    }

    private GameObject CrearOInstanciarCasilla()
    {
        if (prefabCasillaDocumento != null)
        {
            return Instantiate(prefabCasillaDocumento, contenedorDocumentos);
        }

        // Crear casilla dinámicamente si no hay prefab asignado
        GameObject nuevaCasilla = new GameObject("Casilla_Documento", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        nuevaCasilla.transform.SetParent(contenedorDocumentos, false);

        RectTransform rect = nuevaCasilla.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400f, 90f);

        Image imgCasilla = nuevaCasilla.GetComponent<Image>();
        imgCasilla.sprite = spriteCuadro;
        imgCasilla.type = Image.Type.Sliced;
        imgCasilla.color = Color.white;

        // Sub-objeto Icono
        GameObject objIcono = new GameObject("Icono", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        objIcono.transform.SetParent(nuevaCasilla.transform, false);
        RectTransform rectIcono = objIcono.GetComponent<RectTransform>();
        rectIcono.anchorMin = new Vector2(0f, 0.5f);
        rectIcono.anchorMax = new Vector2(0f, 0.5f);
        rectIcono.pivot = new Vector2(0f, 0.5f);
        rectIcono.anchoredPosition = new Vector2(15f, 0f);
        rectIcono.sizeDelta = new Vector2(60f, 60f);

        Image imgIcono = objIcono.GetComponent<Image>();
        imgIcono.sprite = spriteCarpeta;
        imgIcono.preserveAspect = true;

        // Sub-objeto Texto
        GameObject objTexto = new GameObject("Titulo", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        objTexto.transform.SetParent(nuevaCasilla.transform, false);
        RectTransform rectTexto = objTexto.GetComponent<RectTransform>();
        rectTexto.anchorMin = new Vector2(0f, 0f);
        rectTexto.anchorMax = new Vector2(1f, 1f);
        rectTexto.pivot = new Vector2(0.5f, 0.5f);
        rectTexto.offsetMin = new Vector2(85f, 5f);
        rectTexto.offsetMax = new Vector2(-15f, -5f);

        TextMeshProUGUI tmpTexto = objTexto.GetComponent<TextMeshProUGUI>();
        tmpTexto.fontSize = 20f;
        tmpTexto.color = Color.white;
        tmpTexto.alignment = TextAlignmentOptions.MidlineLeft;
        
        // Nota: enableWordWrapping está obsoleto en versiones nuevas de TMP, 
        // pero lo dejo por compatibilidad. Si te da error, cámbialo a textWrappingMode = TextWrappingModes.Normal;
       tmpTexto.textWrappingMode = TextWrappingModes.Normal;
        CasillaDocumentoUI ui = nuevaCasilla.AddComponent<CasillaDocumentoUI>();
        ui.imagenCasilla = imgCasilla;
        ui.imagenIcono = imgIcono;
        ui.textoTitulo = tmpTexto;
        ui.botonCasilla = nuevaCasilla.GetComponent<Button>();

        return nuevaCasilla;
    }

    public void SeleccionarDocumento(ItemData doc)
    {
        if (doc == null) return;
        documentoSeleccionado = doc;

        // --- ARREGLO BUG #2 ---
        // Ahora comparamos el ItemData real, no el texto del nombre.
        foreach (CasillaDocumentoUI casilla in casillasInstanciadas)
        {
            if (casilla != null)
            {
                casilla.EstablecerSeleccionado(casilla.DatosDocumento == doc);
            }
        }

        // Mostrar texto del documento
        if (textoTituloDocumento != null)
        {
            textoTituloDocumento.text = doc.nombre;
        }

        if (textoLecturaDocumento != null)
        {
            string titulo = string.IsNullOrEmpty(doc.nombre) ? "Documento" : doc.nombre;
            string contenido = !string.IsNullOrEmpty(doc.contenidoDocumento)
                ? doc.contenidoDocumento
                : (!string.IsNullOrEmpty(doc.descripcion) ? doc.descripcion : "(Este documento no contiene texto legible)");

            textoLecturaDocumento.text = $"<b><size=120%>{titulo}</size></b>\n\n{contenido}";
        }

        // Vista previa
        if (imagenPreviewDocumento != null)
        {
            if (doc.icono != null)
            {
                imagenPreviewDocumento.sprite = doc.icono;
                imagenPreviewDocumento.color = Color.white;
            }
            else if (spriteCarpeta != null)
            {
                imagenPreviewDocumento.sprite = spriteCarpeta;
                imagenPreviewDocumento.color = Color.white;
            }
            else
            {
                imagenPreviewDocumento.color = Color.clear;
            }
        }
    }
}