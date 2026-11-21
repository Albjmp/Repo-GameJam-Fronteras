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
    public List<ItemData> listaDocumentos = new List<ItemData>();
    public List<ObjetoEnGrid> objetosGenerales = new List<ObjetoEnGrid>();

    [Header("Configuración Grid General")]
    public int columnasGrid = 10;
    public int filasGrid = 6;

    private ItemData[,] gridInventario;

    public System.Action alActualizarInventario;
    
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
        Debug.Log("No hay espacio suficiente en el inventario general");
        return false;
    }

    public bool PuedeCaber(int inicioX, int inicioY, int ancho, int alto)
    {
        if (inicioX < 0 || inicioY < 0 || inicioX + ancho > columnasGrid || inicioY + alto > filasGrid)
        {
            return false;
        }

        for (int x = inicioX; x < inicioX + ancho; x++)
        {
            for (int y = inicioY; y < inicioY + alto; y++)
            {
                if (gridInventario[x, y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    //private void AsignarEspacioGrid(ItemData objeto, int inicioX, int inicioY)
    //{
        //for (int x = inicioX; x < inicioX + objeto.ancho; x++)
        //{
            //for (int y = inicioY; y < inicioY + objeto.alto; y++)
            //{
                //gridInventario[x, y] = objeto;
            //}
        //}
        //Debug.Log($"Objeto '{objeto.nombre}' organizado en el grid en la posición ({inicioX}, {inicioY})");
    //}

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

    public bool TieneObjeto(ItemData objetoBuscado)
    {
        return objetosGenerales.Exists(item => item.datos == objetoBuscado);
        
        //for (int x = 0; x < columnasGrid; x++)
        //{
            //for (int y = 0; y < filasGrid; y++)
            //{
                //if (gridInventario[x, y] == objetoBuscado)
                //{
                    //return true;
                //}
            //}
        //}
        //return false;
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
        
        
        //for (int x = 0; x < columnasGrid; x++)
        //{
            //for (int y = 0; y < filasGrid; y++)
            //{
               // if (gridInventario[x, y] == objetoAEliminar)
                //{
                //    gridInventario[x, y] = null;
                //}
           // }
       // }
    }
}
