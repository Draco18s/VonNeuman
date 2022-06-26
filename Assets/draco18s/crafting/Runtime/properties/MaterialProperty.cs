using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.crafting.properties {
	[System.Serializable]
	public class MaterialProperty
	{
		protected Dictionary<string,object> values;
		public object this[string index]
		{
			get => values[index];
		}
		public virtual object this[string index, MaterialInstance item]
		{
			get => values[index];
		}

		public virtual void Init() {

		}

		public bool HasValue(string name) {
			return values.ContainsKey(name);
		}
	}
}