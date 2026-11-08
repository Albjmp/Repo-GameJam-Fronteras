using UnityEngine;
using UnityEngine.UI;

public class NavegacionInventario : MonoBehaviour
{
    [Header("Paneles a Pantalla Completa")]
    [SerializeField] private GameObject pantallaGeneral;
    [SerializeField] private GameObject pantallaDocumentos;

    [Header("Botones de Control")]
    [SerializeField] private Button botonGeneral;
    [SerializeField] private Button botonDocumentos;

    private void OnEnable()
    {
        MostrarPantallaGeneral();
    }

    public void MostrarPantallaGeneral()
    {
        pantallaGeneral.SetActive(true);
        pantallaDocumentos.SetActive(false);

        ResaltarBoton(botonGeneral, true);
        ResaltarBoton(botonDocumentos, false);
    }

    public void MostrarPantallaDocumentos()
    {
        pantallaGeneral.SetActive(false);
        pantallaDocumentos.SetActive(true);

        ResaltarBoton(botonGeneral, false);
        ResaltarBoton(botonDocumentos, true);
    }

    private void ResaltarBoton(Button boton, bool activado)
    {
        if (boton == null) return;

        ColorBlock colores = boton.colors;
        colores.normalColor = activado ? Color.white : new Color(1f, 1f, 1f, 0.4f);
        boton.colors = colores;
    }
}
