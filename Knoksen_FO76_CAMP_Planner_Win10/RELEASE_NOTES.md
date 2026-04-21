# FO76 CAMP Planner - Release Notes

## Version
v1.1.0-preview+rc1

## Date
2026-03-30

## Release Focus
This milestone finalizes the current WinForms/.NET 8 feature set as a stable release candidate for Windows 10/11 x64 single-file publishing.

## Highlights
- Stabilized route/trap planning with shelter-aware constraints:
  - shelter advisory guidance
  - soft-disabled actions at marker/zone/severity caps
  - reason-aware tooltips on disabled actions
  - inline cap meter with live usage bars and warning/error color states
- Added Device Hub / Quick Launch Center:
  - PC, Mobile Companion, and Console quick actions
  - project-backed JSON launch profiles (`deviceHub`)
  - safer action states with disabled/tooltip feedback for missing targets
- Strengthened planning analysis:
  - defense score and risk hints
  - scenario compare baseline capture/clear workflow
  - expanded route/trap summary signals
- Improved workflow controls:
  - snap mode tri-state (`Strict`, `Relaxed`, `Off`)
  - inspector nudge controls
  - trap-zone directional quick-duplicate actions

## Build and Release Tooling
- `BUILD_AND_PUBLISH_v10.ps1` now runs clean/restore/build/publish with log output.
- Optional `-StopRunningApp` guard prevents EXE lock failures during publish.
- `GENERATE_RELEASE_MANIFEST.ps1` generates SHA256 and file-size manifest in publish output.

## Included in Release Package
- `FO76CampPlanner.exe`
- `README.md`
- `CHANGELOG.md`
- `RELEASE_NOTES.md`
- `INSTALL_WINDOWS.md`

## Known Constraints
- Windows desktop app; no native Linux/macOS GUI runtime target.
- Device Hub console launch actions depend on user-configured external targets.
- QR flow currently exports QR-ready link text (no built-in QR image renderer yet).
