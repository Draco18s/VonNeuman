using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.draco18s.space.stellar;

namespace Assets.draco18s.gameAssets.Editor
{
	[CustomEditor(typeof(SpriteMap))]
	public class SpriteMapEditor : UnityEditor.Editor {
		
		public override void OnInspectorGUI()
		{
			SerializedProperty prop = serializedObject.FindProperty("sprites");
        	if(DropAreaGUI(prop))
				serializedObject.ApplyModifiedProperties();
			EditorGUILayout.PropertyField(prop);
		}

		public bool DropAreaGUI(SerializedProperty prop)
		{
			Event evt = Event.current;
			Rect drop_area = GUILayoutUtility.GetRect (0.0f, 50.0f, GUILayout.ExpandWidth (true));
			GUI.Box (drop_area, "Add Sprite");
		
			switch (evt.type) {
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!drop_area.Contains (evt.mousePosition))
					return false;
				
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			
				if (evt.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag ();
				
					foreach (Object dragged_object in DragAndDrop.objectReferences) {

						if(dragged_object is Texture2D) {
							LoadAllSprites((Texture2D)dragged_object, prop);
							continue;
						}
						if(!(dragged_object is Sprite)) {
							continue;
						}
						if(Duplicate(prop, (Sprite)dragged_object)) continue;
						int i = prop.arraySize;
						prop.InsertArrayElementAtIndex(i);
						SerializedProperty element = prop.GetArrayElementAtIndex(i);
						element.FindPropertyRelative(nameof(SpriteData.sprite)).objectReferenceValue = dragged_object;
						element.FindPropertyRelative(nameof(SpriteData.spriteId)).stringValue = dragged_object.name;
					}
					return true;
				}
				break;
			}
			return false;
		}

		static void LoadAllSprites(Texture2D tex, SerializedProperty prop) {
			string spriteSheet = AssetDatabase.GetAssetPath(tex);
			foreach(Sprite sprite in AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray()) {
				if(Duplicate(prop, sprite)) continue;
				int i = prop.arraySize;
				prop.InsertArrayElementAtIndex(i);
				SerializedProperty element = prop.GetArrayElementAtIndex(i);
				element.FindPropertyRelative(nameof(SpriteData.sprite)).objectReferenceValue = sprite;
				element.FindPropertyRelative(nameof(SpriteData.spriteId)).stringValue = sprite.name;
			}
		}

		static bool Duplicate(SerializedProperty prop, Sprite sprite) {
			for(int i = 0; i < prop.arraySize; i++) {
				SerializedProperty element = prop.GetArrayElementAtIndex(i);
				if(element.FindPropertyRelative("sprite").objectReferenceValue == sprite) return true;
			}
			return false;
		}
	}
}
