using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.translation {
	public interface ITranslatable {
		string Translate();
		ITranslatable Append(ITranslatable child);
		ITranslatable GetNext();
	}
}
