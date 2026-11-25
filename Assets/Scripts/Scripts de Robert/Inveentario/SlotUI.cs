using UnityEngine;

public class SlotUI : MonoBehaviour
{
    public ItemData itemData;
    public GestorDocumentos gestorDoc;
    public Inventario inventario;

    public void AlHacerClic()
    {
        if (itemData == null) return;

        // Le avisamos al inventario qué ítem se seleccionó (por si lo necesitas luego)
        if (inventario != null) inventario.SeleccionarItem(itemData);

        // Si es un documento, abrimos la pantalla de lectura
        if (itemData.tipo == TipoItem.Documento && gestorDoc != null)
        {
            gestorDoc.AbrirDocumento(itemData);
        }
    }
}