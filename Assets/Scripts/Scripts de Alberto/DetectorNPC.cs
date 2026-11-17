using UnityEngine;

public class DetectorNPC : MonoBehaviour
{
    [Header("Configuración de detección")]
    [Tooltip("Distancia máxima a la que el detector percibe al NPC")]
    public float radioDeteccion = 0.02f;
    [Tooltip("Capa (Layer) asignada a NPCs para optimizar el rendimiento")]
    public LayerMask capaNPC;

    [Header("Configuración de audio")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoPitido;
    [Tooltip("Tiempo entre pitidos cuando el NPC está lejos")]
    public float intervaloMaximo = 1.5f;
    [Tooltip("Tiempo entre pitidos cuando el NPC está muy cerca")]
    public float intervaloMinimo = 0.1f;

    private float temporizador;

    void Update()
    {
        float distanciaMasCercana = ObtenerDistanciaNPCMasCercano();

        if (distanciaMasCercana <= radioDeteccion)
        {
            float intensidad = 1f - (distanciaMasCercana / radioDeteccion);

            float intervaloActual = Mathf.Lerp(intervaloMaximo, intervaloMinimo, intensidad);

            temporizador += Time.deltaTime;
            if (temporizador >= intervaloActual)
            {
                ReproducirReaccion(intensidad);
                temporizador = 0f;
            }
        } else
        {
            temporizador = intervaloMaximo;
        }
    }

    float ObtenerDistanciaNPCMasCercano()
    {
        Collider[] npcsDetectados = Physics.OverlapSphere(transform.position, radioDeteccion, capaNPC);
        float distanciaMinima = Mathf.Infinity;

        foreach (Collider npc in npcsDetectados)
        {
            float distancia = Vector3.Distance(transform.position, npc.transform.position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
            }
        }

        return distanciaMinima;
    }

    void ReproducirReaccion(float intensidad)
    {
        if (fuenteAudio != null && sonidoPitido != null)
        {
            fuenteAudio.pitch = Mathf.Lerp(0.8f, 1.5f, intensidad);
            fuenteAudio.PlayOneShot(sonidoPitido);
        }

        Debug.Log($"NPC detectado. Intensidad: {intensidad * 100:F0}%");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}
