using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Assets.draco18s.space.stellar;
using Assets.draco18s.util;
using System.Linq;
using System;
using System.Text;
using System.Text.RegularExpressions;

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
		ReorderableList rl = null;

		private static int skip;
		private static int pskip;
		const int MaxListSize = 20;
		private static List<Dictionary<string, object>> stardict;
		private static Texture leftArrow;
		private static Texture rightArrow;
		private string searchText="";
		private string psearchText="";
		private int totalList;

		void OnEnable() {
			leftArrow = EditorGUIUtility.IconContent("d_scrollleft").image;
			rightArrow = EditorGUIUtility.IconContent("d_scrollright").image;
			totalList = 0;
		}

		public override void OnInspectorGUI()
		{
			StarPositionMap map = (StarPositionMap)target;
			SerializedProperty starsProp = serializedObject.FindProperty("knownStars");
			EditorGUI.BeginDisabledGroup(running);

			searchText = EditorGUILayout.TextField("Search", searchText);

			if(rl == null || psearchText != searchText) {
				psearchText = searchText;
				var plist = map.knownStars.Select((value, index) => new { index, value }).Where(x => string.IsNullOrEmpty(searchText) || x.value.properName.Contains(searchText));
				var list = plist.Skip(skip).Take(MaxListSize).ToList();
				totalList = plist.Count();
				if(totalList == 0) {
					rl = new ReorderableList(list, typeof(StarData)) {
						drawHeaderCallback = (rect) =>
						{
							Rect rect2 = new Rect(rect);
							rect2.x = Screen.width - 60;
							rect2.width = 50;
							starsProp.arraySize = EditorGUI.DelayedIntField(rect2, starsProp.arraySize);
							EditorGUI.LabelField(rect,"Known Stars");
						},
						drawElementCallback = (rect, index, isActive, isFocused) => {
							
						},
						elementHeightCallback = (index) => {
							return 0;
						},
						draggable = false,
						displayAdd = false
					};
				}
				else {
					rl = new ReorderableList(list, typeof(StarData)) {
						drawHeaderCallback = (rect) =>
						{
							Rect rect2 = new Rect(rect);
							rect2.x = Screen.width - 60;
							rect2.width = 50;
							starsProp.arraySize = EditorGUI.DelayedIntField(rect2, starsProp.arraySize);
							EditorGUI.LabelField(rect,"Known Stars");
						},
						drawElementCallback = (rect, index, isActive, isFocused) => {
							//if(index >= skip && index < skip+MaxListSize) {
								EditorGUI.PropertyField(rect, starsProp.GetArrayElementAtIndex(list[index].index));
							//}
						},
						elementHeightCallback = (index) => {
							//if(index >= skip && index < skip+MaxListSize) {
								return EditorGUI.GetPropertyHeight(starsProp.GetArrayElementAtIndex(list[index].index));
							//}
							//return 0;
						},
						draggable = false,
						displayAdd = false
					};
				}
			}
			rl.DoLayoutList();
			if(totalList > MaxListSize) {
				bool back = !(skip > 0);
				bool fore = !(totalList > skip+MaxListSize);
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				EditorGUI.BeginDisabledGroup(back);
				if(GUILayout.Button(new GUIContent(leftArrow), GUILayout.Width(24), GUILayout.Height(24))) {
					skip -= MaxListSize;
					rl = null;
				}
				EditorGUI.EndDisabledGroup();
				GUIContent c = new GUIContent($"{(int)(skip/MaxListSize)+1}/{(int)(totalList / MaxListSize)+1}");
				float w = GUI.skin.label.CalcSize(c).x;
				EditorGUILayout.LabelField(c, GUILayout.Width(w));
				EditorGUI.BeginDisabledGroup(fore);
				if(GUILayout.Button(new GUIContent(rightArrow), GUILayout.Width(24), GUILayout.Height(24))) {
					skip += MaxListSize;
					rl = null;
				}
				EditorGUI.EndDisabledGroup();
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
			}
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
			
			EditorGUI.BeginDisabledGroup(running || reading || map.knownStars.Count == 0);
			EditorGUI.BeginDisabledGroup(filePath.Length == 0);
			skip = EditorGUILayout.DelayedIntField("Skip", skip);
			pskip = EditorGUILayout.DelayedIntField("Take", pskip);
			if (GUILayout.Button("Cleanup")) {
				_ = DoCleanup(filePath, map);
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.LabelField($"{count}/{total}");
		}

		static async Task DoCleanup(string filePath, StarPositionMap map) {
			Debug.Log("Cleaning");
			await Task.Delay(1);
			running = true;
			reading = true;
			Debug.Log("Reading file, please wait");
			if(stardict == null) {
				stardict = await CSVReader.ReadAsync(filePath);
				reading = false;
			}
			Debug.Log("Generating modification list");
			await Task.Delay(1);
			var list = map.knownStars.Where(d => {
				return Regex.IsMatch(d.properName, "HD 123456");
			}).Skip(skip).Take(pskip).ToList();
			total = list.Count();
			count = 0;
			foreach(StarData d in list) {
				running = true;
				string name = "";
				Dictionary<string, object> dictEntry = stardict[d.hygID];
				if(d.hygID < stardict.Count)
					name = (string)stardict[d.hygID]["proper"];
				if(string.IsNullOrEmpty(name)) {
					name = StarNames.GetRandomName();
					if(name == "HD") {
						string hdn = stardict[d.hygID]["hd"].ToString();
						if(string.IsNullOrEmpty(hdn)) {
							hdn = stardict[d.hygID]["hip"].ToString();
							if(string.IsNullOrEmpty(hdn)) {
								hdn = d.hygID.ToString();
								name = $"HYG {hdn}";
							}
							else {
								name = $"HIG {hdn}";
							}
						}
						else {
							name = $"HD {hdn}";
						}
					}
					else if(name.Contains("?°")) {
						name = name.Replace("?",Mathf.FloorToInt((float)stardict[d.hygID]["dec"]).ToString("+#;−#;0"));
					}
				}
				Debug.Log($"{d.properName} -> {name}");
				map.knownStars[map.knownStars.IndexOf(d)] = new StarData() {
					coords = d.coords,
					properName = name,
					hygID = d.hygID,
					brightnessMagnitude = d.brightnessMagnitude,
					spectralType = d.spectralType,
					colorIndex = d.colorIndex,
					mass = d.mass
				};
				count++;
			}
			Debug.Log("Finalizing");
			EditorUtility.SetDirty(map);
			//AssetDatabase.ForceReserializeAssets();
			Debug.Log("Done");
			running = false;
			reading = false;
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
			running = false;
			reading = false;
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