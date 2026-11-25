using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.InputSystem;

public class KeypadSistema : MonoBehaviour
{
    [Header("Configuración de Contraseña")]
    public string codigoCorrecto = "1984"; // Cambia aquí la clave secreta
    public TextMeshProUGUI pantallaNumeros; // Texto donde se ven los números marcados
    public GameObject panelKeypadUI;        // El panel entero del keypad en el Canvas

    [Header("Audio (Opcional)")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoTecla;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoError;

    [Header("Eventos al Acertar")]
    public UnityEvent onCodigoCorrecto; // Aquí conectaremos la puerta y al segundo NPC

    private string entradaActual = "";
    private bool desbloqueado = false;

    private void Start()
    {
        if (panelKeypadUI != null) panelKeypadUI.SetActive(false);
    }

    private void Update()
    {
        // Permitir cerrar el keypad con Escape
        if (panelKeypadUI != null && panelKeypadUI.activeSelf)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CerrarKeypad();
            }
        }
    }

    // Esta función la llamará tu ObjetoInteractivo al presionar 'E'
    public void AbrirKeypad()
    {
        if (desbloqueado) return;

        entradaActual = "";
        ActualizarPantalla();

        if (panelKeypadUI != null) panelKeypadUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarKeypad()
    {
        if (panelKeypadUI != null) panelKeypadUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Conectar a los botones 0-9 enviando el número como texto ("1", "2", etc.)
    public void PresionarNumero(string numero)
    {
        if (desbloqueado || entradaActual.Length >= codigoCorrecto.Length) return;

        if (fuenteAudio != null && sonidoTecla != null) fuenteAudio.PlayOneShot(sonidoTecla);

        entradaActual += numero;
        ActualizarPantalla();
    }

    // Conectar al botón de Borrar (C / Clear)
    public void BorrarEntrada()
    {
        if (desbloqueado) return;
        if (fuenteAudio != null && sonidoTecla != null) fuenteAudio.PlayOneShot(sonidoTecla);

        entradaActual = "";
        ActualizarPantalla();
    }

    // Conectar al botón de Confirmar (OK / Enter)
    public void VerificarCodigo()
    {
        if (desbloqueado) return;

        if (entradaActual == codigoCorrecto)
        {
            desbloqueado = true;
            if (pantallaNumeros != null) pantallaNumeros.text = "OK";
            if (fuenteAudio != null && sonidoCorrecto != null) fuenteAudio.PlayOneShot(sonidoCorrecto);

            StartCoroutine(CerrarConRetraso());
        }
        else
        {
            if (fuenteAudio != null && sonidoError != null) fuenteAudio.PlayOneShot(sonidoError);
            entradaActual = "";
            if (pantallaNumeros != null) pantallaNumeros.text = "ERROR";
        }
    }

    private System.Collections.IEnumerator CerrarConRetraso()
    {
        // Usamos WaitForSecondsRealtime porque Time.timeScale está en 0
        yield return new WaitForSecondsRealtime(0.5f);
        CerrarKeypad();
        onCodigoCorrecto?.Invoke();
    }

    private void ActualizarPantalla()
    {
        if (pantallaNumeros != null) pantallaNumeros.text = entradaActual;
    }
}
