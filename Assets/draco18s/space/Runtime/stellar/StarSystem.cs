using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Assets.draco18s.serialization;
using Assets.draco18s.space.planetary;
using Assets.draco18s.gameAssets;
using Assets.draco18s.util;

namespace Assets.draco18s.space.stellar {
	[JsonResolver(typeof(StarSystemConverter))]
	public class StarSystem {
		private const float G = 4.30091f;//G measured in pc⋅M⊙⁻¹⋅(km/s)², times 1000
		private const float R = 0.004846f;//1 AU as a fraction of 1 parsec, times 1000
		protected StarData data;
		protected List<OrbitalBody> planets;

		public StarData Info => data;
		public float escapeVelocity => Mathf.Sqrt(2*G*data.mass/R);
		public float firstCosmicVelocity => Mathf.Sqrt(G*data.mass/R);
		public Vector3 galacticPos => Info.coords * 3.26156f;
		public Vector3 uiposition => Vector3.Scale(new Vector3(galacticPos.x, galacticPos.z, galacticPos.y), new Vector3(1.75f, 1.75f, 0.3f));
		public SpriteData spriteData;
		public float titiusBodeK0;

		public StarSystem() { }

		public StarSystem(StarData stardata) {
			data = stardata;
			data.generated = true;
			titiusBodeK0 = (RandomExtensions.Shared.NextSingle()/5) + 0.005f;
			spriteData = ScriptableObjectRegistry.GetRegistry<SpriteMap>().GetRandom("star-sprite");
		}

		public StarSystem ChainPlanets() {
			GeneratePlanets();
			return this;
		}

		public void GeneratePlanets() {
			if(planets != null) return;
			planets = new List<OrbitalBody>();
			int num = Mathf.FloorToInt((float)(1+RandomExtensions.Shared.NextDouble()*4+RandomExtensions.Shared.NextDouble()*4+RandomExtensions.Shared.NextDouble()*4));
			for(int i=0;i<num;i++) {
				planets.Add(Planet.Generate(this, i));
			}
		}

		public IReadOnlyCollection<OrbitalBody>GetPlanets() {
			GeneratePlanets();
			return planets.AsReadOnly();
		}

		private class StarSystemConverter : JsonConverter {
			public override bool CanConvert(System.Type objectType) {
				return typeof(StarSystem).IsAssignableFrom(objectType);
			}

			public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) {
				JObject jObject = JObject.Load(reader);
				int id = (int)jObject.GetValue("id");
				string sprite = (string)jObject.GetValue("sprite");
				float tbk0 = (float)jObject.GetValue("tbk0");
				List<OrbitalBody> p = null;
				if(tbk0 <= 0) {
					tbk0 = RandomExtensions.Shared.NextSingle() + 0.00001f;
				}
				if(string.IsNullOrEmpty(sprite))
					sprite = ScriptableObjectRegistry.GetRegistry<SpriteMap>().GetRandom("star").spriteId;
				JArray plts = (JArray)jObject.GetValue("planets");
				if(plts != null) {
					p = plts.ToObject<List<OrbitalBody>>(serializer);
				}
				return new StarSystem() {
					data = ScriptableObjectRegistry.GetRegistry<StarPositionMap>().knownStars.First(x => x.hygID == id),
					spriteData = ScriptableObjectRegistry.GetRegistry<SpriteMap>().First(x => x.spriteId == sprite),
					titiusBodeK0 = tbk0,
					planets = p
				};//.ChainPlanets();
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				StarSystem v = (StarSystem)value;
				JObject o = new JObject();
				o.Add(new JProperty("id", v.Info.hygID));
				o.Add(new JProperty("sprite", v.spriteData.spriteId));
				o.Add(new JProperty("tbk0", v.titiusBodeK0));
				if(v.planets == null) {
					v.GeneratePlanets();
				}
				o.Add(new JProperty("planets", JArray.FromObject(v.planets, serializer)));
				o.WriteTo(writer);
			}

			public override bool CanRead {
				get { return true; }
			}
		}
	}
}