using UnityEngine;
using UnityEngine.UI;


public class BarraDeVida : MonoBehaviour
{

public Image barraDeVida;

    public float vidaMaxima = 100f;
    public float vidaActual = 100f;

    // Update is called once per frame
    void Update()
    {
        barraDeVida.fillAmount = vidaActual / vidaMaxima;
    }
}
