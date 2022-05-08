using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.crafting {
	public class ItemStack /*: ICapabilityProvider*/ {
		public static ItemStack EMPTY = new ItemStack(null, 0);
		public readonly MaterialInstance item;

		private int stackSize; // single units or 1/1000th cubic meter
		private int damage;
		private int maxDamage;

		public ItemStack(MaterialInstance _item, int size = 0) {
			item = _item;
			stackSize = size;
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
			return new ItemStack(item.Clone(), stackSize);
		}

		public ItemStack Merge(ItemStack stack) {
			if(!stack.item.Equals(item)) {}
			int ss = stack.GetSize();
			stack.stackSize = 0;
			return new ItemStack(item, stackSize + ss);
		}

		public override bool Equals(object obj) {
			if(obj is ItemStack other)
				return other.item.Equals(item);
			return false;
		}

		public override int GetHashCode() {
			return item.GetHashCode() * 17 + stackSize;
		}

		public float GetMass() {
			return item.GetMass() * stackSize;
		}

		public float GetVolume() {
			return item.GetVolume() * stackSize;
		}

		/*public LazyOptional<T> GetCapability<T>(Capability<T> cap) {
			return LazyOptional<T>.Empty();
		}*/
	}
}