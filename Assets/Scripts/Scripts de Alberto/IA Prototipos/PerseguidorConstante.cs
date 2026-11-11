using UnityEngine;
using UnityEngine.AI;

public class PerseguidorConstante : MonoBehaviour
{
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
        if (jugador != null)
        {
            agente.SetDestination(jugador.position);
        }
    }
}
