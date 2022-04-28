using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.draco18s.crafting.properties;

namespace Assets.draco18s.crafting.Editor
{
	[CustomPropertyDrawer(typeof(ContainerProperties))]
	public class ContainerEditor : MaterialPropertyDrawer {
		private static GenericMenu menu;
		private static List<string> containerCategories = new List<string>();
		//protected static float SPACING = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		private SerializedProperty containProp;

		private static void RebuildMenu(SerializedProperty containProp) {
			//SerializedProperty containProp = property.FindPropertyRelative("canContainCategory");
			menu = new GenericMenu();
			containerCategories.Clear();
			for(int i = 0; i < containProp.arraySize; i++) {
				containerCategories.Add(containProp.GetArrayElementAtIndex(i).stringValue);
			}
			if(containProp.arraySize == 0) {
				containerCategories.Add("Nothing");
			}
			if(containProp.arraySize == MaterialEditor.categories.Length-2) {
				containerCategories.Add("Everything");
			}
			foreach(string cat in MaterialEditor.categories) {
				menu.AddItem(new GUIContent(cat), containerCategories.Contains(cat), OnCategorySelected, new object[] { containProp, cat });
			}
		}

		private static void OnCategorySelected(object prms)
		{
			object[] prmsArr = (object[])prms;
			SerializedProperty property = (SerializedProperty)prmsArr[0];
			string cat = (string)prmsArr[1];
			SerializedProperty containProp = property;//.FindPropertyRelative("canContainCategory");
			if(cat == "Nothing") {
				containProp.ClearArray();
			}
			else if(cat == "Everything") {
				containProp.ClearArray();
				if(!containerCategories.Contains(cat)) {
					foreach(string cat2 in MaterialEditor.categories) {
						if(cat2 == "Nothing") continue;
						if(cat2 == "Everything") continue;
						containProp.InsertArrayElementAtIndex(containProp.arraySize);
						containProp.GetArrayElementAtIndex(containProp.arraySize-1).stringValue = cat2;
					}
				}
			}
			else {
				if(containerCategories.Contains(cat)) {
					int i = containerCategories.IndexOf(cat);
					containProp.DeleteArrayElementAtIndex(i);
				}
				else {
					containProp.InsertArrayElementAtIndex(containProp.arraySize);
					containProp.GetArrayElementAtIndex(containProp.arraySize-1).stringValue = cat;
				}
			}
			property.serializedObject.ApplyModifiedProperties();
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Rect rect = EditorGUI.IndentedRect(position);
			rect.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.PropertyField(rect, property.FindPropertyRelative("massCapacity"), new GUIContent("Internal Mass Limit"));
			rect.y += SPACING;
			EditorGUI.PropertyField(rect, property.FindPropertyRelative("volumeCapacity"), new GUIContent("Internal Volume"));
			rect.y += SPACING;
			SerializedProperty containProp = property.FindPropertyRelative("canContainCategory");
			
			if (GUI.Button(rect, "Can Contain...")) {
				RebuildMenu(containProp);
				menu.ShowAsContext();
			}
			
			property.serializedObject.ApplyModifiedProperties();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			return SPACING*3;
		}

		/*protected override void OnEnable() {
			base.OnEnable();
			List<string> temp = new List<string>();
			temp.Add("Nothing");
			temp.Add("Everything");
			temp.AddRange(categories);
			categories = temp.ToArray();
			RebuildMenu();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("massCapacity"), new GUIContent("Internal Mass Limit"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("volumeCapacity"), new GUIContent("Internal Volume"));
			
			if (GUILayout.Button("Can Contain...")) {
				RebuildMenu();
				menu.ShowAsContext();
			}
			
			serializedObject.ApplyModifiedProperties();
		}*/
	}	
}
