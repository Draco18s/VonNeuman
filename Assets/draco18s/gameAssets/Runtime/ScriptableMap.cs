using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Linq;

namespace Assets.draco18s.gameAssets
{
#if UNITY_EDITOR
	[CanEditMultipleObjects]
	[CreateAssetMenu(menuName="Game Assets/ScriptableObjects",fileName="New ScriptableObjectMap")]
#endif
	public class ScriptableMap : ScriptableObject
	{
		public List<ScriptableObject> scriptables;
	}
}