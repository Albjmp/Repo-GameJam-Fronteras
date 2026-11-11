using UnityEngine;

public class GestorInventario : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject panelInventario;

    [Header("Configuración de Teclas")]
    [SerializeField] private KeyCode teclaInventario = KeyCode.I;

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
        if (Input.GetKeyDown(teclaInventario))
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

        Debug.Log("Inventario Abierto: Juego Pausado");
    }

    private void CerrarInventario()
    {
        Time.timeScale = 1f;

        Debug.Log("Inventario Cerrado: Juego Reanudado");
    }
}
