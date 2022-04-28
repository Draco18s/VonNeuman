using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.draco18s.space.stellar;

namespace Assets.draco18s.space.Editor
{
	[CustomEditor(typeof(StarNames))]
	public class StarNamesEditor : UnityEditor.Editor {
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Build Bundles")) {
				BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
			}
		}
	}
}
