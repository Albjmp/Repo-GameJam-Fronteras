using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class DIalogueScript : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI DialogueText; 
    public TextMeshProUGUI SpeakerNameText; 

    [Header("Datos del Diálogo")]
    public string[] SpeakerNames; 
    public string[] DialogueLines; 
    public float TextSpeed = 0.01f;

    private int index;
    private bool isTyping = false;

    void Start()
    {
        // Solo limpiamos los textos al inicio
        // Antes -> DialogueText.text = string.Empty;
        if (DialogueText != null) DialogueText.text = string.Empty;
        
        // 🔥 ELIMINADO: StartDialogue(); (No lo pongas aquí)
    }

    void Update()
    {
        if (DialogueText == null || DialogueLines.Length == 0 || index >= DialogueLines.Length) return;
        
        if (Keyboard.current != null && (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.qKey.wasPressedThisFrame))
        {
            CerrarDialogo();
            return;
        }

        bool clickIzquierdo = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool barraEspaciadora = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        if (clickIzquierdo || barraEspaciadora)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                DialogueText.text = DialogueLines[index];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue()
    {
        StopAllCoroutines(); // 🔥 NUEVO: Detiene cualquier escritura anterior por seguridad

        if (DialogueLines == null || DialogueLines.Length == 0)
        {
            Debug.LogWarning("Este diálogo no tiene líneas de texto asignadas");
            CerrarDialogo();
            return;
        }
        
        index = 0;
        DialogueText.text = string.Empty;
        
        if (SpeakerNames.Length > index) 
            SpeakerNameText.text = SpeakerNames[index];

        StartCoroutine(WriteLine());
    }

    IEnumerator WriteLine()
    {
        isTyping = true;
        DialogueText.text = string.Empty; 

        foreach (char letter in DialogueLines[index].ToCharArray())
        {
            DialogueText.text += letter;
            yield return new WaitForSeconds(TextSpeed);
        }
        
        isTyping = false; 
    }

    public void NextLine()
    {
        if (index < DialogueLines.Length - 1)
        {
            index++;
            DialogueText.text = string.Empty;
            
            // Antes -> if (SpeakerNames.Length > index) 
                // Antes -> SpeakerNameText.text = SpeakerNames[index];

            if (SpeakerNames != null && SpeakerNames.Length > index)
            {
                SpeakerNameText.text = SpeakerNames[index];
            }

            StartCoroutine(WriteLine());
        }
        else
        {
            // Antes -> gameObject.SetActive(false);
            CerrarDialogo();
        }
    }

    public void CerrarDialogo()
    {
        StopAllCoroutines();
        isTyping = false;
        gameObject.SetActive(false);
    }
}