using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using System.Linq;
using Assets.draco18s.crafting.properties;

using MaterialProperty = Assets.draco18s.crafting.properties.MaterialProperty;

namespace Assets.draco18s.crafting
{
    [CreateAssetMenu(menuName="Crafting/Material",fileName="New Material")]
    public class Material : ScriptableObject
    {
        public string displayName;
        public string category;
        public float volume;
        public float mass;
		public string unlocalizedName => $"material.{displayName}.name";

		[SerializeReference]
		protected List<MaterialProperty> props = new List<MaterialProperty>();

#if UNITY_EDITOR
        public virtual void OnValidate() {
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
            if(string.IsNullOrEmpty(assetPath)) {
                return;
            }
            displayName = Path.GetFileNameWithoutExtension(assetPath);
            string dirName  = Path.GetDirectoryName(assetPath).Split(Path.DirectorySeparatorChar).Last();
            category = dirName;
        }
#endif
		public virtual void OnEnable() {
			props.ForEach(x => x.Init());
		}

		public T GetProperty<T>(string name) {
			MaterialProperty t = props.First(x => x.HasValue(name));
			return (T)t[name];
		}

		public T GetProperty<T>() where T:MaterialProperty {
			MaterialProperty t = props.FirstOrDefault(x => x is T);
			return (T)t;
		}

		public ReadOnlyCollection<MaterialProperty> GetAllProperties() {
			return props.AsReadOnly();
		}
    }
}
