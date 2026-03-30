param(
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

Write-Host "Restoring and publishing FO76CampPlanner for $Runtime..." -ForegroundColor Cyan
dotnet restore

dotnet publish .\FO76CampPlanner.csproj `
  -c Release `
  -r $Runtime `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true

Write-Host "Done. Output is in .\bin\Release\net8.0-windows\$Runtime\publish\" -ForegroundColor Green
