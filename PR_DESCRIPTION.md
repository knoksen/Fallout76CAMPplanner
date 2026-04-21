PR Title: feat: async project service, autosave, DI, blueprint service, tests, CI

PR Body:

Summary
- Introduces async project I/O and autosave, a blueprint loading service, and simple DI wiring.
- Moves blocking file ops off the UI thread (project load/save, blueprint load, PNG export).
- Adds unit tests and a basic CI workflow, plus developer docs and a PR template.

Changes
- Added: `Knoksen_FO76_CAMP_Planner_Win10/Services/IProjectService.cs`, `ProjectService.cs`
- Added: `Knoksen_FO76_CAMP_Planner_Win10/Services/IBlueprintService.cs`, `BlueprintService.cs`
- Modified: `Knoksen_FO76_CAMP_Planner_Win10/MainForm.cs` (async load/save, autosave timer & recovery, background export, DI usage)
- Modified: `Knoksen_FO76_CAMP_Planner_Win10/Program.cs` (ServiceCollection DI)
- Tests: `tests/FO76CampPlanner.Tests/*` (serialization, autosave, blueprint load)
- CI: `.github/workflows/ci.yml` (build + test)
- Docs: `CHANGES.md`, `README_DEV.md`, `.github/PULL_REQUEST_TEMPLATE.md`

Verification
- Build: `dotnet build -c Release`
- Tests: `dotnet test tests/FO76CampPlanner.Tests/FO76CampPlanner.Tests.csproj -c Release`

Notes & Follow-ups
- Codacy: repository requires `codacy_cli_analyze` after edits; it wasn't run here. Please run Codacy analysis locally or in CI as needed.
- Follow-ups recommended: expose autosave settings in UI, expand unit coverage, and extract more IO to services (PNG export already moved to background save).
- CI: workflow runs on GitHub after opening PR — workflow added at `.github/workflows/ci.yml`.

Checklist
- [ ] Code compiles
- [ ] Tests pass
- [ ] Codacy analysis completed (if applicable)
- [ ] Documentation updated

Suggested reviewers: maintainers familiar with UI and saving code.
