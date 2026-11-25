using UnityEngine;
using System.Collections;

public class DialogoInicial : MonoBehaviour
{
    [Header("Referencias del Cuadro de Diálogo")]
    public GameObject panelDialogo;
    public DIalogueScript scriptDialogo;

    [Header("Conversación al Iniciar el Nivel")]
    public string[] nombresHablantes;
    [TextArea(2, 5)]
    public string[] lineasDialogo;
    public float retrasoInicial = 0.5f;

    private void Start()
    {
        StartCoroutine(IniciarConversacionAutomatica());
    }

    private IEnumerator IniciarConversacionAutomatica()
    {
        yield return new WaitForSeconds(retrasoInicial);

        if (panelDialogo != null && scriptDialogo != null && lineasDialogo.Length > 0)
        {
            scriptDialogo.SpeakerNames = nombresHablantes;
            scriptDialogo.DialogueLines = lineasDialogo;

            panelDialogo.SetActive(true);
            scriptDialogo.StartDialogue();
        }
    }
}
