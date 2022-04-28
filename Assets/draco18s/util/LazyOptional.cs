using System;

namespace Assets.draco18s.util {
	public class LazyOptional<T> {
		private static readonly LazyOptional<object> EMPTY = new LazyOptional<object>(null);

		private readonly Func<T> supplier;

		private LazyOptional(Func<T> instanceSupplier) {
			this.supplier = instanceSupplier;
		}

		public static LazyOptional<T> Empty() {
			return EMPTY.Cast<T>();
		}

		public LazyOptional<X> Cast<X>() {
			return this as LazyOptional<X>;
		}

		public static LazyOptional<T> of(Func<T> instanceSupplier) {
			return instanceSupplier == null ? Empty() : new LazyOptional<T>(instanceSupplier);
		}

		public bool IsPresent()
		{
			return supplier != null;
		}

		private T GetValue() {
			return supplier.Invoke();
		}

		public void IfPresent(Action<T> consumer) {
			if(consumer == null) throw new ArgumentException("Argument cannot be null.");
			T val = GetValue();
			if (val != null)
				consumer.Invoke(val);
		}
	}
}