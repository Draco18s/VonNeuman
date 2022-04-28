namespace Assets.draco18s.events {
	/// <summary>
	/// Base interface for all event types
	/// </summary>
	public interface IEventType {
		/// <summary>
		/// Return a complete, deep-copy of the event. This insures that non-modifiable events do not get modified
		/// </summary>
		/// <returns></returns>
		IEventType Clone();
	}

	/// <summary>
	/// Modifiable event objects need to hide their fields and provide getters for the currnet
	/// value and the original value as well as a setter to modify the current value.
	/// </summary>
	public interface IModifiableEvent : IEventType {
		
	}

	/// <summary>
	/// Cancelable events are ones that are informing that some action the program is about to take.
	/// Canceling such an event prevents that action from occurring.
	/// </summary>
	public interface ICancelableEvent : IModifiableEvent {
		/// <summary>
		/// Whether or not the event is canceled. By default event handler methods do no recieve canceled events.
		/// </summary>
		bool isCanceled { get; set; }
	}	
}