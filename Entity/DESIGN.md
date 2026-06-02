# Entity (`Air.GameCore.Entity`)

GF-style entity lifecycle: **EntityManager** + **EntityLogic** + **EntityGroup**.

## Core types

| Type | Role |
|------|------|
| `EntityTypeId` | Config / asset type id |
| `EntityId` | Runtime serial id (monotonic int) |
| `EntityGroupName` | Group for capacity / batch hide |
| `IEntityLogic` / `EntityLogicBase` | `OnInit` / `OnShow` / `OnHide` / `OnUpdate` / attach / recycle |
| `IEntityManager` / `EntityManager` | `ShowEntity`, `HideEntity`, `AttachEntity`, `Tick` |
| `IEntityLogicFactory` | Creates logic per type id |

## Unity bridge

See `com.air.unity-game-core` — `UnityEntityManager` loads prefabs via `IResManager.LoadInstanceAsync` and binds `UnityEntityInstance`.
