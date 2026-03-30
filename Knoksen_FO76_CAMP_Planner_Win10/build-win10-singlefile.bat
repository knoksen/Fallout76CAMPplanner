@echo off
setlocal
cd /d "%~dp0"
echo Restoring and publishing FO76CampPlanner for win-x64...
dotnet restore
if errorlevel 1 goto :fail

dotnet publish .\FO76CampPlanner.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
if errorlevel 1 goto :fail

echo.
echo Done. Output is in .\bin\Release\net8.0-windows\win-x64\publish\
exit /b 0

:fail
echo Build failed.
exit /b 1
