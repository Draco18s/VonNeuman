using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.translation {
	public class TranslationComponent : ITranslatable {
		protected readonly string text;
		protected readonly object[] opts;
		ITranslatable appendNode;

		public TranslationComponent(string t, params object[] a) {
			text = t;
			opts = a;
		}

		public string Translate() {
			return string.Format($"{Localization.ToLocal(text)}{appendNode?.Translate()}", opts);
		}

		public ITranslatable Append(ITranslatable child) {
			if(appendNode == null) {
				appendNode = child;
			}
			else {
				appendNode.Append(child);
			}
			return this;
		}

		public ITranslatable GetNext() {
			return appendNode;
		}
	}
}
