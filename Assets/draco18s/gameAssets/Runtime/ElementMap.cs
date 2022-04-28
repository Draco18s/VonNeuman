using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.draco18s.util;

namespace Assets.draco18s.gameAssets
{
	[CreateAssetMenu(menuName="Game Assets/ElementMap",fileName="New ElementMap")]
    public class ElementMap : ScriptableObject, IEnumerable<ElementData> {
		public List<ElementData> elements;
		private const float baseVal = 1.732f;

		public Dictionary<ElementData,int> GetRandom(ElementType type, int num) {
			/*elements.ForEach(x => {
				Debug.Log(type.HasFlag(x.elemTyp));
			});*/


			ElementData[] choices = elements.Where(x => type == ElementType.NONE || type.HasFlag(x.elemTyp)).ToArray();
			double max = choices.Sum(x => Math.Pow(baseVal,x.relativeQuant));
			
			Dictionary<ElementData,int> result = new Dictionary<ElementData,int>();
			while(num-- > 0) {
				double n = RandomExtensions.Shared.NextDouble() * max;
				ElementData e = choices.SkipWhile(x => {
					n -= Math.Pow(baseVal,x.relativeQuant);
					return (n >= 0);
				}).Take(1).First();
				if(result.ContainsKey(e)) {
					result[e]++;
				}
				else {
					result.Add(e, 1);
				}
			}
			return result;
		}

		public IEnumerator<ElementData> GetEnumerator()
		{
			return elements.GetEnumerator();
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