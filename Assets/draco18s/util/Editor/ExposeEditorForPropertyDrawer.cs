using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.draco18s.util;

namespace Assets.draco18s.util.Editor {
	[CustomPropertyDrawer(typeof(ExposeEditorFor))]
	public class ExposeEditorForPropertyDrawer : PropertyDrawer
	{
		private UnityEditor.Editor editor;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			if(!editor)
				UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref editor);
			editor.OnInspectorGUI();
		}
	}
}