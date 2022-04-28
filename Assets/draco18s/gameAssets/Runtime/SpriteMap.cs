using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.gameAssets
{
	[CreateAssetMenu(menuName="Game Assets/SpriteMap",fileName="New SpriteMap")]
    public class SpriteMap : ScriptableObject, IEnumerable<SpriteData> {
		[SerializeField] protected List<SpriteData> sprites;

		public SpriteData GetRandom(string type) {
			var choices = sprites.Where(x => x.spriteId.Contains(type)).ToArray();
			int n = (int)(UnityEngine.Random.value * choices.Length);
			return choices[n];
		}

		public IEnumerator<SpriteData> GetEnumerator()
		{
			return sprites.GetEnumerator();
		}

		// Must also implement IEnumerable.GetEnumerator, but implement as a private method.
		private IEnumerator GetEnumerator1()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator1();
		}
	}
}