/***
https://github.com/tiago-peres/blog/tree/master/csvreader
***/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;

public class CSVReader
{
	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };

	public static List<Dictionary<string, object>> Read(string filePath)
	{
		var list = new List<Dictionary<string, object>>();
		TextAsset data = Resources.Load (filePath) as TextAsset;

		var lines = Regex.Split (data.text, LINE_SPLIT_RE);

		if(lines.Length <= 1) return list;

		var header = Regex.Split(lines[0], SPLIT_RE);
		for(var i=1; i < lines.Length; i++) {

			var values = Regex.Split(lines[i], SPLIT_RE);
			if(values.Length == 0 ||values[0] == "") continue;

			var entry = new Dictionary<string, object>();
			for(var j=0; j < header.Length && j < values.Length; j++ ) {
				string value = values[j];
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
				object finalvalue = value;
				int n;
				float f;
				if(int.TryParse(value, out n)) {
					finalvalue = n;
				} else if (float.TryParse(value, out f)) {
					finalvalue = f;
				}
				entry[header[j]] = finalvalue;
			}
			list.Add (entry);
		}
		return list;
	}

	public static async Task<List<Dictionary<string, object>>> ReadAsync(string filePath)
	{
		return await ReadAsync(filePath, Encoding.ASCII);
	}

	public static async Task<List<Dictionary<string, object>>> ReadAsync(string filePath, Encoding encoding)
	{
		var list = new List<Dictionary<string, object>>();
		string text = await ReadAllTextAsync(filePath, encoding);
		Debug.Log("Parsing...");
		var lines = Regex.Split(text, LINE_SPLIT_RE);
		var len = lines.Length;
		Debug.Log(len);
		if(len <= 1) return list;

		var header = Regex.Split(lines[0], SPLIT_RE);
		for(var i=1; i < len; i++) {

			var values = Regex.Split(lines[i], SPLIT_RE);
			if(values.Length == 0 ||values[0] == "") continue;

			var entry = new Dictionary<string, object>();
			for(var j=0; j < header.Length && j < values.Length; j++ ) {
				string value = values[j];
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
				object finalvalue = value;
				int n;
				float f;
				if(int.TryParse(value, out n)) {
					finalvalue = n;
				} else if (float.TryParse(value, out f)) {
					finalvalue = f;
				}
				entry[header[j]] = finalvalue;
			}
			list.Add (entry);
		}
		return list;
	}

	private static async Task<string> ReadAllTextAsync(string filePath, Encoding encoding)  
	{
		using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
		{
			int chunksRead = 0;
			StringBuilder sb = new StringBuilder();

			byte[] buffer = new byte[0x1000];
			int numRead;
			while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
			{
				string text = encoding.GetString(buffer, 0, numRead);
				sb.Append(text);
				chunksRead++;
				if(chunksRead%100==0) Debug.Log(chunksRead);
				if(chunksRead >= 7667) break;
			}

			return sb.ToString();
		}
	}
}