# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

A Unity 3D first-person detective/murder-mystery game (Turkish-language project, editor version
**6000.3.10f1** — see `ProjectSettings/ProjectVersion.txt`). The player investigates a crime scene:
collecting evidence, swabbing DNA samples, photographing clues, interviewing NPCs, and pinning
findings to a case board.

This is a git repository. The modular-architecture refactor (see "Architecture" below) was carried
out as a sequence of phase commits — `git log` is a useful map of how the current package/collaborator
split came together if you need that history.

See `AGENTS.md` for project vision/roadmap context (what the game is trying to become, planned-but-not-
yet-built systems, and notes on the design/UML reference docs) — that file is tool-agnostic and kept in
sync with this one's architecture section.

## Working with the project

There is no CLI build/test/lint pipeline — this is a Unity Editor project (`.csproj`/`.sln` files are
IDE-generated for editing, not for `dotnet build`). All builds, play-mode testing, and scene edits
normally happen inside the Unity Editor (version 6000.3.10f1, Universal Render Pipeline).

When Unity Editor MCP tools are available in this session, use them (`unity-mcp-skill`) to inspect
scenes/prefabs, move/modify GameObjects, edit components, and read console/compile errors, rather than
hand-editing `.unity`/`.prefab` YAML files directly. `.unity` and `.prefab` files are large serialized
YAML — editing them by hand is error-prone; prefer the Editor/MCP tools or C# script changes instead.

No automated test suite exists yet (the `com.unity.test-framework` package is installed but unused —
there are no `*Tests*` asmdefs or files in the repo).

## Architecture

### Modular package structure

Game logic is split into local UPM packages under `Shared-Packages/`, each its own assembly, plus a
default-assembly layer in `Assets/` that has no asmdef of its own (compiles into `Assembly-CSharp`).
Packages are referenced by `Packages/manifest.json` via `file:../Shared-Packages/...`.

Dependency graph (derived from the packages' `.asmdef` references — not all Detective packages depend
on each other):

```
Detective.Controller (base: FPS input/movement prefab, InputSystem_Actions — no script deps)
        ^
        |
Detective.Interaction  --> also depends on Detective.UI and Detective.DNAFingerPrint
        ^
        |
Detective.NPC

Detective.UI              (leaf: only depends on Unity TextMeshPro/InputSystem)
Detective.DNAFingerPrint  (leaf: only depends on Unity TextMeshPro/InputSystem)
```

- **Detective.Controller** (`com.detective.controller`) — base first-person controller setup/input
  actions. No scripts, so it has no `.csproj` of its own.
- **Detective.Interaction** (`com.detective.interaction`) — the hub package: raycast interaction,
  holdable items/hotbar, evidence inspection, DNA collection points, photo capture, object placement.
  Pulls together Controller, UI, and DNAFingerPrint.
- **Detective.NPC** (`com.detective.npc`) — NPC state machine, patrol, dialogue tree/UI.
- **Detective.UI** (`com.detective.ui`) — HUD, computer/window UI, case board (`CinayetTahtasi`)
  controller, pause menu. Does not depend on other Detective packages.
- **Detective.DNAFingerPrint** (`com.detective.dnafingerprint`) — the DNA-matching minigame
  (`DNAMiniGameManager`, `DNAData`). Does not depend on other Detective packages.

All package asmdefs use `"rootNamespace": ""`, so package classes live in the **global namespace** —
there are no `using Detective.X;` imports to reason about when tracing types.

### Game-specific glue layer (`Assets/`)

`Assets/Scripts/` and `Assets/Game/Scripts/` have no asmdef and compile into `Assembly-CSharp`, which
can see every package assembly. Since the packages themselves don't reference each other in reverse
(e.g. DNAFingerPrint doesn't know about Interaction), scene-specific wiring between systems lives here
instead — see `Assets/Game/Scripts/Bridge/` for the pattern (e.g.
`BilgisayarDNASorgulamaKoprusu.cs` pulls DNA evidence out of `Detective.Interaction`'s hotbar/inventory
and feeds it into `Detective.DNAFingerPrint`'s minigame manager).

`Assets/Scripts/` is organized by in-game system, mirroring gameplay features rather than packages:
`Cinayet Tahtasi` (case/evidence board controller/UI glue) and `sorun/` (despite the name — "problem"
— this is the **live** in-game computer/OS window input code, e.g. `BilgisayarModuSistemi.cs`; it is
not superseded by anything). Inventory ("Envanter Sistemi"), generic world-interaction scaffolding
("Etkileşim Scriptleri"), and the old computer-window folder ("Bilgisayar Sistemi") that this note used
to warn about were all dead code and have been deleted — their responsibilities now live in
`Detective.Interaction` (`EldeTutulabilirObje`/`HotbarSistemi`) and `Detective.UI`
(`BilgisayarPenceresi`/`PencereSurukleyici`/`PencereBoyutlayici`) respectively.

Cross-package "mode" entry points (case board, computer, evidence inspection, photo mode, DNA
collection, NPC dialogue) all route through one central reference-counted lock,
`OyuncuKontrolKilidi` (`Shared-Packages/com.detective.interaction/Runtime/Modes/`), instead of each
duplicating its own player-control-disable logic — check there first when tracing why player input is
(or isn't) locked during a UI mode.

### Data-driven content

Gameplay content is authored as ScriptableObject assets under `Assets/Datas/`, not hardcoded:
`ItemData/` (inventory/evidence items, including DNA swab variants), `NPCData/` (NPC definitions,
grouped by role folder, e.g. `Polis`, `Gorgu Tanigi`), `DNAData/` (DNA sample records). New
items/NPCs/DNA samples are typically added as new assets here rather than in code.

### Scenes

- `Assets/Scenes/MainMenu.unity` — main menu.
- `Assets/Scenes/Oyun.unity` — the main gameplay scene (player, NPCs, evidence, case board, computer,
  NavMesh, etc. all live here).

## Conventions

- Identifiers, `Debug.Log` messages, and UI strings are almost entirely in **Turkish** (e.g.
  `OyuncuKontrolKilidi`, `SorgulaVeKarsilastir`, `hotbarSistemi`) — preserve this convention in new code
  rather than switching to English, and don't be thrown by Turkish class/method names when tracing
  logic.
- MonoBehaviour fields are private with `[SerializeField]` and are re-resolved defensively in
  `Awake`/`OnEnable` via `FindFirstObjectByType`/`GetComponentInChildren` when not wired in the
  inspector (see `Bridge/BilgisayarDNASorgulamaKoprusu.cs` for the pattern).
