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

		public virtual void Tick(float delta) {
			if(GetTotalMass() == GetDryMass() && velocityKms.sqrMagnitude > 0) {
				//finding fuel in storage cargo and moving it to fuel tanks
				/*foreach(var part in parts) {
					if(part is Container cont) {
						int fuel = cont.RemoveFromCargo(ResourceType.VOLATILES, 5000);
						parts.ForEach(x => {
							if(x is IFuelTank tn) {
								fuel = tn.ReFuel(fuel);
							}
						});
					}
				}*/
			}
			timeStep += delta;
			while(timeStep > 1440) {
				if(velocityKms.sqrMagnitude > 0) {
					if(inSystem && timeStep >= 1000) {
						//process 1000 seconds of movement at km/s in 1 unit = 1 AU
						timeStep -= 1000;
						pos += (velocityKms / auDivisor);
					}
					else if(timeStep >= 100000000) {
						//process 1000 seconds of movement at km/s in 1 unit = 1 ly
						timeStep += delta;
						timeStep -= 100000000;
						pos += (velocityKms / lyDivisor);
					}
				}
				else {
					timeStep -= 1440;
				}
				ProcessOrders();
			}
		}

		public float GetTotalMass() {
			float m = 0;
			parts.ForEach(x => m += x.GetTotalMass());
			return m*1000 + GetFrameMass();
		}

		public float GetFrameMass() {
			return 400;// * parts.Count;
		}

		public float GetDryMass() {
			float m = 0;
			parts.ForEach(x => m += x.GetDryMass());

			/*parts.ForEach(x => {
				if(x is IFuelTank) {
					IFuelTank th = (IFuelTank)x;
					m += th.GetDryMass();
				}
				else {
					m += x.GetMass();
				}
			});*/
			return m*1000 + GetFrameMass();
		}

		//this should really be stored somewhere else
		private const float gameTimeSliceScalar = 10;

		/// <summary>
		/// Calculates burn time to achieve the desired change in velocityKms (m/s)
		/// </summary>
		/// <param name="vel">meters per second</param>
		/// <returns>time in time slices</returns>
		public static int CalculateBurnTimeToReachVelocityKms(BaseUnit ship, float vel, int draw) {
			float totThrust = ship.GetTotalThrust();
			float totMass = ship.GetTotalMass();
			float dryMass = ship.GetDryMass();
			float toBurn = totMass / (Mathf.Exp(vel / (totThrust * gameTimeSliceScalar)));
			if(totMass-dryMass < 0.1f) return -1;
			float t = (totMass - toBurn) / draw;
			return Mathf.FloorToInt(t);
		}

		public static float GetVelocityKmsFromBurn(BaseUnit ship, int draw) {
			float totThrust = ship.GetTotalThrust();
			float totMass = ship.GetTotalMass();
			float dryMass = totMass - draw;
			return totThrust * gameTimeSliceScalar * Mathf.Log((float)totMass / dryMass);
		}

		public int TotalDeltaV() {
			float totThrust = GetTotalThrust();
			float totMass = GetTotalMass();
			float dryMass = GetDryMass();
			return Mathf.FloorToInt(totThrust * gameTimeSliceScalar * Mathf.Log((float)totMass / dryMass));
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