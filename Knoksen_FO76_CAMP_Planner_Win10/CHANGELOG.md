# CHANGELOG – FO76 CAMP Planner

All notable changes to FO76 CAMP Planner are documented in this file. This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Foundation Release (v1.0) – release preparation pass

#### Implemented
- Added low-risk Scenario Compare controls in Analysis:
  - capture current plan as baseline
  - compare current state vs baseline for defense score, placed budget, and covered ingress count
  - clear baseline to restart A/B planning passes
- Verified and retained v1.0 core feature set for first Windows testing:
  - minimap
  - inspector editing
  - blueprint export/load and slot-library operations
  - visitor-flow overlay and route markers
  - trap-zone overlay and structured trap plans

### Planning Engine (v1.1) – stability and workflow polish

#### Implemented
- Replaced binary smart-snap toggle with explicit tri-state snap mode in Rules:
  - `Strict` = always snap to nearest valid anchors
  - `Relaxed` = snap only when close to valid anchors
  - `Off` = no snapping adjustments
- Updated the toolbar `Snap` action to cycle through `Strict → Relaxed → Off`.
- Added legacy project compatibility for older JSON files that still store `SnapEnabled`.
- Added directional quick-duplicate actions for selected trap zones (`← → ↑ ↓`) in Route/Trap planning controls.
- Added inspector nudge workflow for precise one-cell movement using directional controls.
- Improved placement feedback by surfacing invalid-placement reason text in footer hints during placement preview.
- Improved grouped editing by rotating multi-selection around a true group pivot with placement validation and rollback safety.

### Device Hub / Quick Launch Center – user-facing launch workflows

#### Implemented
- Added a new `Device Hub` tab with large, touch-friendly button groups for `PC`, `Mobile Companion`, and `Console`.
- Added JSON-backed launch profile model in project data (`deviceHub`) for configurable platform targets and notes.
- Added PC quick actions:
  - open project folder
  - open release folder
  - launch published `FO76CampPlanner.exe` when present
  - open GitHub, SourceForge, and configured docs/resources targets
- Added Mobile companion actions:
  - open mobile export folder
  - export compact project summary
  - generate snapshot pack (PNG + summary)
  - generate QR-ready link list file
  - quick switch to compact presentation review mode
- Added Console quick actions for Xbox, PlayStation, and Generic Console using configurable targets.
- Added safety UX:
  - disabled state for unavailable targets
  - friendly tooltip messaging explaining missing configuration
  - status-strip feedback for success/failure of launch actions

### Fallout 76 Logic (v1.2) – defense analysis strengthening

#### Implemented
- Expanded the Analysis panel with approximate defense scoring (`0-100`) based on turret density, defense density, ingress coverage, and trap-zone severity.
- Added ingress coverage summary (`covered/total`) using approximate turret arc/range matching.
- Added actionable risk hints (missing turret coverage, uncovered ingress routes, critical-zone review reminders, and overlay visibility hints).
- Strengthened defense scoring with playstyle-profile targets (turrets/defense items/ingress/trap-zones) and route completeness factors (ingress, checkpoint, egress).
- Expanded route planning summary with explicit ingress/checkpoint/egress composition and count of flow markers inside high/critical trap zones.
- Strengthened mode logic by enforcing Shelter-specific rules on mode switch (auto-shelter rule profile, shelter-safe overlays, incompatible item pruning).
- Upgraded visitor-flow visualization with coverage-aware ingress markers and segment coloring based on trap-zone severity.
- Added concrete shelter-type rule enforcement for shelter presets:
  - per-preset cap for turret count
  - per-preset cap for route markers and trap zones
  - per-preset maximum trap severity
  - analysis/risk hints now surface cap violations for existing projects
- Added shelter advisory text in Route/Trap planning that shows cap headroom and fallback guidance before caps are reached.
- Added soft-disable behavior for shelter-constrained Route/Trap actions so marker/zone add actions are disabled when caps are reached, and high-severity presets are disabled when above the shelter severity cap.
- Added reason-aware tooltips on disabled Route/Trap actions to explain exact shelter constraints (marker cap, zone cap, or severity ceiling).
- Added inline Route/Trap cap meter with live marker/zone usage bars and warning/error coloring for near-cap and cap-reached states.

#### Release workflow
- Added `BUILD_AND_PUBLISH_v10.ps1` wrapper script to keep release automation naming aligned with project workflow instructions.
- Added `GENERATE_RELEASE_MANIFEST.ps1` to produce publish-folder release manifests with file sizes and SHA256 hashes.

---

## [1.1.0-preview] – 2026-03-30

### Planning Engine (v1.1) – roadmap execution pass

#### Implemented
- Quick-duplicate zone actions are now fully integrated in toolbar and quick-start controls (`Dup ←`, `Dup →`, `Dup ↑`, `Dup ↓`) with validation-safe placement.
- Snap toggle is now available as both:
  - persistent rules checkbox in Mode & Rules
  - one-click toolbar action (`Snap`) for fast iteration
- Blueprint library per CAMP slot is now functional for in-memory modules:
  - save loaded blueprint directly into active slot library
  - reload blueprint from slot library without requiring intermediate files
  - slot entries now persist blueprint module payload in project data

#### Stability and architecture
- Fixed nullable unboxing warning in CAMP-slot selection handling.
- Added window-title dirty state indicator groundwork (`*` prefix when project has unsaved changes).

### Fallout 76 Logic (v1.2) – advanced groundwork and partial feature delivery

#### Implemented
- Added visitor-flow overlay rendering toggle and visualization layer.
  - flow lines now render from CAMP center to key functional targets (door/vendor/workbench/ally)
- Added persisted visitor marker system.
  - add ingress, checkpoint and egress markers directly from current hover/selection context
  - markers are saved in project JSON for repeatable route reviews
- Added trap-zone overlay rendering toggle and visualization layer.
  - trap zones now render around defense items and explicitly tagged trap entries
- Added trap-zone tagging actions in Inspector quick actions:
  - `Tag trap zone`
  - `Clear trap tag`
- Added trap-planning label variants for selected zones:
  - Funnel
  - KillBox
  - Delay
- Budget profiles are now fully wired in UI and behavior:
  - Builder
  - Trap CAMP
  - Vendor CAMP
  - Utility CAMP
  - Nuke CAMP
  - Showcase CAMP
  - selecting a profile applies tuned budget/stored values and descriptive guidance
- Added overlay review presets for fast planning passes:
  - Balanced
  - VisitorFlow
  - TrapReview
  - DefenseReview
  - Presentation
- Expanded the route-planning panel into an editor workflow:
  - route steps are now ordered, renamable and type-editable
  - route order is saved in project JSON and rendered on-canvas as a sequential path
- Added structured trap-zone planning data:
  - trap zones now persist label, severity, bounds and review notes
  - quick zone presets create editable Funnel, Kill Box and Delay plans from the current selection
- Added saved defense-review notes per project for documenting weak angles, reset points and visitor-control intent.
- Added direct canvas editing for route and trap planning:
  - drag visitor markers directly on the canvas to reposition route steps
  - drag trap-zone body to move the zone footprint
  - drag trap-zone corner handle to resize the zone footprint

#### Notes
- This pass keeps WinForms/.NET 8 architecture intact.
- `MainForm2.cs.disabled` remains untouched.
- Existing placement-preview and snapping behavior are preserved.

---

## [1.0.1] – 2026-03-29

### UI/UX Polish Sprint (WinForms-focused)

#### Improved visual hierarchy & readability
- Tightened section/card spacing and header rhythm for faster panel scanning.
- Increased consistency in control sizing for action buttons and inspector workflows.
- Improved section header contrast for clearer inspector/library/build boundaries.

#### Better consistency & feedback
- Unified hover/pressed behavior for workflow and action buttons.
- Improved active-state readability in mode/guide messaging and inspector hint text.
- Refined wording in inspector guidance for single vs. multi-select editing intent.

#### Canvas feedback polish
- Increased visibility for marquee selection and placement preview outlines/fills.
- Clarified footer wording around active tool and first-run workflow path.
- Improved shortcut hint readability and reduced ambiguity in action labels.

#### Notes
- Functionality and app direction are unchanged.
- Fallout 76 CAMP planner identity and workflow framing are preserved.

---

## [1.0.0] – 2026-03-29

### Release Summary

FO76 CAMP Planner v1.0.0 is the first official public release, featuring a complete, production-ready CAMP and Shelter planning tool. This release consolidates over 6 iterations of development focusing on UI/UX excellence, intelligent rule-based placement, and professional workflow support.

### ✨ Major Features (MVP+ Complete)

#### Grid-Based Design
- 2D precision grid for CAMP and Shelter layouts
- Foundation, Wall, Door, Stairs, Roof, Workbench, Turret, Power, Light, Decor, Vendor, Resource, Display, and Ally placement tools
- Dual mode support: Surface CAMP and Interior Shelter

#### Intelligent Placement & Validation
- **Foundation-aware snapping**:
  - Walls and doors snap to foundation edges
  - Roofs snap to foundation tiles
  - Stairs snap to open foundation edges
- **Rule profiles**:
  - **Strict**: No overlap except foundation
  - **Relaxed**: Allows soft overlaps (Power, Decor)
  - **Shelter**: Generous stacking with category restrictions
- **Layer lock** for fine-grained control

#### Multi-Select & Advanced Editing
- Multi-select via `Ctrl+Click` or drag-box (marquee)
- Group move and group delete
- Contextual Inspector with real-time note, X/Y, and rotation editing
- Quick actions: duplicate, delete, set CAMP center

#### Blueprint System
- Save any selection as `.blueprint.json`
- Load and paste blueprints onto the canvas
- Build reusable module libraries

#### Analysis & Visualization
- Approximate surface CAMP radius overlay
- Turret coverage arc visualization
- Minimap panel with selected-item highlighting and CAMP center marker
- Layer visibility toggles (Structure, Utility, Defense, Power, Aesthetic, Commerce)
- Focus presets (All, Structure, Systems, Defense, Presentation)

#### Project Management
- Save/load projects as JSON
- Export to PNG
- 9 built-in surface CAMP and shelter presets
- Sample projects and blueprints included

#### Workflow Support
- Build stage guide: Layout → Envelope → Systems → Defense → Polish
- Dynamic quick-start recommendations
- Workflow status dashboard showing progress

#### Quality-of-Life
- Undo/Redo (up to ~50 actions)
- Keyboard shortcuts (R=Rotate, Delete, Ctrl+Z/Y, +/-, arrow keys)
- Live placement preview on canvas
- Hover coordinates and shortcut hints
- Dark, high-contrast UI

### 🛠️ Technical Details

- **Platform**: Windows 10 / Windows 11 (64-bit)
- **Framework**: .NET 8 with Windows Forms
- **Deployment**: Single-file self-contained EXE (no installation or runtimes required)
- **Size**: ~70 MB (includes .NET runtime)

### 📦 Included Assets

- Single executable: `FO76CampPlanner.exe`
- 3 sample project files (JSON)
- 2 sample blueprint files
- 9 built-in presets (Vault Lobby, Utility Room, Missile Silo, Nuclear Test Bunker, Flatlands, Triumph Terrace, Wrangler Casino, Nuke Surface CAMP, Custom Surface CAMP)

### 🎯 Known Limitations

- No 3D visualization (planned for 2.x)
- Blueprints are session-local (library sync planned)
- Large projects (2000+ items) may slow down UI
- PNG export may require scaling for dense layouts

### 📋 Breaking Changes

None – this is the initial release.

---

## Pre-Release Development Milestones

### v6 – Visual Structure & Workflow (2026-02-15)

**Focus**: Structured workflow and visual hierarchy.

#### New
- 5-stage workflow header (Layout, Envelope, Systems, Defense, Polish)
- Dedicated Workflow sidebar section
- Focus presets (All, Structure, Systems, Defense, Presentation)
- Workflow-aware tool descriptions
- Focus summary overlay

#### UX Intent
Reduce cognitive load by guiding players through purposeful build passes instead of treating all tools as equally important.

---

### v5 – UI/UX Redesign (2026-01-20)

**Focus**: Professional interface and responsive design.

#### New
- Tab-based right sidebar (Overview, Build, Library, Inspect)
- Clearer header bar with project status
- Improved dark UI with higher contrast
- Search and layer filters in item lists
- Colour-coded item cards with lock badges
- Live placement preview
- Hover coordinates and shortcut hints

---

### v4 – Rule Profiles & Shelter Mode (2025-12-10)

**Focus**: Shelter-specific rules and flexible placement.

#### New
- Rule Profile system (Strict, Relaxed, Shelter)
- Automatic Shelter restrictions (Vendor, Resource, Display, Ally locked)
- Shelter presets (Vault, Bunker, Casino, etc.)
- CAMP vs. Shelter mode toggle

---

### v3 – Foundation & Snapping (2025-10-05)

**Focus**: Smart placement and grid alignment.

#### New
- Foundation-aware snapping
- Foundation-edge wall alignment
- Foundation-tile roof snapping
- Layer toggles (Structure, Utility, Defense, Power, Aesthetic, Commerce)
- Multi-select (Ctrl+Click and drag-box)
- Group operations (move, rotate, delete)

---

## Versioning

- **Versions 3–6** were internal development releases
- **Version 1.0.0** is the first public release
- Future: v1.1, v1.2, v2.0 (see ROADMAP_NEXT.md)

---

[1.0.0]: https://github.com/knoksen/Fallout76CAMPplanner/releases/tag/v1.0.0
