using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGridInventario : MonoBehaviour
{
    [Header("Referencias UI")]
    public RectTransform contenedorObjetos;
    public RectTransform contenedorCasillas;
    public GameObject prefabItemVisual;

    // Desacoplado del Inspector: resuelto automáticamente por Singleton o búsqueda de escena
    [HideInInspector] public Inventario inventario;

    [Header("Recursos Visuales")]
    public Sprite spriteCasilla;
    public Sprite spriteIconoPorDefecto;

    [Header("Ajuste de tamaño y distribución")]
    [Tooltip("Tamaño de cada celda cuadrada (ancho y alto).")]
    public float tamanoCelda = 100f;
    [Tooltip("Espacio entre celdas adyacentes.")]
    public float espaciado = 20f;
    [Tooltip("Centra automáticamente la cuadrícula dentro de su contenedor.")]
    public bool centrarGrid = true;

    private List<GameObject> iconosInstanciados = new List<GameObject>();

    private void Awake()
    {
        if (inventario == null) inventario = Inventario.Instance != null ? Inventario.Instance : FindAnyObjectByType<Inventario>();
        RefrescarOrganizacion();
    }

    private void OnEnable()
    {
        if (inventario == null) inventario = Inventario.Instance != null ? Inventario.Instance : FindAnyObjectByType<Inventario>();
        RefrescarOrganizacion();
        if (inventario != null)
        {
            inventario.alActualizarInventario += RedibujarTodo;
            RedibujarTodo();
        }
    }

    /// <summary>
    /// Sincroniza la cuadrícula y redibuja los objetos. Úsalo cada vez que se muestra el panel.
    /// </summary>
    public void Refrescar()
    {
        RefrescarOrganizacion();
        RedibujarTodo();
    }

    private void RefrescarOrganizacion()
    {
        SincronizarParametrosGrid();
        AjustarDimensionesGrid();
        AjustarCantidadCasillas();
        ActualizarSpritesCasillas();
    }

    public void SincronizarParametrosGrid()
    {
        if (contenedorCasillas != null)
        {
            GridLayoutGroup gridLayout = contenedorCasillas.GetComponent<GridLayoutGroup>();
            if (gridLayout != null)
            {
                if (gridLayout.cellSize.x > 0) tamanoCelda = gridLayout.cellSize.x;
                espaciado = gridLayout.spacing.x;
            }
        }
    }

    /// <summary>
    /// Ajusta la cantidad de columnas, el tamaño del contenedor y la capa de objetos
    /// para que las celdas y los iconos siempre queden alineados.
    /// </summary>
    public void AjustarDimensionesGrid()
    {
        if (contenedorCasillas == null || inventario == null) return;

        int columnas = Mathf.Max(1, inventario.columnasGrid);
        int filas = Mathf.Max(1, inventario.filasGrid);
        float celda = Mathf.Max(1f, tamanoCelda);
        float espacio = Mathf.Max(0f, espaciado);

        GridLayoutGroup gridLayout = contenedorCasillas.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columnas;
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
        }

        float ancho = columnas * celda + Mathf.Max(0, columnas - 1) * espacio;
        float alto = filas * celda + Mathf.Max(0, filas - 1) * espacio;

        if (gridLayout != null)
        {
            ancho += gridLayout.padding.left + gridLayout.padding.right;
            alto += gridLayout.padding.top + gridLayout.padding.bottom;
        }

        Vector2 tamanoGrid = new Vector2(ancho, alto);

        contenedorCasillas.anchorMin = new Vector2(0f, 1f);
        contenedorCasillas.anchorMax = new Vector2(0f, 1f);
        contenedorCasillas.pivot = new Vector2(0f, 1f);
        contenedorCasillas.sizeDelta = tamanoGrid;

        if (contenedorObjetos != null)
        {
            contenedorObjetos.anchorMin = new Vector2(0f, 1f);
            contenedorObjetos.anchorMax = new Vector2(0f, 1f);
            contenedorObjetos.pivot = new Vector2(0f, 1f);
            contenedorObjetos.anchoredPosition = Vector2.zero;
            contenedorObjetos.sizeDelta = tamanoGrid;
            contenedorObjetos.SetAsLastSibling();
        }

        if (centrarGrid)
        {
            CentrarGrid(tamanoGrid);
        }
    }

    private void CentrarGrid(Vector2 tamanoGrid)
    {
        RectTransform padre = contenedorCasillas.parent as RectTransform;
        if (padre == null) return;

        if (padre.rect.width <= 0f || padre.rect.height <= 0f)
        {
            Canvas.ForceUpdateCanvases();
        }

        Vector2 tamanoPadre = padre.rect.size;
        if (tamanoPadre.x <= 0f || tamanoPadre.y <= 0f) return;

        contenedorCasillas.anchoredPosition = new Vector2(
            (tamanoPadre.x - tamanoGrid.x) * 0.5f,
            -(tamanoPadre.y - tamanoGrid.y) * 0.5f);
    }

    public void AjustarCantidadCasillas()
    {
        if (contenedorCasillas == null || inventario == null) return;
        int totalNecesario = Mathf.Max(1, inventario.columnasGrid) * Mathf.Max(1, inventario.filasGrid);

        List<Transform> casillas = new List<Transform>();
        foreach (Transform hijo in contenedorCasillas)
        {
            if (hijo.name.Contains("Capa_Objetos") || (contenedorObjetos != null && hijo == contenedorObjetos)) continue;
            casillas.Add(hijo);
        }

        for (int i = 0; i < casillas.Count; i++)
        {
            casillas[i].gameObject.SetActive(i < totalNecesario);
        }

        // Si faltan casillas para la cuadrícula configurada, clonamos a partir de la primera
        if (casillas.Count > 0 && casillas.Count < totalNecesario)
        {
            GameObject plantilla = casillas[0].gameObject;
            for (int i = casillas.Count; i < totalNecesario; i++)
            {
                GameObject nueva = Instantiate(plantilla, contenedorCasillas);
                nueva.name = $"Casilla_Muestra ({i})";
                nueva.SetActive(true);
            }
        }

        if (contenedorObjetos != null)
        {
            contenedorObjetos.SetAsLastSibling();
        }
    }

    public void ActualizarSpritesCasillas()
    {
        if (spriteCasilla == null) return;
        Transform contenedor = contenedorCasillas != null ? contenedorCasillas.transform : transform.Find("Contenedor_Grid");
        if (contenedor == null) return;

        foreach (Transform hijo in contenedor)
        {
            if (hijo.name.Contains("Capa_Objetos") || (contenedorObjetos != null && hijo == contenedorObjetos)) continue;
            Image img = hijo.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = spriteCasilla;
                img.color = Color.white;
            }
        }
    }

    private void OnDisable()
    {
        if (inventario != null) inventario.alActualizarInventario -= RedibujarTodo;
    }

    public void RedibujarTodo()
    {
        foreach (GameObject obj in iconosInstanciados) Destroy(obj);
        iconosInstanciados.Clear();

        if (inventario == null || contenedorObjetos == null || prefabItemVisual == null) return;

        SincronizarParametrosGrid();
        AjustarDimensionesGrid();

        foreach (ObjetoEnGrid item in inventario.objetosGenerales)
        {
            GameObject nuevoIcono = Instantiate(prefabItemVisual, contenedorObjetos);
            ItemVisualUI scriptItem = nuevoIcono.GetComponent<ItemVisualUI>();
            if (scriptItem == null) continue;

            scriptItem.Configurar(item, this);
            PosicionarObjetoVisual(nuevoIcono.GetComponent<RectTransform>(), item);
            iconosInstanciados.Add(nuevoIcono);
        }
    }

    public void PosicionarObjetoVisual(RectTransform rectObjeto, ObjetoEnGrid item)
    {
        // Forzamos ancla y pivote arriba-izquierda para que los iconos coincidan con las celdas.
        rectObjeto.anchorMin = new Vector2(0, 1);
        rectObjeto.anchorMax = new Vector2(0, 1);
        rectObjeto.pivot = new Vector2(0, 1);

        float paso = Mathf.Max(1f, Mathf.Max(1f, tamanoCelda) + Mathf.Max(0f, espaciado));
        Vector2 padding = ObtenerPaddingGrid();
        float posX = padding.x + item.x * paso;
        float posY = -(padding.y + item.y * paso);
        rectObjeto.anchoredPosition = new Vector2(posX, posY);
    }

    public Vector2Int ObtenerCoordenadaCelda(Vector2 posicionAnclada)
    {
        float paso = Mathf.Max(1f, Mathf.Max(1f, tamanoCelda) + Mathf.Max(0f, espaciado));

        Vector2 padding = ObtenerPaddingGrid();
        int celdaX = Mathf.RoundToInt((posicionAnclada.x - padding.x) / paso);
        int celdaY = Mathf.RoundToInt((-posicionAnclada.y - padding.y) / paso);

        if (inventario != null)
        {
            celdaX = Mathf.Clamp(celdaX, 0, Mathf.Max(0, inventario.columnasGrid - 1));
            celdaY = Mathf.Clamp(celdaY, 0, Mathf.Max(0, inventario.filasGrid - 1));
        }

        return new Vector2Int(celdaX, celdaY);
    }

    private Vector2 ObtenerPaddingGrid()
    {
        if (contenedorCasillas == null) return Vector2.zero;

        GridLayoutGroup gridLayout = contenedorCasillas.GetComponent<GridLayoutGroup>();
        if (gridLayout == null) return Vector2.zero;

        return new Vector2(gridLayout.padding.left, gridLayout.padding.top);
    }
}