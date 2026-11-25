using UnityEngine;
using UnityEngine.UI;

public class AlertaPeligro : MonoBehaviour
{
    [Header("Referencias")]
    public Transform anomalia; // El enemigo
    public Image bordeRojoUI; // Una imagen roja que cubra los bordes de la pantalla
    public AudioClip sonidoLatido; // ¡Aquí arrastras tu archivo .mp3 o .wav!

    [Header("Configuración")]
    public float distanciaMaxima = 10f;
    public float velocidadParpadeo = 5f;
    public float volumenLatido = 0.5f;

    private AudioSource audioSource;
    private bool reproduciendoLatido = false;

    void Start()
    {
        // Creamos el reproductor de audio automáticamente
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sonidoLatido;
        audioSource.loop = true;
        audioSource.volume = volumenLatido;
    }

    void Update()
    {
        if (anomalia == null) return;

        float distancia = Vector3.Distance(transform.position, anomalia.position);
        bool peligro = distancia <= distanciaMaxima;

        // Sonido
        if (peligro && !reproduciendoLatido && sonidoLatido != null)
        {
            audioSource.Play();
            reproduciendoLatido = true;
        }
        else if (!peligro && reproduciendoLatido)
        {
            audioSource.Stop();
            reproduciendoLatido = false;
        }

        // Parpadeo visual
        if (bordeRojoUI != null)
        {
            if (peligro)
            {
                float alpha = Mathf.PingPong(Time.time * velocidadParpadeo, 1f);
                Color color = bordeRojoUI.color;
                color.a = alpha;
                bordeRojoUI.color = color;
            }
            else
            {
                bordeRojoUI.color = new Color(1, 0, 0, 0);
            }
        }
    }
}