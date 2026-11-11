using UnityEngine;
using UnityEngine.AI;

public class EnemigoVigilante : MonoBehaviour
{
    public float rangoDeVision = 10f;
    public LayerMask capaObstaculos;

    private NavMeshAgent agente;
    private Transform jugador;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= rangoDeVision)
        {
            Vector3 direccionAlJugador = (jugador.position - transform.position).normalized;

            if (!Physics.Raycast(transform.position, direccionAlJugador, distancia, capaObstaculos))
            {
                agente.SetDestination(jugador.position);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeVision);
    }
}
