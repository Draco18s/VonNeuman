using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.space.stellar {

    [CreateAssetMenu(menuName="Stellar/Star Names",fileName="Star Names")]
	public class StarNames : ScriptableObject {
		public List<string> catalogs;
		public List<string> rawNames;

		private static List<string> useOnceNameList;
		private static StarNames asset;

		static void LoadNameAsset() {
			foreach(AssetBundle bn in AssetBundle.GetAllLoadedAssetBundles()) {
				if(bn.name == "galaxy") {
					asset = bn.LoadAsset<StarNames>("Star Names") as StarNames;
					useOnceNameList = new List<string>(asset.rawNames);
					return;
				}
			}
			AssetBundle bundle = AssetBundle.LoadFromFile("Assets/Bundles/galaxy");
        	asset = bundle.LoadAsset<StarNames>("Star Names") as StarNames;
			useOnceNameList = new List<string>(asset.rawNames);
		}

		public static string GetRandomName() {
			if(useOnceNameList == null) {
				LoadNameAsset();
			}
			if(useOnceNameList.Count < 1 || UnityEngine.Random.value < 0.05f) {
				return asset.catalogs[(int)(UnityEngine.Random.value*asset.catalogs.Count)] + " " + UnityEngine.Random.Range(1000, 1000000);
			}
			int n = (int)(UnityEngine.Random.value * useOnceNameList.Count);
			string r = useOnceNameList[n];
			useOnceNameList.RemoveAt(n);
			return r;
		}
	}
}