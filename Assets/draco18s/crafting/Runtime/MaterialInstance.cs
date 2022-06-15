using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Assets.draco18s.util;
using Assets.draco18s.crafting.capabilities;
using Assets.draco18s.crafting.properties;

namespace Assets.draco18s.crafting
{
    public sealed class MaterialInstance {
		public readonly Material item;

		private float curEnergy;
		private float maxEnergy;
		public bool isConglomerate => item.volume != 1;

		// TODO: Minecraft like attributes?
		// Artificer like buff perk system?
		/*public int overallQualityRating { get; private set; }
		public int efficiencyRating { get; private set; }
		public int durabilityRating { get; private set; }
		public int massReductionRating { get; private set; }
		public int materialWastageRating { get; private set; }*/

		public MaterialInstance(Material _item) {
			item = _item;
			if(item.HasProperty<PowerFuelProperties>()){
				PowerFuelProperties prop = item.GetProperty<PowerFuelProperties>();
				if(!prop.isMassConsumed) {
					maxEnergy = prop.energyDensity;
				}
			}
			//overallQualityRating = efficiencyRating = durabilityRating = massReductionRating = materialWastageRating = 1;
		}

		// TOOD: incomplete copy
		public MaterialInstance Clone() {
			return new MaterialInstance(item);
		}

		public override bool Equals(object obj) {
			if(obj is MaterialInstance other)
				return item == other.item && maxEnergy < 1;
			return false;
		}

		public override int GetHashCode() {
			return item.GetHashCode() * 131;
		}

		public float GetMass() {
			return item.mass / (isConglomerate ? 1000 : 1);
		}

		public float GetVolume() {
			return item.volume / (isConglomerate ? 1000 : 1);
		}

		public T GetProperty<T>(string name) {
			return item.GetProperty<T>(name);
		}

		public bool HasProperty(string name) {
			return item.HasProperty(name);
		}

		public bool HasProperty<T>()where T:MaterialProperty {
			return item.HasProperty<T>();
		}

		public T GetProperty<T>() where T:MaterialProperty {
			return item.GetProperty<T>();
		}

		public ReadOnlyCollection<MaterialProperty> GetAllProperties() {
			return item.GetAllProperties();
		}
    }
}