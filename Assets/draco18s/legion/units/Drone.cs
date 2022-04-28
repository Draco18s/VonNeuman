using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.draco18s.crafting;

namespace Assets.draco18s.units {
	[CreateAssetMenu(menuName="Units/Basic Unit",fileName="New Basic Unit")]
	public class Drone : ScriptableObject
	{
		public string displayName;
        public float volume;
        public float mass;
		public Recipe constructionCost;
	}
}