using System.Collections.Generic;
using UnityEngine;

public class UIGridInventario : MonoBehaviour
{
    [Header("Referencias")]
    public Inventario inventario;
    public RectTransform contenedorObjetos;
    public GameObject prefabItemVisual;

    [Header("Ajuste de tamaño")]
    public float tamanoCelda = 80f;

    private List<GameObject> iconosInstanciados = new List<GameObject>();

    private void OnEnable()
    {
        if (inventario != null)
        {
            inventario.alActualizarInventario += RedibujarTodo;
            RedibujarTodo();
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
