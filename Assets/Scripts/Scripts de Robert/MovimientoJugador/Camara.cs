using UnityEngine;

public class Camara : MonoBehaviour
{
    public float Sensibilidad = 100f;
    public Transform Player;
    public float LimiteVertical = 80f;   // evita que la cámara se voltee

    float RotacionVertical = 0f;

    void Start()
    {
        // 🔑 Bloquear el cursor al centro de la ventana Game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 🔑 Conservar el pitch inicial puesto en el editor
        RotacionVertical = transform.localEulerAngles.x;
        if (RotacionVertical > 180f) RotacionVertical -= 360f;
    }

    void Update()
    {
        float valorx = Input.GetAxis("Mouse X") * Sensibilidad * Time.deltaTime;
        float valory = Input.GetAxis("Mouse Y") * Sensibilidad * Time.deltaTime;

        // Pitch (arriba/abajo) en la cámara
        RotacionVertical -= valory;
        RotacionVertical = Mathf.Clamp(RotacionVertical, -LimiteVertical, LimiteVertical);
        transform.localRotation = Quaternion.Euler(RotacionVertical, 0, 0);

        // Yaw (izquierda/derecha) en el Player
        if (Player != null)
            Player.Rotate(Vector3.up * valorx);
    }
}