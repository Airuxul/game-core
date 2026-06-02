# com.air.game-core

Pure C# (`noEngineReferences`). **v3.0.0**

## Modules

| Folder | Namespace | Tag |
|--------|-----------|-----|
| `Pool/` | `Air.GameCore` | `core-foundation` |
| `StateMachine/` | `Air.GameCore.StateMachine` | `core-fsm` |
| `Procedure/` | `Air.GameCore.Procedure` | `core-fsm` |
| `Command/` | `Air.GameCore.Command` | `core-command` (GoF undo only) |
| `Entity/` | `Air.GameCore.Entity` | `core-entity` |
| `Serialization/` | `Air.GameCore.Serialization` | `core-serialization` |

CLI HTTP / invoke / job protocol lives in `com.air.unity-connector` (`Air.UnityConnector.Host`, `.Http`, `.Invoke`, `.Job`).

## JSON

Register `JsonHost.Instance` from `com.air.unity-game-core` before connector or gameplay code deserializes JSON via `Air.GameCore.Serialization`.
