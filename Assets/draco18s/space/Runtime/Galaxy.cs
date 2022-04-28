using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.draco18s.space.stellar;
using Assets.draco18s.space.planetary;
using Newtonsoft.Json;
using Assets.draco18s.gameAssets;
using Assets.draco18s.serialization;

namespace Assets.draco18s.space {
    [Serializable]
	public class Galaxy {
		[JsonProperty] protected List<StarSystem> _systems;
		[JsonIgnore] public IReadOnlyCollection<StarSystem> Systems => _systems.OrderBy(x => x.Info.coords.y).ToList().AsReadOnly();

        public static Galaxy GenerateGalaxy(StarPositionMap starmap, int numStars) {
            Galaxy g = new Galaxy();
			var sol = new StarSystem(starmap.knownStars[0]);
			sol.spriteData = ScriptableObjectRegistry.GetRegistry<SpriteMap>().First(x => x.spriteId == "star");
			g.Add(sol);
			numStars--;
			for(int i=0;i<numStars;) {
				StarData sys = (starmap.GetRandomUnique());
				float d = g._systems.Min(s => Vector3.Scale(s.Info.coords - sys.coords,new Vector3(1,0.01f,1f)/3.26156f).sqrMagnitude);
				if(d < 16) {
					//Debug.Log($"Skipping {sys.properName}, {d}");
					continue;
				}
				else {
					//Debug.Log($"Keeping {sys.properName}, {d}");
				}
				i++;
				g.Add(new StarSystem(sys));
			}
            return g;
        }

		public Galaxy() {
			_systems = new List<StarSystem>();
		}

        public void Add(StarSystem s) => _systems.Add(s);
	}
}