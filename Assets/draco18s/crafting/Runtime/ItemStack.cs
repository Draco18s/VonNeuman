using Assets.draco18s.util;
using Assets.draco18s.crafting.capabilities;

namespace Assets.draco18s.crafting
{
    public sealed class ItemStack /*: ICapabilityProvider*/ {
		public static ItemStack EMPTY = new ItemStack(null, 0);

		private float stackSize;
		public readonly Material item;

		public ItemStack(Material _item, float size = 0) {
			item = _item;
			stackSize = size;
		}

		public bool IsEmpty() {
			return stackSize <= 0 || item == null;
		}

		public float GetSize() {
			return stackSize;
		}

		public void Subtract(float n=0) {
			stackSize -= n;
		}

		public void Add(float n=0) {
			stackSize += n;
		}

		public ItemStack Clone() {
			return new ItemStack(item, stackSize);
		}

		public ItemStack Merge(ItemStack stack) {
			float ss = stack.GetSize();
			stack.stackSize = 0;//(ss);
			return new ItemStack(item, stackSize + ss);
		}

		public override bool Equals(object obj) {
			if(obj is ItemStack other)
				return item == other.item;
			return false;
		}

		public float GetMass() {
			return item.mass * stackSize;
		}

		public float GetVolume() {
			return item.volume * stackSize;
		}
		
		public override int GetHashCode() {
			return item.GetHashCode();
		}

		/*public LazyOptional<T> GetCapability<T>(Capability<T> cap) {
			return LazyOptional<T>.Empty();
		}*/
    }
}