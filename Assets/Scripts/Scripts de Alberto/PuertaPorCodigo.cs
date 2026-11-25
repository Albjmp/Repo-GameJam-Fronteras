using UnityEngine;

public class PuertaPorCodigo : MonoBehaviour
{
    public float anguloApertura = 90f;
    public float velocidadApertura = 3f;

    private bool abierta = false;
    private Quaternion rotacionCerrada;
    private Quaternion rotacionAbierta;

    private void Start()
    {
        rotacionCerrada = transform.rotation;
        rotacionAbierta = Quaternion.Euler(transform.eulerAngles + new Vector3(0, anguloApertura, 0));
    }

    private void Update()
    {
        Quaternion objetivo = abierta ? rotacionAbierta : rotacionCerrada;
        transform.rotation = Quaternion.Slerp(transform.rotation, objetivo, Time.deltaTime * velocidadApertura);
    }

    public void AbrirPuerta()
    {
        abierta = true;
    }
}
