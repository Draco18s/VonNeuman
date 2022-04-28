using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.draco18s.translation {
	public class EnumTranslationComponent<T> : ITranslatable where T : struct, IConvertible {
		protected readonly T text;
		ITranslatable appendNode;

		public EnumTranslationComponent(T t) {
			text = t;
		}

		public string Translate() {
			return string.Format($"{Localization.ToLocal(text)} {appendNode?.Translate()}");
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