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
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasPadre = GetComponentInParent<Canvas>();
    }

    public void Configurar(ObjetoEnGrid item, UIGridInventario gestor)
    {
        itemAsignado = item;
        gestorGrid = gestor;
        imagenIcono.sprite = item.datos.icono;
        ActualizarFormaVisual();
    }

    public void ActualizarFormaVisual()
    {
        float tamCelda = gestorGrid.tamanoCelda;

        rectTransform.sizeDelta = new Vector2(itemAsignado.AnchoActual * tamCelda, itemAsignado.AltoActual * tamCelda);

        RectTransform rectImagen = imagenIcono.GetComponent<RectTransform>();
        rectImagen.sizeDelta = new Vector2(itemAsignado.datos.ancho * tamCelda, itemAsignado.datos.alto * tamCelda);
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

        Vector2Int celdaDestino = gestorGrid.ObtenerCoordenadaCelda(rectTransform.anchoredPosition);

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
