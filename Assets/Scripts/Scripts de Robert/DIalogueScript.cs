using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem; // Mantenlo si usaste el Input System nuevo

public class DIalogueScript : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI DialogueText; // Tu texto de la burbuja
    public TextMeshProUGUI SpeakerNameText; // <-- NUEVO: Aquí arrastrarás el texto del Nombre

    [Header("Datos del Diálogo")]
    public string[] SpeakerNames; // <-- NUEVO: Array para los nombres
    public string[] DialogueLines; // Array para los textos
    public float TextSpeed = 0.01f;

    private int index;
    private bool isTyping = false;

    void Start()
    {
        DialogueText.text = string.Empty;
        SpeakerNameText.text = string.Empty; // Limpiamos el nombre al inicio
        StartDialogue();
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
        index = 0;
        DialogueText.text = string.Empty;
        
        // Ponemos el nombre del primer personaje antes de empezar a escribir
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
            
            // NUEVO: Actualizamos el nombre al pasar de línea
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