# Game Core (`com.air.game-core`)

? C# ??????**??? Unity ??**????????????????

## ??

| ?? | ???? | ?? |
|------|----------|------|
| ?? | `Air.GameCore` | ?? `Singleton<T>` |
| ??? | `Air.GameCore` | `ObjectPool<T>`?`ListPool<T>`?`IPoolable` |
| ??? | `Air.GameCore.StateMachine` | `StateMachine`?`State`?`StateTransition` |
| ????? | `Air.GameCore.StateMachine.Layered` | `LayeredStateMachine`???????? |

## ??

```json
"com.air.game-core": "file:../CustomPackages/packages/com.air.game-core"
```

## ?????

```csharp
using Air.GameCore.StateMachine;

public class IdleState : State
{
    public override void Enter() { }
    public override void Update(float deltaTime) { }
    public override void Exit() { }
}

var machine = new StateMachine(new IdleState());
machine.Tick(Time.deltaTime);
```

## ?????

```csharp
using Air.GameCore;

var pool = new ObjectPool<MyClass>(() => new MyClass(), obj => obj.Reset());
var item = pool.Get();
pool.Release(item);
```

## ??

??? `package.json` dependencies??

## ???

- [Unity Game Core](../com.air.unity-game-core/README.md) ? Unity ?????????
