using UnityEngine;

// Creamos un enum para clasificar los ítems
public enum TipoItem
{
    Objeto,
    Documento
}

[CreateAssetMenu(fileName = "NuevoItem", menuName = "Inventario/ItemData")]
public class ItemData : ScriptableObject
{
    public string id; 
    public string nombre;
    public Sprite icono;
    [TextArea] public string descripcion;
    
    [Header("Clasificación")]
    public TipoItem tipo = TipoItem.Objeto; // Por defecto será Objeto
}