param(
    [string]$Runtime = "win-x64",
    [string]$Configuration = "Release",
    [switch]$StopRunningApp
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$projectPath = Join-Path $PSScriptRoot "FO76CampPlanner.csproj"
$logDir = Join-Path $PSScriptRoot "build_logs"
New-Item -ItemType Directory -Path $logDir -Force | Out-Null
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$logPath = Join-Path $logDir "build-$timestamp.log"

if ($StopRunningApp) {
    Get-Process FO76CampPlanner -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
}

Write-Host "Running v1.0+ build/publish wrapper for FO76CampPlanner ($Runtime)..." -ForegroundColor Cyan

try {
    "[$(Get-Date -Format o)] Starting build/publish" | Tee-Object -FilePath $logPath -Append | Out-Null

    & dotnet clean $projectPath 2>&1 | Tee-Object -FilePath $logPath -Append | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "dotnet clean failed" }

    & dotnet restore $projectPath 2>&1 | Tee-Object -FilePath $logPath -Append | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed" }

    & dotnet build $projectPath -c $Configuration 2>&1 | Tee-Object -FilePath $logPath -Append | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }

    & dotnet publish $projectPath -c $Configuration -r $Runtime --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true 2>&1 | Tee-Object -FilePath $logPath -Append | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }

    & "$PSScriptRoot\GENERATE_RELEASE_MANIFEST.ps1" 2>&1 | Tee-Object -FilePath $logPath -Append | Out-Null

    "[$(Get-Date -Format o)] Build/publish completed" | Tee-Object -FilePath $logPath -Append | Out-Null
}
catch {
    Write-Host "BUILD_AND_PUBLISH_v10 failed. See log: $logPath" -ForegroundColor Red
    throw
}

Write-Host "BUILD_AND_PUBLISH_v10 completed successfully." -ForegroundColor Green
Write-Host "Build log: $logPath" -ForegroundColor DarkGray
