# 待办 — `com.air.game-core`

**最后更新：** 2026-06-03 · **范围：** L0 纯 C# 现有 API 的后续优化（中文）

> **职责边界：** 对象池、FSM/流程、GoF 命令撤销、GF 实体模型、JSON **契约**。  
> **不负责：** Unity、Newtonsoft 实现、HTTP/CLI、`GameRuntime`、UI。  
> Agent 英文条目：[`docs/TODO.md`](docs/TODO.md) · 跨包统筹：[`docs/TODO_ROADMAP.md`](https://github.com/Airuxul/AirUnityPackage/blob/main/docs/TODO_ROADMAP.md)

## 现有能力概要

- `ObjectPool<T>`、`ListPool<T>`、`Singleton<T>`
- 平面与分层状态机；`ProcedureMachine` / `ProcedureBase`
- `ICommand` / `CommandHistory` / `CompositeCommand`
- `EntityManager` 同步显示/隐藏、编组、父子挂接、`Tick`
- `IJsonSerializer` + `JsonHost`（实现由上层注册）

## 待办列表

| ID | 优先级 | 标题 | 说明 |
|----|--------|------|------|
| GC-01 | P0 | L0 单元测试 | 覆盖命令栈裁剪/重做、组合命令、实体容量/暂停、分层 FSM LCA、`JsonHost` 未注册等。 |
| GC-02 | P1 | 实体编组对外配置 | 暴露 `InstanceCapacity` / `Paused` 等配置 API（当前仅内部创建编组）。 |
| GC-03 | P1 | 自动释放间隔 | 实现 `InstanceAutoReleaseInterval` 或在文档中标为保留字段。 |
| GC-04 | P1 | 实体管理器热路径分配 | 使用 `ListPool`；按编组维护计数，避免每次 Show 全表扫描。 |
| GC-05 | P2 | `JsonHost.Deserialize<T>` | 与 `IJsonSerializer` 对齐，避免绕过 Host。 |
| GC-06 | P2 | 挂接/隐藏语义 | 防环；按 `Entity/DESIGN.md` 处理级联分离/隐藏。 |
| GC-07 | P2 | 查询 API 打磨 | 增加 `TryGetEntity`；README 示例与 `(typeId, group, userData)` 签名一致。 |
| GC-08 | P2 | `CommandHistory.Trim` 分配 | 深度溢出时用环形缓冲等，避免整栈复制。 |
| GC-09 | P3 | FSM/流程设计文档 | 补充 `StateMachine/DESIGN.md`；重复 `Start` 抛 `InvalidOperationException`。 |
| GC-10 | P3 | 文档与标签 | 同步 `README.zh-CN.md`、`docs/AGENTS.md`、meta 标签索引。 |
| GC-11 | P3 | 对象池上限 | 可选容量上限与 `Clear`，便于编辑器循环与测试。 |

## 请勿在本包实现

| 主题 | 归属包 |
|------|--------|
| `GameRuntime` 与服务组装 | `com.air.unity-game-core` |
| Unity 实体视图、异步加载 | `com.air.unity-game-core` |
| Newtonsoft 引导 | `com.air.unity-game-core` |
| 输入绑定撤销 | `com.air.unity-game-core` |
| HTTP/CLI | `com.air.unity-connector` |
| UI 面板 | `com.air.unity-ui` |
