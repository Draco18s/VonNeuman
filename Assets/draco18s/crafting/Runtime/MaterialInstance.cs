using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Assets.draco18s.util;
using Assets.draco18s.crafting.capabilities;
using Assets.draco18s.crafting.properties;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.draco18s.crafting
{
    public sealed class MaterialInstance {
		public readonly Material item;

		private float maxEnergy;
		public bool isConglomerate => item.volume != 1;

		// TODO: Minecraft like attributes?
		// Artificer like buff perk system?
		/*public int overallQualityRating { get; private set; }
		public int efficiencyRating { get; private set; }
		public int durabilityRating { get; private set; }
		public int massReductionRating { get; private set; }
		public int materialWastageRating { get; private set; }*/
		internal Dictionary<string,float> qualityModifiers = new Dictionary<string,float>();

		public MaterialInstance([NotNull] Material itemin) {
			item = itemin;
			if(item.HasProperty<PowerFuelProperties>()){
				PowerFuelProperties prop = item.GetProperty<PowerFuelProperties>();
				if(!prop.isMassConsumed) {
					maxEnergy = prop.energyDensity;
				}
			}
			qualityModifiers.Add("overallQualityRating", 1);
			qualityModifiers.Add("efficiencyRating", 1);
			qualityModifiers.Add("durabilityRating", 1);
			qualityModifiers.Add("massReductionRating", 1);
			qualityModifiers.Add("materialWastageRating", 1);
		}

		public MaterialInstance([NotNull] MaterialInstance instancein) : this(instancein.item) {
			qualityModifiers["overallQualityRating"] = qualityModifiers["overallQualityRating"];
			qualityModifiers["efficiencyRating"] = qualityModifiers["efficiencyRating"];
			qualityModifiers["durabilityRating"] = qualityModifiers["durabilityRating"];
			qualityModifiers["massReductionRating"] = qualityModifiers["massReductionRating"];
			qualityModifiers["materialWastageRating"] = qualityModifiers["materialWastageRating"];
		}

		public MaterialInstance Clone() {
			MaterialInstance r = new MaterialInstance(item);
			r.qualityModifiers.Add("overallQualityRating",qualityModifiers["overallQualityRating"]);
			r.qualityModifiers.Add("efficiencyRating",qualityModifiers["efficiencyRating"]);
			r.qualityModifiers.Add("durabilityRating",qualityModifiers["durabilityRating"]);
			r.qualityModifiers.Add("massReductionRating",qualityModifiers["massReductionRating"]);
			r.qualityModifiers.Add("materialWastageRating",qualityModifiers["materialWastageRating"]);
			return r;
		}

		public override bool Equals(object obj) {
			if(obj is MaterialInstance other)
				return item == other.item && maxEnergy < 1 && HasSameQualityModifiers(other);
			return false;
		}

		private bool HasSameQualityModifiers(MaterialInstance other) {
			return qualityModifiers.Keys.All(key => Mathf.Approximately(qualityModifiers[key], other.qualityModifiers[key]));
		}

		public override int GetHashCode() {
			return item.GetHashCode() * 131;
		}

		public float GetMass() {
			return item.mass / (isConglomerate ? 1000 : 1) * qualityModifiers["massReductionRating"];
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