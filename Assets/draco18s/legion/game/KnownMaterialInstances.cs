using System.Collections.Generic;
using Assets.draco18s.crafting;

namespace Assets.draco18s.legion.game {
	public class KnownMaterialInstances {
		protected Dictionary<Material,List<MaterialInstance>> knownItems;

		public KnownMaterialInstances() {
			knownItems = new Dictionary<Material,List<MaterialInstance>>();
		}

		public IReadOnlyCollection<MaterialInstance> Get(Material mat) {
			if(knownItems.TryGetValue(mat, out List<MaterialInstance> ret))
				return ret.AsReadOnly();
			return null;
		}

		public void MaterialFound(MaterialInstance mat) {
			if(knownItems.ContainsKey(mat.item)) {
				knownItems[mat.item].Add(mat);
			}
			else {
				knownItems.Add(mat.item, new List<MaterialInstance>(new MaterialInstance[]{ mat }));
			}
		}
	}
}