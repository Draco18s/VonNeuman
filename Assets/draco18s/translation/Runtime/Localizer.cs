using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.translation {
	[CreateAssetMenu(fileName = "EnumLocalizer", menuName = "Translation/Create Enum Localizer")]
	public class Localizer : ScriptableObject, ISerializationCallbackReceiver {
		public string LanguageKey = "en-US";
		
		[NonSerialized]SortedList<int, Dictionary<int, string>> kvps = new SortedList<int, Dictionary<int, string>>();
		[Serializable]
		public class DualList {
			public string asmQualName;
			public string fullyQualHashName;
			public List<int> keys;
			public List<string> values;
		}

		public List<DualList> serializedEnumTranslations = new List<DualList>();

		public string GetLocalStringFor<T>(T key) where T:struct, IConvertible {
			return kvps[typeof(T).FullName.GetHashCode()][Convert.ToInt32(key)];
		}

		public Dictionary<string, string> strings = new Dictionary<string, string>();
		public List<string> stringKeys = new List<string>();
		public List<string> stringValues = new List<string>();

		public bool IsEnumTranslated(Type t) {
			return t==null?false:kvps.ContainsKey(t.FullName.GetHashCode());
		}

		public void AddEnumValue(Type enumType) {
			bool isFlags = Attribute.GetCustomAttribute(enumType, typeof(FlagsAttribute)) != null;
			string fqn = enumType.FullName;
			if(kvps.ContainsKey(fqn.GetHashCode())) return;
			Dictionary<int, string> dict = new Dictionary<int, string>();
			string[] nms = Enum.GetNames(enumType);
			List<int> nums = new List<int>();
			List<string> vals = new List<string>();
			if(!isFlags) {
				for(int j = 0; j < nms.Length; j++) {
					dict.Add(j, nms[j]);
					nums.Add(j);
					vals.Add(Localization.Fallback(nms[j], false));
				}
			}
			else {
				for(int j = 0; j < nms.Length; j++) {
					if(j == 0) {
						dict.Add(j, nms[j]);
						nums.Add(j);
						vals.Add(Localization.Fallback(nms[j], false));
					}
					else {
						dict.Add(1 << (j - 1), nms[j]);
						nums.Add(1 << (j - 1));
						vals.Add(Localization.Fallback(nms[j], false));
					}
				}
			}
			kvps.Add(fqn.GetHashCode(), dict);

			DualList item = new DualList {
				asmQualName = enumType.AssemblyQualifiedName,
				fullyQualHashName = fqn,
				keys = nums,
				values = vals
			};
			serializedEnumTranslations.Add(item);
		}

		public void UpdateEnumValues(Type enumType) {
			bool isFlags = Attribute.GetCustomAttribute(enumType, typeof(FlagsAttribute)) != null;
			string fqn = enumType.FullName;
			if(!kvps.ContainsKey(fqn.GetHashCode())) return;
			Dictionary<int, string> dict = kvps[fqn.GetHashCode()];// new Dictionary<int, string>();
			string[] nms = Enum.GetNames(enumType);
			DualList list = serializedEnumTranslations.Find(x => x.asmQualName == enumType.AssemblyQualifiedName);
			if(!isFlags) {
				for(int j = 0; j < nms.Length; j++) {
					if(!dict.ContainsKey(j)) {
						//Debug.Log("    " + nms[j] + " missing from the dictionary.");
						dict.Add(j, nms[j]);
						list.keys.Add(j);
						list.values.Add(Localization.Fallback(nms[j], false));
						//Debug.Log("Here");
					}
				}
			}
			else {
				for(int j = 0; j < nms.Length; j++) {
					if(j == 0) {
						if(!dict.ContainsKey(0)) {
							dict.Add(j, nms[j]);
							list.keys.Add(j);
							list.values.Add(Localization.Fallback(nms[j], false));
						}
					}
					else {
						if(!dict.ContainsKey(1 << (j - 1))) {
							Debug.Log(j);
							dict.Add(1 << (j - 1), nms[j]);
							list.keys.Add(1 << (j - 1));
							list.values.Add(Localization.Fallback(nms[j], false));
						}
					}
				}
			}
		}

		public void RemoveEnumValue(Type enumType) {
			string fqn = enumType.FullName;
			kvps.Remove(fqn.GetHashCode());
			serializedEnumTranslations.RemoveAll(x => x.fullyQualHashName.Equals(fqn));
		}

		public bool ValidKey(string key) {
			return !stringKeys.Contains(key);
		}

		public void OnAfterDeserialize() {
			strings.Clear();
			for(int i = 0; i != Math.Min(stringKeys.Count, stringValues.Count); i++)
				strings.Add(stringKeys[i], stringValues[i]);
			kvps.Clear();
			for(int i = 0; i < serializedEnumTranslations.Count; i++) {
				Dictionary<int, string> dict = new Dictionary<int, string>();
				for(int j = 0; j < serializedEnumTranslations[i].keys.Count; j++) {
					dict.Add(serializedEnumTranslations[i].keys[j], serializedEnumTranslations[i].values[j]);
				}
				kvps.Add(serializedEnumTranslations[i].fullyQualHashName.GetHashCode(), dict);
				Type tp = Type.GetType(serializedEnumTranslations[i].asmQualName);
				if(serializedEnumTranslations[i].keys.Count != Enum.GetValues(tp).Length) {
					UpdateEnumValues(tp);
				}
			}
		}

		public void OnBeforeSerialize() {
			stringKeys.Clear();
			stringValues.Clear();

			foreach(var kvp in strings) {
				stringKeys.Add(kvp.Key);
				stringValues.Add(kvp.Value);
			}
		}
	}
}
