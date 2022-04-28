using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using Assets.draco18s.serialization;

namespace Assets.draco18s.gameAssets.Editor
{
	[CustomEditor(typeof(ScriptableMap))]
	public class ScriptableMapEditor : UnityEditor.Editor {
		private GenericMenu menu;

		private struct MenuData {
			public Type type;
			public SerializedProperty prop;
		}

		void OnEnable() {
			var scriptableTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes())
				.Where(t => System.Attribute.GetCustomAttributes(t).Any(attr => (attr is CreateAssetMenuAttribute)) && t != typeof(ScriptableMap));
			menu = new GenericMenu();
			foreach(var type in scriptableTypes.OrderBy(i => i.FullName.Split('.').Last())) {
				MenuData md = new MenuData() {
					type = type,
					prop = serializedObject.FindProperty(nameof(ScriptableMap.scriptables))
				};
				menu.AddItem(new GUIContent(type.FullName.Split('.').Last()), false, AddFromMenu, md);
			}
		}

		private void AddFromMenu(object param) {
			MenuData md = (MenuData)param;
			Type t = md.type;
			SerializedProperty prop = md.prop;
			Type mapGeneric = typeof(ScriptableObjectMap<>);
			Type mapConcrete = mapGeneric.MakeGenericType(new Type[] {t});
			var obj = Activator.CreateInstance(mapConcrete);
			var so = ScriptableObject.CreateInstance<ScriptableGenerics>();
			so.Set(obj);
			AssetDatabase.CreateAsset(so, $"Assets/Scriptables/{t.FullName.Split('.').Last()}Map.asset");
			//Debug.Log(AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(prop.serializedObject.targetObject)).assetBundleName);
			//return;
			AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(so)).assetBundleName = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(prop.serializedObject.targetObject)).assetBundleName;
			int index = prop.arraySize++;
			prop.GetArrayElementAtIndex(index).objectReferenceValue = so;
			prop.serializedObject.ApplyModifiedProperties();
		}

		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			if(GUILayout.Button("New Scriptable Map Asset") ){
				menu.ShowAsContext();
			}
		}
	}
}