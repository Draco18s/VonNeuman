using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;
using System.Linq;

using MaterialProperty = Assets.draco18s.crafting.properties.MaterialProperty;

namespace Assets.draco18s.crafting.Editor
{
	[CustomEditor(typeof(Material))]
	public class MaterialEditor : UnityEditor.Editor {
		//Material material;
		protected ReorderableList propertyList;
		internal static string[] categories;
		int index = -1;

		protected virtual void OnEnable()
		{
			//material = (Material)target;
			string[] a = AssetDatabase.FindAssets("t:CategoryList");
			if(a != null && a.Length > 0) {
				CategoryList list = AssetDatabase.LoadAssetAtPath<CategoryList>(AssetDatabase.GUIDToAssetPath(a[0]));
				categories = list.categories.ToArray();
			}
			SerializedProperty listProp = serializedObject.FindProperty("props");
			propertyList = new ReorderableList(serializedObject, listProp) {
				drawHeaderCallback = rect => {
					EditorGUI.LabelField(rect, "Material Properties");
				},
				onAddCallback = list => {
					AddPropertyMenu(listProp).ShowAsContext();
				},
				drawElementCallback = (rect, index, active, focused) => {
					var elem = propertyList.serializedProperty.GetArrayElementAtIndex(index);
					EditorGUI.BeginChangeCheck();
					EditorGUI.indentLevel++;
					EditorGUI.PropertyField(rect, elem);
					if(EditorGUI.EndChangeCheck()) {
						listProp.serializedObject.ApplyModifiedProperties();
					}
					EditorGUI.indentLevel--;
				},
				elementHeightCallback = index => {
					if(propertyList.serializedProperty.arraySize == 0) return EditorGUIUtility.singleLineHeight;
					return EditorGUI.GetPropertyHeight(propertyList.serializedProperty.GetArrayElementAtIndex(index), true);
				}
			};
		}

		public override void OnInspectorGUI()
		{
			string cat = serializedObject.FindProperty(nameof(Material.category)).stringValue;
			if(index < 0) {
				index = Array.IndexOf(categories, cat);
			}
			serializedObject.Update();
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Material.displayName)), new GUIContent("Name"));
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(index >= 0);
			index = EditorGUILayout.Popup(index, categories);
			if(index >= 0)
				serializedObject.FindProperty(nameof(Material.category)).stringValue = categories[index];
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.BeginHorizontal();
			EditorGUIUtility.labelWidth /= 1.5f;
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Material.mass)), new GUIContent("Mass (tons)"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Material.volume)), new GUIContent("Volume (m³)"));
			EditorGUILayout.EndHorizontal();

			propertyList.DoLayoutList();


			bool b = serializedObject.hasModifiedProperties;
			serializedObject.ApplyModifiedProperties();
			EditorGUIUtility.labelWidth *= 1.5f;
		}

		private GenericMenu AddPropertyMenu(SerializedProperty listProp) {
			GenericMenu menu = new GenericMenu();
			List<Type> types = GetTypes(typeof(MaterialProperty));
			foreach(Type t in types) {
				menu.AddItem(new GUIContent(t.Name), false, AddPropertyToList, new object[] { t, listProp });
			}
			return menu;
		}

		private static void AddPropertyToList(object prams) {
			if(!(prams is object[] pramsarr) || !(pramsarr[0] is Type t) || !(pramsarr[1] is SerializedProperty listProp)) return;

			MaterialProperty newProp = Activator.CreateInstance(t) as MaterialProperty;
			int i = listProp.arraySize;
			listProp.InsertArrayElementAtIndex(i);
			var elem = listProp.GetArrayElementAtIndex(i);
			elem.managedReferenceValue = newProp;
			elem.isExpanded = true;

			listProp.serializedObject.ApplyModifiedProperties();
		}

		private static List<Type> GetTypes(Type paramType) {
			List<Type> derivedTypes = new List<Type>();
			foreach(var domAss in AppDomain.CurrentDomain.GetAssemblies()) {
				derivedTypes.AddRange(
					domAss.GetTypes().Where(type => type.IsSubclassOf(paramType) && !type.IsAbstract)
				);
			}
			return derivedTypes;
		}
	}	
}
