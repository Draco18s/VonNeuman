using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Assets.draco18s.serialization;
using Assets.draco18s.space;
using Assets.draco18s.space.stellar;
using Assets.draco18s.space.planetary;
using Assets.draco18s.util;
using Assets.draco18s.gameAssets;
using Assets.draco18s.crafting;
using Assets.draco18s.crafting.capabilities;

using Assets.draco18s.legion.ui;
using Assets.draco18s.legion.units.parts;

using Material = Assets.draco18s.crafting.Material;
using MaterialProperty = Assets.draco18s.crafting.properties.MaterialProperty;

namespace Assets.draco18s.legion.game {
	public class Game : MonoBehaviour {
		public static Game instance;
		private GameData _data;
		public GameData data {
			get {
				return _data;
			}
			private set {
				_data = value;
			}
		}
		public ScriptableMap scriptableMaps;
		private List<ITickable> gameTickables => data.tickables.objects;

		void Start() {
			instance = this;

			AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "galaxy"));

			scriptableMaps = bundle.LoadAsset<ScriptableMap>("ScriptableMaps.asset");

			foreach(ScriptableObject so in scriptableMaps.scriptables) {
				ScriptableObjectRegistry.AddRegistry(so);
			}
			ScriptableObjectRegistry.Lock();

			ContractResolver.jsonSettings = new JsonSerializerSettings{
				MissingMemberHandling = MissingMemberHandling.Ignore,
				ContractResolver = new ContractResolver()
			};
			if(DataAccess.Load(out _data, ContractResolver.jsonSettings)) {
				data.FinishLoad();
			}
			else {
				data = new GameData(ScriptableObjectRegistry.GetRegistry<StarPositionMap>());
				DataAccess.Save(data, ContractResolver.jsonSettings);
			}
			//data = new GameData(ScriptableObjectRegistry.GetRegistry<StarPositionMap>());
			//DataAccess.Save(data, ContractResolver.jsonSettings);

			StartCoroutine(PopulateUI());
		}

		private IEnumerator PopulateUI() {
			yield return new WaitUntil(() => GuiManager.instance != null);
			GuiManager.instance.BuildGalaxyUI(data.galaxy);
		}

		void Update() {
			float dt = Time.deltaTime;

			gameTickables.ForEach(x => x.Tick(dt));
			data.tickables.Update(dt);
		}

		public void AddTickableGameObject(ITickable obj) {
			gameTickables.Add(obj);
		}
	}
}