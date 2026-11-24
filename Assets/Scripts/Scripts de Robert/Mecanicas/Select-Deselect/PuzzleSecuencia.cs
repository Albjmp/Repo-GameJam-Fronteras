using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventoPasoPuzzle : UnityEvent<int> { }

public class PuzzleSecuencia : MonoBehaviour
{
    [Header("Configuración de Secuencia (Estilo Pokémon)")]
    [Tooltip("Orden de IDs que el jugador debe presionar en secuencia. Ej: [1, 3, 2]")]
    [SerializeField] private int[] secuenciaCorrecta = new int[] { 1, 2, 3 };

    [Tooltip("Si es verdadero, al equivocarse en un paso la secuencia vuelve al inicio.")]
    [SerializeField] private bool reiniciarAlFallar = true;

    [Tooltip("Si es verdadero, una vez resuelto el puzzle no se puede volver a alterar.")]
    [SerializeField] private bool resolverUnaSolaVez = true;

    [Header("Interruptores Vinculados (Opcional)")]
    [Tooltip("Lista de interruptores vinculados a este puzzle para reiniciarlos automáticamente si el jugador falla.")]
    [SerializeField] private List<ObjetoInteractivo> interruptoresVinculados = new List<ObjetoInteractivo>();

    [Header("Eventos del Puzzle")]
    [Tooltip("Se invoca cuando el jugador acierta un paso (envía el número de paso actual).")]
    public EventoPasoPuzzle onPasoCorrecto;

    [Tooltip("Se invoca cuando el jugador presiona un interruptor en el orden incorrecto.")]
    public UnityEvent onPasoIncorrecto;

    [Tooltip("Se invoca cuando toda la secuencia es completada con éxito (ej: abrir puerta, activar luz).")]
    public UnityEvent onPuzzleResuelto;

    [Tooltip("Se invoca cuando el puzzle se reinicia.")]
    public UnityEvent onPuzzleReiniciado;

    // Estado interno
    private int indicePasoActual = 0;
    private bool estaResuelto = false;

    public bool EstaResuelto => estaResuelto;
    public int PasoActual => indicePasoActual;
    public int TotalPasos => secuenciaCorrecta != null ? secuenciaCorrecta.Length : 0;

    /// <summary>
    /// Registra el intento de presionar un interruptor con su respectivo ID.
    /// </summary>
    public void RegistrarPaso(int idInterruptor)
    {
        if (estaResuelto && resolverUnaSolaVez)
        {
            Debug.Log($"[PuzzleSecuencia - {name}] El puzzle ya está resuelto.");
            return;
        }

        if (secuenciaCorrecta == null || secuenciaCorrecta.Length == 0)
        {
            Debug.LogWarning($"[PuzzleSecuencia - {name}] La secuencia correcta no tiene pasos definidos.");
            return;
        }

        // Comprobar si el ID coincide con el paso esperado en la secuencia
        if (secuenciaCorrecta[indicePasoActual] == idInterruptor)
        {
            indicePasoActual++;
            Debug.Log($"[PuzzleSecuencia - {name}] ¡Paso correcto! ({indicePasoActual}/{secuenciaCorrecta.Length}) - ID: {idInterruptor}");
            onPasoCorrecto?.Invoke(indicePasoActual);

            // ¿Completó toda la secuencia?
            if (indicePasoActual >= secuenciaCorrecta.Length)
            {
                estaResuelto = true;
                Debug.Log($"[PuzzleSecuencia - {name}] ★ ¡PUZZLE RESUELTO!");
                onPuzzleResuelto?.Invoke();
            }
        }
        else
        {
            Debug.LogWarning($"[PuzzleSecuencia - {name}] Paso incorrecto (ID: {idInterruptor}). Se esperaba ID: {secuenciaCorrecta[indicePasoActual]}.");
            onPasoIncorrecto?.Invoke();

            if (reiniciarAlFallar)
            {
                ReiniciarPuzzle();
            }
        }
    }

    /// <summary>
    /// Reinicia la secuencia al inicio y resetea el estado de los interruptores vinculados.
    /// </summary>
    public void ReiniciarPuzzle()
    {
        indicePasoActual = 0;

        foreach (var inter in interruptoresVinculados)
        {
            if (inter != null)
            {
                inter.ResetearInterruptor();
            }
        }

        Debug.Log($"[PuzzleSecuencia - {name}] Secuencia reiniciada.");
        onPuzzleReiniciado?.Invoke();
    }
}
