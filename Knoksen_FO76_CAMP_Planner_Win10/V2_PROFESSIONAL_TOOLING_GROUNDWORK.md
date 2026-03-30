# v2.0 Professional Tooling - Safe Groundwork Decisions

Date: 2026-03-30
Scope: groundwork only, no high-risk runtime integration

## Goals

- Prepare an architecture path for 3D preview.
- Prepare an architecture path for print-ready PDF export.
- Prepare a path for a darker Fallout-inspired theme system.
- Keep v1.x stable and buildable.

## Constraints

- Do not change framework (stay on WinForms/.NET 8).
- Do not destabilize planner canvas and edit workflow.
- Do not add external rendering/runtime dependencies yet.

## Groundwork Added

### 1) 3D preview adapter boundary

Added in code:
- `IThreeDPreviewAdapter`
- `ThreeDPreviewPayload`
- `ThreeDPreviewItem`

Why:
- Keeps 3D concerns outside `MainForm` and `PlannerCanvas`.
- Allows trying multiple rendering backends later without rewiring core planner logic.

Recommended path:
1. Keep planner as source-of-truth in 2D.
2. Build one-way export from `PlannerProject` to `ThreeDPreviewPayload`.
3. Implement preview in isolated adapter (sidecar/webview/other process).

Not done yet:
- No real 3D renderer.
- No event-stream sync to 3D runtime.

### 2) Print/PDF export adapter boundary

Added in code:
- `IPrintExportAdapter`
- `PrintExportDocument`

Why:
- Print/PDF generation should be independent from UI interactions.
- Existing PNG snapshot can be reused as a stable base for print templates.

Recommended path:
1. Generate `CanvasSnapshot` from current render.
2. Add legend + metadata from project in `SummaryLines`.
3. Adapter writes PDF once library choice is finalized.

Not done yet:
- No PDF library integrated.
- No print layouts/templates.

### 3) Theme groundwork in project model

Added in code:
- `ThemeProfile` enum (`Classic`, `WastelandDark`, `PipBoyContrast`)
- `PlannerProject.ThemeProfile`

Why:
- Makes theme preference persistable now with low risk.
- Enables later UI palette migration without breaking project compatibility.

Recommended path:
1. Introduce palette object in UI layer (colors/fonts/spacing).
2. Replace direct color constants gradually.
3. Add user toggle only after palette migration reaches parity.

Not done yet:
- No runtime theme switch.
- No complete palette abstraction in `MainForm` yet.

### 4) Safety gating

Added in code:
- `FutureFeatureFlags` (all disabled)

Why:
- Prevent accidental partial activation during v1.x stabilization.

## What Should Wait Until After v1.1/v1.2

- Any embedded 3D runtime in-process.
- New dependency-heavy PDF stacks.
- Full UI-wide theming refactor touching all controls.
- Any changes that alter placement rules, undo/redo semantics, or canvas interaction loops.

## v2.0 Readiness Checklist (next)

1. Freeze v1.x feature behavior and add regression checks for save/load + placement + overlays.
2. Select one PDF adapter candidate and benchmark output quality + package footprint.
3. Implement a thin `PlannerProject -> ThreeDPreviewPayload` mapper with no UI coupling.
4. Start palette extraction in small batches (header/cards/toolstrip first).
