using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Assets.draco18s.space.planetary;
using Assets.draco18s.util;
using Assets.draco18s.gameAssets;
using Assets.draco18s.serialization;
using Assets.draco18s.crafting;

namespace Assets.draco18s.crafting {
	[JsonResolver(typeof(StructureConverter))]
	public class BaseStructure : ITickable
	{
		public Transform transform {get;set;}
		public readonly Factory factory;
		public float processingTime { get; protected set; }
		public List<Recipe> currentRecipes;
		protected bool canProcess;
		protected int processIndex;
		protected OrbitalBody location;

		public BaseStructure(Factory factoryType) {
			factory = factoryType;
			currentRecipes = new List<Recipe>();
		}

		public void SetLocation(OrbitalBody surface) {
			location = surface;
		}

		public virtual void Tick(float deltaTime) {
			if(!canProcess) {
				processingTime = 0;
				for(processIndex = 0; processIndex < currentRecipes.Count; processIndex++) {
					canProcess = CheckIfCanStart();
					if(canProcess) {
						ConsumeIngredients();
						return;
					}
				}
				return;
			}
			processingTime += deltaTime;
			if(processingTime >= currentRecipes[processIndex].time) {
				//do outputs
				canProcess = false;
			}
		}

		protected bool CheckIfCanStart() {
			return false;
		}

		public void ConsumeIngredients() {

		}

		private class StructureConverter : JsonConverter {
			public override bool CanConvert(System.Type objectType) {
				return typeof(BaseStructure).IsAssignableFrom(objectType);
			}

			public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) {
				JObject jObject = JObject.Load(reader);
				return null;
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				BaseStructure v = (BaseStructure)value;
				JObject o = new JObject();
				o.Add(new JProperty("factory", v.factory.name));
				o.WriteTo(writer);
			}

			public override bool CanRead {
				get { return true; }
			}
		}
	}
}