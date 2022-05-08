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
		int InsertItem(MaterialInstance mat, int amount, bool simulate);
		ItemStack ExtractItem(ItemStack stack, bool simulate);
		int ExtractItem(MaterialInstance mat, int amount, bool simulate);
		bool IsItemValid(ItemStack stack);
		List<ItemStack> GetItems();
	}
}