param(
    [string]$RootPath = (Get-Location).Path
)

Write-Host "Codacy analysis helper"

if (-not (Get-Command codacy_cli_analyze -ErrorAction SilentlyContinue)) {
    Write-Host "codacy_cli_analyze not found on PATH. Please install the Codacy CLI / MCP tools and re-run this script." -ForegroundColor Yellow
    Write-Host "See: https://docs.codacy.com/ for installation instructions."
    exit 2
}

$files = @(
    'Knoksen_FO76_CAMP_Planner_Win10/Services/IProjectService.cs',
    'Knoksen_FO76_CAMP_Planner_Win10/Services/ProjectService.cs',
    'Knoksen_FO76_CAMP_Planner_Win10/Services/IBlueprintService.cs',
    'Knoksen_FO76_CAMP_Planner_Win10/Services/BlueprintService.cs',
    'Knoksen_FO76_CAMP_Planner_Win10/MainForm.cs',
    'Knoksen_FO76_CAMP_Planner_Win10/Program.cs',
    'Knoksen_FO76_CAMP_Planner_Win10/Models.cs',
    'Knoksen_FO76_CAMP_Planner_Win10/AppServices.cs',
    '.github/workflows/ci.yml',
    'CHANGES.md',
    'README_DEV.md',
    'PR_DESCRIPTION.md',
    '.github/PULL_REQUEST_TEMPLATE.md',
    'README.md',
    'tests/FO76CampPlanner.Tests/ProjectServiceTests.cs',
    'tests/FO76CampPlanner.Tests/BlueprintServiceTests.cs',
    'tests/FO76CampPlanner.Tests/FO76CampPlanner.Tests.csproj'
)

foreach ($file in $files) {
    $full = Join-Path -Path $RootPath -ChildPath $file
    if (-Not (Test-Path $full)) {
        Write-Host "Skipping missing file: $file" -ForegroundColor DarkGray
        continue
    }

    Write-Host "Analyzing: $file"
    # Run Codacy analyzer for this file. Adjust params as required by your Codacy setup.
    codacy_cli_analyze --rootPath "$RootPath" --file "$file"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "codacy analysis returned non-zero exit code for $file" -ForegroundColor Red
    }
}

Write-Host "Codacy analysis helper complete."
