## Summary

Brief description of changes and rationale.

## Changes
- Added `IProjectService` and `ProjectService` for async project IO and autosave.
- Refactored `MainForm` to use `ProjectService` for load/save and added autosave/recovery.
- Added unit tests and CI workflow.

## Verification
- Build succeeds: `dotnet build -c Release`
- Tests pass: `dotnet test tests/FO76CampPlanner.Tests/FO76CampPlanner.Tests.csproj`

## Notes
- Codacy analysis should be run after merging; if not available in CI please run `codacy_cli_analyze` locally per repository instructions.

## Checklist
- [ ] Code compiles
- [ ] Tests added/updated
- [ ] Documentation updated (`CHANGES.md`, `README_DEV.md`)
