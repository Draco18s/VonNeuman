using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Assets.draco18s.translation;

namespace Assets.draco18s.translation.Editor {
	[CustomEditor(typeof(Localizer))]
	public class LocalizerInspector : UnityEditor.Editor {
		static GUIStyle BoldFoldout {
			get {
				if(null == boldFoldout) {
					boldFoldout = new GUIStyle(EditorStyles.foldout);
					boldFoldout.fontStyle = FontStyle.Bold;
				}
				return boldFoldout;
			}
		}
		static GUIStyle boldFoldout = null;
		ReorderableList strings;
		int focusedIndex = -1;

		SerializedProperty stringKeys;
		SerializedProperty stringValues;
		private void OnEnable() {
			Localizer localz = (Localizer)target;
			
			stringKeys = serializedObject.FindProperty("stringKeys");
			stringValues = serializedObject.FindProperty("stringValues");
			strings = new ReorderableList(serializedObject, stringKeys, false, true, true, true);
			strings.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Localization entries", EditorStyles.boldLabel);
			strings.onAddCallback = list => {
				stringKeys.arraySize++;
				stringValues.arraySize++;
				stringKeys.GetArrayElementAtIndex(stringKeys.arraySize - 1).stringValue = Guid.NewGuid().ToString();
				stringValues.GetArrayElementAtIndex(stringKeys.arraySize - 1).stringValue = string.Empty;
				serializedObject.ApplyModifiedProperties();
			};
			strings.onRemoveCallback = list => {
				if(focusedIndex >= 0 && focusedIndex < stringKeys.arraySize) {
					stringKeys.DeleteArrayElementAtIndex(focusedIndex);
					stringValues.DeleteArrayElementAtIndex(focusedIndex);
					serializedObject.ApplyModifiedProperties();
				}
			};
			strings.drawElementCallback = (rect, index, isActive, isFocused) => {
				EditorGUI.indentLevel++;
				if(isFocused)
					focusedIndex = index;
				EditorGUI.BeginChangeCheck();
				Rect keyRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
				SerializedProperty curStringKey = stringKeys.GetArrayElementAtIndex(index);
				string origValue = curStringKey.stringValue;
				EditorGUI.DelayedTextField(keyRect, curStringKey, GUIContent.none);
				if(EditorGUI.EndChangeCheck() && !Localization.ValidKey(curStringKey.stringValue)) {
					Debug.Log(curStringKey.stringValue + " already exists!");
					curStringKey.stringValue = origValue;
				}
				Rect valueRect = new Rect(keyRect.xMax, rect.y, EditorGUIUtility.currentViewWidth - keyRect.width, rect.height);
				EditorGUI.PropertyField(valueRect, stringValues.GetArrayElementAtIndex(index), GUIContent.none);
				EditorGUI.indentLevel--;
			};
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			Localizer localz = (Localizer)target;

			localz.LanguageKey = EditorGUILayout.TextField("Language Key", localz.LanguageKey);

			strings.DoLayoutList();
			EditorGUILayout.Separator();

			if(GUILayout.Button("Add or Remove Enum")) {
				EnumSelectorEditor.OpenEnumSelectorEditorWindow();
			}
			EditorGUILayout.Separator();

			SerializedProperty serializedEnumTranslations = serializedObject.FindProperty("serializedEnumTranslations");
			for(int n = 0; n < serializedEnumTranslations.arraySize; n++) {
				SerializedProperty serializedEnumTranslation = serializedEnumTranslations.GetArrayElementAtIndex(n);
				Type tp = Type.GetType(serializedEnumTranslation.FindPropertyRelative("asmQualName").stringValue);
				serializedEnumTranslation.isExpanded = EditorGUILayout.Foldout(serializedEnumTranslation.isExpanded, tp.Name, true, BoldFoldout);

				SerializedProperty keys = serializedEnumTranslation.FindPropertyRelative("keys");
				SerializedProperty vals = serializedEnumTranslation.FindPropertyRelative("values");
				if(serializedEnumTranslation.isExpanded) {
					EditorGUI.indentLevel++;
					for(int i = 0; i < keys.arraySize; i++){
						SerializedProperty key = keys.GetArrayElementAtIndex(i);
						SerializedProperty val = vals.GetArrayElementAtIndex(i);
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(Enum.GetName(tp, key.intValue));
						EditorGUILayout.PropertyField(val, GUIContent.none);
						EditorGUILayout.EndHorizontal();
					}
					EditorGUI.indentLevel--;
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		private bool DoesValueExistIn(SerializedProperty keys, int v) {
			for(int i = 0; i < keys.arraySize; i++) {
				SerializedProperty key = keys.GetArrayElementAtIndex(i);
				if(key.intValue == v) {
					return true;
				}
			}
			return false;
		}
	}
}
