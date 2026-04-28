# Development notes

Quick instructions to build, test, and validate the changes made in this branch.

Prerequisites
- .NET 8 SDK
- Windows / macOS / Linux for local builds

Build
```bash
dotnet build -c Release
```

Run app (Windows Forms)
```powershell
dotnet run --project Knoksen_FO76_CAMP_Planner_Win10/FO76CampPlanner.csproj
```

Run tests
```bash
dotnet test tests/FO76CampPlanner.Tests/FO76CampPlanner.Tests.csproj -c Release
```

Run Codacy analysis (if you have Codacy CLI / MCP tools)
1. Ensure Codacy CLI is installed and configured for this repository.
2. Run the helper script which invokes `codacy_cli_analyze` for the common edited files:

```powershell
.\tools\run_codacy_analysis.ps1 -RootPath "$(pwd)"
```

If `codacy_cli_analyze` is not available in your environment, the helper will prompt you. You can either install Codacy locally or run equivalent static checks and let CI run the full checks.

Preparing a PR (recommended)
```bash
git checkout -b feat/async-project-service
git add .
git commit -m "Add ProjectService, autosave, tests, and CI"
git push origin feat/async-project-service
# Open a PR in GitHub and include a short summary and the changelog entry from CHANGES.md
```
