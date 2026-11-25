using UnityEngine;
using UnityEngine.Events;

public class GestorAnomalia : MonoBehaviour
{
    public static GestorAnomalia Instance { get; private set; }
    
    [Header("Configuración")]
    public ItemData primerDocumentoAdvertencia;
    public bool anomaliaActiva = false;

    [Header("Eventos")]
    public UnityEvent onAnomaliaActivada;

    private void Awake() => Instance = this;

    // Llamar a este método desde el Inventario justo después de recoger un ítem
    public void VerificarDocumento(ItemData itemRecogido)
    {
        if (!anomaliaActiva && itemRecogido == primerDocumentoAdvertencia)
        {
            anomaliaActiva = true;
            Debug.Log("¡La anomalía se ha activado!");
            onAnomaliaActivada.Invoke(); // Aquí conectas el script de la IA perseguidora
        }
    }
}