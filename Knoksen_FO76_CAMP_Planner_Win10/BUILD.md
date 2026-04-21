# Build Guide – FO76 CAMP Planner Developer Setup

**Platform**: Windows (primary), Linux/macOS (source only, no GUI)  
**Framework**: .NET 8  
**Target**: Windows 10/11 (64-bit)

---

## System Requirements for Building

| Requirement | Version | Download |
|---|---|---|
| **.NET SDK** | 8.0 or later | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0) |
| **Git** | Any recent version | [git-scm.com](https://git-scm.com) |
| **Visual Studio** (optional) | 2022 or later | [visualstudio.microsoft.com](https://visualstudio.microsoft.com) |
| **Operating System** | Windows 10 Build 1909+ or Windows 11 | For GUI compilation |

---

## Quick Start – Build from Source

### Step 1: Clone the Repository

```bash
git clone https://github.com/knoksen/Fallout76CAMPplanner.git
cd Fallout76CAMPplanner/Knoksen_FO76_CAMP_Planner_Win10
```

### Step 2: Restore Dependencies

```bash
dotnet restore .\FO76CampPlanner.csproj
```

### Step 3: Build (Debug)

For development and testing:

```bash
dotnet build .\FO76CampPlanner.csproj -c Debug
```

Output: `bin\Debug\net8.0-windows\win-x64\FO76CampPlanner.dll`

### Step 4: Publish (Release)

For production distribution:

```bash
dotnet publish .\FO76CampPlanner.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true
```

Output: `bin\Release\net8.0-windows\win-x64\publish\FO76CampPlanner.exe`

The resulting `.exe` is a single, self-contained file (~70 MB) that includes the .NET 8 runtime.

---

## Build Scripts

### PowerShell Script

Use the provided build script:

```powershell
.\BUILD_AND_PUBLISH_v10.ps1
```

This script:
1. Cleans previous builds
2. Restores dependencies
3. Builds in Release mode
4. Publishes a single-file EXE
5. Reports results and build logs

Output logs go to `build_logs\build-YYYYMMDD-HHMMSS.log`.

### Release Manifest Script

After publish, generate a hash/size manifest for release uploads:

```powershell
.\GENERATE_RELEASE_MANIFEST.ps1
```

By default this writes `release-manifest.txt` inside `bin\Release\net8.0-windows\win-x64\publish`.

### Batch Script

For CMD users:

```batch
build-win10-singlefile.bat
```

---

## Project Structure

```
Knoksen_FO76_CAMP_Planner_Win10/
├── FO76CampPlanner.csproj          ← Main project file
├── Program.cs                       ← Entry point
├── MainForm.cs                      ← Main application window
├── PlannerCanvas.cs                 ← Grid and drawing engine
├── Models.cs                        ← Data models (Item, Layout, etc.)
├── MinimapPanel.cs                  ← Minimap visualization
├── app.manifest                     ← Windows app manifest
├── sample-blueprints/               ← Example blueprint files
├── build_logs/                      ← Build output logs
├── bin/                             ← Compiled binaries
│   ├── Debug/                       ← Debug builds
│   └── Release/                     ← Release builds (includes publish/)
└── obj/                             ← Intermediate build files
```

---

## Key Files for Developers

| File | Purpose |
|------|---------|
| **FO76CampPlanner.csproj** | Project configuration (version, dependencies, properties) |
| **Program.cs** | Application entry point and error handling |
| **MainForm.cs** | Main window UI and event handlers |
| **PlannerCanvas.cs** | Grid rendering, placement logic, and drawing |
| **Models.cs** | Data structures (PlacedItem, Layout, RuleProfile, etc.) |
| **MinimapPanel.cs** | Minimap visualization |
| **app.manifest** | Windows application metadata and capabilities |

---

## Development Workflow

### Open in Visual Studio

1. Open `FO76CampPlanner.sln` (or create one)
2. Load `FO76CampPlanner.csproj`
3. Set target framework to **.NET 8 – Windows**
4. Press `F5` to build and run

### Open in VS Code

1. Install the **C# Dev Kit** extension
2. Open the folder in VS Code
3. Run in terminal:
   ```powershell
   dotnet run --project .\FO76CampPlanner.csproj
   ```

---

## Configuration & Versioning

### Version Metadata

Located in `FO76CampPlanner.csproj`:

```xml
<Version>1.0.0</Version>
<FileVersion>1.0.0.0</FileVersion>
<AssemblyVersion>1.0.0.0</AssemblyVersion>
<InformationalVersion>1.0.0</InformationalVersion>
<Product>FO76 CAMP Planner</Product>
<Company>Knoksen</Company>
```

Update these fields before release:

1. Edit `FO76CampPlanner.csproj`
2. Change version strings (e.g., `1.0.0` → `1.1.0`)
3. Rebuild with the new version
4. Tag the release in Git

### Assembly Information

Visible in Windows file properties:

- Product name: `FO76 CAMP Planner`
- Version: `1.0.0` (from csproj)
- Company: `Knoksen`
- Copyright: `© 2026 Knoksen`

---

## Publish as Single-File EXE

The project is configured to build a **single, self-contained executable** that includes:
- .NET 8 runtime
- All dependencies
- Application code

### Why Single-File?

✅ No installation required  
✅ No Redistributable needed  
✅ Works on any Windows 10+ x64 machine  
✅ Easy to distribute and run  

### Build Settings (in .csproj)

```xml
<PublishSingleFile>true</PublishSingleFile>
<SelfContained>true</SelfContained>
<RuntimeIdentifier>win-x64</RuntimeIdentifier>
<IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
```

### Build Command

```powershell
dotnet publish .\FO76CampPlanner.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true
```

---

## Debugging

### Debug Mode

Build and run in debug mode:

```powershell
dotnet build .\FO76CampPlanner.csproj -c Debug
dotnet run --project .\FO76CampPlanner.csproj
```

Or press `F5` in Visual Studio.

### Check for Errors

If the app crashes, check:

1. **Error Log**: `%AppData%\FO76CampPlanner_Error.log`
2. **Build Output**: Check for compiler warnings
3. **Console Output**: Any exception messages

### Enable Logging

Add logging statements in key areas:

```csharp
System.Diagnostics.Debug.WriteLine($"Debug: {message}");
```

Output appears in Visual Studio's Debug Output window.

---

## Clean Build

To remove all build artifacts and start fresh:

```powershell
dotnet clean .\FO76CampPlanner.csproj
```

Or manually delete:
- `bin/` folder
- `obj/` folder

Then rebuild:

```powershell
dotnet build .\FO76CampPlanner.csproj -c Release
```

---

## Dependencies

The project has **no external NuGet dependencies** – it uses only:
- .NET 8 base libraries
- Windows Forms (built-in)
- System.Text.Json (built-in)

All dependencies are automatically restored by `dotnet restore`.

---

## Release Checklist

Before releasing a new version:

1. ✅ Update version in `FO76CampPlanner.csproj`
2. ✅ Test build: `dotnet build -c Release`
3. ✅ Test publish: `dotnet publish -c Release -r win-x64 --self-contained`
4. ✅ Run the published `.exe` and test functionality
5. ✅ Update `CHANGELOG.md` with new features
6. ✅ Tag release in Git: `git tag v1.0.0`
7. ✅ Push tag: `git push origin v1.0.0`
8. ✅ Create GitHub release with assets
9. ✅ Upload `.exe` to release page

---

## Troubleshooting Build Issues

### Error: "TargetFramework net8.0-windows is not recognized"

**Solution**: Install .NET 8 SDK

```bash
dotnet --version  # Check installed version
```

Download .NET 8 from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0).

### Error: "The type or namespace name 'MainForm' could not be found"

**Solution**: Ensure all `.cs` files are in the project directory and project loads correctly

```bash
dotnet build .\FO76CampPlanner.csproj  # Full rebuild
```

### Error: "Cannot find project or directory 'FO76CampPlanner.csproj'"

**Solution**: Navigate to the correct directory

```bash
cd Fallout76CAMPplanner/Knoksen_FO76_CAMP_Planner_Win10
```

### Build Succeeds But No EXE

**Solution**: Check `bin/Release/net8.0-windows/win-x64/publish/` folder

```powershell
Get-ChildItem .\bin\Release\net8.0-windows\win-x64\publish\
```

If empty, try a clean rebuild:

```powershell
dotnet clean
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

---

## Contributing

To contribute improvements:

1. Fork the repository on GitHub
2. Clone your fork
3. Create a feature branch: `git checkout -b feature/my-feature`
4. Make changes in `*.cs` files
5. Build and test: `dotnet build -c Release`
6. Commit with clear messages: `git commit -m "Add feature X"`
7. Push to your fork: `git push origin feature/my-feature`
8. Open a Pull Request on GitHub

---

## References

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Windows Forms](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/)
- [Publishing .NET Apps](https://learn.microsoft.com/en-us/dotnet/core/deploying/)
- [GitHub Repository](https://github.com/knoksen/Fallout76CAMPplanner)

---

**Happy building! 🚀**
