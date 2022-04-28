using Assets.draco18s.util;
using Assets.draco18s.serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;

using Material = Assets.draco18s.crafting;

namespace Assets.draco18s.crafting.capabilities {
	public interface IItemHandler {
		ItemStack InsertItem(ItemStack stack, bool simulate);
		float InsertItem(Material mat, float amount, bool simulate);
		ItemStack ExtractItem(ItemStack stack, bool simulate);
		float ExtractItem(Material mat, float amount, bool simulate);
		bool IsItemValid(ItemStack stack);
		List<ItemStack> GetItems();
	}
}