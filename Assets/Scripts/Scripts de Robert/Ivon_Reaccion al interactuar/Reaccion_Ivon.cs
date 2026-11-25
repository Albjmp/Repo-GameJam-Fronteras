using UnityEngine;
using TMPro;
using System.Collections;

public class ReaccionIvo : MonoBehaviour
{
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    
    [Header("Configuración")]
    public float tiempoEnPantalla = 4f;

    public void MostrarReaccion(string mensaje)
    {
        StartCoroutine(RutinaDialogo(mensaje));
    }

    private IEnumerator RutinaDialogo(string mensaje)
    {
        panelDialogo.SetActive(true);
        textoDialogo.text = mensaje;
        
        // No pausamos el tiempo, solo mostramos el texto flotante
        yield return new WaitForSeconds(tiempoEnPantalla);
        
        panelDialogo.SetActive(false);
    }
}