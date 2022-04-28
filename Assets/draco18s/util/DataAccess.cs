using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.draco18s.util {
	public class DataAccess {
		[DllImport("__Internal")]
		private static extern void SyncFiles();

		[DllImport("__Internal")]
		private static extern void WindowAlert(string message);
		private static readonly string saveFile = "savedata.dat";

		public static void DeleteSave() {
			string dataPath = Path.Combine(Application.persistentDataPath, saveFile);

			try {
				if(File.Exists(dataPath)) {
					File.Delete(dataPath);
				}
			}
			catch(Exception e) {
				PlatformSafeMessage($"Failed to delete: {e.Message}");
			}
		}

		public static void Save<T>(T gameDetails, JsonSerializerSettings settings) {
			string dataPath = Path.Combine(Application.persistentDataPath, saveFile);
			FileStream fileStream;

			try {
				if(File.Exists(dataPath)) {
					File.Delete(dataPath);
				}
				fileStream = File.Create(dataPath);
				StreamWriter writer = new StreamWriter(fileStream, System.Text.Encoding.ASCII);
				string json = JsonConvert.SerializeObject(gameDetails, Formatting.Indented, settings);
				writer.Write(json);
				writer.Close();
				fileStream.Close();

				if(Application.platform == RuntimePlatform.WebGLPlayer) {
					SyncFiles();
				}
			}
			catch(Exception e) {
				PlatformSafeMessage($"Failed to Save: {e.Message}\n{e.GetBaseException()}\n{e.StackTrace}");
			}
		}

		public static bool Load<T>(out T gameDetails, JsonSerializerSettings settings) {
			gameDetails = default(T);
			string dataPath = Path.Combine(Application.persistentDataPath, saveFile);
			Debug.Log(dataPath);
			try {
				if(File.Exists(dataPath)) {
					FileStream fileStream = File.Open(dataPath, FileMode.Open);
					StreamReader reader = new StreamReader(fileStream, System.Text.Encoding.ASCII);
					string json = reader.ReadToEnd();
					gameDetails = JsonConvert.DeserializeObject<T>(json, settings);
					reader.Close();
					fileStream.Close();
				}
			}
			catch(Exception e) {
				PlatformSafeMessage($"Failed to Load: {e.Message}\n{e.GetBaseException()}\n{e.StackTrace}");
			}
			return gameDetails != null;
		}

		public static void PlatformSafeMessage(string message) {
			if(Application.platform == RuntimePlatform.WebGLPlayer) {
				WindowAlert(message);
			}
			else {
				Debug.Log(message);
			}
		}
	}
}