using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ItemVisualUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image imagenIcono;

    private ObjetoEnGrid itemAsignado;
    private UIGridInventario gestorGrid;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvasPadre;

    private int origenX, origenY;
    private bool origenRotado;
    private bool arrastrando = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // ARREGLO BUG 4: Evitar duplicar el CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasPadre = GetComponentInParent<Canvas>();

        // Forzar pivot y ancla en esquina superior-izquierda para alinearse con el grid
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
    }

    public void Configurar(ObjetoEnGrid item, UIGridInventario gestor)
    {
        itemAsignado = item;
        gestorGrid = gestor;

        if (item.datos != null && item.datos.icono != null)
        {
            imagenIcono.sprite = item.datos.icono;
            imagenIcono.color = Color.white;
        }
        else if (gestor != null && gestor.spriteIconoPorDefecto != null)
        {
            imagenIcono.sprite = gestor.spriteIconoPorDefecto;
            imagenIcono.color = Color.white;
        }
        else
        {
            imagenIcono.color = new Color(1f, 1f, 1f, 0.4f);
        }

        ActualizarFormaVisual();
    }

    public void ActualizarFormaVisual()
    {
        if (itemAsignado == null || itemAsignado.datos == null || gestorGrid == null) return;

        float tamCelda = gestorGrid.tamanoCelda;
        float esp = gestorGrid.espaciado;

        // Tamaño del área ocupada (si está rotado, ancho y alto se intercambian)
        float anchoTotal = itemAsignado.AnchoActual * tamCelda + (itemAsignado.AnchoActual - 1) * esp;
        float altoTotal = itemAsignado.AltoActual * tamCelda + (itemAsignado.AltoActual - 1) * esp;
        rectTransform.sizeDelta = new Vector2(anchoTotal, altoTotal);

        RectTransform rectImagen = imagenIcono.GetComponent<RectTransform>();

        // La imagen conserva el tamaño original del objeto y luego se gira.
        // Así, al rotarla 90°, ocupa exactamente el área visual del objeto.
        float anchoBase = itemAsignado.datos.ancho * tamCelda + (itemAsignado.datos.ancho - 1) * esp;
        float altoBase = itemAsignado.datos.alto * tamCelda + (itemAsignado.datos.alto - 1) * esp;
        rectImagen.anchorMin = new Vector2(0.5f, 0.5f);
        rectImagen.anchorMax = new Vector2(0.5f, 0.5f);
        rectImagen.pivot = new Vector2(0.5f, 0.5f);
        rectImagen.anchoredPosition = Vector2.zero;
        rectImagen.sizeDelta = new Vector2(anchoBase, altoBase);

        // Aplicamos rotación visual
        rectImagen.localEulerAngles = new Vector3(0, 0, itemAsignado.rotado ? -90f : 0f);
    }

    private void Update()
    {
        if (arrastrando && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            itemAsignado.rotado = !itemAsignado.rotado;
            ActualizarFormaVisual();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        arrastrando = true;
        origenX = itemAsignado.x;
        origenY = itemAsignado.y;
        origenRotado = itemAsignado.rotado;

        gestorGrid.inventario.LiberarCeldas(itemAsignado);

        transform.SetAsLastSibling();
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float factorEscala = (canvasPadre != null) ? canvasPadre.scaleFactor : 1f;
        rectTransform.anchoredPosition += eventData.delta / factorEscala;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        arrastrando = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (gestorGrid == null || gestorGrid.inventario == null || itemAsignado == null) return;

        Vector2Int celdaDestino = gestorGrid.ObtenerCoordenadaCelda(rectTransform.anchoredPosition);

        // Ajustamos al último hueco válido para que el objeto no salte al borde y se pierda el drop.
        celdaDestino.x = Mathf.Clamp(celdaDestino.x, 0, Mathf.Max(0, gestorGrid.inventario.columnasGrid - itemAsignado.AnchoActual));
        celdaDestino.y = Mathf.Clamp(celdaDestino.y, 0, Mathf.Max(0, gestorGrid.inventario.filasGrid - itemAsignado.AltoActual));

        if (gestorGrid.inventario.PuedeCaber(celdaDestino.x, celdaDestino.y, itemAsignado.AnchoActual, itemAsignado.AltoActual))
        {
            itemAsignado.x = celdaDestino.x;
            itemAsignado.y = celdaDestino.y;
        } else
        {
            itemAsignado.x = origenX;
            itemAsignado.y = origenY;
            itemAsignado.rotado = origenRotado;
            ActualizarFormaVisual();
        }

        gestorGrid.inventario.OcuparCeldas(itemAsignado);
        gestorGrid.PosicionarObjetoVisual(rectTransform, itemAsignado);
    }
}