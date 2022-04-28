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
using Assets.draco18s.util;

namespace Assets.draco18s.ui {
	public class GuiAction {
		public readonly Action enable;
		public readonly Action disable;

		public GuiAction(Action forward, Action backward) {
			enable = forward;
			disable = backward;
		}

		public void DoEnable() {
			enable();
		}

		public void DoDisable() {
			disable();
		}
	}
}