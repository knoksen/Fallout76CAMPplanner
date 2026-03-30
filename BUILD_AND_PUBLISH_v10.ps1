param(
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$projectRoot = Join-Path $PSScriptRoot "Knoksen_FO76_CAMP_Planner_Win10"
Set-Location $projectRoot

$logDir = Join-Path $projectRoot "build_logs"
New-Item -ItemType Directory -Force -Path $logDir | Out-Null
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$logFile = Join-Path $logDir "build-$timestamp.log"

function Write-Step($msg) {
    Write-Host "`n=== $msg ===" -ForegroundColor Cyan
}

Write-Step "Environment"
dotnet --info | Tee-Object -FilePath $logFile

Write-Step "Clean"
dotnet clean .\FO76CampPlanner.csproj | Tee-Object -FilePath $logFile -Append

Write-Step "Restore"
dotnet restore .\FO76CampPlanner.csproj | Tee-Object -FilePath $logFile -Append

Write-Step "Build Release"
dotnet build .\FO76CampPlanner.csproj -c Release | Tee-Object -FilePath $logFile -Append

Write-Step "Publish single-file EXE"
dotnet publish .\FO76CampPlanner.csproj `
  -c Release `
  -r $Runtime `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true | Tee-Object -FilePath $logFile -Append

$publishDir = Join-Path $projectRoot "bin\Release\net8.0-windows\$Runtime\publish"
$exePath = Join-Path $publishDir "FO76CampPlanner.exe"

Write-Step "Result"
if (Test-Path $exePath) {
    Write-Host "EXE built successfully:" -ForegroundColor Green
    Write-Host $exePath -ForegroundColor Green
    Write-Host "Build log:" -ForegroundColor Yellow
    Write-Host $logFile -ForegroundColor Yellow
    exit 0
}
else {
    Write-Host "Publish completed, but EXE was not found where expected:" -ForegroundColor Red
    Write-Host $exePath -ForegroundColor Red
    Write-Host "Check build log:" -ForegroundColor Yellow
    Write-Host $logFile -ForegroundColor Yellow
    exit 1
}
