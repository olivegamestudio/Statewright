namespace Statewright;

/// <summary>
/// A state machine that supports time limits in states.
/// </summary>
/// <typeparam name="TState">The state enumeration.</typeparam>
/// <typeparam name="TEvent">The event for changing state.</typeparam>
public sealed class TimedStateMachine<TState, TEvent> : 
    StateMachine<TState, TEvent>
    where TState : Enum
    where TEvent : Enum
{
    /// <summary>
    /// The state timeouts.
    /// </summary>
    readonly Dictionary<TState, (TState toState, float timeout)> _timeouts = [];

    /// <summary>
    /// The time spent in the current state.
    /// </summary>
    public float TimeInCurrentState { get; private set; }

    /// <summary>
    /// Creates a new timed state machine.
    /// </summary>
    /// <param name="initialState">The initial state.</param>
    public TimedStateMachine(TState initialState) : base(initialState)
    {
        OnEnter += (fromState, toState) => TimeInCurrentState = 0;
    }
    
    /// <summary>
    /// Configure the timeout.
    /// </summary>
    /// <param name="fromState">The `from` state.</param>
    /// <param name="timeout">The amount of time before auto-transitioning state.</param>
    /// <param name="toState">The `to` (destination) state.</param>
    public void ConfigureTimeout(TState fromState, float timeout, TState toState)
    {
        _timeouts[fromState] = (toState, timeout);
    }

    /// <summary>
    /// Update the state machine.
    /// </summary>
    /// <param name="deltaTime">The delta time.</param>
    public override void Update(float deltaTime)
    {
        TimeInCurrentState += deltaTime;
        
        if (_timeouts.TryGetValue(CurrentState, out (TState toState, float timeout) timeoutConfig) &&
            TimeInCurrentState >= timeoutConfig.timeout)
        {
            TransitionTo(timeoutConfig.toState);
        }
        
        base.Update(deltaTime);
    }
}
