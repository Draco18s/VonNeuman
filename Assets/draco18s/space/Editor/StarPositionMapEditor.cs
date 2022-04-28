using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.draco18s.space.stellar;
using Assets.draco18s.util;
using System.Linq;
using System;

namespace Assets.draco18s.space.Editor
{
	[CustomEditor(typeof(StarPositionMap))]
	public class StarPositionMapEditor : UnityEditor.Editor
	{
		private static bool reading = false;
		private static bool running = false;
		string filePath = "";
		private static int total;
		private static int count;

		public override void OnInspectorGUI()
		{
			StarPositionMap map = (StarPositionMap)target;
			EditorGUI.BeginDisabledGroup(running);
			base.OnInspectorGUI();
			
			EditorGUILayout.LabelField(string.IsNullOrEmpty(filePath) ? "" : PathExtensions.MakeRelative(filePath,Application.dataPath));
			if (GUILayout.Button("Select CSV")) {
				filePath = EditorUtility.OpenFilePanel("Select CSV","","csv");
			}
			EditorGUI.BeginDisabledGroup(filePath.Length == 0);
			if (GUILayout.Button("Build From CSV")) {
				_ = ParseFileAsync(filePath, map);
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
			if(!reading)
				running = false;
			EditorGUI.BeginDisabledGroup(running || reading || map.knownStars.Count == 0);
			if (GUILayout.Button("Cleanup")) {
				_ = DoCleanup(map);
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.LabelField($"{count}/{total}");
		}

		static async Task DoCleanup(StarPositionMap map) {
			Debug.Log("Cleaning");
			await Task.Delay(1);
			var list = map.knownStars.Where(d => true).ToList();
			total = list.Count();
			count = 0;
			foreach(StarData d in list) {
				running = true;
				Debug.Log($"{d.properName}, {d.coords}, {d.hygID}");
				map.knownStars[map.knownStars.IndexOf(d)] = new StarData() {
					coords = d.coords,
					properName = d.properName,
					hygID = d.hygID,
					brightnessMagnitude = d.brightnessMagnitude,
					spectralType = d.spectralType,
					colorIndex = d.colorIndex,
					mass = d.mass
				};
				count++;
				if(count % 100 == 0) return;//await Task.Delay(1);
			}
			/*map.knownStars.ForEach(star => {
				k++;
				float bm = star.baseMass;
				star.mass = bm + ((UnityEngine.Random.value-0.5f)*bm*0.15f);
				if(j == k) {
					Debug.Log($"{star.properName}: {star.mass}");
				}
			});*/
			//map.knownStars.RemoveAll(x => x.coords.magnitude > 1000);
			EditorUtility.SetDirty(map);
			//AssetDatabase.ForceReserializeAssets();
			Debug.Log("Done");
		}

		static async Task ParseFileAsync(string filePath, StarPositionMap map) {
			running = true;
			reading = true;
			Debug.Log("Reading file, please wait");
			List<Dictionary<string, object>> stardict = await CSVReader.ReadAsync(filePath);
			reading = false;
			var validStars = stardict;
			Debug.Log("Populating map");
			total = validStars.Count();
			count = 0;
			foreach(Dictionary<string, object> dat in validStars) {
				running = true;
				string name = string.IsNullOrEmpty((string)dat["proper"]) ? "" : (string)dat["proper"];
				float x,y,z,ci,absmag;
				x = dat["x"].GetType() == typeof(int) ? ((int)dat["x"]) : ((float)dat["x"]);
				y = dat["y"].GetType() == typeof(int) ? ((int)dat["y"]) : ((float)dat["y"]);
				z = dat["z"].GetType() == typeof(int) ? ((int)dat["z"]) : ((float)dat["z"]);
				ci = dat["ci"].GetType() == typeof(int) ? ((int)dat["ci"]) : (dat["ci"].GetType() == typeof(float) ? ((float)dat["ci"]) : 0);
				absmag = dat["absmag"].GetType() == typeof(int) ? ((int)dat["absmag"]) : (dat["absmag"].GetType() == typeof(float) ? ((float)dat["absmag"]) : 0);
				var sdat = new StarData() {
					/*coords = new Vector3(x,y,z),
					properName = name,
					hygID = (int)dat["id"],
					brightnessMagnitude = absmag,
					spectralType = (string)dat["spect"],
					colorIndex = ci*/
				};
				map.knownStars.Add(sdat);
				count++;
				if(count % 100 == 0) await Task.Delay(1);
			}
			Debug.Log("Fixing names");
			var list = map.knownStars.Where(d => string.IsNullOrEmpty(d.properName)).ToList();
			total = list.Count();
			count = 0;
			foreach(var d in list) {
				running = true;
				string nn = GetNewName(map.knownStars.Where(d => !string.IsNullOrEmpty(d.properName)).Select(x => x.properName).ToList());
				map.knownStars[map.knownStars.IndexOf(d)] = new StarData() {
					/*coords = d.coords,
					properName = nn,
					hygID = d.hygID,
					brightnessMagnitude = d.brightnessMagnitude,
					spectralType = d.spectralType,
					colorIndex = d.colorIndex*/
				};
				count++;
				if(count % 100 == 0) await Task.Delay(1);
			}
			EditorUtility.SetDirty(map);
			running = false;
			Debug.Log($"Done! {validStars.Count()} mapped");
		}

		static string GetNewName(List<string> names) {
			string nm;
			do {
				nm = StarNames.GetRandomName();
			} while(names.Contains(nm));
			return nm;
		}
	}
}