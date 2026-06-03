# AGENTS — `com.air.game-core`

**Last Updated:** 2026-06-02 · **Owner:** package maintainers · **Scope:** canonical agent entry (this repository, English)

## Canonical rule

This file is the agent entrypoint for **this** Git repository (`com.air.game-core`). If another file in this repo conflicts with it, this file wins.

## Package scope

| Property | Value |
|----------|--------|
| **UPM id** | `com.air.game-core` |
| **Layer** | L0 — pure C# (`noEngineReferences`) |
| **Repository** | [game-core](https://github.com/Airuxul/game-core.git) |
| **Meta index** | [AirUnityPackage `config/registry.json`](https://github.com/Airuxul/AirUnityPackage/blob/main/config/registry.json) |

**In scope:** Pool, Singleton, FSM/Procedure, GoF Command/undo, GF Entity model, JSON serializer **contract** (`IJsonSerializer`, `JsonHost`).

**Out of scope:** `UnityEngine`, Newtonsoft wiring, CLI/HTTP (`com.air.unity-connector`), UI, `GameRuntime` (`com.air.unity-game-core`).

## User documentation

| File | Language |
|------|----------|
| [README.md](../README.md) | English |
| [README.zh-CN.md](../README.zh-CN.md) | Chinese — must stay in sync with English |

## Agent documentation (this repo)

| File | Purpose |
|------|---------|
| [AGENTS.md](AGENTS.md) | This file |
| [DOC_GOVERNANCE.md](DOC_GOVERNANCE.md) | Doc workflow; skills live in meta repo only |
| [CHANGELOG_AGENT.md](CHANGELOG_AGENT.md) | Agent-side changelog |
| [../Command/DESIGN.md](../Command/DESIGN.md) | GoF command / undo (keep when editing Command/) |
| [../Entity/DESIGN.md](../Entity/DESIGN.md) | Entity model (keep when editing Entity/) |

Do **not** add `QUICKSTART.md` when install and modules fit in README. Do **not** create `.cursor/skills/` in this package.

## Meta repository standards

When editing C#, layering, or cross-package boundaries, follow [AirUnityPackage](https://github.com/Airuxul/AirUnityPackage) docs:

| Document | Use when |
|----------|----------|
| [C_SHARP_STANDARDS](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/C_SHARP_STANDARDS.md) | File layout, namespaces, formatting |
| [ARCHITECTURE](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/PACKAGE_ARCHITECTURE.md) | L0/L1 ownership, what belongs in game-core |
| [CONSTRAINTS](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/PACKAGE_CONSTRAINTS.md) | v2 API boundaries, forbidden duplicates |
| [PACKAGE_TAGS](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/PACKAGE_TAGS.md) | Feature tags (`core-foundation`, `core-fsm`, …) |

## Cursor skills (meta repo only)

| Skill | Location | Use |
|-------|----------|-----|
| `doc-read-index` | `AirUnityPackage/.cursor/skills/doc-read-index/SKILL.md` | Read-only doc inventory, gaps, language parity |
| `doc-generate-update` | `AirUnityPackage/.cursor/skills/doc-generate-update/SKILL.md` | Apply doc updates with dual-track rules |

**Do not** copy skills into this submodule. Package doc maintenance is driven from the meta repo using those skills.

## Required reads before doc updates

1. `docs/AGENTS.md` (this file)
2. `docs/DOC_GOVERNANCE.md`
3. `README.md` and `README.zh-CN.md`
4. Meta [DOC_GOVERNANCE](https://github.com/Airuxul/AirUnityPackage/blob/main/docs/DOC_GOVERNANCE.md) when validation or hooks apply to a meta-repo commit

## Language policy

- Agent markdown in `docs/` is English.
- User-facing changes require both `README.md` and `README.zh-CN.md`.

## Dual-track update rule

| Change type | Action |
|-------------|--------|
| User-visible (install, modules, behavior) | Update both READMEs **and** relevant `docs/*.md` / `*/DESIGN.md` |
| Agent-only `docs/*.md` | Update README only if user-visible behavior changed |
| Code-only, no user impact | README optional; append `CHANGELOG_AGENT.md` for non-trivial agent edits |

Mechanical typos/link fixes may skip dual-track if the commit message states the exemption.

## Code change reminders

- One main type per file; namespace matches `com.air.GameCore.asmdef` / folder.
- No `UnityEditor` or `UnityEngine` in this package.
- Command pattern here is **undo stack only** — not connector invoke/job types.
- After C# edits in a Unity workspace, follow meta-repo compile verification when available.
