using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using JetBrains.Annotations;
using UnityEngine;

namespace Assets.draco18s.events {
	/// <summary>
	/// MessengerMode controls the behavior of the Broadcast
	/// </summary>
	public enum MessengerMode {
		/// <summary>
		/// Default
		/// </summary>
		DONT_REQUIRE_LISTENER,
		/// <summary>
		/// Throws an exception if there are no listeners
		/// </summary>
		REQUIRE_LISTENER
	}

	internal static class MessengerInternal {
		public static readonly Dictionary<Type, Delegate> eventTable = new Dictionary<Type, Delegate>();
		public static readonly Dictionary<Delegate, Delegate> wrapperTable = new Dictionary<Delegate, Delegate>();

		public static void AddListener(Type eventType, Delegate callback) {
			OnListenerAdding(eventType, callback);
			eventTable[eventType] = Delegate.Combine(eventTable[eventType], callback);
		}

		public static void Clear() {
			eventTable.Clear();
			wrapperTable.Clear();
		}

		public static void RemoveListener(Type eventType, Delegate handler) {
			OnListenerRemoving(eventType, handler);
			eventTable[eventType] = Delegate.Remove(eventTable[eventType], handler);
			OnListenerRemoved(eventType);
		}

		public static T[] GetInvocationList<T>(Type eventType) {
			Delegate d;
			if (!eventTable.TryGetValue(eventType, out d)) return new T[0];
			try {
				return d.GetInvocationList().Cast<T>().ToArray();
			}
			catch {
				throw CreateBroadcastSignatureException(eventType);
			}
		}
		
		public static Delegate[] GetGenericInvocationList(Type eventType) {
			Delegate d;
			if(!eventTable.TryGetValue(eventType, out d)) return new Delegate[0];
			return d.GetInvocationList().ToArray();
		}

		public static void OnListenerAdding(Type eventType, Delegate listenerBeingAdded) {
			if(!eventTable.ContainsKey(eventType)) {
				eventTable.Add(eventType, null);
			}

			Delegate d = eventTable[eventType];
			if(d != null && d.GetType() != listenerBeingAdded.GetType()) {
				throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
			}
		}

		public static void OnListenerRemoving(Type eventType, Delegate listenerBeingRemoved) {
			if(eventTable.ContainsKey(eventType)) {
				Delegate d = eventTable[eventType];
				if(d.GetType() != listenerBeingRemoved.GetType()) {
					throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
				}
			}
		}

		public static void OnListenerRemoved(Type eventType) {
			if(eventTable[eventType] == null) {
				eventTable.Remove(eventType);
			}
		}

		public static void OnBroadcasting(Type eventType, MessengerMode mode) {
			if(mode == MessengerMode.REQUIRE_LISTENER && !eventTable.ContainsKey(eventType)) {
				throw new BroadcastException(string.Format("Broadcasting message {0} but no listener found.", eventType));
			}
		}

		public static BroadcastException CreateBroadcastSignatureException(Type eventType) {
			return new BroadcastException(string.Format("Broadcasting message {0} but listeners have a different signature than the broadcaster.", eventType));
		}

		[Serializable]
		public class BroadcastException : Exception {
			public BroadcastException(string msg)
				: base(msg) {
			}
		}

		[Serializable]
		public class ListenerException : Exception {
			public ListenerException(string msg)
				: base(msg) {
			}
		}
	}

	internal static class MessengerInternalHelper {
		public static void ClearAllListeners() {
			MessengerInternal.Clear();
		}
	}

	/// <summary>
	/// <para>The messenger class is the generic class that allows listening and broadcasting events.</para>
	/// <para>Usage:</para>
	/// <para>Messenger&lt;<typeparamref name="T">T</typeparamref>&gt;.AddListener(Actionr&lt;T&gt; handlerFunc);</para>
	/// <para>Messenger&lt;<typeparamref name="T">T</typeparamref>&gt;.Broadcast(T eventObj);</para>
	/// </summary>
	/// <typeparam name="T">A Type T where T:IEventType</typeparam>
	public static class Messenger<T> where T : IEventType {
		private static Action<T> GetCanceledHandler(Action<T> handle, bool recieve) {
			return arg => {
				if(!(arg is ICancelableEvent) || (!((ICancelableEvent)arg).isCanceled) || recieve) {
					handle(arg);
				}
			};
		}

		/// <summary>
		/// <para>Create a listener for a given event message type. Always return the original event object.</para>
		/// <para>IModifiableEvent objects may have their contents modified, however be aware that event handler invokation order is not guaranteed.</para>
		/// <para>The final resulting event object is sent to the network for logging, etc.</para>
		/// <para>ICancelableEvent s can be canceled. Pass true as the second parameter to recieve canceled events anyway.</para>
		/// </summary>
		/// <param name="handler">The event handler method; takes and returns the event Type object</param>
		/// <param name="getCanceled">Whether or not to recieve canceled events (optional)</param>
		public static void AddListener(Action<T> handler, bool getCanceled = false) {
			Type eventType = typeof(T);

			Delegate h = GetCanceledHandler(handler, getCanceled);
			MessengerInternal.wrapperTable.Add(handler, h);
			MessengerInternal.AddListener(eventType, h);
		}

		/// <summary>
		/// Removes the specified listener method.
		/// </summary>
		/// <param name="handler">The event handler method to remove</param>
		public static void RemoveListener(Action<T> handler) {
			Type eventType = typeof(T);
			if (MessengerInternal.wrapperTable.ContainsKey(handler)) {
				MessengerInternal.RemoveListener(eventType, MessengerInternal.wrapperTable[handler]);
				MessengerInternal.wrapperTable.Remove(handler);
			}
		}

		/// <summary>
		/// Broadcast an event. IModifiableEvents that are modified should affect program behavior (eg. a cancelable event, if canceled, should skip behaviour that it was notifying was about to occur).
		/// </summary>
		/// <param name="arg1">The event object to notify.</param>
		/// <param name="mode">The messaging mode. If MessengerMode.REQUIRE_LISTENER and there are no listeners, an exception will be thrown.</param>
		/// <returns>The event object (potentially modified).</returns>
		public static T Broadcast(ref T arg1, MessengerMode mode = MessengerMode.DONT_REQUIRE_LISTENER) {
			Type eventType = typeof(T);
			MessengerInternal.OnBroadcasting(eventType, mode);
			Action<T>[] invocationList = MessengerInternal.GetInvocationList<Action<T>>(eventType);
			T clone = (T)arg1.Clone();
			foreach(Action<T> del in invocationList) {
				if(arg1 is IModifiableEvent)
					del.Invoke(clone);
				else
					del.Invoke((T)clone.Clone());
			}
			Type derived = eventType.BaseType;
			while(derived != null && typeof(IEventType).IsAssignableFrom(derived)) {
				Delegate[] invocationList2 = MessengerInternal.GetGenericInvocationList(derived);

				foreach(Delegate del in invocationList2) {
					ParameterInfo[] inf = del.Method.GetParameters();
					if(inf.Length != 1 || inf[0].ParameterType != derived) {
						throw new ArgumentException("Delegate does not accept the correct type! (How did you manage this?)");
					}
					if(arg1 is IModifiableEvent)
						del.DynamicInvoke(clone);
					else
						del.DynamicInvoke((T)clone.Clone());
				}
				derived = derived.BaseType;
			}
			if(arg1 is IModifiableEvent)
				arg1 = clone;
			return arg1;
		}

		public static void Broadcast(T arg1, MessengerMode mode = MessengerMode.DONT_REQUIRE_LISTENER) {
			Type eventType = typeof(T);
			MessengerInternal.OnBroadcasting(eventType, mode);
			Action<T>[] invocationList = MessengerInternal.GetInvocationList<Action<T>>(eventType);
			T clone = (T)arg1.Clone();
			foreach(Action<T> del in invocationList) {
				del.Invoke((T)clone.Clone());
			}
			Type derived = eventType.BaseType;
			while(derived != null && typeof(IEventType).IsAssignableFrom(derived)) {
				Delegate[] invocationList2 = MessengerInternal.GetGenericInvocationList(derived);

				foreach(Delegate del in invocationList2) {
					ParameterInfo[] inf = del.Method.GetParameters();
					if(inf.Length != 1 || inf[0].ParameterType != derived) {
						throw new ArgumentException("Delegate does not accept the correct type! (How did you manage this?)");
					}
					del.DynamicInvoke((T)clone.Clone());
				}
				derived = derived.BaseType;
			}
		}
	}
}
