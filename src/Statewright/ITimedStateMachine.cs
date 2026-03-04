namespace Statewright;

public interface ITimedStateMachine<TState, TEvent> : IStateMachine<TState, TEvent>
	where TState : Enum
	where TEvent : Enum
{
	/// <summary>
	/// The time spent in the current state.
	/// </summary>
	float TimeInCurrentState { get; }

	/// <summary>
	/// Configure the timeout.
	/// </summary>
	/// <param name="fromState">The `from` state.</param>
	/// <param name="timeout">The amount of time before auto-transitioning state.</param>
	/// <param name="toState">The `to` (destination) state.</param>
	void ConfigureTimeout(TState fromState, float timeout, TState toState);
}
