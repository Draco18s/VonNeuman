using Assets.draco18s.legion.units;
using Assets.draco18s.crafting;
using Assets.draco18s.crafting.properties;
using Assets.draco18s.crafting.capabilities;
using Assets.draco18s.util;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;


using Material = Assets.draco18s.crafting.Material;
using MaterialProperty = Assets.draco18s.crafting.properties.MaterialProperty;

namespace Assets.draco18s.legion.units.parts
{
	public sealed class UnitComponent : ICapabilityProvider {
		private MaterialInstance item;
		private IItemHandler inventory;
		private LazyOptional<IItemHandler> contents = LazyOptional<IItemHandler>.Empty();
		private Dictionary<Capability<IMaterialProperty>,LazyOptional<IMaterialProperty>> materialProps = new Dictionary<Capability<IMaterialProperty>,LazyOptional<IMaterialProperty>>();

		public UnitComponent(MaterialInstance _item) {
			item = _item;
			foreach(MaterialProperty p in item.GetAllProperties()) {
				if(p is ContainerProperties) {
					inventory = new ContainerCapability(item);
					contents = LazyOptional<IItemHandler>.of(() => inventory);
					continue;
				}
				MaterialProperty propType = p;
				var c = MaterialCapability.RegisterCapability(propType);
				LazyOptional<IMaterialProperty> lo = LazyOptional<IMaterialProperty>.of(() => {
					Type d1 = typeof(MaterialCapability<>);
					Type[] typeArgs = { propType.GetType() };
					Type c1 = d1.MakeGenericType(typeArgs);

					return Activator.CreateInstance(c1) as IMaterialProperty;
				});
				materialProps.Add(c, lo);
			}
		}

		public float GetDryMass() {
			return item.GetMass();
		}

		public float GetTotalMass() {
			float m = item.GetMass();
			contents.IfPresent(c => {
				m += c.GetItems().Sum(x => x.GetMass());
			});
			return m;
		}

		public LazyOptional<T> GetCapability<T>(Capability<T> cap) {
			if(cap.Equals(ContainerCapability.CAPABILITY)) {
				return contents.Cast<T>();
			}
			return LazyOptional<T>.Empty();
		}
    }
}