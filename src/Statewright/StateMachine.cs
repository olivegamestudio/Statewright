namespace Statewright;

/// <summary>
/// A simple state machine.
/// </summary>
/// <param name="initialState">The initial state from the TState enumeration.</param>
/// <typeparam name="TState">The state enumeration.</typeparam>
/// <typeparam name="TEvent">The event for changing state.</typeparam>
public class StateMachine<TState, TEvent>(TState initialState) : IStateMachine<TState, TEvent>
	where TState : Enum where TEvent : Enum
{
	/// <inheritdoc />
	public TState CurrentState { get; private set; } = initialState;

	/// <summary>
	/// The available and configurable transitions between states.
	/// </summary>
	readonly Dictionary<TState, Dictionary<TEvent, TState>> _transitions = [];

	/// <inheritdoc />
	public event Action<TState, TState>? OnEnter;

	/// <inheritdoc />
	public event Action<TState, TState>? OnExit;

	/// <inheritdoc />
	public void Configure(TState fromState, TEvent @event, TState toState)
	{
		if (_transitions.TryGetValue(fromState, out Dictionary<TEvent, TState>? transitions))
		{
			// add another trigger for state
			transitions.Add(@event, toState);
			return;
		}

		// add first trigger for state
		_transitions[fromState] = new Dictionary<TEvent, TState>
		{
			[@event] = toState
		};
	}

	/// <summary>
	/// Transition to a new state.
	/// </summary>
	/// <param name="toState">The state to transition to.</param>
	protected void TransitionTo(TState toState)
	{
		// we have a configurable transition
		TState fromState = CurrentState;

		// inform exiting the current state.
		OnExit?.Invoke(fromState, toState);

		// changed state
		CurrentState = toState;

		// inform entering the new state.
		OnEnter?.Invoke(fromState, toState);
	}

	/// <inheritdoc />
	public bool TryTransition(TEvent @event)
	{
		if (_transitions.TryGetValue(CurrentState, out Dictionary<TEvent, TState>? transitions))
		{
			if (transitions.TryGetValue(@event, out TState? toState))
			{
				TransitionTo(toState);
				return true;
			}
		}

		// there was no configurable transition!
		return false;
	}

	/// <inheritdoc />
	public virtual void Update(float deltaTime)
	{
		// this is here for a uniform state machine API.
	}
}
