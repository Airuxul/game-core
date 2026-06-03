# Game Core（`com.air.game-core`）

[English](README.md)

纯 C# 游戏基础库（`noEngineReferences`）。不引用 `UnityEngine`，不包含 CLI/HTTP 协议，不包含 Newtonsoft 具体实现。

## 速览

| 项 | 值 |
|----|-----|
| **层级** | L0 — 纯 C# 基础层 |
| **UPM id** | `com.air.game-core` |
| **版本** | 3.1.0 |
| **Asmdef** | `com.air.GameCore` → 命名空间 `Air.GameCore` |
| **Unity** | 2020.3+（仅包元数据；无引擎引用） |
| **UPM 依赖** | 无 |
| **常见上游** | `com.air.unity-game-core`、单元测试、无头工具 |

在元仓库 [AirUnityPackage](https://github.com/Airuxul/AirUnityPackage) 的 `config/registry.json` 中登记（`installDefault: true`）。

## 安装

在 Unity 项目 `Packages/manifest.json` 中（路径按你的克隆位置调整）：

```json
{
  "dependencies": {
    "com.air.game-core": "file:../CustomPackages/packages/com.air.game-core"
  }
}
```

AirUnityPackage 子模块路径：`packages/com.air.game-core` → 仓库 [game-core](https://github.com/Airuxul/game-core.git)。

## 模块

| 目录 | 命名空间 | Tag | 说明 |
|------|----------|-----|------|
| `Pool/` | `Air.GameCore` | `core-foundation` | `ObjectPool<T>`、`ListPool`、`IPoolable` |
| *（根目录）* | `Air.GameCore` | `core-foundation` | `Singleton<T>` |
| `StateMachine/` | `Air.GameCore.StateMachine` | `core-fsm` | `State`、`StateMachine`、分层 FSM |
| `Procedure/` | `Air.GameCore.Procedure` | `core-fsm` | `ProcedureBase`、`ProcedureMachine` |
| `Command/` | `Air.GameCore.Command` | `core-command` | GoF 执行/撤销 — 非 CLI |
| `Entity/` | `Air.GameCore.Entity` | `core-entity` | GF 风格实体管理 |
| `Serialization/` | `Air.GameCore.Serialization` | `core-serialization` | `IJsonSerializer`、`JsonHost` 契约 |

设计说明：[Command/DESIGN.md](Command/DESIGN.md)、[Entity/DESIGN.md](Entity/DESIGN.md)。

**不在本包：** CLI HTTP / 调用 / 延迟任务 → `com.air.unity-connector`。Unity `GameObject` 绑定 → `com.air.unity-game-core`。

## JSON

`JsonHost.Instance` 为 `IJsonSerializer` 的静态注入点。在通过 `Air.GameCore.Serialization` 反序列化前，由 `com.air.unity-game-core` 注册实现。

## 最小示例

### 命令撤销

```csharp
using Air.GameCore.Command;

var history = new CommandHistory();
history.Run(new MyUndoableCommand());
history.Undo();
```

### 实体显示 / Tick

```csharp
using Air.GameCore.Entity;

var manager = new EntityManager(factory);
manager.ShowEntity(typeId, userData);
manager.Tick(deltaTime);
```

### 状态机

```csharp
using Air.GameCore.StateMachine;

// 继承 StateMachine，通过 StateTransition 注册状态 — 见 StateMachine/ 源码。
```

## 相关文档

- [AirUnityPackage 架构](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/PACKAGE_ARCHITECTURE.md)
- [C# 规范](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/C_SHARP_STANDARDS.md)
- [Unity Game Core](../com.air.unity-game-core/README.md) — 运行时装配、`JsonHost`、`UnityEntityManager`
- [Unity Connector](../unity-cli/com.air.unity-connector/README.md) — Editor/Player HTTP CLI
