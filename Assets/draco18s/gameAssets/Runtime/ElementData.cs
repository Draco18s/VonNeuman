using System;
using Assets.draco18s.crafting;

namespace Assets.draco18s.gameAssets {
	[Serializable]
	public struct ElementData {
		public int id;
		public string name;
		public float relativeQuant;
		[Obsolete]
		public float greenhousePotential;
		public ElementType elemTyp;
		public Material material;
	}

	[Flags]
	public enum ElementType {
		NONE		= 0,
		ALKALI		= 1<<0,
		METAL		= 1<<1,
		METALOID	= 1<<2,
		NONMETAL	= 1<<3,
		HALOGEN_GAS	= 1<<4,
		NOBLE_GAS	= 1<<5,
		EXOTIC		= 1<<6,
	}
}