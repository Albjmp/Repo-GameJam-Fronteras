using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform camaraObjetivo;
    [Tooltip("Pon 90 o -90 si el sprite sigue viéndose de perfil en la vista Game.")]
    public float rotacionExtraY = 0f;

    private void Start()
    {
        if (camaraObjetivo == null)
        {
            if (Camera.main != null)
            {
                camaraObjetivo = Camera.main.transform;
            }
            else
            {
                Camera cam = FindAnyObjectByType<Camera>();
                if (cam != null) camaraObjetivo = cam.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (camaraObjetivo == null) return;

        Vector3 frente = camaraObjetivo.forward;
        frente.y = 0f;

        if (frente.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(frente) * Quaternion.Euler(0f, rotacionExtraY, 0f);
        }
    }
}