using System;
using System.Collections;
using System.Collections.Generic;
using Assets.draco18s.crafting.properties;
using System.Linq;

namespace Assets.draco18s.crafting.capabilities {
	public class ContainerCapability : IItemHandler {
		public static Capability<IItemHandler> CAPABILITY = new Capability<IItemHandler>();

		protected List<ItemStack> stacks;
		protected Material material;
		protected ContainerProperties containerProperties;

		public ContainerCapability(Material mat) {
			material = mat;
			containerProperties = material.GetProperty<ContainerProperties>();
			stacks = new List<ItemStack>();
		}

		public ItemStack InsertItem(ItemStack stack, bool simulate) {
			return new ItemStack(stack.item,InsertItem(stack.item, stack.GetSize(), simulate));
		}

		public int InsertItem(Material mat, int amount, bool simulate) {
			if(!IsItemValid(mat)) return amount;

			float curMass = stacks.Sum(x => x.GetMass());
			float curVolm = stacks.Sum(x => x.GetVolume());

			ItemStack exist = stacks.Where(s => s.item == mat)
				.DefaultIfEmpty(ItemStack.EMPTY)
				.FirstOrDefault();

			float remainingMass = Math.Max(curMass + (mat.mass*amount) - containerProperties.massCapacity, 0);
			float remainingVolm = Math.Max(curVolm + (mat.volume*amount) - containerProperties.volumeCapacity, 0);
			int amtExtra = (int)Math.Ceiling(Math.Max(remainingMass / mat.mass, remainingVolm / mat.volume)*1000);

			if(simulate) return amtExtra;
			
			if(exist.IsEmpty()) {
				stacks.Add(new ItemStack(mat,amount - amtExtra));
			}
			else {
				exist.Add(exist.GetSize() + amount - amtExtra);
			}

			return amtExtra;
		}

		public ItemStack ExtractItem(ItemStack stack, bool simulate) {
			return new ItemStack(stack.item,ExtractItem(stack.item, stack.GetSize(), simulate));
		}

		public int ExtractItem(Material mat, int amount, bool simulate) {
			if(!IsItemValid(mat)) return amount;

			ItemStack exist = stacks.Where(s => s.item == mat)
				.DefaultIfEmpty(ItemStack.EMPTY)
				.FirstOrDefault();

			int remaining = amount - exist.GetSize();

			if(simulate) return remaining;

			exist.Subtract(amount - remaining);

			return remaining;
		}

		public bool IsItemValid(ItemStack stack) {
			return IsItemValid(stack.item);
		}

		public bool IsItemValid(Material mat) {
			return containerProperties.canContainCategory.Contains(mat.category);
		}

		public List<ItemStack> GetItems() {
			return stacks;
		}		
	}
}