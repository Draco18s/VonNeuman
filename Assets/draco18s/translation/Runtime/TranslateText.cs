namespace Assets.draco18s.translation {
	public class TranslateText : ITranslatable {
		protected readonly string text;
		protected readonly object[] opts;
		ITranslatable appendNode;

		public TranslateText(string text, params object[] a) {
			this.text = text;
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