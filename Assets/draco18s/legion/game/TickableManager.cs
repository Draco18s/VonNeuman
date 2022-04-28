using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Assets.draco18s.util;
using Assets.draco18s.space;
using Assets.draco18s.space.stellar;

namespace Assets.draco18s.legion.game {
	public class TickableManager {
		public List<ITickable> objects;

		public TickableManager() {
			objects = new List<ITickable>();
		}

		public void Update(float deltaTime) {
			objects.ForEach(x => x.Tick(deltaTime));
		}
	}
}