using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Assets.draco18s.util;
using Assets.draco18s.crafting.capabilities;
using Assets.draco18s.crafting.properties;

namespace Assets.draco18s.crafting
{
    public sealed class ItemStack /*: ICapabilityProvider*/ {
		public static ItemStack EMPTY = new ItemStack(null, 0);
		public readonly Material item;

		private int stackSize; //in 1/1000ths of a cubic meter
		private int damage;
		private int maxDamage;
		private float curEnergy;
		private float maxEnergy;
		private bool isConglomerate;

		public ItemStack(Material _item, int size = 0) {
			item = _item;
			stackSize = size;
			isConglomerate = item.volume != 1;
			if(item.HasProperty<PowerFuelProperties>()){
				PowerFuelProperties prop = item.GetProperty<PowerFuelProperties>();
				if(!prop.isMassConsumed) {
					maxEnergy = prop.energyDensity;
				}
			}
		}

		public bool IsEmpty() {
			return stackSize <= 0 || item == null;
		}

		public int GetSize() {
			return stackSize;
		}

		public void Subtract(int n=0) {
			stackSize -= n;
		}

		public void Add(int n=0) {
			stackSize += n;
		}

		public ItemStack Clone() {
			return new ItemStack(item, stackSize);
		}

		public ItemStack Merge(ItemStack stack) {
			int ss = stack.GetSize();
			stack.stackSize = 0;//(ss);
			return new ItemStack(item, stackSize + ss);
		}

		public override bool Equals(object obj) {
			if(obj is ItemStack other)
				return item == other.item;
			return false;
		}

		public float GetMass() {
			return item.mass * stackSize / (isConglomerate ? 1000 : 1);
		}

		public float GetVolume() {
			return item.volume * stackSize / (isConglomerate ? 1000 : 1);
		}
		
		public override int GetHashCode() {
			return item.GetHashCode();
		}

		public T GetProperty<T>(string name) {
			return item.GetProperty<T>(name);
		}

		public bool HasProperty(string name) {
			return item.HasProperty(name);
		}

		public bool HasProperty<T>()where T:MaterialProperty {
			return item.HasProperty<T>();
		}

		public T GetProperty<T>() where T:MaterialProperty {
			return item.GetProperty<T>();
		}

		public ReadOnlyCollection<MaterialProperty> GetAllProperties() {
			return item.GetAllProperties();
		}

		/*public LazyOptional<T> GetCapability<T>(Capability<T> cap) {
			return LazyOptional<T>.Empty();
		}*/
    }
}