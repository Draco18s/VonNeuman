using UnityEditor;
using UnityEngine;

namespace Assets.draco18s.crafting.Editor {
	[CustomPropertyDrawer(typeof(Ingredient))]
	public class IngredientDrawer : PropertyDrawer {
		protected static float SPACING = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			position.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("mat"));
			SerializedProperty q = property.FindPropertyRelative("quant");
			SerializedProperty f = property.FindPropertyRelative("frac");
			Rect pos2 = new Rect(position);
			pos2.y += SPACING;
			EditorGUI.LabelField(pos2,new GUIContent("Quantity"));
			pos2.x += EditorGUIUtility.labelWidth+2;
			pos2.width -= EditorGUIUtility.labelWidth + 15 + 2;
			pos2.width /= 2;
			q.intValue = EditorGUI.DelayedIntField(pos2, q.intValue);
			pos2.x += pos2.width + 5;
			EditorGUI.LabelField(pos2,new GUIContent("/"));
			pos2.x += 10;
			f.intValue = Mathf.Max(EditorGUI.DelayedIntField(pos2, f.intValue),1);
			//property.ApplyModifiedProperties();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return SPACING*2;
		}
	}
}