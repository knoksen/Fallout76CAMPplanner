@echo off
setlocal EnableExtensions

set "ROOT=C:\Users\knoksen\Appz\Fallout76CAMPplanner\Knoksen_FO76_CAMP_Planner_Win10"

echo.
echo ================================================
echo FO76 CAMP Planner v10 - One Click Build Launcher
echo ================================================
echo.

echo Target folder:
echo %ROOT%
echo.

if not exist "%ROOT%\FO76CampPlanner.csproj" (
  echo [ERROR] Project file not found:
  echo %ROOT%\FO76CampPlanner.csproj
  echo.
  echo Make sure the v10 package is extracted to:
  echo C:\Users\knoksen\Appz\Fallout76CAMPplanner\Knoksen_FO76_CAMP_Planner_Win10
  echo.
  pause
  exit /b 1
)

cd /d "%ROOT%"

echo [1/4] Checking dotnet...
where dotnet >nul 2>nul
if errorlevel 1 (
  echo [ERROR] dotnet was not found in PATH.
  echo Install .NET 8 SDK and try again.
  pause
  exit /b 1
)

echo [2/4] Running build and publish script...
powershell -NoProfile -ExecutionPolicy Bypass -File ".\BUILD_AND_PUBLISH_v10.ps1"
if errorlevel 1 (
  echo.
  echo [ERROR] Build/publish failed. Check build_logs inside the project folder.
  pause
  exit /b 1
)

set "EXE=%ROOT%\bin\Release\net8.0-windows\win-x64\publish\FO76CampPlanner.exe"

echo [3/4] Checking EXE...
if not exist "%EXE%" (
  echo [ERROR] EXE not found where expected:
  echo %EXE%
  pause
  exit /b 1
)

echo [4/4] Launching EXE...
start "FO76 CAMP Planner" "%EXE%"

echo.
echo Success:
echo %EXE%
echo.
pause
exit /b 0
