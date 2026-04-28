# FO76 CAMP Planner

A desktop tool to design, plan, and analyze Fallout 76 CAMP layouts. Provides placement rules, budget tracking, blueprints, zone planning and defense analysis overlays.

Key features
- Visual canvas with grid-based placement and multi-tool palette
- Blueprint import/export and library
- Autosave with crash recovery
- Budget tracking and preset playstyles
- Layer visibility & locking, trap zone planning, visitor routing

Quick start
1. Clone the repo:

```bash
git clone https://github.com/knoksen/Fallout76CAMPplanner.git
cd Fallout76CAMPplanner
```

2. Build (requires .NET 8 SDK):

```bash
dotnet build -c Release
```

3. Run the Windows Forms app (Windows):

```powershell
dotnet run --project Knoksen_FO76_CAMP_Planner_Win10/FO76CampPlanner.csproj
```

Developer notes
- Unit tests live under `tests/FO76CampPlanner.Tests` — run with `dotnet test`.
- A GitHub Actions workflow is included at `.github/workflows/ci.yml` to build and run tests on push/PR.
- Development instructions and local verification are in `README_DEV.md`.

Branching & PRs
- New features and fixes should be developed on feature branches and opened as pull requests against `main`/`master`.
- Use the provided PR template and the `PR_DESCRIPTION.md` file in the branch for an easy PR body.

Code of conduct & contributing
- See `CONTRIBUTING.md` (if present) or open an issue to discuss large changes first.

License
- This project is licensed under the MIT License — see `Knoksen_FO76_CAMP_Planner_Win10/LICENSE.md` or the `release` folder for license text.

Contact
- Repo: https://github.com/knoksen/Fallout76CAMPplanner
