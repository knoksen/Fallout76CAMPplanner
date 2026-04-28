# Changes (Unreleased)

Most notable changes in this working branch:

- Add `IProjectService` and `ProjectService` for async JSON load/save and atomic autosave.
- Wire `ProjectService` into application entry in `Program.cs` and accept `IProjectService` in `MainForm`.
- Refactor `OpenProject` and `SaveProject` to use async service methods and avoid blocking the UI thread.
- Add an autosave timer with safe background writes and a startup recovery prompt in `MainForm.cs`.
- Introduce a small unit test project (`tests/FO76CampPlanner.Tests`) with tests for serialization and autosave behavior.
- Add GitHub Actions workflow at `.github/workflows/ci.yml` to build and run tests on push/PR.

Files added/modified (high level):

- `Knoksen_FO76_CAMP_Planner_Win10/Services/IProjectService.cs` (new)
- `Knoksen_FO76_CAMP_Planner_Win10/Services/ProjectService.cs` (new)
- `Knoksen_FO76_CAMP_Planner_Win10/MainForm.cs` (modified)
- `Knoksen_FO76_CAMP_Planner_Win10/Program.cs` (modified)
- `tests/FO76CampPlanner.Tests/*` (new tests)
- `.github/workflows/ci.yml` (new CI workflow)

Notes:

- The workspace does not expose the Codacy MCP tool here. Per repository instructions, after edits a `codacy_cli_analyze` run should be performed; I could not run it from this environment. Please run Codacy analysis locally or in CI (see `README_DEV.md` for suggested commands).
