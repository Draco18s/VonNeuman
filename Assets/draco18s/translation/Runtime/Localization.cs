using System;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Assets.draco18s.translation {
	public static class Localization {
		/// <summary>
		/// Translates one string to a localized string
		/// </summary>
		/// <param name="key">the string to translate</param>
		/// <returns></returns>
		public static string ToLocal(string key) {
			string ret = "";
			if(!instance.strings.TryGetValue(key, out ret)) {
				ret = Fallback(key);
			}
			return ret;
		}

		public static void SetTranslated(Type enumType, bool value) {
			if(value) {
				instance.AddEnumValue(enumType);
			}
			else {
				instance.RemoveEnumValue(enumType);
			}
		}

		/// <summary>
		/// Translates an enum value to a human readable name
		/// </summary>
		/// <typeparam name="T">enum Type</typeparam>
		/// <param name="key">enum value to translate</param>
		/// <returns></returns>
		public static string ToLocal<T>(T key) where T : struct, IConvertible {
			try {
				return instance.GetLocalStringFor(key);
			}
			catch(Exception e) {
				Debug.LogError(e);
				return Fallback(key);
			}
		}

		/// <summary>
		/// Used by Inspector.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool ValidKey(string key) {
			return instance.ValidKey(key);
		}

		public static bool IsEnumTranslated(Type t) {
			return instance.IsEnumTranslated(t);
		}

		private static Localizer instance {
			get {
				if(null == @object) {
					string tag = System.Globalization.CultureInfo.CurrentCulture.Name;
					Resources.LoadAll<Localizer>("lang");
					Localizer[] lz = Resources.FindObjectsOfTypeAll<Localizer>();
					foreach(Localizer lc in lz) {
						if(lc.LanguageKey.Equals(tag)) {
							@object = lc;
							break;
						}
					}
					if(null == @object) {
						string ex = "Language for " + tag + " not found! Language assets must be in Resources/lang.";
						if(lz.Length > 0) {
							@object = lz[0];
							ex += " Using " + @object.LanguageKey + " instead.";
						}
						Debug.Break();
						throw new Exception(ex);
					}
				}
				return @object;
			}
		}
		private static Localizer @object = null;

		public static string Fallback(object param, bool generateWarning = true) {
			if(generateWarning) {
				Type t = param.GetType();
				Debug.LogError("No localization set for `" + ((t == typeof(string)) ? "" : (t + ".")) + param + "`");
			}

			// Replace snake case with spaces.
			string sz = Regex.Replace(param.ToString(), @"_", " ");

			// Convert to title case.
			sz = Regex.Replace(sz.ToLower(), @"(^[a-z])|( [a-z])", match => match.ToString().ToUpper());

			return sz;
		}
	}
}
