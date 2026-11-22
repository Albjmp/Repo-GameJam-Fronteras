using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class ControladorJugador : MonoBehaviour
{
    public enum Protagonista { Ivo, Sean }

    [Header("Configuración del Nivel Actual")]
    public Protagonista protagonistaActivo = Protagonista.Ivo;

    [Header("Referencias Internas")]
    [SerializeField] private Transform camaraTransform;
    private CharacterController characterController;

    private float velocidadMovimiento;
    private float velocidadBase;
    private float alturaCamara;

    [Header("Configuración de correr (Sean)")]
    public float velocidadCorrerSean = 8f;
    public float estaminaMaxima = 100f;
    public float consumoEstamina = 25f;
    public float recargaEstamina = 15f;
    public Slider barraEstaminaUI;
    public float alturaSaltoSean = 1.2f;
    private float bufferSaltoTimer = 0.5f;

    private float estaminaActual;
    private bool agotado = false;


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
        ManejarEstaminaYCorrer();
        ManejarMovimiento();
    }

    private void AplicarEstadisticasProtagonista()
    {
        if (protagonistaActivo == Protagonista.Ivo)
        {
            velocidadBase = 7f;
            alturaCamara = 1.8f;

            if (barraEstaminaUI != null) barraEstaminaUI.gameObject.SetActive(false);
        } else if (protagonistaActivo == Protagonista.Sean)
        {
            velocidadBase = 4.5f;
            alturaCamara = 1.4f;
            estaminaActual = estaminaMaxima;

            if (barraEstaminaUI != null)
            {
                barraEstaminaUI.gameObject.SetActive(true);
                barraEstaminaUI.maxValue = estaminaMaxima;
                barraEstaminaUI.value = estaminaActual;
            }
        }

        velocidadMovimiento = velocidadBase;
        camaraTransform.localPosition = new Vector3(0f, alturaCamara, 0f);
    }

    private Vector2 ObtenerInputMovimiento()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current == null) return input;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;

        return input.normalized;
    }
    private void ManejarEstaminaYCorrer()
    {
        if (protagonistaActivo != Protagonista.Sean) return;

        bool presionandoShift = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        //bool estaMoviendose = Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f;
        bool estaMoviendose = ObtenerInputMovimiento().magnitude > 0.1f;

        if (presionandoShift && estaMoviendose && estaminaActual > 0f && !agotado)
        {
            velocidadMovimiento = velocidadCorrerSean;
            estaminaActual -= consumoEstamina * Time.deltaTime;

            if (estaminaActual <= 0f)
            {
                estaminaActual = 0f;
                agotado = true;
            }
        } else
        {
            velocidadMovimiento = velocidadBase;

            if (estaminaActual < estaminaMaxima)
            {
                estaminaActual += recargaEstamina * Time.deltaTime;
                if (estaminaActual >= estaminaMaxima)
                {
                    estaminaActual = estaminaMaxima;
                }
            }

            if (agotado && estaminaActual >= estaminaMaxima * 0.25f)
            {
                agotado = false;
            }
        }

        if (barraEstaminaUI != null)
        {
            barraEstaminaUI.value = estaminaActual;
        }
    }

    private void ManejarRotacionCamara()
    {
        if (Mouse.current == null) return;

        Vector2 deltaRaton = Mouse.current.delta.ReadValue();
        //float ratonX = Input.GetAxis("Mouse X") * sensibilidadRaton;
        //float ratonY = Input.GetAxis("Mouse Y") * sensibilidadRaton;

        float ratonX = deltaRaton.x * sensibilidadRaton;
        float ratonY = deltaRaton.y * sensibilidadRaton;

        rotacionX -= ratonY;
        rotacionX = Mathf.Clamp(rotacionX, -85f, 85f);
        camaraTransform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        transform.Rotate(Vector3.up * ratonX);
    }

    private void ManejarMovimiento()
    {
        //float moverX = Input.GetAxis("Horizontal");
        //float moverZ = Input.GetAxis("Vertical");

        Vector2 input = ObtenerInputMovimiento();
        //Vector3 direccion = transform.right * moverX + transform.forward * moverZ;
        Vector3 direccion = transform.right * input.x + transform.forward * input.y;

        characterController.Move(direccion * velocidadMovimiento * Time.deltaTime);

        if (characterController.isGrounded && velocidadGravedad.y < 0)
        {
            velocidadGravedad.y = -2f;
        }

        if (protagonistaActivo == Protagonista.Sean)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                bufferSaltoTimer = 0.65f;
            } else if (bufferSaltoTimer > 0.5f)
            {
                bufferSaltoTimer -= Time.deltaTime;
            }
            
            if (characterController.isGrounded && bufferSaltoTimer > 0.5f)
            {
                velocidadGravedad.y= Mathf.Sqrt(alturaSaltoSean * -2f * gravedad);
                bufferSaltoTimer = 0.5f;
            }
        }
        
        

        velocidadGravedad.y += gravedad * Time.deltaTime;
        characterController.Move(velocidadGravedad * Time.deltaTime);
    }
}
