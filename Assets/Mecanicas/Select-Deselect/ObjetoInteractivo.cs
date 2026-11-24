using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TipoInteraccion
{
    RecogerItem,
    Gaveta,
    InterruptorPuzzle,
    Personalizado
}

public class ObjetoInteractivo : MonoBehaviour
{
    [Header("Clasificación de Objeto")]
    [Tooltip("Define el comportamiento del objeto al interactuar con la tecla E.")]
    [SerializeField] private TipoInteraccion tipo = TipoInteraccion.RecogerItem;

    [Header("Configuración: Recoger Ítem")]
    [Tooltip("Datos del ítem para agregar al inventario (opcional).")]
    [SerializeField] private ItemData itemData;
    [Tooltip("¿Destruir el GameObject de la escena tras recogerlo?")]
    [SerializeField] private bool destruirAlRecoger = true;

    [Header("Configuración: Gaveta")]
    [Tooltip("Estado actual de la gaveta (abierta o cerrada).")]
    [SerializeField] private bool estaAbierta = false;
    [Tooltip("Objeto o ítem dentro de la gaveta que se hace visible al abrir y se oculta al cerrar (opcional).")]
    [SerializeField] private GameObject contenidoInterior;
    [SerializeField] private string textoAbrirGaveta = "E - Abrir gaveta";
    [SerializeField] private string textoCerrarGaveta = "E - Cerrar gaveta";

    [Header("Configuración: Interruptor Puzzle")]
    [Tooltip("Gestor del puzzle al cual pertenece este interruptor.")]
    [SerializeField] private PuzzleSecuencia gestorPuzzle;
    [Tooltip("Identificador numérico de este interruptor dentro de la secuencia (ej: 1, 2, 3...).")]
    [SerializeField] private int idInterruptor = 1;
    [Tooltip("Si es verdadero, no puede volverse a presionar hasta que el puzzle lo reinicie.")]
    [SerializeField] private bool unSoloUso = false;

    [Header("Eventos de Interacción (Para animaciones, sonidos o lógica adicional)")]
    [Tooltip("Se invoca siempre que el jugador interactúa con el objeto.")]
    public UnityEvent onInteractuar;

    [Tooltip("Eventos específicos para gavetas.")]
    public UnityEvent onAbrirGaveta;
    public UnityEvent onCerrarGaveta;
    public UnityEvent<bool> onCambioEstadoGaveta;

    [Tooltip("Evento al pulsar el interruptor.")]
    public UnityEvent onActivarInterruptor;

    // Estado interno
    private bool interruptorYaPulsado = false;

    // Propiedades públicas de consulta
    public TipoInteraccion Tipo => tipo;
    public bool EstaAbierta => estaAbierta;
    public bool InterruptorYaPulsado => interruptorYaPulsado;

    private void Start()
    {
        // Inicializar estado visual del contenido de la gaveta si existe
        if (tipo == TipoInteraccion.Gaveta && contenidoInterior != null)
        {
            contenidoInterior.SetActive(estaAbierta);
        }
    }

    /// <summary>
    /// Llamado desde Selected.cs cuando el jugador presiona 'E'.
    /// </summary>
    public void ActivarObjeto()
    {
        onInteractuar?.Invoke();

        switch (tipo)
        {
            case TipoInteraccion.RecogerItem:
                EjecutarRecogerItem();
                break;

            case TipoInteraccion.Gaveta:
                EjecutarInteraccionGaveta();
                break;

            case TipoInteraccion.InterruptorPuzzle:
                EjecutarInteraccionInterruptor();
                break;

            case TipoInteraccion.Personalizado:
                // Solo ejecuta onInteractuar configurado desde el Inspector
                break;
        }
    }

    // ==========================================
    // MECÁNICA 1: GAVETA (ABRIR / CERRAR)
    // ==========================================
    private void EjecutarInteraccionGaveta()
    {
        estaAbierta = !estaAbierta;

        if (contenidoInterior != null)
        {
            contenidoInterior.SetActive(estaAbierta);
        }

        onCambioEstadoGaveta?.Invoke(estaAbierta);

        if (estaAbierta)
        {
            Debug.Log($"[Gaveta] '{gameObject.name}' abierta.");
            onAbrirGaveta?.Invoke();
        }
        else
        {
            Debug.Log($"[Gaveta] '{gameObject.name}' cerrada.");
            onCerrarGaveta?.Invoke();
        }
    }

    // ==========================================
    // MECÁNICA 2: INTERRUPTOR DE PUZZLE
    // ==========================================
    private void EjecutarInteraccionInterruptor()
    {
        if (unSoloUso && interruptorYaPulsado)
        {
            Debug.Log($"[Interruptor] '{gameObject.name}' ya se encuentra pulsado.");
            return;
        }

        interruptorYaPulsado = true;
        onActivarInterruptor?.Invoke();

        if (gestorPuzzle != null)
        {
            gestorPuzzle.RegistrarPaso(idInterruptor);
        }
        else
        {
#if UNITY_2023_1_OR_NEWER
            PuzzleSecuencia gestorEnEscena = FindAnyObjectByType<PuzzleSecuencia>();
#else
            PuzzleSecuencia gestorEnEscena = FindObjectOfType<PuzzleSecuencia>();
#endif
            if (gestorEnEscena != null)
            {
                gestorEnEscena.RegistrarPaso(idInterruptor);
            }
            else
            {
                Debug.LogWarning($"[Interruptor] No hay un PuzzleSecuencia asignado al interruptor '{gameObject.name}'.");
            }
        }
    }

    public void ResetearInterruptor()
    {
        interruptorYaPulsado = false;
    }

    // ==========================================
    // MECÁNICA 3: RECOGER ÍTEM
    // ==========================================
    private void EjecutarRecogerItem()
    {
        Debug.Log($"[ObjetoInteractivo] Intentando recoger ítem '{gameObject.name}'...");

        if (itemData != null)
        {
            Inventario inventario = Inventario.Instance != null ? Inventario.Instance :
#if UNITY_2023_1_OR_NEWER
            FindAnyObjectByType<Inventario>();
#else
            FindObjectOfType<Inventario>();
#endif
            if (inventario != null)
            {
                bool guardado = inventario.RecogerObjeto(itemData);
                if (!guardado)
                {
                    Debug.LogWarning($"[Inventario] No se pudo guardar '{itemData.nombre}', inventario lleno.");
                    return;
                }
                else
                {
                    Debug.Log($"[Inventario] ¡Objeto '{itemData.nombre}' guardado exitosamente en el inventario!");
                }
            }
            else
            {
                Debug.LogWarning($"[Inventario] No se encontró el componente 'Inventario' en la escena actual. Si quieres que se guarde, asegúrate de que la escena tenga el script 'Inventario'.");
            }
        }
        else
        {
            Debug.LogWarning($"[ObjetoInteractivo] '{gameObject.name}' no tiene asignado un ItemData en el Inspector.");
        }

        if (destruirAlRecoger)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Devuelve el texto contextual que se mostrará en la interfaz del jugador.
    /// </summary>
    public string ObtenerTextoInteraccion()
    {
        switch (tipo)
        {
            case TipoInteraccion.Gaveta:
                return estaAbierta ? textoCerrarGaveta : textoAbrirGaveta;

            case TipoInteraccion.InterruptorPuzzle:
                return "E - Presionar interruptor";

            case TipoInteraccion.RecogerItem:
                return itemData != null && !string.IsNullOrEmpty(itemData.nombre)
                    ? $"E - Recoger {itemData.nombre}"
                    : "E - Recoger";

            default:
                return "E - Interactuar";
        }
    }
}
