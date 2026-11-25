using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteraccionComputadora : MonoBehaviour
{
    [Header("Referencias del Keypad")]
    public KeypadSistema keypadSistema;

    [Header("Interfaz de aviso (Opcional)")]
    public TextMeshProUGUI textoInteractuarUI;
    public string mensajeAviso = "Presiona [E] para usar la computadora";

    private bool jugadorCerca = false;

    private void Update()
    {
        if (Time.timeScale == 0f || ControladorCinematica.EnCinematica) return;
        if (!jugadorCerca || keypadSistema == null) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (textoInteractuarUI != null)
            {
                textoInteractuarUI.gameObject.SetActive(false);
            }
            keypadSistema.AbrirKeypad();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorCerca = true;
        if (textoInteractuarUI != null)
        {
            textoInteractuarUI.text = mensajeAviso;
            textoInteractuarUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorCerca = false;
        if (textoInteractuarUI != null)
        {
            textoInteractuarUI.gameObject.SetActive(false);
        }
    }
}