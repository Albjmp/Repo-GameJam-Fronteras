using System.Collections.Generic;
using UnityEngine;

public class Inventario : MonoBehaviour
{
    public List<ItemData> listaDocumentos = new List<ItemData>();

    [Header("Configuración Grid General")]
    public int columnasGrid = 10;
    public int filasGrid = 6;

    private ItemData[,] gridInventario;

    private void Awake()
    {
        gridInventario = new ItemData[columnasGrid, filasGrid];
    }

    public bool RecogerObjeto(ItemData nuevoObjeto)
    {
        if (nuevoObjeto.tipoObjeto == ItemType.Documento)
        {
            listaDocumentos.Add(nuevoObjeto);
            Debug.Log($"Documento '{nuevoObjeto.nombre}' guardado sin límites"); 
            return true;
        } else
        {
            return IntentarInsertarEnGrid(nuevoObjeto);
        }
    }

    private bool IntentarInsertarEnGrid(ItemData objeto)
    {
        for (int y = 0; y < filasGrid; y++)
        {
            for (int x = 0; x < columnasGrid; x++)
            {
                if (PuedeCaber(objeto, x, y))
                {
                    AsignarEspacioGrid(objeto, x, y);
                    return true;
                }
            }
        }

        Debug.Log("No hay espacio suficiente en el inventario general");
        return false;
    }

    private bool PuedeCaber(ItemData objeto, int inicioX, int inicioY)
    {
        if (inicioX + objeto.ancho > columnasGrid || inicioY + objeto.alto > filasGrid)
        {
            return false;
        }

        for (int x = inicioX; x < inicioX + objeto.ancho; x++)
        {
            for (int y = inicioY; y < inicioY + objeto.alto; y++)
            {
                if (gridInventario[x, y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void AsignarEspacioGrid(ItemData objeto, int inicioX, int inicioY)
    {
        for (int x = inicioX; x < inicioX + objeto.ancho; x++)
        {
            for (int y = inicioY; y < inicioY + objeto.alto; y++)
            {
                gridInventario[x, y] = objeto;
            }
        }
        Debug.Log($"Objeto '{objeto.nombre}' organizado en el grid en la posición ({inicioX}, {inicioY})");
    }

    /// <summary>
    /// Elimina o descarta un objeto del inventario (tanto si es Documento como si está en el Grid).
    /// </summary>
    public bool DescartarObjeto(ItemData objeto)
    {
        if (objeto == null) return false;

        if (objeto.tipoObjeto == ItemType.Documento)
        {
            if (listaDocumentos.Remove(objeto))
            {
                Debug.Log($"Documento '{objeto.nombre}' descartado del inventario.");
                return true;
            }
            return false;
        }
        else
        {
            bool encontrado = false;
            for (int y = 0; y < filasGrid; y++)
            {
                for (int x = 0; x < columnasGrid; x++)
                {
                    if (gridInventario[x, y] == objeto)
                    {
                        gridInventario[x, y] = null;
                        encontrado = true;
                    }
                }
            }

            if (encontrado)
            {
                Debug.Log($"Objeto '{objeto.nombre}' descartado del grid del inventario.");
            }
            return encontrado;
        }
    }

    /// <summary>
    /// Elimina o descarta el objeto que ocupe una celda específica del grid.
    /// </summary>
    public bool DescartarObjetoEnPosicion(int x, int y)
    {
        if (x < 0 || x >= columnasGrid || y < 0 || y >= filasGrid) return false;

        ItemData obj = gridInventario[x, y];
        if (obj == null) return false;

        return DescartarObjeto(obj);
    }
}
