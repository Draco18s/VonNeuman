using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.crafting.properties {
	[System.Serializable]
	public class RocketFuelProperties : MaterialProperty
	{
		public float energyDensity;
		public OxyEnum oxidizer;

		public override void Init() {
			values = new Dictionary<string,object>();
			values.Add("energyDensity",energyDensity);
			values.Add("oxidizer",oxidizer);
		}

		[Flags]
		public enum OxyEnum {
			NONE = 0,
			OXIDIZER = 1<<0,
			NEEDS_OXIDIZER = 1<<1,
			MONOPROPELLANT = 1<<2
		}
	}
}