# Command pattern (`Air.GameCore.Command`)

Pure GoF command / undo stack for gameplay logic. No HTTP, CLI, JSON, or Unity.

| Type | Role |
|------|------|
| `ICommand` / `IUndoable` | Execute / Undo |
| `CommandHistory` | Undo / redo (`Do`, `Run`, `Undo`, `Redo`) |
| `CompositeCommand` | Macro undo |

CLI / HTTP invoke and deferred jobs live in `com.air.unity-connector` (`Air.UnityConnector.Invoke`, `.Job`).

See [ARCHITECTURE.md](../../../docs/ARCHITECTURE.md) for where CLI and input live.
