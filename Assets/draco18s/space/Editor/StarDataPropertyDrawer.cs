using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Assets.draco18s.space.stellar;
using Assets.draco18s.util;
using System.Linq;
using System;

namespace Assets.draco18s.space.Editor
{
	[CustomPropertyDrawer(typeof(StarData))]
	public class StarDataPropertyDrawer : PropertyDrawer
	{
		private static float SPACING = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			position = EditorGUI.IndentedRect(position);
			Rect rect = new Rect(position);
			rect.x += 15;
			rect.width -= 15;
			rect.height = EditorGUIUtility.singleLineHeight;
			SerializedProperty nameProp = property.FindPropertyRelative(nameof(StarData.properName));
			SerializedProperty magProp = property.FindPropertyRelative(nameof(StarData.brightnessMagnitude));
			SerializedProperty specProp = property.FindPropertyRelative(nameof(StarData.spectralType));
			SerializedProperty colorProp = property.FindPropertyRelative(nameof(StarData.colorIndex));
			SerializedProperty massProp = property.FindPropertyRelative(nameof(StarData.mass));
			property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, nameProp.stringValue);
			if(property.isExpanded) {
				EditorGUI.indentLevel++;
				GUIContent c = new GUIContent($"HYG id {property.FindPropertyRelative(nameof(StarData.hygID)).intValue}");
				float x = rect.x;	
				rect.x = Screen.width - GUI.skin.label.CalcSize(c).x - 50;
				EditorGUI.LabelField(rect, c);
				rect.x = x;
				rect = EditorGUI.IndentedRect(rect);
				rect.y += SPACING;
				nameProp.stringValue = EditorGUI.TextField(rect, nameProp.stringValue);
				rect.y += SPACING;
				magProp.floatValue = EditorGUI.FloatField(rect, magProp.floatValue);
				rect.y += SPACING;
				specProp.stringValue = EditorGUI.TextField(rect, specProp.stringValue);
				rect.y += SPACING;
				colorProp.floatValue = EditorGUI.FloatField(rect, colorProp.floatValue);
				rect.y += SPACING;
				massProp.floatValue = EditorGUI.FloatField(rect, massProp.floatValue);
				EditorGUI.indentLevel--;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return SPACING * (property.isExpanded ? 6 : 1);
		}
	}
}
