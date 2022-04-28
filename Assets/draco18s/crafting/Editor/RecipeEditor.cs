using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Assets.draco18s.crafting.Editor
{
	[CustomEditor(typeof(Recipe))]
	public class RecipeEditor : UnityEditor.Editor {
		Recipe material;

		void OnEnable()
		{
			material = (Recipe)target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("inputs"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("outputs"));

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("building"));
			EditorGUI.EndDisabledGroup();
			
			SerializedProperty time = serializedObject.FindProperty("time");
			EditorGUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth /= 2;
			int hr = (time.intValue / 60) / 60;
			int min = (time.intValue / 60) - hr * 60;
			hr = EditorGUILayout.DelayedIntField("Hours", hr);
			min = EditorGUILayout.DelayedIntField("Minutes", min);
			time.intValue = (hr*60+min)*60;
			EditorGUIUtility.labelWidth *= 2;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.IntField(GUIContent.none, time.intValue, GUILayout.Width(70));
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
			serializedObject.ApplyModifiedProperties();
		}
	}	
}
