using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using TMPro;

public class SistemaQTE : MonoBehaviour
{
    public GameObject panelQTE;
    public Image barraTiempo;
    public TextMeshProUGUI textoBoton; // "Presiona A" o "Presiona E"

    public float tiempoLimite = 3f;
    public KeyCode teclaRequerida = KeyCode.E;

    public UnityEvent onExito;
    public UnityEvent onFallo;

    private bool qteActivo = false;
    private float tiempoActual = 0f;

    public void IniciarQTE()
    {
        panelQTE.SetActive(true);
        textoBoton.text = "¡Presiona " + teclaRequerida.ToString() + "!";
        qteActivo = true;
        tiempoActual = tiempoLimite;
        Time.timeScale = 0f; // Pausa para que el jugador reaccione
    }

    private void Update()
    {
        if (!qteActivo) return;

        // Usamos unscaledDeltaTime porque el juego está en pausa (Time.timeScale = 0)
        tiempoActual -= Time.unscaledDeltaTime;
        barraTiempo.fillAmount = tiempoActual / tiempoLimite;

        if (tiempoActual <= 0)
        {
            Fallar();
            return;
        }

        if (Input.GetKeyDown(teclaRequerida))
        {
            Exito();
        }
    }

    private void Exito()
    {
        qteActivo = false;
        panelQTE.SetActive(false);
        Time.timeScale = 1f;
        onExito.Invoke();
    }

    private void Fallar()
    {
        qteActivo = false;
        panelQTE.SetActive(false);
        Time.timeScale = 1f;
        onFallo.Invoke();
    }
}