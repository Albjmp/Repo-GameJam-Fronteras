using UnityEngine;
using UnityEngine.InputSystem;

public class InspeccionDocumento3D : MonoBehaviour
{
    [Header("Configuración")]
    public Transform puntoAparicion;
    public float velocidadRotacion = 0.5f;

    private GameObject modeloActual;
    private Animator animadorActual;
    private bool estaDoblado = false;

    public void CargarDocumento3D(ItemData datosDocumento)
    {
        if (modeloActual != null)
        {
            Destroy(modeloActual);
        }

        estaDoblado = false;

        if (datosDocumento.prefabModelo3D != null)
        {
            modeloActual = Instantiate(datosDocumento.prefabModelo3D, puntoAparicion.position, Quaternion.identity, puntoAparicion);
            animadorActual = modeloActual.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (modeloActual == null) return;

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 deltaRaton = Mouse.current.delta.ReadValue();

            modeloActual.transform.Rotate(Vector3.up, -deltaRaton.x * velocidadRotacion, Space.World);
            modeloActual.transform.Rotate(Vector3.right, deltaRaton.y * velocidadRotacion, Space.World);
        }

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            AlternarDoblez();
        }
    }

    public void AlternarDoblez()
    {
        if (animadorActual == null) return;

        estaDoblado = !estaDoblado;

        animadorActual.SetBool("Doblado", estaDoblado);
    }
}
