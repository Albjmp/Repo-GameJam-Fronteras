using UnityEngine;

public enum TipoItem { Objeto, Documento }

[CreateAssetMenu(fileName = "NuevoItem", menuName = "Inventario/ItemData")]
public class ItemData : ScriptableObject
{
    public string id; 
    public string nombre;
    public Sprite icono;
    [TextArea] public string descripcion;
    
    [Header("Clasificación")]
    public TipoItem tipo = TipoItem.Objeto; 

    [Header("Para soltar en el mundo 3D")]
    public GameObject prefabMundo; // Aquí arrastrarás el Prefab de la llave 3D

    [Header("Audio Específico")]
    public AudioClip sonidoAlRecoger;
}