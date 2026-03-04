namespace Statewright;

public interface IStateMachine<TState, TEvent> where TState : Enum where TEvent : Enum
{
	/// <summary>
	/// The current state.
	/// </summary>
	TState CurrentState { get; }

	/// <summary>
	/// Action fired on entrance to a state. Reporting the current state and the destination state.
	/// </summary>
	event Action<TState, TState>? OnEnter;

	/// <summary>
	/// Action fired on exiting from a state. Reporting the current state and the destination state.
	/// </summary>
	event Action<TState, TState>? OnExit;

	/// <summary>
	/// Configure a permissible state transition.
	/// </summary>
	/// <param name="fromState">The from state.</param>
	/// <param name="event">The triggering event.</param>
	/// <param name="toState">The to (destination) state.</param>
	void Configure(TState fromState, TEvent @event, TState toState);

	/// <summary>
	/// Try transitioning based on the current state and event that has occurred.
	/// </summary>
	/// <param name="event">The event.</param>
	/// <returns>Returns true if the transition successfully changed the state.</returns>
	bool TryTransition(TEvent @event);

	/// <summary>
	/// Update the state machine.
	/// </summary>
	/// <param name="deltaTime">The delta time.</param>
	void Update(float deltaTime);
}
