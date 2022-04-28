using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.draco18s.space;
using Assets.draco18s.space.stellar;

public class Universe : MonoBehaviour
{
	public static Universe instance;
	public GameObject starPrefab;
	
    void Start()
    {
		instance = this;
    }

	/*public static IEnumerator PopulatePrefabs(Galaxy galaxy) {
		yield return new WaitUntil(() => instance != null && GuiManager.instance != null);
		GuiManager.instance.BuildGalaxyUI(galaxy);
	}*/
}
