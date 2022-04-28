using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Linq;

namespace Assets.draco18s.crafting
{
#if UNITY_EDITOR
	[CanEditMultipleObjects]
    [CreateAssetMenu(menuName="Crafting/Recipe",fileName="New Recipe")]
#endif
    public class Recipe : ScriptableObject
    {
        public List<Ingredient> inputs;
		public List<Ingredient> outputs;
		public Factory building;
		public int time;

#if UNITY_EDITOR
		public void OnValidate() {
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
            if(string.IsNullOrEmpty(assetPath)) {
                return;
            }
            string dirName  = Path.GetDirectoryName(assetPath).Split(Path.DirectorySeparatorChar).Last();
            //Debug.Log(dirName);
			string[] a = AssetDatabase.FindAssets($"{dirName} t:Factory");
			if(a == null || a.Length == 0) return;
			string guid = a[0];
			if(a.Length > 0)
				guid = (a.First(x => AssetDatabase.GUIDToAssetPath(x).Split(Path.DirectorySeparatorChar).Last().Replace(".asset","") == dirName));
			
			Factory fact = AssetDatabase.LoadAssetAtPath<Factory>(AssetDatabase.GUIDToAssetPath(guid));
			building = fact;
        }

		public override string ToString() {
			return $"{name}";
		}
#endif
    }
}
