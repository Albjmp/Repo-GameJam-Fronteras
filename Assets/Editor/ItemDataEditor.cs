using UnityEngine;
using UnityEditor;

/// <summary>
/// Inspector personalizado para ItemData.
/// Desacopla la configuración de objetos generales y documentos, mostrando únicamente
/// los campos relevantes según el tipo de objeto seleccionado (General vs Documento).
/// </summary>
[CustomEditor(typeof(ItemData))]
[CanEditMultipleObjects]
public class ItemDataEditor : Editor
{
    private SerializedProperty idProp;
    private SerializedProperty nombreProp;
    private SerializedProperty descripcionProp;
    private SerializedProperty iconoProp;
    private SerializedProperty tipoObjetoProp;
    private SerializedProperty anchoProp;
    private SerializedProperty altoProp;
    private SerializedProperty contenidoDocumentoProp;
    private SerializedProperty prefabModelo3DProp;

    private void OnEnable()
    {
        idProp = serializedObject.FindProperty("id");
        nombreProp = serializedObject.FindProperty("nombre");
        descripcionProp = serializedObject.FindProperty("descripcion");
        iconoProp = serializedObject.FindProperty("icono");
        tipoObjetoProp = serializedObject.FindProperty("tipoObjeto");
        anchoProp = serializedObject.FindProperty("ancho");
        altoProp = serializedObject.FindProperty("alto");
        contenidoDocumentoProp = serializedObject.FindProperty("contenidoDocumento");
        prefabModelo3DProp = serializedObject.FindProperty("prefabModelo3D");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Encabezado
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Identificación y Datos Básicos", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(idProp, new GUIContent("ID único"));
        EditorGUILayout.PropertyField(nombreProp, new GUIContent("Nombre"));
        EditorGUILayout.PropertyField(descripcionProp, new GUIContent("Descripción"));
        EditorGUILayout.PropertyField(iconoProp, new GUIContent("Icono (Sprite)"));

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Clasificación de Ítem", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(tipoObjetoProp, new GUIContent("Tipo de Ítem"));

        ItemType tipoActual = (ItemType)tipoObjetoProp.enumValueIndex;

        EditorGUILayout.Space(6);

        // Desacoplamiento condicional de campos según el tipo
        if (tipoActual == ItemType.General)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Propiedades de Grid (Objeto General)", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(anchoProp, new GUIContent("Ancho en Grid (celdas)"));
            if (anchoProp.intValue < 1) anchoProp.intValue = 1;

            EditorGUILayout.PropertyField(altoProp, new GUIContent("Alto en Grid (celdas)"));
            if (altoProp.intValue < 1) altoProp.intValue = 1;

            EditorGUILayout.HelpBox($"Ocupará {anchoProp.intValue} x {altoProp.intValue} casillas en el inventario.", MessageType.None);
            EditorGUILayout.EndVertical();
        }
        else if (tipoActual == ItemType.Documento)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Contenido del Documento", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Los documentos se almacenan en la pestaña Documentos y no ocupan espacio en la cuadrícula general.", MessageType.Info);
            EditorGUILayout.PropertyField(contenidoDocumentoProp, new GUIContent("Texto del Documento"));
            EditorGUILayout.EndVertical();
        }

        // Inspección 3D (común a ambos si aplica)
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Inspección Visual 3D", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(prefabModelo3DProp, new GUIContent("Prefab Modelo 3D"));

        serializedObject.ApplyModifiedProperties();
    }
}
