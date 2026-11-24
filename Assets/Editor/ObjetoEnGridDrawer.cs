using UnityEngine;
using UnityEditor;

/// <summary>
/// PropertyDrawer para ObjetoEnGrid.
/// Dibuja cada objeto del grid en una sola fila compacta y limpia en lugar de varias líneas desordenadas.
/// </summary>
[CustomPropertyDrawer(typeof(ObjetoEnGrid))]
public class ObjetoEnGridDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty datosProp = property.FindPropertyRelative("datos");
        SerializedProperty xProp = property.FindPropertyRelative("x");
        SerializedProperty yProp = property.FindPropertyRelative("y");
        SerializedProperty rotadoProp = property.FindPropertyRelative("rotado");

        float lineaAltura = EditorGUIUtility.singleLineHeight;
        float anchoTotal = position.width;

        // Distribución en fila única:
        // [ItemData (50%)] [X (15%)] [Y (15%)] [Rot (20%)]
        float campoItemAncho = anchoTotal * 0.45f;
        float campoXAncho = anchoTotal * 0.18f;
        float campoYAncho = anchoTotal * 0.18f;
        float campoRotAncho = anchoTotal * 0.19f;

        Rect rectItem = new Rect(position.x, position.y, campoItemAncho, lineaAltura);
        Rect rectX = new Rect(position.x + campoItemAncho + 4, position.y, campoXAncho - 4, lineaAltura);
        Rect rectY = new Rect(position.x + campoItemAncho + campoXAncho + 4, position.y, campoYAncho - 4, lineaAltura);
        Rect rectRot = new Rect(position.x + campoItemAncho + campoXAncho + campoYAncho + 4, position.y, campoRotAncho - 4, lineaAltura);

        // Dibujar ItemData
        EditorGUI.PropertyField(rectItem, datosProp, GUIContent.none);

        // Dibujar X e Y compactos con prefijo
        int indentPrevio = EditorGUI.indentLevel;
        float labelWidthPrevio = EditorGUIUtility.labelWidth;
        EditorGUI.indentLevel = 0;

        EditorGUIUtility.labelWidth = 14f;
        EditorGUI.PropertyField(rectX, xProp, new GUIContent("X"));
        EditorGUI.PropertyField(rectY, yProp, new GUIContent("Y"));

        EditorGUIUtility.labelWidth = 26f;
        EditorGUI.PropertyField(rectRot, rotadoProp, new GUIContent("Rot"));

        EditorGUIUtility.labelWidth = labelWidthPrevio;
        EditorGUI.indentLevel = indentPrevio;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
