using UnityEngine;

public enum ItemType
{
    General,
    Documento
}

[CreateAssetMenu(fileName = "Nuevo Objeto", menuName = "Inventario/Objeto")]
public class ItemData : ScriptableObject
{
    public string id;
    public string nombre;
    [TextArea] public string descripcion;
    public Sprite icono;
    public ItemType tipoObjeto;

    [Header("Configuración para Apartado General")]
    public int ancho = 1;
    public int alto = 1;

    [Header("Contenido si es Documento")]
    [TextArea(10, 20)] public string contenidoDocumento;
}
