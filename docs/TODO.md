# TODO — `com.air.game-core`

**Last Updated:** 2026-06-03 · **Owner:** package maintainers · **Scope:** follow-up optimizations on **existing** L0 APIs (English)

> **User doc (Chinese):** [../TODO.zh-CN.md](../TODO.zh-CN.md)

> **Boundary:** Pool, FSM/Procedure, GoF Command/undo, GF Entity model, JSON **contract** only.  
> **Out of scope:** `UnityEngine`, Newtonsoft wiring, HTTP/CLI, `GameRuntime`, UI.  
> **Meta rollup:** [AirUnityPackage `docs/TODO_ROADMAP.md`](https://github.com/Airuxul/AirUnityPackage/blob/main/docs/TODO_ROADMAP.md)

## Capability baseline

- `ObjectPool<T>`, `ListPool<T>`, `Singleton<T>`
- Flat + layered FSM; `ProcedureMachine` / `ProcedureBase`
- `ICommand` / `CommandHistory` / `CompositeCommand`
- `EntityManager` sync show/hide, groups, attach/detach, `Tick`
- `IJsonSerializer` + `JsonHost` facade (implementation registered upstream)

## TODO

| ID | Pri | Title | Description |
|----|-----|-------|-------------|
| GC-01 | P0 | L0 unit tests | Cover `CommandHistory` trim/redo, `CompositeCommand`, `EntityManager` capacity/pause, layered FSM LCA, `JsonHost` when unset. |
| GC-02 | P1 | Public entity group configuration | Expose configure API for `InstanceCapacity` / `Paused`; today groups are internal-only. |
| GC-03 | P1 | `InstanceAutoReleaseInterval` | Implement in `Tick` or remove/document as reserved (currently unused). |
| GC-04 | P1 | Entity manager hot-path allocations | Use `ListPool<EntityId>`; maintain per-group counts instead of scanning all entities. |
| GC-05 | P2 | `JsonHost.Deserialize<T>` | Parity with `IJsonSerializer` so L0 callers do not bypass the host. |
| GC-06 | P2 | Entity attach/hide semantics | Guard cycles; cascade detach or hide dependents per `Entity/DESIGN.md`. |
| GC-07 | P2 | `IEntityManager` query polish | `TryGetEntity`; align README samples with `(typeId, group, userData)` signature. |
| GC-08 | P2 | `CommandHistory.Trim` allocation | Replace full-stack copy on depth overflow with ring buffer or drop-from-bottom. |
| GC-09 | P3 | FSM / procedure design docs | Add `StateMachine/DESIGN.md`; document layered vs flat usage; `InvalidOperationException` on double `Start`. |
| GC-10 | P3 | Doc and tag hygiene | Sync `README.zh-CN.md`, package `docs/AGENTS.md`, `core-serialization` tag with meta `package-tags.json`. |
| GC-11 | P3 | `ObjectPool<T>` caps | Optional max size / `Clear` for editor cycles and tests. |

## Do not assign here

| Topic | Owner package |
|-------|----------------|
| `GameRuntime`, services composition | `com.air.unity-game-core` |
| Unity entity views, async show, prefab load | `com.air.unity-game-core` |
| Newtonsoft / `JsonSerializationBootstrap` | `com.air.unity-game-core` |
| Input → undo bindings | `com.air.unity-game-core` |
| HTTP/CLI invoke, jobs | `com.air.unity-connector` |
| UI panels | `com.air.unity-ui` |
