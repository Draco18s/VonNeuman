using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.crafting.properties {
	[System.Serializable]
	public class AtomicProperties : MaterialProperty
	{
		public float molarMass;
		public float greenhousePotential;
		public float meltingPoint;
		public float boilingPoint;
		public float albedo;

		public override void Init() {
			values = new Dictionary<string,object>();
			values.Add("molarMass",molarMass);
			values.Add("greenhousePotential",greenhousePotential);
			values.Add("meltingPoint",meltingPoint);
			values.Add("boilingPoint",boilingPoint);
			values.Add("albedo",albedo);
		}
	}
}