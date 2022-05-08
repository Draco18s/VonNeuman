using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.crafting.properties {
	[System.Serializable]
	public class PowerFuelProperties : MaterialProperty
	{
		public float energyDensity;
		public float consumeRate;
		public bool isMassConsumed;
		public bool isHalfLife;

		public override void Init() {
			values = new Dictionary<string,object>();
			values.Add("energyDensity",energyDensity); //MJ/kg or kWh per unit (3.6 kWh per MJ)
			values.Add("consumeRate",consumeRate); //volume per kWh (mass consumed), kW depleted per hour (battery charge), or half-life years
			values.Add("isMassConsumed",isMassConsumed);
			values.Add("isHalfLife",isHalfLife);
		}
	}
}