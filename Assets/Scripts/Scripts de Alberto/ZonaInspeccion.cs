using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ZonaInspeccion : MonoBehaviour
{
    [Header("Referencias del sistema de diálogo")]
    public GameObject panelDialogo;
    public DIalogueScript scriptDialogo;
    public TextMeshProUGUI textoAvisoUI;

    [Header("Información de esta zona")]
    public string tituloInspeccion = "Observación";
    [TextArea(3, 10)]
    public string[] lineasDeInformacion;

    private bool jugadorDentro = false;

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        bool presionoE = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;

        if (jugadorDentro && presionoE && !panelDialogo.activeSelf)
        {
            Inspeccionar();
        }
    }

    private void Inspeccionar()
    {
        if (textoAvisoUI != null) textoAvisoUI.gameObject.SetActive(false);

        string[] nombres = new string[lineasDeInformacion.Length];
        for (int i = 0; i < nombres.Length; i++)
        {
            nombres[i] = tituloInspeccion;
        }

        scriptDialogo.SpeakerNames = nombres;
        scriptDialogo.DialogueLines = lineasDeInformacion;

        panelDialogo.SetActive(true);
        scriptDialogo.StartDialogue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            if (textoAvisoUI != null && !panelDialogo.activeSelf)
            {
                textoAvisoUI.gameObject.SetActive(true);
                textoAvisoUI.text = "Presiona [E] para inspeccionar";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            if (textoAvisoUI != null)
            {
                textoAvisoUI.gameObject.SetActive(false);
            }
        }
    }
}
