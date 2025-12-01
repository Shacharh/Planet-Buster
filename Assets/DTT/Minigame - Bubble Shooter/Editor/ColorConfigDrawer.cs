using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DTT.BubbleShooter.Editor
{
    /// <summary>
    /// Custom property drawer for the colorConfig.
    /// </summary>
    [CustomPropertyDrawer(typeof(ColorConfig))]
    public class ColorConfigDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draw the property.
        /// </summary>
        /// <param name="position"> Rect of the property.</param>
        /// <param name="property"> Property to be drawn.</param>
        /// <param name="label"> Label of the property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            string index = label.text.Split(' ')[1];
            label.text = "color " + index;

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            // Get the width of each element
            float colorWidth = position.width * 0.55f;
            float weightLabelWidth = position.width * 0.10f;
            float weightWidth = position.width * 0.30f;
            float spacing = position.width * 0.05f;
            
            // Set each element rect.
            var color = new Rect(position.x, position.y, colorWidth, position.height);
            var weightLabel = new Rect(position.x + colorWidth+spacing, position.y, weightLabelWidth, position.height);
            var weight = new Rect(position.x + colorWidth + spacing+weightLabelWidth, position.y, weightWidth , position.height);

            // Draw each element of the property
            EditorGUI.PropertyField(color, property.FindPropertyRelative("_bubbleColors"), GUIContent.none);
            EditorGUI.LabelField(weightLabel,"weight");
            EditorGUI.PropertyField(weight, property.FindPropertyRelative("_weight"), GUIContent.none);
            
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}