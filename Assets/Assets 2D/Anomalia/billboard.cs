using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camara;

    void Start()
    {
        if (Camera.main != null)
        {
            camara = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (camara == null) return;

        // Mantiene el sprite vertical mirando siempre hacia la cámara
        Vector3 frente = camara.forward;
        frente.y = 0f;

        if (frente.sqrMagnitude > 0.001f)
        {
            transform.forward = frente;
        }
    }
}