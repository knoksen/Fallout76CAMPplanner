# Installation Guide – FO76 CAMP Planner for Windows

**Platform**: Windows 10 / Windows 11 (64-bit)  
**Version**: 1.0.0  
**Installation Type**: Single-file executable (no installation wizard)

---

## Quick Start (3 Steps)

### Step 1: Download

Download `FO76CampPlanner.exe` from the [latest release](https://github.com/knoksen/Fallout76CAMPplanner/releases).

**File Details**:
- File: `FO76CampPlanner.exe`
- Size: ~70 MB (includes .NET 8 runtime)
- Hash: [See release page](https://github.com/knoksen/Fallout76CAMPplanner/releases)

### Step 2: Save Somewhere Convenient

Choose a location for the app:

- **Desktop** – Easy access, but may clutter desktop
- **Documents\Games** – Organized, keeps projects nearby
- **C:\Program Files\FO76CampPlanner** – Standard program location (requires Admin to create)
- **Local USB drive** – Portable, use on any Windows machine

**Recommendation**: Create a folder like `C:\Games\FO76CampPlanner` and place the `.exe` there.

### Step 3: Run It

Double-click `FO76CampPlanner.exe`.

The app will start immediately. If this is the first run, Windows may:
1. Show a security dialog (see below)
2. Take a few seconds to initialize (extracting .NET runtime)
3. Open the main window

---

## Security Prompts & Permissions

The first time you run the app, Windows or your antivirus may ask for permission. This is normal.

### Windows Defender SmartScreen

**What you see**:
```
"Windows Defender SmartScreen prevented an unrecognized app from starting.
Running this app might put your PC at risk."
```

**What to do**:
1. Click **"More info"**
2. Click **"Run anyway"** at the bottom

**Why this happens**: The app is not signed with a commercial certificate yet (typical for free open-source tools). The executable is safe – source code is available on GitHub for verification.

### User Access Control (UAC)

**What you see**:
```
"Do you want to allow this app to make changes to your device?"
```

**What to do**:
1. Click **"Yes"** to proceed

**Why this happens**: The app needs write permissions for logs and project files.

### Antivirus Warning

If your third-party antivirus (Norton, McAfee, etc.) blocks it:
1. Check your antivirus dashboard
2. Add the app to an exclusion/allowlist
3. Or temporarily disable the antivirus for the first run

---

## System Requirements

| Requirement | Details |
|---|---|
| **OS** | Windows 10 Build 1909 or later, or Windows 11 |
| **Architecture** | 64-bit (x64) Intel or AMD processor |
| **RAM** | 4 GB minimum; 8 GB recommended |
| **Storage** | ~50 MB free space for the app |
| **GPU** | Integrated graphics sufficient (no discrete GPU needed) |
| **.NET Runtime** | Bundled in the executable (no separate install) |

### Check Your Windows Version

1. Press `Win + R`
2. Type `winver` and press Enter
3. Confirm your version is **Windows 10 (Build 1909+)** or **Windows 11**

---

## First Run – Initializing the App

On the very first launch:

1. **Startup delay** (~5–10 seconds) – The app extracts the .NET 8 runtime
2. **Blue window appears** – The UI initializes
3. **Main window opens** – Ready to build

**Note**: Subsequent launches are instant.

---

## Your First Project

### Option A: Open a Sample Project (Recommended)

1. Launch the app
2. Click **File** → **Open Sample Project**
3. Choose one:
   - **sample-foundation-layout.json** – Standard surface CAMP with foundations
   - **sample-missile-silo-layout.json** – Shelter (interior) design
   - **sample-defense-layout.json** – Defensive layout with turrets
4. Explore the canvas, try placing items, test multi-select

### Option B: Create a New Project

1. Launch the app
2. Click **File** → **New Project**
3. Choose a **Preset**:
   - **Custom Surface CAMP** – Blank outdoor layout
   - **Vault Lobby Shelter** – Pre-built interior template
   - **Missile Silo Shelter** – Underground vault variant
   - Others...
4. Give it a name and start building

### Option C: Open an Existing Project

If you have a saved project (`.json` file):

1. Click **File** → **Open Project**
2. Navigate to the `.json` file and select it
3. The project loads on the canvas

---

## Managing Your Projects

### Save Your Work

1. Click **File** → **Save Project As** (or `Ctrl+S`)
2. Choose a folder (e.g., **Documents\FO76 Projects**)
3. Give your project a name, e.g., `my-camp-v2.json`
4. Click **Save**

**Tip**: Save frequently – use `Ctrl+S` after major changes.

### Export to PNG

To share your layout as an image:

1. Click **File** → **Export to PNG**
2. Choose a location and filename
3. A screenshot of your current canvas view is saved
4. Share on Discord, Reddit, or forums

### Create a Blueprint

To save a reusable module:

1. **Multi-select** items on your canvas (using `Ctrl+Click`)
2. Click **Library** → **Save Selection as Blueprint**
3. Give it a name, e.g., `defense-tower.blueprint.json`
4. Click **Save**

Later, paste this blueprint into any project:
1. Click **Library** → **Load Blueprint**
2. Select your `.blueprint.json` file
3. Click on the canvas to place the blueprint

---

## Keyboard Shortcuts

Learn these shortcuts for efficient building:

| Action | Shortcut |
|--------|----------|
| Rotate selected item(s) | `R` |
| Delete selected item(s) | `Delete` |
| Undo last action | `Ctrl+Z` |
| Redo last action | `Ctrl+Y` |
| Zoom in | `+` or Mouse wheel up |
| Zoom out | `-` or Mouse wheel down |
| Pan canvas | Hold `Space` + drag mouse |
| Move selection (1 cell) | Arrow keys |
| Multi-select add/remove | `Ctrl+Click` |
| Marquee selection (box) | Click and drag |
| Save project | `Ctrl+S` |

---

## Troubleshooting

### The App Won't Start

**Problem**: Clicking the .exe does nothing

**Solutions**:
1. **Verify Windows version**: Press `Win+R`, type `winver`. Must be Windows 10 Build 1909+ or Windows 11
2. **Check disk space**: Ensure at least 50 MB free
3. **Disable antivirus temporarily**: Try running with antivirus off
4. **Reinstall**: Delete the `.exe` and download it fresh
5. **Check error log**: Look in `%AppData%\FO76CampPlanner_Error.log`

---

### Security Alert on First Run

**Problem**: Windows or antivirus blocks the app

**Solution**: See [Security Prompts & Permissions](#security-prompts--permissions) above

---

### Slow Performance

**Problem**: App is sluggish or freezes

**Solutions**:
1. Disable unnecessary **layer visibility** (toggle Structure, Utility, etc.)
2. Disable **analysis overlays** (turret arcs, CAMP radius)
3. **Reduce project complexity** – break large projects into smaller files
4. Close other apps to free up RAM

---

### Projects Won't Save

**Problem**: Error when saving a project

**Solutions**:
1. **Check folder permissions**: Try saving to Desktop or Documents
2. **Check disk space**: Ensure at least 10 MB free
3. **Use a simpler filename**: Avoid special characters like `< > * ? "|`
4. **Check the error log**: Look in `%AppData%\FO76CampPlanner_Error.log`

---

### Can't Place Items

**Problem**: Items won't place on the canvas

**Solutions**:
1. **Verify the tool is selected**: Check the toolbar – is the correct tool highlighted?
2. **Check rule profile**: Are you in **Strict**, **Relaxed**, or **Shelter** mode? Some modes restrict placement
3. **Check layer lock**: Ensure the target layer isn't locked (see **Layers** panel)
4. **Check overlap rules**: With **Strict**, no overlaps allowed except foundations

---

### PNG Export is Blurry

**Problem**: Exported image is hard to read

**Solutions**:
1. **Zoom in before exporting**: Zoom to 150–200%, then export
2. **Export sections separately**: Split a large project into multiple exports
3. **Try different zoom levels** to find the best clarity

---

## Getting Help

### In-App Help

- Hover over toolbar buttons and labels – tooltips will appear
- Check the **Inspector** tab for contextual guidance
- Open **sample projects** to learn by example

### Online Resources

- **[README.md](README_v1_RELEASE.md)** – Full feature overview and FAQ
- **[RELEASE_NOTES_v1.md](RELEASE_NOTES_v1.md)** – What's new in v1.0.0
- **[GitHub Issues](https://github.com/knoksen/Fallout76CAMPplanner/issues)** – Report bugs
- **[GitHub Discussions](https://github.com/knoksen/Fallout76CAMPplanner/discussions)** – Ask questions and suggest features

### Contact

Post an issue or discussion on the [GitHub repository](https://github.com/knoksen/Fallout76CAMPplanner).

---

## Updating to Newer Versions

When a new version is released:

1. Download the latest `FO76CampPlanner.exe` from the [releases page](https://github.com/knoksen/Fallout76CAMPplanner/releases)
2. Replace your old `.exe` with the new one
3. Your existing projects (`.json` files) remain compatible – no migration needed

---

## What's Next?

- **Learn the workflow**: Layout → Envelope → Systems → Defense → Polish
- **Explore sample projects** to understand features
- **Build your first CAMP** and experiment
- **Create blueprints** for reusable modules
- **Share your designs** on Discord, Reddit, or forums

---

## License

FO76 CAMP Planner is licensed under the **MIT License** and is free software.

**Source code**: [github.com/knoksen/Fallout76CAMPplanner](https://github.com/knoksen/Fallout76CAMPplanner)

---

**Enjoy planning your CAMP! 🏗️**
