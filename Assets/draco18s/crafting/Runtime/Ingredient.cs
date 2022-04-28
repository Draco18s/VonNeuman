using System;

namespace Assets.draco18s.crafting
{
	[Serializable]
	public struct Ingredient {
		public Material mat;
		public int quant;
		public int frac;
	}
}