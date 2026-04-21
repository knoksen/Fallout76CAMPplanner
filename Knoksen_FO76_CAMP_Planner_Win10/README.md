# Knoksen FO76 CAMP Planner - Windows 10 Planner MVP++

Dette er et viderebygget fundament for et Fallout 76 CAMP-planleggingsverktøy for Windows 10.

## Hva appen gjør nå

- 2D grid-basert CAMP-planlegging
- Verktøy for foundation, wall, door, stairs, roof, workbench, turret, power, light, decor, vendor, resource, display og ally
- CAMP- og Shelter-modus
- Regelprofiler: Strict, Relaxed, Shelter
- Budsjettmåler med placed/stored budget
- Lagre og åpne prosjekter som JSON
- Eksport til PNG
- **Undo / Redo**
- **Layer toggles** for Structure, Utility, Defense, Power, Aesthetic og Commerce
- **Shelter- og surface-presets**
- **Smartere snapping**
  - wall og door snapper mot **foundation-kanter**
  - roof snapper mot **foundation-felt**
  - stairs snapper mot **åpne foundation-kanter**
- **Multi-select**
  - Ctrl-klikk for å bygge utvalg
  - drag-boks for marquee selection
  - gruppedrag og gruppesletting
# FO76 CAMP Planner

> **A Windows desktop tool for planning Fallout 76 C.A.M.P. and Shelter layouts**

[![Release](https://img.shields.io/github/v/release/knoksen/Fallout76CAMPplanner?label=latest%20release&color=brightgreen)](https://github.com/knoksen/Fallout76CAMPplanner/releases/latest)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-blue?logo=windows)](https://github.com/knoksen/Fallout76CAMPplanner/releases/latest)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple?logo=dotnet)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![License: MIT](https://img.shields.io/badge/license-MIT-yellow)](LICENSE)

Plan your Fallout 76 base before you place a single brick. FO76 CAMP Planner gives you a 2D canvas editor with placement rules, layered overlays, visitor flow analysis, trap zone planning, blueprint modules, and a minimap — all in a single self-contained Windows `.exe` with no install required.

---

## Screenshots

| Overview | Analysis & Blueprints |
|---|---|
| ![Planner overview](https://github.com/user-attachments/assets/1975c46b-8103-4659-8106-ab01b5a2f7b2) | ![Analysis and stored blueprints](https://github.com/user-attachments/assets/3939d7e0-28fa-48a2-b49f-15f8dfbf24a5) |

---

## Download

Grab the latest release — a single self-contained `.exe`, no .NET install required:

**[⬇ Download FO76 CAMP Planner (Windows x64)](https://github.com/knoksen/Fallout76CAMPplanner/releases/latest)**

---

## Features

### Canvas & Placement
- 2D grid canvas with live placement preview and zoom
- **Snap modes**: Strict, Relaxed, Off — cycle from the toolbar or Rules panel
- **Multi-select**: Ctrl+click or drag-box selection, group move, rotate, delete
- **Smart snapping**: walls/doors snap to foundation edges, roofs to foundation fields, stairs to open edges
- **Layer toggles + layer lock**: Structure, Utility, Defense, Power, Aesthetic, Commerce
- Full **Undo / Redo** (Ctrl+Z / Ctrl+Y)
- **Live placement preview** with footer feedback when placement is blocked

### Planning Modes
- **Surface CAMP** and **Shelter** modes with separate rule enforcement
- **Rule profiles**: Strict (no overlap), Relaxed (soft layers allowed), Shelter (stacking with guardrails)
- Shelter mode automatically blocks Vendor, Resource, Display, and Ally categories to reflect actual game constraints
- Shelter presets enforce caps on turrets, route markers, trap zones, and max trap severity

### Analysis Overlays
- **CAMP radius overlay** — approximate build/control zone for surface planning
- **Turret coverage arcs** — approximate defense cone per turret rotation
- **Visitor flow overlay** — main lines from CAMP center to key targets (doors, workbench, vendor)
  - Ingress markers colored by coverage status
  - Route segments tinted by trap severity
- **Trap zone overlay** — risk/defense zones around trap/defense items
- **Defense analysis summary** — approximate score (0–100), ingress coverage, and weak-angle hints
- **Scenario compare (A/B)** — save a baseline and diff score, budget, and ingress coverage against it

### Route & Trap Planning
- Build ordered **ingress → checkpoint → egress** sequences with editable labels
- Drag markers directly on canvas; reorder in the panel
- Create and resize **trap zones** on canvas with corner handles
- Set severity per zone: Low / Medium / High / Critical
- Add private review notes per zone
- **Inline cap meter** in the Route/Trap panel shows live bars for Markers, Zones, and Max severity with warn/error coloring when approaching shelter caps
- Soft-disabled actions with explanatory tooltips when a shelter cap is reached

### Blueprints & Budget
- Save any multi-select as a reusable `.blueprint.json` module
- Load and paste blueprint modules onto the canvas
- Blueprint library per CAMP slot (Slot 1–4)
- **Budget profiles**: Builder, Trap CAMP, Vendor CAMP, Utility CAMP, Nuke CAMP, Showcase CAMP
- Budget meter shows placed vs. stored budget with profile-aware targets

### Minimap & Inspector
- **Minimap** with selected-item highlight, CAMP center crosshair, and hover cell feedback
- **Contextual inspector** for single-item note, X/Y, and rotation edits
- Nudge controls for precise single-cell movement
- Quick-duplicate in toolbar and via directional quick actions (`Dup ←/→/↑/↓`)

### Device Hub
Quick-launch panel for PC tools, mobile companion workflow, and console shortcuts:
- Open project/release folders, launch the published EXE, open GitHub/SourceForge/docs links
- Export a mobile-friendly project summary or snapshot pack (PNG + summary)
- Generate a QR-ready links file
- Configurable Xbox / PlayStation / Generic console action buttons

---

## Built-in Presets

| Preset | Type |
|---|---|
| Custom Surface CAMP | Surface |
| Vault Lobby Shelter | Shelter |
| Vault Utility Room | Shelter |
| Missile Silo Shelter | Shelter |
| Nuclear Test Bunker | Shelter |
| The Flatlands Shelter | Shelter |
| Triumph Terrace | Shelter |
| Wrangler Casino | Shelter |
| Nuke Surface CAMP | Surface |

---

## Quick Start

1. Download the latest release EXE from the [Releases page](https://github.com/knoksen/Fallout76CAMPplanner/releases/latest)
2. Run `FO76CampPlanner.exe` — no installation needed
3. Open a sample project: `sample-foundation-layout.json` or `sample-missile-silo-layout.json`
4. Try Ctrl+click and drag-box selection, then **Save selection as blueprint**
5. Toggle overlays (visitor flow, trap zones, defense arcs) from the Analysis panel

---

## Keyboard Shortcuts

| Key | Action |
|---|---|
| `R` | Rotate selected item / group |
| `Delete` | Delete selected item / group |
| `Ctrl+Z` | Undo |
| `Ctrl+Y` | Redo |
| `+ / -` | Zoom in / out |
| Arrow keys | Move selected item / group one cell |
| `Ctrl+click` | Add / remove from selection |

---

## Building from Source

**Requirements:** .NET 8 SDK on Windows 10/11

```powershell
# Full release build + publish (single-file EXE)
.\Knoksen_FO76_CAMP_Planner_Win10\BUILD_AND_PUBLISH_v10.ps1 -Runtime win-x64 -Configuration Release
```

Or use the build scripts directly:

```powershell
cd Knoksen_FO76_CAMP_Planner_Win10
.\build-win10-singlefile.ps1
```

Published EXE lands at:
```
Knoksen_FO76_CAMP_Planner_Win10\bin\Release\net8.0-windows\win-x64\publish\FO76CampPlanner.exe
```

---

## Sample Files

**Projects:**
- `sample-foundation-layout.json`
- `sample-missile-silo-layout.json`
- `sample-defense-layout.json`

**Blueprints:**
- `sample-blueprints/foundation-ring.blueprint.json`
- `sample-blueprints/nuke-lane.blueprint.json`

---

## Disclaimer

This is a planning and visualization tool, not a 1:1 simulation of Fallout 76's internal placement engine. Placement rules approximate real game behavior; always verify complex builds in-game.

---

*Not affiliated with Bethesda Softworks or ZeniMax Media.*

