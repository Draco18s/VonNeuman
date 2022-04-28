using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Assets.draco18s.util;

namespace Assets.draco18s.space.stellar
{
	[CreateAssetMenu(menuName="Stellar/StarPositionMap",fileName="New StarPositionMap")]
    public class StarPositionMap : ScriptableObject {
		public List<StarData> knownStars;
		private List<StarData> shuffledStars;
		private int shuffleIndex=0;

		void OnEnable() {
			shuffledStars = knownStars.Skip(1).ToList();
			shuffledStars.Shuffle();
		}

		public StarData GetRandom() {
			//don't generate Sol
			int n = (int)(UnityEngine.Random.value * (knownStars.Count-1))+1;
			return knownStars[n];
		}

		public StarData GetRandomUnique() {
			return shuffledStars[++shuffleIndex];
		}
	}
}