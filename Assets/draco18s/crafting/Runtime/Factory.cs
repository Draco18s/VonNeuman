using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

namespace Assets.draco18s.crafting
{
    [CreateAssetMenu(menuName="Crafting/Factory",fileName="New Factory")]
    public class Factory : ScriptableObject
    {
        [Flags]
        public enum FactoryRequirements {
            NONE = 0,
            LIGHT = 1<<0,
            SOIL = 1<<1,
            ATMO = 1<<2,
            LIQUID = 1<<3,
            ORE = 1<<4,
        }
        public string displayName;
        [TextArea(3,5)]
        public string description;
        public int area;
        public int powerUse;
        public FactoryRequirements reqs;
		public Recipe constructionCost;
		public string unlocalizedName => $"material.{displayName}.name";

#if UNITY_EDITOR
        public void OnValidate() {
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
            if(string.IsNullOrEmpty(assetPath)) {
                return;
            }
            displayName = Path.GetFileNameWithoutExtension(assetPath);
			//constructionCost = null;
			if(constructionCost == null && !string.IsNullOrEmpty(displayName)) {
				string path = $"Assets/Scriptables/Crafting/Recipes/Buildings/{displayName}.asset";
				AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<Recipe>(), path);
				constructionCost = AssetDatabase.LoadAssetAtPath(path, typeof(Recipe)) as Recipe;
			}
        }
#endif
    }
}
