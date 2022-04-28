using System;
using System.IO;

namespace Assets.draco18s.util {
	public static class PathExtensions {
		public static string MakeRelative(string filePath, string referencePath) {
			var fileUri = new Uri(filePath);
			var referenceUri = new Uri(referencePath);
			return Uri.UnescapeDataString(referenceUri.MakeRelativeUri(fileUri).ToString()).Replace('/',Path.DirectorySeparatorChar);
		}
	}
}