using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PerseguidorConstante : MonoBehaviour
{
    [Header("Configuración de persecución")]
    public float tiempoEsperaInicial = 30f;
    private bool puedePerseguir = false;

    private NavMeshAgent agente;
    private Transform jugador;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }

        // Iniciamos la cuenta regresiva antes de empezar a perseguir
        StartCoroutine(EsperarParaPerseguir());
    }

    private IEnumerator EsperarParaPerseguir()
    {
        yield return new WaitForSeconds(tiempoEsperaInicial);
        puedePerseguir = true;
    }

    void Update()
    {
        // Solo persigue cuando hayan pasado los 5 segundos
        if (puedePerseguir && jugador != null && agente != null && agente.isOnNavMesh)
        {
            agente.SetDestination(jugador.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SistemaVidaJugador vida = other.GetComponent<SistemaVidaJugador>();
            if (vida != null)
            {
                vida.RecibirGolpe();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SistemaVidaJugador vida = other.GetComponent<SistemaVidaJugador>();
            if (vida != null)
            {
                vida.RecibirGolpe();
            }
        }
    }
}
