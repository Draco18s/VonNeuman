using Assets.draco18s.util;
using Assets.draco18s.serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using MaterialProperty = Assets.draco18s.crafting.properties.MaterialProperty;

namespace Assets.draco18s.crafting.capabilities {
	public interface IMaterialProperty {
		IMaterialProperty<X> As<X>() where X:MaterialProperty;
	}

	public interface IMaterialProperty<T> : IMaterialProperty where T:MaterialProperty {
		T GetMaterialProperties();
	}
}