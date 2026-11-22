using UnityEngine;
using UnityEngine.InputSystem;

public class GestorInventario : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject panelInventario;
    public GameObject barraLinternaUI;
    public GameObject barraEstaminaUI;

    [Header("Configuración de Teclas")]
    [SerializeField] private KeyCode teclaInventario = KeyCode.I;

    public bool EstaAbierto => inventarioAbierto;
    private bool inventarioAbierto = false;

    private void Awake()
    {
        if (panelInventario != null)
        {
            panelInventario.SetActive(false);
        }
    }

    private void Update()
    {
        bool presionoTeclaI = false;
        bool presionoEscape = false;

        if (Keyboard.current != null)
        {
            presionoTeclaI = Keyboard.current.iKey.wasPressedThisFrame;
            presionoEscape = Keyboard.current.escapeKey.wasPressedThisFrame;
        }

        if (!presionoTeclaI && Input.GetKeyDown(teclaInventario))
        {
            presionoTeclaI = true;
        }

        if (!presionoEscape && Input.GetKeyDown(KeyCode.Escape))
        {
            presionoEscape = true;
        }

        if (presionoTeclaI)
        {
            AlternarInventario();
        }
        else if (presionoEscape && inventarioAbierto)
        {
            AlternarInventario();
        }
    }

    public void AlternarInventario()
    {
        inventarioAbierto = !inventarioAbierto;

        panelInventario.SetActive(inventarioAbierto);

        if (inventarioAbierto)
        {
            AbrirInventario();
        } else
        {
            CerrarInventario();
        }
    }

    private void AbrirInventario()
    {
        Time.timeScale = 0f;

        if (barraLinternaUI != null)
        {
            barraLinternaUI.SetActive(false);
        }

        if (barraEstaminaUI != null)
        {
            barraEstaminaUI.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Inventario Abierto: Juego Pausado");
    }

    private void CerrarInventario()
    {
        Time.timeScale = 1f;

        ControladorJugador jugador = FindAnyObjectByType<ControladorJugador>();
        //if (jugador != null && jugador.protagonistaActivo == ControladorJugador.Protagonista.Ivo)
        //{
            //if (barraLinternaUI != null)
            //{
                //barraLinternaUI.SetActive(true);
            //}
            //if (barraEstaminaUI != null)
            //{
                //barraEstaminaUI.SetActive(true);
            //}
        //}

        if (jugador != null)
        {
            if (barraLinternaUI != null)
            {
                barraLinternaUI.SetActive(jugador.protagonistaActivo == ControladorJugador.Protagonista.Ivo);
            }

            if (barraEstaminaUI != null)
            {
                barraEstaminaUI.SetActive(jugador.protagonistaActivo == ControladorJugador.Protagonista.Sean);
            }
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Inventario Cerrado: Juego Reanudado");
    }
}
