using System;
using System.Collections;
using System.Collections.Generic;
using Assets.draco18s.crafting.properties;
using System.Linq;

namespace Assets.draco18s.crafting.capabilities {
	public class ContainerCapability : IItemHandler {
		public static Capability<IItemHandler> CAPABILITY = new Capability<IItemHandler>();

		protected List<ItemStack> stacks;
		protected MaterialInstance material;
		protected ContainerProperties containerProperties;

		public ContainerCapability(MaterialInstance mat) {
			material = mat;
			containerProperties = material.GetProperty<ContainerProperties>();
			stacks = new List<ItemStack>();
		}

		public ItemStack InsertItem(ItemStack stack, bool simulate) {
			return new ItemStack(stack.item,InsertItem(stack.item, stack.GetSize(), simulate));
		}

		public int InsertItem(MaterialInstance matinst, int amount, bool simulate) {
			if(!IsItemValid(matinst)) return amount;

			Material mat = matinst.item;

			float curMass = stacks.Sum(x => x.GetMass());
			float curVolm = stacks.Sum(x => x.GetVolume());

			ItemStack exist = stacks.Where(s => s.item.item == mat)
				.DefaultIfEmpty(ItemStack.EMPTY)
				.FirstOrDefault();

			float remainingMass = Math.Max(curMass + (mat.mass*amount) - containerProperties.massCapacity, 0);
			float remainingVolm = Math.Max(curVolm + (mat.volume*amount) - containerProperties.volumeCapacity, 0);
			int amtExtra = (int)Math.Ceiling(Math.Max(remainingMass / mat.mass, remainingVolm / mat.volume)*1000);

			if(simulate) return amtExtra;
			
			if(exist.IsEmpty()) {
				stacks.Add(new ItemStack(matinst,amount - amtExtra));
			}
			else {
				exist.Add(exist.GetSize() + amount - amtExtra);
			}

			return amtExtra;
		}

		public ItemStack ExtractItem(ItemStack stack, bool simulate) {
			return new ItemStack(stack.item,ExtractItem(stack.item, stack.GetSize(), simulate));
		}

		public int ExtractItem(MaterialInstance mat, int amount, bool simulate) {
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

		public bool IsItemValid(MaterialInstance mat) {
			return containerProperties.canContainCategory.Contains(mat.item.category);
		}

		public List<ItemStack> GetItems() {
			return stacks;
		}		
	}
}