using Assets.draco18s.util;
using Assets.draco18s.serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Assets.draco18s.crafting.capabilities {
	public class Capability<T> {
		public string GetName() { return name; }
		private readonly string name;

		public Capability() {
			this.name = typeof(T).AssemblyQualifiedName;
		}
	}
}