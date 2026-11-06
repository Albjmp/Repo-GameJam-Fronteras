using UnityEngine;
using UnityEngine.InputSystem; // Necesario para el nuevo sistema de inputs

public class NPCInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject dialogueBubble; // Arrastra aquí tu panel de UI (Burbuja)
    public DIalogueScript dialogueScript; // Arrastra aquí el componente del script de diálogo

    private bool isPlayerInRange = false;

    void Update()
    {
        // Detectamos si presiona la 'E' y si el jugador está dentro del trigger
        bool presionoE = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;

        if (isPlayerInRange && presionoE)
        {
            // Solo iniciamos el diálogo si la burbuja está apagada
            if (!dialogueBubble.activeSelf)
            {
                dialogueBubble.SetActive(true);
                dialogueScript.StartDialogue();
            }
        }
    }

    // Detecta cuando el jugador entra al área (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Presiona 'E' para hablar.");
        }
    }

    // Detecta cuando el jugador sale del área
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}