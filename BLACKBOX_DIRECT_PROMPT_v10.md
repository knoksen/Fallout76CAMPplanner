# Blackbox AI – direkte one-shot prompt for VS Code

Kopier alt under og lim det inn i Blackbox AI i VS Code:

```text
You are helping me finish and publish a Windows 10 desktop app written in C# WinForms (.NET 8) located in the currently opened workspace folder.

Project name: FO76CampPlanner
Target: net8.0-windows, WinForms, single-file self-contained EXE for win-x64.

Your job:
1. Audit the whole project for compile blockers, duplicate types, missing members, event handler mismatches, namespace issues, nullability issues, and broken references.
2. Fix all build errors directly in the workspace.
3. Preserve existing functionality and UI/UX intent.
4. Use the included build scripts and project structure already present.
5. Build and publish the app for Windows 10 x64.
6. If publish succeeds, tell me the exact path to FO76CampPlanner.exe.
7. If publish fails, keep fixing until the project builds cleanly.
8. After success, provide a short summary with:
   - files changed
   - major fixes made
   - remaining warnings (if any)
   - exact command used

Important context already known in this project:
- MainForm2.cs.disabled is intentionally disabled because it conflicted with MainForm.cs.
- PlannerCanvas.cs previously had a duplicate DrawPlacementPreview issue that should stay resolved.
- The expected publish path is:
  bin\Release\net8.0-windows\win-x64\publish\FO76CampPlanner.exe

Required workflow:
- First inspect FO76CampPlanner.csproj, MainForm.cs, PlannerCanvas.cs, Models.cs, Program.cs, MinimapPanel.cs.
- Then run:
  dotnet clean .\FO76CampPlanner.csproj
  dotnet restore .\FO76CampPlanner.csproj
  dotnet build .\FO76CampPlanner.csproj -c Release
- Fix any errors.
- Then run:
  powershell -ExecutionPolicy Bypass -File .\BUILD_AND_PUBLISH_v10.ps1
- If anything fails, fix and rerun until green.

Do not stop at analysis only. Make the code changes and complete the build.
```
