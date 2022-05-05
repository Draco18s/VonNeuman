using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.draco18s.space.stellar {

    [CreateAssetMenu(menuName="Stellar/Star Names",fileName="Star Names")]
	public class StarNames : ScriptableObject {
		public List<string> catalogs;
		public List<string> rawNames;

		private static List<string> useOnceNameList;
		private static StarNames asset;

		static void LoadNameAsset() {
			/*foreach(AssetBundle bn in AssetBundle.GetAllLoadedAssetBundles()) {
				if(bn.name == "galaxy") {
					asset = bn.LoadAsset<StarNames>("Star Names") as StarNames;
					useOnceNameList = new List<string>(asset.rawNames);
					return;
				}
			}
			AssetBundle bundle = AssetBundle.LoadFromFile("Assets/Bundles/galaxy");
        	asset = bundle.LoadAsset<StarNames>("Star Names") as StarNames;
			useOnceNameList = new List<string>(asset.rawNames);*/
			string _bundle = "galaxy";
#if UNITY_EDITOR
			if(Application.isPlaying) {
				AssetBundle bundle = AssetBundle.GetAllLoadedAssetBundles().First(x => x.name == _bundle);
				asset = bundle.LoadAsset<StarNames>("Star Names");
			}
			else {
				Debug.Log("   AssetDatabase Load Asset");
				asset = AssetDatabase.LoadAssetAtPath("Star Names",typeof(StarNames)) as StarNames;
			}
#else		
			Debug.Log("   AssetBundle Load Bundles");
			AssetBundle bundle = AssetBundle.GetAllLoadedAssetBundles().First(x => x.name == _bundle);
			asset = bundle.LoadAsset<StarNames>("Star Names");
#endif
			useOnceNameList = new List<string>(asset.rawNames);
		}

		void OnValidate() {
			asset = this;
			useOnceNameList = new List<string>(asset.rawNames);
		}

		public static string GetRandomName() {
			if(useOnceNameList == null) {
				Debug.Log("Fetching names asset");
				LoadNameAsset();
				Debug.Log("Ready");
			}
			if(useOnceNameList.Count < 1 || UnityEngine.Random.value < 0.05f) {
				int cat = (int)(UnityEngine.Random.value*asset.catalogs.Count);
				int a = 0;
				if(cat <= 1) {
					a = (int)Mathf.Pow(10, UnityEngine.Random.Range(1,4));
					return $"{asset.catalogs[cat]}?°{UnityEngine.Random.Range(a,a*10)}";//{UnityEngine.Random.Range(-90,91).ToString("+#;−#;0")}°{UnityEngine.Random.Range(a,a*10)}";
				}
				else if(cat <= 3) {
					a = 5;
					return $"{asset.catalogs[cat]}{UnityEngine.Random.Range(1,(cat==2?5:3))} {UnityEngine.Random.Range(a, a*10)}-{UnityEngine.Random.Range(a, a*10)}";
				}
				else if(cat <= 4) {
					return $"{asset.catalogs[cat]}";
				}
				a = (int)Mathf.Pow(10, UnityEngine.Random.Range(3,7));
				return asset.catalogs[cat] + " " + UnityEngine.Random.Range(a, a*10);
			}
			int n = (int)(UnityEngine.Random.value * useOnceNameList.Count);
			string r = useOnceNameList[n];
			useOnceNameList.RemoveAt(n);
			return r;
		}
	}
}