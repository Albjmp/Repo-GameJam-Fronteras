using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class SistemaVidaJugador : MonoBehaviour
{
    [Header("Confugiración de vida")]
    public int golpesMaximos = 5;
    private int golpesRecibidos = 0;

    [Header("Invulnerabilidad tras golpe")]
    public float tiempoInvulnerable = 1.5f;
    private float temporizadorInvulnerable = 0f;

    [Header("Interfaz de usuario (6 imágenes)")]
    public Image imagenVidaUI;

    [Header("Audio")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoDano;

    [Tooltip("Asigna 6 sprites en orden: [0] = 0 vidas, [1] = 1 vida, [2] = 2 vidas, [3] = 3 vidas, [4] = 4 vidas, [5] = Vida completa")]
    public Sprite[] spritesDeVida = new Sprite[6];


    private void Start()
    {
        golpesRecibidos = 0;
        ActualizarImagenVida();
    }

    private void Update()
    {
        if (temporizadorInvulnerable > 0f)
        {
            temporizadorInvulnerable -= Time.deltaTime;
        }

        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            temporizadorInvulnerable = 0f;
            RecibirGolpe();
        }
    }

    public void RecibirGolpe()
    {
        if (temporizadorInvulnerable > 0f) return;

        golpesRecibidos++;
        temporizadorInvulnerable = tiempoInvulnerable;

        ActualizarImagenVida();

        if (fuenteAudio != null && sonidoDano != null)
        {
            fuenteAudio.PlayOneShot(sonidoDano);
        }

        Debug.Log($"¡Jugador golpeado! Golpes: {golpesRecibidos} / {golpesMaximos}");

        if (golpesRecibidos >= golpesMaximos)
        {
            ReiniciarNivel();
        }
    }

    private void ActualizarImagenVida()
    {
        if (imagenVidaUI == null || spritesDeVida.Length < 6) return;

        int vidaRestante = Mathf.Clamp(golpesMaximos - golpesRecibidos, 0, 5);

        if (spritesDeVida[vidaRestante] != null)
        {
            imagenVidaUI.sprite = spritesDeVida[vidaRestante];
        }
    }

    private void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
