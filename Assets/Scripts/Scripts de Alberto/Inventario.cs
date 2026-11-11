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
}
