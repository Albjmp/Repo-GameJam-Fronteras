using UnityEngine;

public class ControladorJugador : MonoBehaviour
{
    public enum Protagonista { Ivo, Sean }

    [Header("Configuración del Nivel Actual")]
    public Protagonista protagonistaActivo = Protagonista.Ivo;

    [Header("Referencias Internas")]
    [SerializeField] private Transform camaraTransform;
    private CharacterController characterController;

    private float velocidadMovimiento;
    private float alturaCamara;

    [Header("Ajustes del Ratón")]
    [SerializeField] private float sensibilidadRaton = 2f;
    private float rotacionX = 0f;

    [Header("Física Básica")]
    [SerializeField] private float gravedad = -9.81f;
    private Vector3 velocidadGravedad;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (camaraTransform == null)
        {
            camaraTransform = GetComponentInChildren<Camera>().transform;
        }
    }

    private void Start()
    {
        AplicarEstadisticasProtagonista();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        ManejarRotacionCamara();
        ManejarMovimiento();
    }

    private void AplicarEstadisticasProtagonista()
    {
        if (protagonistaActivo == Protagonista.Ivo)
        {
            velocidadMovimiento = 7f;
            alturaCamara = 1.8f;
        } else if (protagonistaActivo == Protagonista.Sean)
        {
            velocidadMovimiento = 4.5f;
            alturaCamara = 1.4f;
        }

        camaraTransform.localPosition = new Vector3(0f, alturaCamara, 0f);
    }

    private void ManejarRotacionCamara()
    {
        float ratonX = Input.GetAxis("Mouse X") * sensibilidadRaton;
        float ratonY = Input.GetAxis("Mouse Y") * sensibilidadRaton;

        rotacionX -= ratonY;
        rotacionX = Mathf.Clamp(rotacionX, -85f, 85f);
        camaraTransform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        transform.Rotate(Vector3.up * ratonX);
    }

    private void ManejarMovimiento()
    {
        float moverX = Input.GetAxis("Horizontal");
        float moverZ = Input.GetAxis("Vertical");

        Vector3 direccion = transform.right * moverX + transform.forward * moverZ;

        characterController.Move(direccion * velocidadMovimiento * Time.deltaTime);

        if (characterController.isGrounded && velocidadGravedad.y < 0)
        {
            velocidadGravedad.y = -2f;
        }

        velocidadGravedad.y += gravedad * Time.deltaTime;
        characterController.Move(velocidadGravedad * Time.deltaTime);
    }
}
