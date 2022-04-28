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
	public static class BacktrackStack {
		public static Stack<GuiAction> stack = new Stack<GuiAction>();

		public static void Navigate(GuiAction action) {
			stack.Push(action);
			action.DoEnable();
		}

		public static void GoBack() {
			Close();
			if(stack.Count() > 0)
				stack.Peek().DoEnable();
		}

		public static void Close() {
			if(stack.Count() > 0)
				stack.Pop().DoDisable();
		}
	}
}