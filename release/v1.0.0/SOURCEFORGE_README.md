# FO76 CAMP Planner v1.0.0 – SourceForge Release

**Release Date**: March 29, 2026  
**Version**: 1.0.0  
**Platform**: Windows 10/11 (64-bit)  
**License**: MIT (Free)

---

## What is FO76 CAMP Planner?

FO76 CAMP Planner is a **professional desktop tool for designing Fallout 76 CAMP and Shelter layouts** with precision grid-based planning, intelligent placement rules, and advanced visualization features.

### Why Use It?

✅ **Iterate Faster** – Plan your CAMP before spending budget in-game  
✅ **Save Time** – Reuse modules with blueprints  
✅ **Build Smarter** – Turret coverage arcs, budget tracking, snapping rules  
✅ **Share Designs** – Export to PNG or JSON  
✅ **No Installation** – Single EXE file, no setup wizard  
✅ **Completely Free** – MIT licensed, open source  

---

## Key Features

### 🎮 Building Tools
- 14 item categories: Foundation, Wall, Door, Stairs, Roof, Workbench, Turret, Power, Light, Decor, Vendor, Resource, Display, Ally
- Surface CAMP and Interior Shelter modes
- 9 built-in presets (Vault Lobby, Missile Silo, Bunker, Casino, etc.)

### 🎯 Smart Placement
- Foundation-aware snapping (walls snap to edges, roofs to tiles, stairs to open edges)
- Rule profiles: Strict, Relaxed, Shelter
- Layer locks and visibility toggles
- Real-time collision detection

### 📐 Advanced Editing
- Multi-select (Ctrl+Click or drag-box)
- Group operations (move, duplicate, delete)
- Undo/Redo (~50 actions)
- Contextual Inspector for fine tuning

### 🗺️ Visualization
- Minimap with selected-item highlighting
- Turret coverage arcs
- CAMP radius overlay
- Focus presets (All, Structure, Systems, Defense, Presentation)

### 💾 Data Management
- Save/load projects as JSON
- Reusable blueprints
- Export to PNG
- Sample projects included

### ⌨️ Productivity
- Keyboard shortcuts (R=Rotate, Delete, Ctrl+Z/Y, etc.)
- Live placement preview
- Workflow guidance (Layout → Envelope → Systems → Defense → Polish)
- Dark UI with high contrast

---

## System Requirements

| Requirement | Details |
|---|---|
| **Operating System** | Windows 10 Build 1909+ or Windows 11 |
| **Architecture** | 64-bit (x64) processor |
| **RAM** | 4 GB minimum, 8 GB recommended |
| **Storage** | ~50 MB free space |
| **Installation** | None – single EXE file |

---

## Installation

### Quick Start

1. **Download** `FO76CampPlanner.exe` from this SourceForge release
2. **Run** the executable (double-click)
3. **Grant permissions** when prompted (normal Windows security dialog)
4. **Start building!** Open a sample project or create new

### First Run

On first launch, the app may take a few seconds to initialize (extracting the .NET 8 runtime). Subsequent launches are instant.

### No Installation Wizard

Unlike many applications, FO76 CAMP Planner requires **no installation**:
- Single `.exe` file (~70 MB)
- No Windows Registry changes
- No separate runtime installation
- Works on any Windows 10/11 x64 machine

### File Location

Place the `.exe` anywhere convenient:
- Desktop (for quick access)
- Documents\Games (organized)
- C:\Program Files\FO76CampPlanner (standard location)
- USB drive (portable)

We recommend creating a folder like `C:\Games\FO76CampPlanner` and keeping it there.

---

## Getting Started

### Step 1: Launch the App
Double-click `FO76CampPlanner.exe`

### Step 2: Grant Permissions
If Windows shows a security dialog, click **"Run anyway"** or **"Yes"** to allow.

### Step 3: Open a Sample Project

1. Click **File** → **Open Sample Project**
2. Choose one of the included samples:
   - `sample-foundation-layout.json` – Surface CAMP with foundations
   - `sample-missile-silo-layout.json` – Shelter interior
   - `sample-defense-layout.json` – Defensive layout
3. Explore the interface and try features

### Step 4: Create Your Own CAMP

1. Click **File** → **New Project**
2. Select a preset (Custom Surface CAMP, Vault Lobby, etc.)
3. Give it a name and start building!

---

## Quick Usage Tips

### Build Your First CAMP

1. **Select a tool** from the toolbar (e.g., Foundation)
2. **Click on the canvas** to place items
3. **Hold Ctrl and click** to select multiple items
4. **Press `R`** to rotate, **`Delete`** to remove
5. **Save frequently** with `Ctrl+S`

### Multi-Select & Group Operations

- **Ctrl+Click** individual items to add to selection
- **Click & drag** for marquee selection
- **Drag selected items** to move as a group
- **Press Delete** to remove all selected

### Create Reusable Blueprints

1. Select multiple items on your canvas
2. Click **Library** → **Save Selection as Blueprint**
3. Name the blueprint (e.g., `my-tower.blueprint.json`)
4. Later, load the blueprint and paste it into any project

### Export Your Design

- **File** → **Export to PNG** – Share as image
- **File** → **Save Project** – Save as JSON

### Keyboard Shortcuts

| Action | Key |
|--------|-----|
| Rotate | `R` |
| Delete | `Delete` |
| Undo | `Ctrl+Z` |
| Redo | `Ctrl+Y` |
| Zoom in | `+` |
| Zoom out | `-` |
| Move (1 cell) | Arrow keys |
| Multi-select | `Ctrl+Click` |
| Save | `Ctrl+S` |

---

## Features Overview

### ✅ Supported Features

- 2D grid-based CAMP and Shelter design
- Foundation-aware snapping
- Real-time budget tracking
- Multi-select and group operations
- Undo/Redo
- JSON save/load
- PNG export
- Reusable blueprints
- Analysis overlays (turret arcs, CAMP radius)
- 9 built-in presets
- 3 sample projects
- Keyboard shortcuts
- Dark, accessible UI

### ⏳ Planned for Future (v1.1+)

- 3D visualization
- Blueprint library syncing
- Visitor flow visualization
- Trap logic zones
- Print-ready PDF exports
- Expanded preset library

---

## Troubleshooting

### The App Won't Start

1. Ensure you're on **Windows 10 Build 1909+** or **Windows 11** (64-bit)
2. Check that you have **at least 50 MB free space**
3. Look in `%AppData%\FO76CampPlanner_Error.log` for error details
4. Try downloading a fresh copy of the `.exe`

### Windows Security Warning

If Windows Defender or another antivirus blocks the app:

1. Click **"More info"** in the security dialog
2. Click **"Run anyway"**

This is normal for free open-source software. The source code is available on [GitHub](https://github.com/knoksen/Fallout76CAMPplanner) for verification.

### Slow Performance

- Disable unnecessary layer visibility (toggle Structure, Utility, etc.)
- Disable analysis overlays (turret arcs, CAMP radius)
- Reduce project size if possible

### Can't Save Projects

- Ensure you have write permissions in the save folder
- Try saving to Desktop or Documents
- Use a simple filename without special characters

### PNG Export is Blurry

- Zoom in before exporting (150–200%)
- Export sections separately
- Try different zoom levels

---

## Documentation & Support

| Resource | Link |
|---|---|
| **GitHub Repository** | https://github.com/knoksen/Fallout76CAMPplanner |
| **Issue Tracker** | https://github.com/knoksen/Fallout76CAMPplanner/issues |
| **Discussions** | https://github.com/knoksen/Fallout76CAMPplanner/discussions |
| **Full README** | See included `README_v1_RELEASE.md` |
| **Installation Guide** | See included `INSTALL_WINDOWS.md` |
| **Release Notes** | See included `RELEASE_NOTES_v1.md` |
| **Build Instructions** | See included `BUILD.md` |
| **Changelog** | See included `CHANGELOG.md` |

---

## Included Files

This SourceForge release package contains:

- **FO76CampPlanner.exe** – Main application
- **README_v1_RELEASE.md** – Full documentation
- **INSTALL_WINDOWS.md** – Installation guide
- **RELEASE_NOTES_v1.md** – What's new in v1.0.0
- **CHANGELOG.md** – Version history
- **BUILD.md** – For developers
- **sample-foundation-layout.json** – Example surface CAMP
- **sample-missile-silo-layout.json** – Example shelter
- **sample-defense-layout.json** – Example defensive layout
- **sample-blueprints/** – Reusable module examples

---

## License & Attribution

**FO76 CAMP Planner** is licensed under the **MIT License** – free for personal and commercial use.

**Source Code**: Available on [GitHub](https://github.com/knoksen/Fallout76CAMPplanner)

**Copyright © 2026 Knoksen**

**Fallout 76** is © Bethesda Softworks LLC. This tool is a fan-made utility not affiliated with Bethesda.

---

## Roadmap – What's Next?

### v1.1 – Planning Engine (Q2 2026)
Enhanced selection and duplication tools, blueprint per-CAMP-slot organization

### v1.2 – Fallout 76 Logic (Q3 2026)
Visitor flow visualization, trap logic zones, budget profiles

### v2.0 – Professional Tooling (Q4 2026)
3D preview, print-ready exports, Fallout-inspired UI redesign

See the full roadmap in the GitHub repository.

---

## Contributing

This project is **open source**. If you'd like to contribute:

1. Fork the repository on [GitHub](https://github.com/knoksen/Fallout76CAMPplanner)
2. Create a feature branch
3. Make your changes
4. Submit a pull request

All contributors are welcome!

---

## Contact & Support

**Have questions or found a bug?**

- 📋 File an issue: [GitHub Issues](https://github.com/knoksen/Fallout76CAMPplanner/issues)
- 💬 Join discussions: [GitHub Discussions](https://github.com/knoksen/Fallout76CAMPplanner/discussions)
- 🌐 Visit the repository: https://github.com/knoksen/Fallout76CAMPplanner

---

## Thank You!

A huge thank you to all the Fallout 76 builders, testers, and community members who inspired and supported the development of this tool.

**Enjoy planning your CAMP! 🏗️**

---

**FO76 CAMP Planner v1.0.0**  
*Professional CAMP Design Tool for Fallout 76*

Free | Open Source | MIT License
