using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
namespace Assets.draco18s.util.Editor {
	public static class EditorTypeExtensions {
		public static T[] GetAtPath<T>(string path) {
			ArrayList al = new ArrayList();
			string[] fileEntries = Directory.GetFiles(Application.dataPath+"/"+path);
			foreach(string fileName in fileEntries)
			{

				int index = fileName.LastIndexOf("/");
				string localPath = "Assets/" + path;
			
				if (index > 0)
					localPath += fileName.Substring(index);
				
				Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
	
				if(t != null)
					al.Add(t);
			}
			string[] dirEntries = Directory.GetDirectories(Application.dataPath+"/"+path);
			foreach(string dirName in dirEntries) {
				string localPath = dirName;
				int index = dirName.LastIndexOf("Assets/")+7;
				if (index > 0)
					localPath = dirName.Substring(index);
				al.AddRange(GetAtPath<T>(localPath));
			}
			T[] result = new T[al.Count];
			for(int i=0;i<al.Count;i++)
				result[i] = (T)al[i];
			
			return result;
		}
	}
}
#endif