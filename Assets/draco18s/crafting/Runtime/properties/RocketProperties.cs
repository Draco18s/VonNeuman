using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.crafting.properties {
	[System.Serializable]
	public class RocketProperties : MaterialProperty
	{
		public float thrustCoefficient;
		[Tooltip("kg/s")]
		public float massFlowRate;

		public override void Init() {
			values = new Dictionary<string,object>();
			values.Add("thrustCoefficient",thrustCoefficient);
			values.Add("massFlowRate",massFlowRate);
		}
	}
}