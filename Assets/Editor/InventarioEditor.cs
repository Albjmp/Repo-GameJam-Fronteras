using UnityEngine;
using UnityEditor;

/// <summary>
/// Inspector personalizado para Inventario.
/// Muestra las dimensiones del grid, la capacidad, y organiza limpiamente los objetos
/// y documentos en tiempo de ejecución para evitar amontonamiento y acoplamiento visual.
/// </summary>
[CustomEditor(typeof(Inventario))]
public class InventarioEditor : Editor
{
    private SerializedProperty columnasGridProp;
    private SerializedProperty filasGridProp;
    private SerializedProperty objetosGeneralesProp;
    private SerializedProperty listaDocumentosProp;

    private bool foldoutObjetos = true;
    private bool foldoutDocumentos = true;

    private void OnEnable()
    {
        columnasGridProp = serializedObject.FindProperty("columnasGrid");
        filasGridProp = serializedObject.FindProperty("filasGrid");
        objetosGeneralesProp = serializedObject.FindProperty("objetosGenerales");
        listaDocumentosProp = serializedObject.FindProperty("listaDocumentos");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Inventario inv = (Inventario)target;

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Configuración del Grid", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(columnasGridProp, new GUIContent("Columnas del Grid"));
        EditorGUILayout.PropertyField(filasGridProp, new GUIContent("Filas del Grid"));

        int totalCasillas = Mathf.Max(1, columnasGridProp.intValue) * Mathf.Max(1, filasGridProp.intValue);
        EditorGUILayout.HelpBox($"Dimensiones: {columnasGridProp.intValue} x {filasGridProp.intValue}  |  Capacidad total: {totalCasillas} casillas", MessageType.None);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Contenido del Inventario (Tiempo Real)", EditorStyles.boldLabel);

        // Foldout Objetos Generales
        int cantObjetos = objetosGeneralesProp.arraySize;
        foldoutObjetos = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutObjetos, $"Objetos en Grid ({cantObjetos})");
        if (foldoutObjetos)
        {
            EditorGUI.indentLevel++;
            if (cantObjetos == 0)
            {
                EditorGUILayout.LabelField("No hay objetos generales en el inventario.", EditorStyles.miniLabel);
            }
            else
            {
                for (int i = 0; i < cantObjetos; i++)
                {
                    SerializedProperty elem = objetosGeneralesProp.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(elem, new GUIContent($"Slot {i}"));
                }
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Añadir Slot", EditorStyles.miniButton))
            {
                objetosGeneralesProp.InsertArrayElementAtIndex(cantObjetos);
            }
            if (cantObjetos > 0 && GUILayout.Button("- Quitar Último", EditorStyles.miniButton))
            {
                objetosGeneralesProp.DeleteArrayElementAtIndex(cantObjetos - 1);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(4);

        // Foldout Documentos
        int cantDocs = listaDocumentosProp.arraySize;
        foldoutDocumentos = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutDocumentos, $"Documentos Recolectados ({cantDocs})");
        if (foldoutDocumentos)
        {
            EditorGUI.indentLevel++;
            if (cantDocs == 0)
            {
                EditorGUILayout.LabelField("No hay documentos recolectados.", EditorStyles.miniLabel);
            }
            else
            {
                for (int i = 0; i < cantDocs; i++)
                {
                    SerializedProperty elem = listaDocumentosProp.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(elem, new GUIContent($"Doc {i}"));
                }
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Añadir Documento", EditorStyles.miniButton))
            {
                listaDocumentosProp.InsertArrayElementAtIndex(cantDocs);
            }
            if (cantDocs > 0 && GUILayout.Button("- Quitar Último", EditorStyles.miniButton))
            {
                listaDocumentosProp.DeleteArrayElementAtIndex(cantDocs - 1);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (Application.isPlaying)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Acciones de Depuración (Play Mode)", EditorStyles.boldLabel);
            if (GUILayout.Button("Refrescar Interfaz de Inventario"))
            {
                inv.alActualizarInventario?.Invoke();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
