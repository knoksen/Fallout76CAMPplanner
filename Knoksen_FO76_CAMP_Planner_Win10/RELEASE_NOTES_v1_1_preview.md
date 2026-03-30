# FO76 CAMP Planner v1.1.0-preview

Date: 2026-03-30

## Highlights

- Route planning is now an editor workflow, not just an overlay.
- Trap planning now supports structured zones with severity and review notes.
- Direct canvas manipulation is now available for both route markers and trap zones.

## New in this preview

- Ordered route markers with editable label and type.
- Marker reorder controls in the Route & Trap Planning section.
- Structured trap zone model with:
  - label
  - severity
  - notes
  - persistent bounds
- Defense review notes are saved per project.
- Canvas interactions:
  - drag visitor markers to reposition route steps
  - drag trap zone body to move the zone
  - drag trap zone corner handle to resize

## Stability

- Release build succeeds with 0 warnings and 0 errors.
- Publish pipeline succeeds and produces a single-file Win64 executable.

## Notes

- Architecture remains WinForms on .NET 8.
- MainForm2.cs.disabled remains untouched.
