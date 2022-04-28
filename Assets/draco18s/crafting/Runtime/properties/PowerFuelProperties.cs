using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.crafting.properties {
	[System.Serializable]
	public class PowerFuelProperties : MaterialProperty
	{
		public float energyDensity;
		public float consumeRate;
		public bool isHalfLife;

		public override void Init() {
			values = new Dictionary<string,object>();
			values.Add("energyDensity",energyDensity);
			values.Add("consumeRate",consumeRate);
			values.Add("isHalfLife",isHalfLife);
		}
	}
}