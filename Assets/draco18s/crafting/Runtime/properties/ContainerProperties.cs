using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using Assets.draco18s.crafting.capabilities;

using Material = Assets.draco18s.crafting;

namespace Assets.draco18s.crafting.properties
{
	public class ContainerProperties : MaterialProperty {
		
		public float volumeCapacity;
		public float massCapacity;
		public string[] canContainCategory;

		public override void Init() {
			values = new Dictionary<string,object>();
			values.Add("volumeCapacity",volumeCapacity);
			values.Add("massCapacity",massCapacity);
			values.Add("canContainCategory",canContainCategory);
		}
    }
}
