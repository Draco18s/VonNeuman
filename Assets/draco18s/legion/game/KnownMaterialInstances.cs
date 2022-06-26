using System.Collections.Generic;
using System.Linq;
using Assets.draco18s.crafting;
using Assets.draco18s.gameAssets;
using Assets.draco18s.serialization;
using Assets.draco18s.util;

namespace Assets.draco18s.legion.game {
	public class KnownMaterialInstances {
		protected Dictionary<Material,List<MaterialInstance>> knownItems;
		private static List<Recipe> naturalRecipes;
		private ElementMap baseElements;

		public KnownMaterialInstances() {
			baseElements ??= ScriptableObjectRegistry.GetRegistry<ElementMap>();
			naturalRecipes ??= ScriptableObjectRegistry.GetRegistry2<Recipe>().Where(x =>
				x.building == default && x.outputs.Count() == 1 && !x.name.Contains("Melt")).ToList();
			AddAllNaturalInstances();
		}
		
		private void AddAllNaturalInstances() {
			knownItems = baseElements
				.GroupBy(e => e.material)
				.ToDictionary(
					g => g.Key,
					g => g.Select(_ => new MaterialInstance(g.Key)).ToList()
				);
			naturalRecipes
				.GroupBy(e => e.outputs.First().mat)
				.ToDictionary(
					g => g.Key,
					g => g.Select(_ => new MaterialInstance(g.Key)).ToList()
				).ForEach(x => knownItems.Add(x.Key, x.Value));
			foreach(Ingredient output in naturalRecipes.SelectMany(recipe => recipe.outputs)) {
				if(!knownItems.ContainsKey(output.mat)) knownItems.Add(output.mat, new List<MaterialInstance>());
				knownItems[output.mat].Add(new MaterialInstance(output.mat));
			}
		}

		public IReadOnlyCollection<MaterialInstance> Get(Material mat) {
			return knownItems.TryGetValue(mat, out List<MaterialInstance> ret) ? ret.AsReadOnly() : null;
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