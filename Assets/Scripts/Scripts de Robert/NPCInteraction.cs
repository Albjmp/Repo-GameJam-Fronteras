using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject dialogueBubble; 
    public DIalogueScript dialogueScript; 
    
    [Header("Interfaz de Usuario")]
    public GameObject interactionPrompt; // <-- NUEVO: El texto de "Presiona E"

    private bool isPlayerInRange = false;

    void Update()
    {
        bool presionoE = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;

        if (isPlayerInRange && presionoE)
        {
            if (!dialogueBubble.activeSelf)
            {
                dialogueBubble.SetActive(true);
                dialogueScript.StartDialogue();
                
                // Ocultamos el aviso de "Presiona E" cuando empieza la conversación
                if (interactionPrompt != null) interactionPrompt.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            
            // Encendemos el aviso en la pantalla
            if (interactionPrompt != null) interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            
            // Apagamos el aviso si el jugador se aleja
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
        }
    }
}