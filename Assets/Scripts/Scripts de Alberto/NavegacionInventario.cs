using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class NavegacionInventario : MonoBehaviour
{
    public enum PestanaInventario { Objetos, Documentos }

    [Header("Paneles a Pantalla Completa")]
    [SerializeField] private GameObject pantallaGeneral;
    [SerializeField] private GameObject pantallaDocumentos;

    [Header("Botones de Control")]
    [SerializeField] private Button botonGeneral;
    [SerializeField] private Button botonDocumentos;

    [Header("Estado Actual")]
    public PestanaInventario pestanaActual = PestanaInventario.Objetos;

    private void Awake()
    {
        if (botonGeneral != null)
        {
            botonGeneral.onClick.RemoveListener(MostrarPantallaGeneral);
            botonGeneral.onClick.AddListener(MostrarPantallaGeneral);
        }

        if (botonDocumentos != null)
        {
            botonDocumentos.onClick.RemoveListener(MostrarPantallaDocumentos);
            botonDocumentos.onClick.AddListener(MostrarPantallaDocumentos);
        }
    }

    private void OnEnable()
    {
        if (pestanaActual == PestanaInventario.Documentos)
        {
            MostrarPantallaDocumentos();
        }
        else
        {
            MostrarPantallaGeneral();
        }
    }

    private void Update()
    {
        bool cambiarPestana = false;
        bool irAObjetos = false;
        bool irADocumentos = false;

        if (Keyboard.current != null)
        {
            cambiarPestana = Keyboard.current.tabKey.wasPressedThisFrame;
            irAObjetos = Keyboard.current.qKey.wasPressedThisFrame || Keyboard.current.digit1Key.wasPressedThisFrame;
            irADocumentos = Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.digit2Key.wasPressedThisFrame;
        }

        if (!cambiarPestana && Input.GetKeyDown(KeyCode.Tab))
        {
            cambiarPestana = true;
        }
        if (!irAObjetos && (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Alpha1)))
        {
            irAObjetos = true;
        }
        if (!irADocumentos && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Alpha2)))
        {
            irADocumentos = true;
        }

        if (cambiarPestana)
        {
            AlternarPestana();
        }
        else if (irAObjetos)
        {
            MostrarPantallaGeneral();
        }
        else if (irADocumentos)
        {
            MostrarPantallaDocumentos();
        }
    }

    public void AlternarPestana()
    {
        if (pestanaActual == PestanaInventario.Objetos)
        {
            MostrarPantallaDocumentos();
        }
        else
        {
            MostrarPantallaGeneral();
        }
    }

    public void MostrarPantallaGeneral()
    {
        pestanaActual = PestanaInventario.Objetos;

        if (pantallaGeneral != null) pantallaGeneral.SetActive(true);
        if (pantallaDocumentos != null) pantallaDocumentos.SetActive(false);

        ResaltarBoton(botonGeneral, true);
        ResaltarBoton(botonDocumentos, false);

        if (pantallaGeneral != null)
        {
            UIGridInventario grid = pantallaGeneral.GetComponent<UIGridInventario>();
            if (grid != null) grid.Refrescar();
        }
    }

    public void MostrarPantallaDocumentos()
    {
        pestanaActual = PestanaInventario.Documentos;

        if (pantallaGeneral != null) pantallaGeneral.SetActive(false);
        if (pantallaDocumentos != null) pantallaDocumentos.SetActive(true);

        ResaltarBoton(botonGeneral, false);
        ResaltarBoton(botonDocumentos, true);

        if (pantallaDocumentos != null)
        {
            UIDocumentosInventario docsUI = pantallaDocumentos.GetComponent<UIDocumentosInventario>();
            if (docsUI != null) docsUI.ActualizarDocumentos();
        }
    }

    private void ResaltarBoton(Button boton, bool activado)
    {
        if (boton == null) return;

        ColorBlock colores = boton.colors;
        colores.normalColor = activado ? Color.white : new Color(1f, 1f, 1f, 0.4f);
        boton.colors = colores;
    }
}
