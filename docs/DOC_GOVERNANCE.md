# Documentation Governance — `com.air.game-core`

**Last Updated:** 2026-06-02 · **Owner:** package maintainers · **Scope:** documentation workflow (English)

## Purpose

Define how user and agent documentation is structured and updated in **this** Git submodule, while linking enforceable standards in the [AirUnityPackage](https://github.com/Airuxul/AirUnityPackage) meta repository.

## Document layout

| Track | Paths | Language |
|-------|--------|----------|
| User | `README.md`, `README.zh-CN.md` | English + Chinese (must stay in sync) |
| Agent | `docs/AGENTS.md`, `docs/DOC_GOVERNANCE.md`, `docs/CHANGELOG_AGENT.md` | English |
| Module design | `Command/DESIGN.md`, `Entity/DESIGN.md` | English (keep when README would be too long) |

There is **no** `config/` or `tools/validate-docs.ps1` in this package repository. Validation hooks run in the meta repo when this submodule is part of an AirUnityPackage working tree.

## Cursor skills — meta repository only

Cursor Agent skills **`doc-read-index`** and **`doc-generate-update`** exist **only** under the meta repo:

`https://github.com/Airuxul/AirUnityPackage/tree/main/.cursor/skills/`

**Do not** create `.cursor/skills/` or any `SKILL.md` under `com.air.game-core`. Submodule doc updates are performed from the meta repo (or by following the same rules manually).

## Metadata (recommended)

Each managed markdown file should include near the top:

- **Last Updated** (date)
- **Owner** (`package maintainers` or team name)
- **Scope** (user vs agent vs module design)

## Update workflow

1. Read [AGENTS.md](AGENTS.md) and this file.
2. From the meta repo, run skill `doc-read-index` when unsure what is out of date.
3. Classify impact: user README / agent `docs/` / `*/DESIGN.md` / code-only.
4. Apply changes; keep `README.md` and `README.zh-CN.md` aligned for user-visible edits.
5. Append a dated summary to [CHANGELOG_AGENT.md](CHANGELOG_AGENT.md) for non-trivial agent doc work.
6. If committing from **AirUnityPackage**, run `.\tools\validate-docs.ps1` or use `.\tools\install-git-hooks.ps1` (meta repo only).

## Cross-repo standards

| Topic | Authority |
|-------|-----------|
| Layering (L0) | [ARCHITECTURE.md](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/PACKAGE_ARCHITECTURE.md) |
| C# layout and style | [C_SHARP_STANDARDS.md](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/C_SHARP_STANDARDS.md) |
| Package boundaries | [CONSTRAINTS.md](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/PACKAGE_CONSTRAINTS.md) |
| Feature tags | [PACKAGE_TAGS.md](https://github.com/Airuxul/AirUnityPackage/blob/main/.cursor/rules/PACKAGE_TAGS.md) |
| Meta doc governance | [DOC_GOVERNANCE.md](https://github.com/Airuxul/AirUnityPackage/blob/main/docs/DOC_GOVERNANCE.md) |

## What not to add

- `.cursor/skills/` or copied SKILL.md files in this package
- Redundant `QUICKSTART.md` when README covers install and modules
- Duplicate registry tables (source of truth: meta `config/registry.json`)
- CLI/HTTP protocol documentation (belongs in `com.air.unity-connector`)

## Preserved design files

Do not remove or relocate without explicit request:

- `Command/DESIGN.md`
- `Entity/DESIGN.md`
