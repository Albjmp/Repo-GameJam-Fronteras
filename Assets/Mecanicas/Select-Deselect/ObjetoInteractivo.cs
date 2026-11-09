using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObjetoInteractivo : MonoBehaviour
{
    public void ActivarObjeto()
    {
        // Lógica para seleccionar el objeto interactivo
        Destroy(gameObject);
    }
}
