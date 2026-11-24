using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjetoEnGrid
{
    public ItemData datos;
    public int x;
    public int y;
    public bool rotado;

    public int AnchoActual => rotado ? datos.alto : datos.ancho;
    public int AltoActual => rotado ? datos.ancho : datos.alto;
}

public class Inventario : MonoBehaviour
{
    public static Inventario Instance { get; private set; }
    public static Inventario Instancia => Instance;

    [Header("Configuración Grid General")]
    [Tooltip("Cantidad de columnas disponibles en el grid de objetos.")]
    public int columnasGrid = 5;
    [Tooltip("Cantidad de filas disponibles en el grid de objetos.")]
    public int filasGrid = 5;

    [Header("Contenido del Inventario (Tiempo de Ejecución)")]
    [Tooltip("Documentos recolectados.")]
    public List<ItemData> listaDocumentos = new List<ItemData>();
    [Tooltip("Objetos colocados actualmente en el grid.")]
    public List<ObjetoEnGrid> objetosGenerales = new List<ObjetoEnGrid>();

    private ItemData[,] gridInventario;

    public System.Action alActualizarInventario;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[Inventario] Se encontró otra instancia de Inventario. Destruyendo la duplicada en " + gameObject.name);
            Destroy(this);
            return;
        }
        Instance = this;

        gridInventario = new ItemData[columnasGrid, filasGrid];
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool RecogerObjeto(ItemData nuevoObjeto)
    {
        if (nuevoObjeto.tipoObjeto == ItemType.Documento)
        {
            listaDocumentos.Add(nuevoObjeto);
            alActualizarInventario?.Invoke();
            return true;
        } else
        {
            return IntentarInsertarEnGrid(nuevoObjeto);
        }
    }

    private bool IntentarInsertarEnGrid(ItemData objeto)
    {
        bool[] orientaciones = { false, true };

        foreach (bool rot in orientaciones)
        {
            int w = rot ? objeto.alto : objeto.ancho;
            int h = rot ? objeto.ancho : objeto.alto;

            for (int y = 0; y < filasGrid; y++)
            {
                for (int x = 0; x < columnasGrid; x++)
                {
                    if (PuedeCaber(x, y, w, h))
                    {
                        ObjetoEnGrid nuevoItem = new ObjetoEnGrid
                        {
                            datos = objeto,
                            x = x,
                            y = y,
                            rotado = rot
                        };
                        objetosGenerales.Add(nuevoItem);
                        OcuparCeldas(nuevoItem);
                        alActualizarInventario?.Invoke();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool PuedeCaber(int inicioX, int inicioY, int ancho, int alto)
    {
        if (inicioX < 0 || inicioY < 0 || inicioX + ancho > columnasGrid || inicioY + alto > filasGrid)
            return false;

        for (int x = inicioX; x < inicioX + ancho; x++)
        {
            for (int y = inicioY; y < inicioY + alto; y++)
            {
                if (gridInventario[x, y] != null) return false;
            }
        }
        return true;
    }

    public void OcuparCeldas(ObjetoEnGrid item)
    {
        for (int x = item.x; x < item.x + item.AnchoActual; x++)
        {
            for (int y = item.y; y < item.y + item.AltoActual; y++)
            {
                gridInventario[x, y] = item.datos;
            }
        }
    }

    public void LiberarCeldas(ObjetoEnGrid item)
    {
        for (int x = item.x; x < item.x + item.AnchoActual; x++)
        {
            for (int y = item.y; y < item.y + item.AltoActual; y++)
            {
                gridInventario[x, y] = null;
            }
        }
    }

    public bool DescartarObjeto(ItemData objeto)
    {
        if (objeto == null) return false;

        if (objeto.tipoObjeto == ItemType.Documento)
        {
            if (listaDocumentos.Remove(objeto))
            {
                alActualizarInventario?.Invoke(); // Refrescar UI
                return true;
            }
            return false;
        }
        else
        {
            // ARREGLO BUG 1: Buscar el objeto en la lista real, liberar celdas y quitarlo
            ObjetoEnGrid objetoEncontrado = objetosGenerales.Find(item => item.datos == objeto);
            if (objetoEncontrado != null)
            {
                LiberarCeldas(objetoEncontrado);
                objetosGenerales.Remove(objetoEncontrado);
                alActualizarInventario?.Invoke(); // Refrescar UI
                return true;
            }
            return false;
        }
    }

    public bool TieneObjeto(ItemData objetoBuscado)
    {
        return objetosGenerales.Exists(item => item.datos == objetoBuscado);
    }

    public void EliminarObjeto(ItemData objetoAEliminar)
    {
        ObjetoEnGrid encontrado = objetosGenerales.Find(item => item.datos == objetoAEliminar);
        if (encontrado != null)
        {
            LiberarCeldas(encontrado);
            objetosGenerales.Remove(encontrado);
            alActualizarInventario?.Invoke();
        }
    }
}