param(
    [string]$PublishDir = ".\bin\Release\net8.0-windows\win-x64\publish",
    [string]$OutputFile = "release-manifest.txt"
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$resolvedPublishDir = Resolve-Path $PublishDir -ErrorAction Stop
$files = Get-ChildItem -Path $resolvedPublishDir -File | Sort-Object Name

if ($files.Count -eq 0) {
    throw "No files found in publish directory: $resolvedPublishDir"
}

$lines = @()
$lines += "FO76 CAMP Planner Release Manifest"
$utcNow = (Get-Date).ToUniversalTime().ToString('yyyy-MM-dd HH:mm:ss')
$lines += "Generated (UTC): $utcNow"
$lines += "Publish directory: $resolvedPublishDir"
$lines += ""
$lines += "FileName | SizeBytes | SHA256"
$lines += "-------- | --------- | ------"

foreach ($file in $files) {
    $hash = (Get-FileHash -Path $file.FullName -Algorithm SHA256).Hash
    $lines += "{0} | {1} | {2}" -f $file.Name, $file.Length, $hash
}

$manifestPath = Join-Path $resolvedPublishDir $OutputFile
Set-Content -Path $manifestPath -Value $lines -Encoding UTF8

Write-Host "Release manifest written:" -ForegroundColor Green
Write-Host $manifestPath
