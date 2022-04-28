using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Assets.draco18s.space.stellar;
using Assets.draco18s.gameAssets;
using Assets.draco18s.crafting;
using Assets.draco18s.serialization;
using Material = Assets.draco18s.crafting.Material;

namespace Assets.draco18s.space.planetary {
	[JsonResolver(typeof(OrbitalConverter))]
	public abstract class OrbitalBody
	{
		protected const float G = 0.012903f;//G measured in au⋅Mⴲ⁻¹⋅(km/s)², times 1000
		//or at least it should be, but all I know is I took the solar values and did
		//the math to get earth-relative values instead
		//and it comes out shockingly close G measured in ft³/(slug s²): .00001291
		protected const float R = 0.000207f;//1 Rⴲ as a fraction of 1 au, times 1000
		//or at least it should be, I took the solar values and did the math to get
		//earth-relative values
		//DON'T ASK, IT WORKS, OK?!
		protected const float mol = 60.2214076f;
		protected const float K = 1.38f;
		public float escapeVelocity => GetASDF();

		public float GetASDF() {
			float e = Mathf.Sqrt(2*G*mass/(R*radius));
			if(float.IsNaN(e)) {
				Debug.Log($"ABORT! 1 {radius}");
				Debug.Log($"ABORT! 2 {(R*radius)}");
				Debug.Log($"ABORT! 3 {(mass)}");
				Debug.Log($"ABORT! 4 {(2*G*mass/(R*radius))}");
			}
			return e;
		}

		protected StarSystem system;
		public float orbitalDistance {get; protected set; }
		public float radius {get; protected set; }
		public float mass {get; protected set; }
		public float averageSurfaceTemp {get; protected set; }
		public SpriteData spriteData;
		protected Material[] resources;
		protected List<BaseStructure> structures;

		public virtual Material[] GetAtmosphere() {
			return new Material[0];
		}

		private class OrbitalConverter : JsonConverter {
			public override bool CanConvert(System.Type objectType) {
				return typeof(OrbitalBody).IsAssignableFrom(objectType);
			}

			public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) {
				JObject jObject = JObject.Load(reader);
				string classname = (string)jObject.GetValue("class");
				Type type = Type.GetType(classname);
				if(type == null) throw new JsonReaderException($"Unable to read {classname}");
				JsonConverter converter = ContractResolver.jsonSettings.ContractResolver.ResolveContract(type).Converter;
				return converter.ReadJson(reader, objectType, jObject, serializer);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				throw new JsonWriterException("Json writer converter for abstract classes is not implemented!");
			}

			public override bool CanRead {
				get { return true; }
			}
		}
	}
}