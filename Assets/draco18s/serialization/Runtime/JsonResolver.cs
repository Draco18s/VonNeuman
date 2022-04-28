using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Assets.draco18s.serialization {
	[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
	public class JsonResolver : Attribute {
		public Type converter;
		public JsonResolver(Type converter) {
			this.converter = converter;
		}
	}
}