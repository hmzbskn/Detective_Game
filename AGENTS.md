# AGENTS.md

Generic guidance for any AI coding agent (Claude, GPT, Gemini, Copilot, Cursor, etc.) working in this
repository. Tool-specific instructions for Claude Code live in `CLAUDE.md`; the technical-architecture
section below is intentionally duplicated from there so this file is self-contained for other tools.

This document also captures **project vision and direction**, synthesized from the owner's design
documents, which live outside this repository at:
`C:\Users\Başkan\Desktop\HAMZA\HAMZA\OYUN GELİŞTİRME\Oyun Fikirleri\Detective\` (proposal/design docs,
`gpt/` subfolder) and `C:\Users\Başkan\Desktop\HAMZA\HAMZA\UML\` (UML diagrams, some outdated — see
"UML diagrams" section below). Those folders are not part of the Unity project and may not be present
in every environment this repo is opened in; the summary below is meant to make that context available
even when they aren't.

## What this project is

A Unity 3D first-person detective/murder-mystery simulation game (`Detective_Game`, working title
"Unity Tabanlı 3D Dedektiflik Simülasyon Oyunu"), built by Hamza Başkan. It originated as a Computer
Engineering "Mühendislik Tasarımı" (Engineering Design) project at Selçuk University (Teknoloji
Fakültesi, Bilgisayar Mühendisliği), advised by Arş. Gör. Musa Doğan, formally accepted 01/06/2026.
Development continues past the academic deliverable, with an eventual Steam release in the original
plan (Steam Direct registration, Steamworks SDK achievements, store page).

Player fantasy: a young detective on the murder squad investigates crime scenes — examining evidence
in 3D, collecting DNA/photos, questioning NPCs, running lab mini-games, and pinning findings to a
"zihin tahtası" (mind/case board) — and pieces together what happened instead of being walked through
a scripted deduction.

**Core design principle from the project's own literature review** (worth respecting when designing new
features): the game is explicitly a reaction against "passive detective" games where the player follows
a pre-built narrative through cutscenes and auto-resolved menus (cited critique of L.A. Noire's
lab/analysis segments, contrasted with Shadows of Doubt's freer case-board). The stated goal is high
player *agency* — the mind board should let the player make **wrong** connections and get a "faulty
hypothesis" result rather than only ever accepting correct ones, and lab/interrogation steps should be
active mini-games, not cutscenes. Treat "let the player be wrong" as a first-class requirement, not
something to smooth over with hand-holding UI.

## Current architecture (mirrors CLAUDE.md)

Unity Editor **6000.3.10f1**, Universal Render Pipeline. This is a git repository (a modular-
architecture refactor was carried out as a sequence of phase commits — `git log` maps how the current
package/collaborator split came together). There is no CLI build/test/lint pipeline — this is developed
inside the Unity Editor. `.csproj`/`.sln` files are IDE-generated, not for `dotnet build`. Prefer Unity
Editor tooling (or an Editor-connected MCP/agent integration, if available in your environment) over
hand-editing `.unity`/`.prefab` YAML.

### Modular package structure

Game logic is split into local UPM packages under `Shared-Packages/`, each its own assembly, plus a
default-assembly layer in `Assets/` with no asmdef of its own (compiles into `Assembly-CSharp`).
Referenced by `Packages/manifest.json` via `file:../Shared-Packages/...`.

Real dependency graph (from each package's `.asmdef` GUID references):

```
Detective.Controller (base: FPS input/movement prefab, InputSystem_Actions — no scripts)
        ^
        |
Detective.Interaction  --> also depends on Detective.UI and Detective.DNAFingerPrint
        ^
        |
Detective.NPC

Detective.UI              (leaf: only depends on Unity TextMeshPro/InputSystem)
Detective.DNAFingerPrint  (leaf: only depends on Unity TextMeshPro/InputSystem)
```

- **Detective.Controller** — base first-person controller/input actions. No scripts.
- **Detective.Interaction** — the hub: raycast interaction (`OyuncuEtkilesim`), holdable items/hotbar,
  evidence inspection (`IncelemeSistemi`, `EvidenceManager`), DNA collection points, photo capture,
  object placement/throwing. Pulls together Controller, UI, and DNAFingerPrint.
- **Detective.NPC** — NPC state machine (`NPCController`/`NPCState`: Idle/Talking/Disabled), patrol
  (`NPCPatrol`), branching dialogue tree (`DialogueNode`/`DialogueChoice`, evidence-gated branches via
  `gerekliDelil`), dialogue UI, "konuşma kilidi" (input lock while talking).
- **Detective.UI** — HUD (crosshair, interaction panel), computer/window UI (draggable/resizable
  windows, taskbar), case board (`CinayetTahtasi`) controller, pause menu. `UIControllerBase` is the
  shared open/close/pause base class. Does not depend on other Detective packages.
- **Detective.DNAFingerPrint** — the DNA-matching minigame (`DNAMiniGameManager`, `DNAData`: a
  simplified 0–4 numeric "spectrum" per sample, not a literal DNA strand). Does not depend on other
  Detective packages.

All package asmdefs use `"rootNamespace": ""` — package classes live in the **global namespace**, so
there are no `using Detective.X;` imports to reason about.

### Game-specific glue layer (`Assets/`)

`Assets/Scripts/` and `Assets/Game/Scripts/` have no asmdef and compile into `Assembly-CSharp`, which
sees every package assembly. Since packages don't reference each other in reverse, scene-specific wiring
lives here — see `Assets/Game/Scripts/Bridge/BilgisayarDNASorgulamaKoprusu.cs` (pulls DNA evidence out
of Interaction's hotbar and feeds Detective.DNAFingerPrint's minigame manager).

`Assets/Scripts/` is organized by in-game system: `Cinayet Tahtasi` (case/evidence board controller/UI
glue) and `sorun/` (despite the name — "problem" — this is the **live** in-game computer/OS window
input code, `BilgisayarModuSistemi.cs`; it is not superseded by anything). A prior modular-architecture
refactor deleted three folders that had accumulated as dead code: `Bilgisayar Sistemi` (an unused
older computer/OS-window implementation), `Envanter Sistemi` (an unused older inventory
implementation), and `Etkileşim Scriptleri` (unused older world-interaction scaffolding, including a
second, shadowing `IEtkilesebilir` interface definition that produced compiler warnings). Their live
equivalents are `Detective.Interaction`'s `EldeTutulabilirObje`/`HotbarSistemi`/`IEtkilesebilir` (in
`Interfaces/IEtkilesim.cs`) and `Detective.UI`'s `BilgisayarPenceresi`/`PencereSurukleyici`/
`PencereBoyutlayici` (`Computer/`).

Cross-package "mode" entry points (case board, computer, evidence inspection, photo mode, DNA
collection, NPC dialogue) all route through one central reference-counted lock, `OyuncuKontrolKilidi`
(`Shared-Packages/com.detective.interaction/Runtime/Modes/`), rather than each duplicating its own
player-control-disable logic.

### Data-driven content

ScriptableObject assets under `Assets/Datas/`: `ItemData/` (inventory/evidence items incl. DNA swab
variants), `NPCData/` (grouped by role: `Polis`, `Gorgu Tanigi`/witnesses), `DNAData/`. New
items/NPCs/DNA samples are added as assets here, not hardcoded.

### Scenes

- `Assets/Scenes/MainMenu.unity`
- `Assets/Scenes/Oyun.unity` — the main gameplay scene (player, NPCs, evidence, case board, computer,
  NavMesh, etc.).

### Conventions

- Identifiers, `Debug.Log` messages, and UI strings are almost entirely **Turkish** (e.g.
  `OyuncuKontrolKilidi`, `SorgulaVeKarsilastir`, `hotbarSistemi`). Preserve this in new code.
- MonoBehaviour fields are private `[SerializeField]`, defensively re-resolved in `Awake`/`OnEnable` via
  `FindFirstObjectByType`/`GetComponentInChildren` when not wired in the inspector.
- Interfaces drive cross-cutting behavior: `IEtkilesebilir` (interactable), `IHighlightable`
  (look-at highlight), `IIncelenebilir` (examinable in the inspection mode).

## Systems implemented so far (per the project's own engineering-design writeup)

Control (Starter Assets FirstPerson/URP), core interaction (`IEtkilesilebilir` +
`EtkilesilebilirObje`), player raycast interaction + highlight, item system (`ItemData`), hotbar +
inventory, hold/carry system, placement (ghost preview + collision check), throw system, examine
system, evidence manager, photo capture, full UI layer (HUD/crosshair/inventory/menus/case
board/computer windows), DNA matching minigame, and the NPC state/patrol/dialogue system with
evidence-gated dialogue branches.

Per that same document's own assessment, this is a **functional prototype**, not a finished game — see
"Not yet built / planned" below for what's explicitly called out as missing.

## Not yet built / planned (do not assume these exist without checking)

From the requirements checklist, weekly plan, and GDD notes (all aspirational at time of writing):

- **Save/Load** — JSON-based local save system (repeatedly listed as not yet done/tested).
- **Quest/mission tracking system** ("Görev Takip Sistemi" — active objectives, completed goals, new
  clues, tied to story flow).
- **Two-chapter story build** with lighting/post-processing pass per chapter.
- **Additional mini-games** beyond DNA analysis: lock picking ("kilit kırma"), hacking/password
  cracking ("hackleme" / "Bilgisayar şifresi çözme"), fingerprint analysis ("parmak izi analiz"), lie
  detector / QTE-based interrogation ("yalan dedektörü"/"yalan makinesi"), missing-persons database
  lookup (shown 4 decoys + 1 real match against a physical description).
- **Skill tree / XP system** — case-solving grants XP spent on detective skills (observation,
  analysis, persuasion); accessed through the in-game computer.
- **End-of-case scoring** — evidence count/accuracy/completion time → star rating.
- **Ambient/SFX audio and music integration.**
- **Steamworks SDK integration** (achievements) and store page — business/release track, not code, but
  referenced repeatedly in the roadmap as the finish line.
- Longer-term GDD ideas (treat as directional, not committed scope): cold-case archive ("Arşiv") as
  side content off the main story; an NPC relationship/messaging network (lab techs, on-scene units);
  a possible later multiplayer/random-map mode — explicitly described as a *later* extension, not
  part of the current single-player case structure.

## Case flow structure (narrative pacing model from design notes)

Cases are designed around a 5-phase arc — useful context if asked to build quest/story-flow tooling:

1. **Hook & crime scene** — scene opens at the crime scene; initial body/clue overview.
2. **Connecting clues & first contacts** — return to precinct, confirm findings, first interrogations,
   database/photo lookups, lab visits (DNA/blood tests as mini-games).
3. **Deep investigation & complications** — field work with squadmates, verifying earlier claims (acting
   on an unverified claim can lead to accusing the wrong person), solving locked
   evidence (safes, torn documents, passwords).
4. **Confrontation & resolution** — the one non-skippable phase; time-limited QTE choices; building the
   accusation from gathered evidence.
5. **Outcome & closure** — truth revealed, culprit caught or not, consequences shown (in main-story
   chapters, wrong calls affect later continuity rather than being immediately fatal to the case).

## Known open issues (informal bug notes from the owner)

- Sitting on a chair lets the player place objects underneath themselves, pushing the player upward.
- The UI doesn't currently tell the player they need to press Space to stand up from a seated
  interaction.

## UML diagrams (`...\HAMZA\UML\`)

A set of ~30 PNG class diagrams exists covering interaction, UI, DNA, NPC, and inventory systems. They
were largely produced *from* the current package architecture and mostly still match it (e.g.
`Envanter Veri Modeli UML Diyagramı2.png` matches `ItemData`/`InventoryItemStack` in
`Detective.Interaction` almost exactly). The `Bilgisayar Modu ve Lab Sistemi` and `Bilgisayar Pencere
Sistemi` diagrams mix classes from `Detective.UI` (`LabPenceresiAcici`, `BilgisayarPenceresi`,
`DNAMiniGameManager`) with `Assets/Scripts/sorun/` (`BilgisayarEtkilesim`, `BilgisayarModuSistemi`) —
both sides are live code, just split across the package/glue-layer boundary described above. Treat any
UML diagram as a starting hypothesis to verify against the actual `.cs` files, not as ground truth — the
owner has flagged them as not fully up to date, and a subsequent refactor changed internals (e.g.
`HotbarSistemi` now delegates to `EnvanterDeposu`/`EnvanterPanelDurumu` collaborators) that predate the
diagrams.

The `gpt/SınıfAçıklamaları.docx` file (also under the Detective design-docs folder) contains prose
walkthroughs of several of these classes/diagrams (IEtkilesilebilir, OyuncuEtkilesim, EtkilesilebilirObje,
BilgisayarEtkilesim, KoltukEtkilesim/sitting mechanic) in the owner's preferred short, bullet-point,
documentation-register style — useful as a tone/format reference if asked to document a script the same
way.
