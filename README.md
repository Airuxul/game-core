# Game Core (`com.air.game-core`)

Pure C# utilities with **no Unity engine dependency** (`noEngineReferences`).

## Modules

| Module | Namespace | Description |
|--------|-----------|-------------|
| Singleton | `Air.GameCore` | `Singleton<T>` |
| Pool | `Air.GameCore` | static `ObjectPool<T>`, `ListPool<T>`, `IPoolable` |
| State machine | `Air.GameCore.StateMachine` | `StateMachine`, `State`, `StateTransition` |
| Layered FSM | `Air.GameCore.StateMachine.Layered` | hierarchical states |

**Pool vs Unity:** `ObjectPool<T>` is for plain C# objects. Unity `GameObject` / `Component` pooling lives in `com.air.unity-game-core` (`PoolManager` / `UnityObjectPool`).

## State machine

Avoid passing `this` before the machine exists. Prefer `Initialize` from a `StateMachine` subclass:

```csharp
using Air.GameCore.StateMachine;

public sealed class IdleState : State { }

public sealed class GameStateMachine : StateMachine
{
    public GameStateMachine() => Initialize(new IdleState());
}

var sm = new GameStateMachine();
sm.Tick(0.016f);
```

Legacy constructor `StateMachine(State initial)` still works and calls `Initialize` internally.

## Object pool

```csharp
using Air.GameCore;

ObjectPool<MyClass>.SetFactory(() => new MyClass());
var item = ObjectPool<MyClass>.Get();
ObjectPool<MyClass>.Return(item);
```

## Related

- [Unity Game Core](../com.air.unity-game-core/README.md)
