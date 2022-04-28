using System;
using UnityEngine;

namespace Assets.draco18s.space.stellar {
	[Serializable]
	public struct StarData {
		public bool generated { get; set; }
		public Vector3 coords;
		public string properName;
		public int hygID;
		public float brightnessMagnitude;
		public string spectralType;
		public float colorIndex;
		public float mass;

		public float baseMass => GetBaseMass(spectralType);
		
		static float GetBaseMass(string type) {
			if(string.IsNullOrEmpty(type)) type = "??";
			if(type.Length < 2) type += "?";
			char t = type[0];
			bool b = int.TryParse(type[1].ToString(), out int n);
			if(!b) n = 1;
			if(t == 'D') {
				return 1.25f;
			}
			string size = "";
			if(type[type.Length-1] == '0' || type[type.Length-1] == 'a' || type[type.Length-1] == 'b') size = type[type.Length-1].ToString();
			if(type[type.Length-1] == 'I') {
				size = "I";
				if(type.Length >= 2 && type[type.Length-2] == 'I') {
					size = "I" + size;
					if(type.Length >= 3 && type[type.Length-3] == 'I' || type[type.Length-3] == 'V') {
						size = type[type.Length-3].ToString() + size;
					}
				}
				if(type.Length >= 2 && type[type.Length-2] == 'V') {
					size = "V" + size;
				}
			}
			if(type[type.Length-1] == 'V') {
				size = "V";
				if(type.Length >= 2 && type[type.Length-2] == 'I' || type[type.Length-2] == 'V') {
					size = type[type.Length-2].ToString() + size;
				}
			}
			if(size == "0") {
				return 80;
			}
			if(size == "a") {
				return 40;
			}
			if(size == "b") {
				return 30;
			}
			if(size == "II") {
				return 20;
			}
			if(size == "III") {
				return 10;
			}
			if(size == "IV") {
				return 5;
			}
			if(size == "V") {
				if(!mainSeq.Contains(t.ToString())) {
					t = mainSeq[(int)(UnityEngine.Random.value * mainSeq.Length)];
				}
				return MainSequenceMass(t, n);
			}
			if(size == "VI") {
				return 0.75f;
			}
			if(size == "VII") {
				return 0.5f;
			}
			if(!mainSeq.Contains(t.ToString())) {
				t = mainSeq[(int)(UnityEngine.Random.value * mainSeq.Length)];
			}
			return MainSequenceMass(t, n);
		}

		const string mainSeq = "OBAFGKMLT";

		static float MainSequenceMass(char t, int n) {
			switch(t) {
				case 'O':
					switch(n) {
						case 5:
							return 30.3f;
						case 6:
							return 22.9f;
						case 7:
							return 21.7f;
						case 8:
							return 19.7f;
						case 9:
							return 17.6f;
					}
					return 12;
				case 'B':
					switch(n) {
						case 1:
							return 8.24f;
						case 2:
							return 7.14f;
						case 3:
							return 5.48f;
						case 5:
							return 4.36f;
						case 6:
							return 3.98f;
						case 7:
							return 3.64f;
						case 8:
							return 3.16f;
						case 9:
							return 2.81f;
					}
					return 6;
				case 'A':
					switch(n) {
						case 0:
							return 2.17f;
						case 1:
							return 2.06f;
						case 2:
							return 1.97f;
						case 3:
							return 1.86f;
						case 4:
							return 1.78f;
						case 5:
							return 1.73f;
						case 7:
							return 1.61f;
					}
					return 1.55f;
				case 'F':
					switch(n) {
						case 0:
							return 1.44f;
						case 2:
							return 1.35f;
						case 3:
							return 1.29f;
						case 5:
							return 1.25f;
						case 6:
							return 1.20f;
						case 7:
							return 1.16f;
						case 8:
							return 1.14f;
					}
					return 1.08f;
				case 'G':
					switch(n) {
						case 0:
							return 1.07f;
						case 1:
							return 1.04f;
						case 2:
							return 1.00f;
						case 5:
							return 0.963f;
						case 8:
							return 0.908f;
					}
					return 1;
				case 'K':
					switch(n) {
						case 0:
							return 0.857f;
						case 1:
							return 0.824f;
						case 2:
							return 0.785f;
						case 3:
							return 0.746f;
						case 4:
							return 0.700f;
						case 5:
							return 0.660f;
						case 7:
							return 0.562f;
					}
					return 0.875f;
				case 'M':
					switch(n) {
						case 0:
							return 0.513f;
						case 1:
							return 0.503f;
						case 2:
							return 0.482f;
						case 3:
							return 0.463f;
						case 4:
							return 0.442f;
						case 5:
							return 0.402f;
						case 6:
							return 0.385f;
						case 7:
							return 0.346f;
						case 8:
							return 0.311f;
					}
					return 0.3f;
				case 'L':
					switch(n) {
						case 0:
							return 0.293f;
						case 3:
							return 0.227f;
						case 8:
							return 0.126f;
					}
					return 0.195f;
				case 'T':
					switch(n) {
						case 0:
							return 0.114f;
						case 3:
							return 0.068f;
						case 8:
							return 0.0483f;
					}
					return 0.0251f;
			}
			return 1;
		}
	}
}