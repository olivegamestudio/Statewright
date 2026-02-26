# Statewright

A lightweight, enum-driven state machine library for .NET with support for timed transitions.

[![NuGet](https://img.shields.io/nuget/v/Statewright.svg)](https://www.nuget.org/packages/Statewright)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## Installation

```bash
dotnet add package Statewright
```

---

## Quick Start

Define your states and events as enums, configure transitions, then fire events to move between states.

```csharp
using Statewright;

enum FlightState { Idle, TakingOff, Flying, Landing }
enum FlightEvent { TakeOff, ReachAltitude, Land, TouchDown }

var machine = new StateMachine<FlightState, FlightEvent>(FlightState.Idle);

machine.Configure(FlightState.Idle,      FlightEvent.TakeOff,      FlightState.TakingOff);
machine.Configure(FlightState.TakingOff, FlightEvent.ReachAltitude, FlightState.Flying);
machine.Configure(FlightState.Flying,    FlightEvent.Land,          FlightState.Landing);
machine.Configure(FlightState.Landing,   FlightEvent.TouchDown,     FlightState.Idle);

machine.OnEnter += (from, to) => Console.WriteLine($"{from} → {to}");

machine.TryTransition(FlightEvent.TakeOff);      // Idle → TakingOff
machine.TryTransition(FlightEvent.ReachAltitude); // TakingOff → Flying
```

---

## StateMachine

### Configuring Transitions

```csharp
machine.Configure(fromState, triggerEvent, toState);
```

Multiple events can trigger transitions from the same state:

```csharp
machine.Configure(FlightState.Flying, FlightEvent.Land,      FlightState.Landing);
machine.Configure(FlightState.Flying, FlightEvent.EmergencyLand, FlightState.Landing);
```

### Transitioning

`TryTransition` returns `true` if a valid transition was found, `false` if the event is not configured for the current state:

```csharp
if (!machine.TryTransition(FlightEvent.Land))
{
    Console.WriteLine("Cannot land from current state");
}
```

### Events

`OnEnter` and `OnExit` fire on every state change, reporting both the origin and destination states:

```csharp
machine.OnEnter += (from, to) => Console.WriteLine($"Entering {to} from {from}");
machine.OnExit  += (from, to) => Console.WriteLine($"Exiting {from} toward {to}");
```

---

## TimedStateMachine

`TimedStateMachine` extends `StateMachine` with the ability to automatically transition after a duration.

```csharp
enum DoorState { Open, Closing, Closed }
enum DoorEvent { Close, Latch }

var door = new TimedStateMachine<DoorState, DoorEvent>(DoorState.Open);

door.Configure(DoorState.Open,    DoorEvent.Close, DoorState.Closing);
door.Configure(DoorState.Closing, DoorEvent.Latch, DoorState.Closed);

// Automatically fire Latch after 2 seconds in the Closing state
door.ConfigureTimer(DoorState.Closing, DoorEvent.Latch, duration: 2f);

// Call each frame with elapsed seconds
door.Update(deltaTime);
```

---

## Subclassing

Both `StateMachine` and `TimedStateMachine` are designed to be subclassed. `TransitionTo` is `protected`, allowing subclasses to force transitions directly when needed:

```csharp
public class AirshipStateMachine : StateMachine<FlightState, FlightEvent>
{
    public AirshipStateMachine() : base(FlightState.Idle)
    {
        Configure(FlightState.Idle,    FlightEvent.TakeOff, FlightState.Flying);
        Configure(FlightState.Flying,  FlightEvent.Land,    FlightState.Idle);

        OnEnter += HandleEnter;
    }

    void HandleEnter(FlightState from, FlightState to)
    {
        if (to == FlightState.Flying)
            Console.WriteLine("Airship is airborne");
    }
}
```

---

## Design Goals

- **No dependencies** — pure .NET, no third-party packages
- **Enum-driven** — states and events are enums, keeping configurations readable and refactor-friendly
- **Explicit transitions** — invalid events are silently ignored unless you check the return value of `TryTransition`
- **Extensible** — subclass to add domain logic without modifying the core machine

---

## License

MIT
