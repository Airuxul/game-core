# Game Core (`com.air.game-core`)

[简体中文](README.zh-CN.md)

Pure C# game framework (`noEngineReferences`). No `UnityEngine`, no CLI/HTTP protocol, no Newtonsoft implementation.

## Quick facts

| Item | Value |
|------|--------|
| **Layer** | L0 — pure C# foundation |
| **UPM id** | `com.air.game-core` |
| **Version** | 3.1.0 |
| **Asmdef** | `com.air.GameCore` → `Air.GameCore` |
| **Unity** | 2020.3+ (package metadata only; no engine references) |
| **UPM dependencies** | none |
| **Typical consumers** | `com.air.unity-game-core`, tests, headless tools |

Indexed in meta repo [AirUnityPackage](https://github.com/Airuxul/AirUnityPackage) (`config/registry.json`, `installDefault: true`).

## Install

In your Unity project `Packages/manifest.json` (adjust path to your clone):

```json
{
  "dependencies": {
    "com.air.game-core": "file:../CustomPackages/packages/com.air.game-core"
  }
}
```

Git submodule path in AirUnityPackage: `packages/com.air.game-core` → [game-core](https://github.com/Airuxul/game-core.git).

## Modules

| Folder | Namespace | Tag | Notes |
|--------|-----------|-----|--------|
| `Pool/` | `Air.GameCore` | `core-foundation` | `ObjectPool<T>`, `ListPool`, `IPoolable` |
| *(root)* | `Air.GameCore` | `core-foundation` | `Singleton<T>` |
| `StateMachine/` | `Air.GameCore.StateMachine` | `core-fsm` | `State`, `StateMachine`, layered FSM |
| `Procedure/` | `Air.GameCore.Procedure` | `core-fsm` | `ProcedureBase`, `ProcedureMachine` |
| `Command/` | `Air.GameCore.Command` | `core-command` | GoF execute/undo only — not CLI |
| `Entity/` | `Air.GameCore.Entity` | `core-entity` | GF-style entity manager + logic |
| `Serialization/` | `Air.GameCore.Serialization` | `core-serialization` | `IJsonSerializer`, `JsonHost` contract |

Module design notes: [Command/DESIGN.md](Command/DESIGN.md), [Entity/DESIGN.md](Entity/DESIGN.md).

**Out of scope here:** CLI HTTP / invoke / job protocol → `com.air.unity-connector` (`Air.UnityConnector.Host`, `.Http`, `.Invoke`, `.Job`). Unity `GameObject` binding → `com.air.unity-game-core`.

## JSON

`JsonHost.Instance` is a static slot for `IJsonSerializer`. Register an implementation from `com.air.unity-game-core` before connector or gameplay code deserializes via `Air.GameCore.Serialization`.

## Minimal examples

### Command undo

```csharp
using Air.GameCore.Command;

var history = new CommandHistory();
history.Run(new MyUndoableCommand());
history.Undo();
```

### Entity show / tick

```csharp
using Air.GameCore.Entity;

var manager = new EntityManager(factory);
manager.ShowEntity(typeId, userData);
manager.Tick(deltaTime);
```

### FSM

```csharp
using Air.GameCore.StateMachine;

// Subclass StateMachine, register states via StateTransition — see StateMachine/ sources.
```

## Related

- [AirUnityPackage architecture](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/PACKAGE_ARCHITECTURE.md)
- [C# standards](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/C_SHARP_STANDARDS.md)
- [Unity Game Core](../com.air.unity-game-core/README.md) — runtime wiring, `JsonHost`, `UnityEntityManager`
- [Unity Connector](../unity-cli/com.air.unity-connector/README.md) — Editor/Player HTTP CLI
