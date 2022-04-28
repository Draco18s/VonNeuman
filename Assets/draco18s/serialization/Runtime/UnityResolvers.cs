using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Assets.draco18s.serialization {
	public class Vector2IntConverter : JsonConverter {
		public override bool CanConvert(System.Type objectType) {
			return typeof(Vector2Int).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) {
			JObject jObject = JObject.Load(reader);
			return new Vector2Int((int)jObject.GetValue("x"), (int)jObject.GetValue("y"));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			Vector2Int v = (Vector2Int)value;
			JObject o = new JObject();
			o.Add(new JProperty("x", v.x));
			o.Add(new JProperty("y", v.y));
			o.WriteTo(writer);
		}

		public override bool CanRead {
			get { return true; }
		}
	}

	public class Vector3IntConverter : JsonConverter {
		public override bool CanConvert(System.Type objectType) {
			return typeof(Vector3Int).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) {
			JObject jObject = JObject.Load(reader);
			return new Vector3Int((int)jObject.GetValue("x"), (int)jObject.GetValue("y"), (int)jObject.GetValue("z"));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			Vector3Int v = (Vector3Int)value;
			JObject o = new JObject();
			o.Add(new JProperty("x", v.x));
			o.Add(new JProperty("y", v.y));
			o.Add(new JProperty("z", v.z));
			o.WriteTo(writer);
		}

		public override bool CanRead {
			get { return true; }
		}
	}

	public class Vector2Converter : JsonConverter {
		public override bool CanConvert(System.Type objectType) {
			return typeof(Vector2).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) {
			JObject jObject = JObject.Load(reader);
			return new Vector2((float)jObject.GetValue("x"), (float)jObject.GetValue("y"));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			Vector2 v = (Vector2)value;
			JObject o = new JObject();
			o.Add(new JProperty("x", v.x));
			o.Add(new JProperty("y", v.y));
			o.WriteTo(writer);
		}

		public override bool CanRead {
			get { return true; }
		}
	}

	public class Vector3Converter : JsonConverter {
		public override bool CanConvert(System.Type objectType) {
			return typeof(Vector3).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) {
			JObject jObject = JObject.Load(reader);
			return new Vector3((float)jObject.GetValue("x"), (float)jObject.GetValue("y"), (float)jObject.GetValue("z"));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			Vector3 v = (Vector3)value;
			JObject o = new JObject();
			o.Add(new JProperty("x", v.x));
			o.Add(new JProperty("y", v.y));
			o.Add(new JProperty("z", v.z));
			o.WriteTo(writer);
		}

		public override bool CanRead {
			get { return true; }
		}
	}
}