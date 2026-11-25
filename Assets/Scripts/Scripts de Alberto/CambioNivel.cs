using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioNivel : MonoBehaviour
{
    [Header("Configuración de Destino")]
    [Tooltip("Escribe el nombre exacto de la escena a cargar (ej. Nivel2)")]
    public string nombreSiguienteNivel;

    [Tooltip("Si es verdadero, cargará automáticamente la siguiente escena en la lista de Build Settings")]
    public bool cargarSiguienteEnLista = true;

    [Header("Activación por Zona (Trigger)")]
    [Tooltip("Marca esto si quieres que cambie de nivel apenas el jugador entre en el collider")]
    public bool activarAlEntrar = true;

    private void OnTriggerEnter(Collider other)
    {
        if (activarAlEntrar && other.CompareTag("Player"))
        {
            CargarNivel();
        }
    }

    // Puedes llamar a esta función pública desde un evento (como PuertaConLlave o PuzzleSecuencia)
    public void CargarNivel()
    {
        Time.timeScale = 1f; // Asegura que el tiempo esté normal al entrar al nuevo nivel

        if (cargarSiguienteEnLista)
        {
            int indiceSiguiente = SceneManager.GetActiveScene().buildIndex + 1;

            if (indiceSiguiente < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(indiceSiguiente);
            }
            else
            {
                Debug.LogWarning("No hay más niveles en Build Settings. Regresando al Menú Principal...");
                SceneManager.LoadScene(0);
            }
        }
        else if (!string.IsNullOrEmpty(nombreSiguienteNivel))
        {
            SceneManager.LoadScene(nombreSiguienteNivel);
        }
    }
}
