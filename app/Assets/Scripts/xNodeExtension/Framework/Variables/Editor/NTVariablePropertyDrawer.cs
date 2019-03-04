namespace NT.Variables
{
    using UnityEngine;
    using UnityEditor;
    using System;


    //FIXME: Research about lists...
    //[CustomPropertyDrawer(typeof(NTVariable))]
    public class NTVariablePropertyDrawer: PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            
            var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
            NTVariable myDataClass = obj as NTVariable;

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Calculate rects
            var amountRect = new Rect(position.x, position.y, 30, position.height);
            var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
            var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.LabelField(amountRect, "ss");
            EditorGUI.LabelField(unitRect, "ss");
            EditorGUI.LabelField(nameRect, "ss");



            EditorGUI.EndProperty();
        }
    }
}