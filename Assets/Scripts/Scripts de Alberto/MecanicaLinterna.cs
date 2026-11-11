using UnityEngine;
using UnityEngine.UI;

public class MecanicaLinterna : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject luzLinterna;
    [SerializeField] private Slider barraBateriaUI;
    private ControladorJugador controladorJugador;

    [Header("Configuración de Batería")]
    [SerializeField] private float bateriaMaxima = 100f;
    [SerializeField] private float velocidadConsumo = 10f;

    private float bateriaActual;
    private float velocidadRecarga;
    private bool linternaEncendida = false;
    private bool tieneAcceso = false;

    private void Awake()
    {
        controladorJugador = GetComponent<ControladorJugador>();
    }

    private void Start()
    {
        bateriaActual = bateriaMaxima;

        velocidadRecarga = velocidadConsumo / 1.2f;

        if (barraBateriaUI != null)
        {
            barraBateriaUI.maxValue = bateriaMaxima;
            barraBateriaUI.value = bateriaActual;
        }

        if (controladorJugador != null && controladorJugador.protagonistaActivo == ControladorJugador.Protagonista.Ivo)
        {
            tieneAcceso = true;

            linternaEncendida = false;
            if (luzLinterna != null) luzLinterna.SetActive(false);
        } else
        {
            tieneAcceso = false;
            if (luzLinterna != null) luzLinterna.SetActive(false);
            if (barraBateriaUI != null) barraBateriaUI.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!tieneAcceso || Time.timeScale == 0f) return;

        ManejarInput();
        GestionarEnergia();
        ActualizarUI();
    }

    private void ManejarInput()
    {
        if (Input.GetMouseButtonDown(1) && bateriaActual > 0f)
        {
            linternaEncendida = !linternaEncendida;
            luzLinterna.SetActive(linternaEncendida);
        }
    }

    private void GestionarEnergia()
    {
        if (linternaEncendida)
        {
            bateriaActual -= velocidadConsumo * Time.deltaTime;

            if (bateriaActual <= 0f)
            {
                bateriaActual = 0f;
                linternaEncendida = false;
                luzLinterna.SetActive(false);
            }
        } else
        {
            if (bateriaActual < bateriaMaxima)
            {
                bateriaActual += velocidadRecarga * Time.deltaTime;
                if (bateriaActual > bateriaMaxima) bateriaActual = bateriaMaxima;
            }
        }
    }

    private void ActualizarUI()
    {
        if (barraBateriaUI != null)
        {
            barraBateriaUI.value = bateriaActual;
        }
    }
}
