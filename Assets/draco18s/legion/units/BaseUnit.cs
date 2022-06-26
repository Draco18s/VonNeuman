using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.draco18s.util;
using Assets.draco18s.crafting;
using Assets.draco18s.legion.units.parts;

using Material = Assets.draco18s.crafting;

namespace Assets.draco18s.legion.units {
	public class BaseUnit : ITickable {
		private const float lyDivisor = 94500f;
		private const float auDivisor = 149600f;

		public Transform transform {get;set;}
		public Vector3 pos;
		public Vector3 velocityKms;
		protected float timeStep = 0;
		protected bool inSystem;
		protected List<UnitComponent> parts;

		public virtual void Tick(float realtimedelta) {
			timeStep += realtimedelta;
			if(velocityKms.sqrMagnitude > 0) {
				if(inSystem && timeStep >= 1000) {
					//process 1000 seconds of movement at km/s in 1 unit = 1 AU
					timeStep -= 1000;
					pos += (velocityKms / auDivisor);
				}
				else if(timeStep >= 1000 * lyDivisor) {
					//process 1000 seconds of movement at km/s in 1 unit = 1 ly
					timeStep += realtimedelta;
					timeStep -= 1000 * lyDivisor;
					pos += (velocityKms / lyDivisor);
				}
			}
			else {
				timeStep -= 1440;
			}
			ProcessOrders();
		}

		public float GetTotalMass() {
			float m = 0;
			parts.ForEach(x => m += x.GetTotalMass());
			return m*1000 + GetFrameMass();
		}

		public float GetFrameMass() {
			return 400;// placeholder
		}

		public float GetDryMass() {
			float m = 0;
			parts.ForEach(x => m += x.GetDryMass());
			return m*1000 + GetFrameMass();
		}

		/// <summary>
		/// Calculates burn time to achieve the desired change in velocityms (m/s)
		/// </summary>
		/// <param name="vel">meters per second</param>
		/// <returns>time in time slices</returns>
		public static int CalculateBurnTimeToReachVelocityKms(BaseUnit ship, float vel, int draw) {
			float totThrust = ship.GetTotalThrust();
			float totMass = ship.GetTotalMass();
			float dryMass = ship.GetDryMass();
			float toBurn = totMass / (Mathf.Exp(vel / (totThrust)));
			if(totMass-dryMass < 0.1f) return -1;
			float t = (totMass - toBurn) / draw;
			return Mathf.FloorToInt(t);
		}

		public static float GetVelocityKmsFromBurn(BaseUnit ship, int draw) {
			float totThrust = ship.GetTotalThrust();
			float totMass = ship.GetTotalMass();
			float dryMass = totMass - draw;
			return totThrust * Mathf.Log((float)totMass / dryMass);
		}

		public int TotalDeltaV() {
			float totThrust = GetTotalThrust();
			float totMass = GetTotalMass();
			float dryMass = GetDryMass();
			return Mathf.FloorToInt(totThrust * Mathf.Log((float)totMass / dryMass));
		}

		public int GetTotalThrust() {
			int m = 0;
			//Get propellants
			//var characteristicVelocity = Math.Pow(fuel.energyDensity,0.275f)*oxidizer.energyDensity*100; // meters/sec
			//var exhaustVel = characteristicVelocity * motor.thrustCoefficient;
			//var m = exhaustVel;
			
			//old
			/*parts.ForEach(x => {
				if(x is IThrustProvider th) {
					m += th.GetThrust();
				}
				//if(x is LightSail) {
				//	StarSystem sys = Main.instance.GetNearest(pos);
				//	if(sys.planets.Any(p => p.GetStructures().Any(s => s is LaserAccelerator))) {
				//		sys.planets.Find(p => p.GetStructures().Any(s => s is LaserAccelerator))
				//		.GetStructures().Where(s => s is LaserAccelerator).ToList().ForEach(t => {
				//			LaserAccelerator l = (LaserAccelerator)t;
				//			m += l.GetThrustFor(this);
				//		});
				//		//LaserAccelerator laser = (LaserAccelerator)strc;
				//	}
				//}
			});*/
			return m;
		}

		private void ProcessOrders() {
		}
	}
}