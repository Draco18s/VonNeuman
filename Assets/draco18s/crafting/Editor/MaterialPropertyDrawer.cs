using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.draco18s.util.Editor;
using Assets.draco18s.crafting.properties;

using Object = UnityEngine.Object;
using Material = Assets.draco18s.crafting.Material;
using MaterialProperty = Assets.draco18s.crafting.properties.MaterialProperty;

namespace Assets.draco18s.crafting
{
	[CustomPropertyDrawer(typeof(MaterialProperty))]
	public class MaterialPropertyDrawer : PropertyDrawer {
		protected static float SPACING = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		protected static Dictionary<string,Type> reflectionTypeCache = new Dictionary<string,Type>();
		protected static Dictionary<string,FieldInfo[]> reflectionFieldCache = new Dictionary<string,FieldInfo[]>();

		protected void BaseOnGui(Rect position, SerializedProperty property, GUIContent label) {
			Type t;
			if(!reflectionTypeCache.ContainsKey(property.managedReferenceFullTypename)) {
				var parts = property.managedReferenceFullTypename.Split(' ');
				if (parts.Length != 2) return;
				var assemblyPart = parts[0];
				var nsClassnamePart = parts[1];
				t = Type.GetType($"{nsClassnamePart}, {assemblyPart}");
				reflectionTypeCache.Add(property.managedReferenceFullTypename, t);

				FieldInfo[] fieldsInfo = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
				reflectionFieldCache.Add(property.managedReferenceFullTypename, fieldsInfo);
			}
			else t = reflectionTypeCache[property.managedReferenceFullTypename];
			Rect foldoutRect = position;
			foldoutRect.height = EditorGUIUtility.singleLineHeight;
			property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, new GUIContent(t.Name));
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			BaseOnGui(position, property, label);
			if(!property.isExpanded) return;
			EditorGUIUtility.labelWidth *= 1.5f;
			Rect rect = EditorGUI.IndentedRect(position);
			rect.height = EditorGUIUtility.singleLineHeight;
			//Debug.Log($"Cache contains: {string.Join(",",reflectionFieldCache.Keys)}");
			FieldInfo[] fieldsInfo = reflectionFieldCache[property.managedReferenceFullTypename];
			foreach(FieldInfo info in fieldsInfo) {
				rect.y += SPACING;
				EditorGUI.PropertyField(rect, property.FindPropertyRelative(info.Name));
			}
			EditorGUIUtility.labelWidth /= 1.5f;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			FieldInfo[] fieldsInfo;
			if(!reflectionFieldCache.ContainsKey(property.managedReferenceFullTypename)) {
				var parts = property.managedReferenceFullTypename.Split(' ');
				if (parts.Length != 2) return SPACING;
				var assemblyPart = parts[0];
				var nsClassnamePart = parts[1];
				Type t = Type.GetType($"{nsClassnamePart}, {assemblyPart}");
				reflectionTypeCache.Add(property.managedReferenceFullTypename, t);

				fieldsInfo = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
				reflectionFieldCache.Add(property.managedReferenceFullTypename, fieldsInfo);
			}
			else fieldsInfo = reflectionFieldCache[property.managedReferenceFullTypename];
			float height = EditorGUIUtility.standardVerticalSpacing;
			foreach(FieldInfo info in fieldsInfo) {
				height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(info.Name), true);
			}
			
			return SPACING + (property.isExpanded ? (height) : 0) + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}