# CHANGELOG – FO76 CAMP Planner

All notable changes to FO76 CAMP Planner are documented in this file. This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
