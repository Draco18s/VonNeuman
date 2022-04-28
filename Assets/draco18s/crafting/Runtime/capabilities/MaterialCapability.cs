using System;
using System.Collections.Generic;
using Assets.draco18s.crafting.properties;
using System.Linq;

namespace Assets.draco18s.crafting.capabilities {
	public class MaterialCapability : MaterialProperty {
		private static Dictionary<MaterialProperty,Capability<IMaterialProperty>> CAPABILITIES;
		public static Capability<IMaterialProperty> RegisterCapability(MaterialProperty propType) {
			if(CAPABILITIES.ContainsKey(propType)) {
				Type d1 = typeof(IMaterialProperty<>);
				Type d2 = typeof(Capability<>);
				Type[] typeArgs = { propType.GetType() };
				Type c1 = d1.MakeGenericType(typeArgs);
				Type[] typeArgs2 = { c1.GetType() };
				Type c2 = d2.MakeGenericType(typeArgs2);

				Capability<IMaterialProperty> cap = Activator.CreateInstance(c2) as Capability<IMaterialProperty>;
				CAPABILITIES.Add(propType, cap);
			}
			return Get(propType);
		}
		public static Capability<IMaterialProperty> Get(MaterialProperty propType) => CAPABILITIES[propType];

		public IMaterialProperty<X> As<X>() where X:MaterialProperty {
			return this as MaterialCapability<X>;
		}
	}

	public class MaterialCapability<T> : MaterialCapability,IMaterialProperty<T> where T:MaterialProperty {

		//public static Capability<IItemHandler> CAPABILITY = new Capability<IItemHandler>();
		protected Material material;
		protected T properties;

		private MaterialCapability(Material mat) {
			material = mat;
			properties = material.GetProperty<T>();
		}

		public T GetMaterialProperties() {
			return properties;
		}
	}
}