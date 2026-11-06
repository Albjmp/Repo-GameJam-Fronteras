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
        DialogueText.text = string.Empty;
        
        
        // 🔥 ELIMINADO: StartDialogue(); (No lo pongas aquí)
    }

    void Update()
    {
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
            
            if (SpeakerNames.Length > index) 
                SpeakerNameText.text = SpeakerNames[index];

            StartCoroutine(WriteLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}