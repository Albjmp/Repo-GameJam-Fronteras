using UnityEngine;
using UnityEngine.UI;

public class GeneradorCeldasGrid : MonoBehaviour
{
    [Header("Configuración de la Cuadrícula")]
    public int columnas = 10;
    public int filas = 6;
    public float tamanoCelda = 80f;
    
    [Header("Visuales")]
    public Sprite spriteCelda; // Arrastra aquí una imagen de un cuadro (opcional)
    public RectTransform contenedorCeldas; // El objeto donde se crearán las celdas

    [ContextMenu("Generar Celdas Visuales")]
    public void GenerarCeldas()
    {
        if (contenedorCeldas == null)
        {
            Debug.LogError("¡Asigna el Contenedor de Celdas en el Inspector!");
            return;
        }

        // Limpiar celdas anteriores (por si acaso)
        foreach (Transform hijo in contenedorCeldas)
        {
            DestroyImmediate(hijo.gameObject);
        }

        // Crear la cuadrícula
        for (int y = 0; y < filas; y++)
        {
            for (int x = 0; x < columnas; x++)
            {
                GameObject celda = new GameObject($"Celda_{x}_{y}");
                celda.transform.SetParent(contenedorCeldas, false);

                RectTransform rect = celda.AddComponent<RectTransform>();
                
                // Anclar arriba a la izquierda
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                
                // Tamaño y posición exacta
                rect.sizeDelta = new Vector2(tamanoCelda, tamanoCelda);
                rect.anchoredPosition = new Vector2(x * tamanoCelda, -y * tamanoCelda);

                // Añadir la imagen de fondo
                Image img = celda.AddComponent<Image>();
                if (spriteCelda != null) img.sprite = spriteCelda;
                
                // Color gris semitransparente para que se vea la cuadrícula
                img.color = new Color(1f, 1f, 1f, 0.1f); 
            }
        }
        Debug.Log("¡Cuadrícula generada con éxito!");
    }
}