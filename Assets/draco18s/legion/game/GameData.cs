using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Assets.draco18s.space;
using Assets.draco18s.space.stellar;

using Newtonsoft.Json;

namespace Assets.draco18s.legion.game {
	public class GameData {
		public static int saveVersionFromDisk;

		public int saveVersion {
			get {
				return 1;
			}
			set {
				saveVersionFromDisk = value;
			}
		}

		public Galaxy galaxy;
		[JsonIgnore] public TickableManager tickables;
		[JsonIgnore] public PlayerActivityManager player;

		public GameData() { }

		public GameData(StarPositionMap starmapData) {
			galaxy = Galaxy.GenerateGalaxy(starmapData, 1000);
		}

		internal void FinishLoad() {
			tickables = new TickableManager();
			player = new PlayerActivityManager();
		}
	}
}