using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGridInventario : MonoBehaviour
{
    [Header("Referencias")]
    public Inventario inventario;
    public RectTransform contenedorObjetos;
    public RectTransform contenedorCasillas;
    public GameObject prefabItemVisual;

    [Header("Recursos Visuales")]
    public Sprite spriteCasilla;
    public Sprite spriteIconoPorDefecto;

    [Header("Ajuste de tamaño")]
    public float tamanoCelda = 80f;

    private List<GameObject> iconosInstanciados = new List<GameObject>();

    private void Awake()
    {
        if (inventario == null)
        {
            inventario = FindAnyObjectByType<Inventario>();
        }

        ActualizarSpritesCasillas();
    }

    private void OnEnable()
    {
        if (inventario == null)
        {
            inventario = FindAnyObjectByType<Inventario>();
        }

        if (inventario != null)
        {
            inventario.alActualizarInventario += RedibujarTodo;
            RedibujarTodo();
        }

        ActualizarSpritesCasillas();
    }

    public void ActualizarSpritesCasillas()
    {
        if (spriteCasilla == null) return;

        Transform contenedor = contenedorCasillas != null ? contenedorCasillas.transform : transform.Find("Contenedor_Grid");
        if (contenedor == null) return;

        foreach (Transform hijo in contenedor)
        {
            // Ignoramos la capa de objetos
            if (hijo.name.Contains("Capa_Objetos")) continue;

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
        if (inventario != null)
        {
            inventario.alActualizarInventario -= RedibujarTodo;
        }
    }

    public void RedibujarTodo()
    {
        foreach (GameObject obj in iconosInstanciados)
        {
            Destroy(obj);
        }
        iconosInstanciados.Clear();

        if (inventario == null) return;

        foreach (ObjetoEnGrid item in inventario.objetosGenerales)
        {
            GameObject nuevoIcono = Instantiate(prefabItemVisual, contenedorObjetos);
            ItemVisualUI scriptItem = nuevoIcono.GetComponent<ItemVisualUI>();

            scriptItem.Configurar(item, this);
            PosicionarObjetoVisual(nuevoIcono.GetComponent<RectTransform>(), item);

            iconosInstanciados.Add(nuevoIcono);
        }
    }

    public void PosicionarObjetoVisual(RectTransform rectObjeto, ObjetoEnGrid item)
    {
        float posX = item.x * tamanoCelda;
        float posY = -item.y * tamanoCelda;
        rectObjeto.anchoredPosition = new Vector2(posX, posY);
    }

    public Vector2Int ObtenerCoordenadaCelda(Vector2 posicionAnclada)
    {
        //Vector2 puntoLocal;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(contenedorObjetos, posicionMundial, null, out puntoLocal);

        //int celdaX = Mathf.RoundToInt(puntoLocal.x / tamanoCelda);
        //int celdaY = Mathf.RoundToInt(-puntoLocal.y / tamanoCelda);

        int celdaX = Mathf.RoundToInt(posicionAnclada.x / tamanoCelda);
        int celdaY = Mathf.RoundToInt(-posicionAnclada.y / tamanoCelda);

        return new Vector2Int(celdaX, celdaY);
    }
}
