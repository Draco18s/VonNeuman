using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.draco18s.space.stellar;

namespace Assets.draco18s.gameAssets.Editor
{
	[CustomEditor(typeof(ElementMap))]
	public class ElementMapEditor : UnityEditor.Editor {
		Dictionary<ElementData,int> results = new Dictionary<ElementData,int>();
		
		public override void OnInspectorGUI()
		{
			SerializedProperty prop = serializedObject.FindProperty(nameof(ElementMap.elements));
        	EditorGUILayout.PropertyField(prop);
			if(GUILayout.Button("Get Random")) {
				results.Clear();
				((ElementMap)target).GetRandom(ElementType.METAL|ElementType.METALOID, 5).ToList().ForEach(x => {
					if(results.ContainsKey(x.Key))
						results[x.Key] += x.Value;
					else
						results[x.Key] = x.Value;
				});
			}
			GUILayout.Space(5);
			foreach(var ent in results.ToList().OrderByDescending(x => Mathf.Sqrt(x.Key.id) * (x.Value+2))) {
				EditorGUILayout.LabelField($"{ent.Key.name}*{ent.Value}: {Mathf.Sqrt(ent.Key.id) * (ent.Value+2)}");
			}
		}
	}
}
