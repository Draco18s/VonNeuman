using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.draco18s.space;
using Assets.draco18s.space.stellar;
using Assets.draco18s.space.planetary;
using Assets.draco18s.space.ui;
using Assets.draco18s.translation;
using Assets.draco18s.serialization;
using Assets.draco18s.gameAssets;
using Assets.draco18s.ui;
using Assets.draco18s.util;

namespace Assets.draco18s.legion.ui {
	public class GuiManager : MonoBehaviour {
		public RectTransform mainCanvas;
		public RectTransform galaxyContainer;
		public RectTransform systemContainer;
		public RectTransform shipsContainer;
		public GameObject starPrefab;

		private StarSystem activeSystem;

		public static GuiManager instance;
		private List<ITickable> uiTickables;

		void Start() {
			instance = this;
			uiTickables = new List<ITickable>();
		}

		void Update() {
			float dt = Time.deltaTime;
			uiTickables.ForEach(x => x.Tick(dt));
			
			/***
			Old input system
			***/

			if(Input.GetButtonDown("Cancel")) {
				OnNavBack();
			}
		}

		public void OnNavBack() {
			BacktrackStack.GoBack();
		}

		internal void InitUI() {
		}

		public void ShowGalaxy() {
			activeSystem = null;
			galaxyContainer.parent.parent.parent.GetComponent<Canvas>().enabled = true;
			systemContainer.parent.GetComponent<Canvas>().enabled = false;
			//galaxyContainer.parent.parent.parent.gameObject.SetActive(true);
			//systemContainer.parent.gameObject.SetActive(false);
			systemContainer.transform.Clear();
		}

		private void ShowSystem(StarSystem system) {
			if(activeSystem != system) systemContainer.Clear();
			activeSystem = system;
			galaxyContainer.parent.parent.parent.GetComponent<Canvas>().enabled = false;
			systemContainer.parent.GetComponent<Canvas>().enabled = true;
			//galaxyContainer.parent.parent.parent.gameObject.SetActive(false);
			//systemContainer.parent.gameObject.SetActive(true);
			BuildSystem(system);
		}

		public void BuildGalaxyUI(Galaxy galaxy) {
			Rect rect = new Rect(0, 0, 0, 0);
			galaxy.Systems.ForEach(star => {
				StarSystem system = star;
				GameObject go = Instantiate(starPrefab, galaxyContainer);
				go.name = system.Info.properName;
				system.GalaxyStarGraphic(go, ShowSystem, ShowGalaxy, AddTickableGameObject);
				if(star.uiposition.x < rect.xMin) {
					rect.xMin = star.uiposition.x;
				}
				if(star.uiposition.y < rect.yMin) {
					rect.yMin = star.uiposition.y;
				}
				if(star.uiposition.x > rect.xMax) {
					rect.xMax = star.uiposition.x;
				}
				if(star.uiposition.y > rect.yMax) {
					rect.yMax = star.uiposition.y;
				}
			});
			RectTransform rt = (RectTransform)galaxyContainer.transform;
			((RectTransform)rt.parent).sizeDelta = new Vector2(rect.width + 2*Screen.width, rect.height + 2*Screen.height);
			rt.localPosition = new Vector3(-rect.x + Screen.width, rect.y - Screen.height, 0);
			((RectTransform)rt.parent).localPosition = new Vector3(rect.x - Screen.width/2, -rect.y + Screen.height/2, 0);
		}

		public void BuildSystem(StarSystem system) {
			//float angle = Mathf.Atan2(Screen.height,Screen.width);
			Vector3 angle = new Vector3(Screen.width,Screen.height).normalized;
			GameObject star = Instantiate(starPrefab, systemContainer);
			system.PopulateSystemUI(star, angle);
		}

		public void AddTickableGameObject(ITickable obj) {
			uiTickables.Add(obj);
		}
	}
}