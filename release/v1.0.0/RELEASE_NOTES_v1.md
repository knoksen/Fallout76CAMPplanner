# FO76 CAMP Planner v1.0.0 – Release Notes

**Release Date**: March 29, 2026  
**Version**: 1.0.0  
**Platform**: Windows 10/11 (64-bit)

---

## Welcome to v1.0.0! 🎉

This is the first official public release of FO76 CAMP Planner – a professional tool for designing Fallout 76 CAMPs and Shelters. After months of development and refinement, the planner is production-ready and packed with features to accelerate your building workflow.

---

## What's New in v1.0.0

### Complete Feature Set

✅ **Precise Grid-Based Design** – Build on a 2D grid with snap-to-grid accuracy  
✅ **Smart Placement Rules** – Foundation-aware snapping and collision detection  
✅ **Budget Tracking** – Real-time placed vs. stored budget display  
✅ **Multi-Select Editing** – Group operations on multiple items at once  
✅ **Reusable Blueprints** – Save and paste favourite modules  
✅ **Advanced Visualization** – Turret coverage, CAMP radius, layer toggles  
✅ **Workflow Guidance** – Structured build stages with smart recommendations  
✅ **Rich Presets** – Start from surface CAMPs or specialized shelter templates  
✅ **Export Options** – Save as JSON or export to PNG for sharing  
✅ **Professional Polish** – Dark UI, keyboard shortcuts, undo/redo  

### For First-Time Users

If you're new to FO76 CAMP Planner:

1. **Download the EXE** from the [GitHub release page](https://github.com/knoksen/Fallout76CAMPplanner/releases)
2. **Run it** – No installation required; just double-click `FO76CampPlanner.exe`
3. **Open a sample project** to explore the interface
4. **Try the quick-start workflow**: Layout → Envelope → Systems → Defense → Polish
5. **Read [INSTALL_WINDOWS.md](INSTALL_WINDOWS.md)** for detailed guidance

### For Existing Users (v6 and earlier)

The transition from v6 to v1.0.0 is seamless – no breaking changes:

- All existing projects (`.json` files) remain fully compatible
- All blueprints (`.blueprint.json` files) work as before
- All presets and sample files are included
- The workflow and UI remain largely the same, with refinement

**No action required** – simply use the new EXE and enjoy the stability of an official release.

---

## Key Highlights

### Workflow Excellence
The app now guides you through a structured build process:
- **Layout**: Sketch your foundation boundaries
- **Envelope**: Add walls, doors, and roof
- **Systems**: Place workbenches, utilities, and power
- **Defense**: Position turrets and defensive elements
- **Polish**: Decor, lighting, and presentation

### Smart Placement
- Walls snap to foundation edges; roofs snap to tiles
- Stairs snap to open edges; no invalid placements allowed
- Soft overlaps allowed for Power and Decor (Relaxed mode)
- Shelter-specific rules for Vault and Bunker designs

### Productivity Features
- **Multi-select** any number of items and move/delete them together
- **Blueprints** save time by storing reusable modules
- **Undo/Redo** lets you experiment without fear
- **Layer toggles** hide distractions while focusing on specific systems
- **Quick recommendations** suggest your next best action

### Analysis & Visualization
- **Minimap** gives you a bird's-eye view of your entire design
- **Turret arcs** show defensive coverage at a glance
- **CAMP radius** overlay helps with surface layout planning
- **Layer locks** freeze completed sections while you iterate

### Professional Output
- Save projects as human-readable JSON
- Export to PNG for sharing on Discord, Reddit, or forums
- Create libraries of reusable blueprints
- Preset variety (9 built-in templates)

---

## System Requirements

| Requirement | Specification |
|---|---|
| **OS** | Windows 10 or later (64-bit) |
| **Architecture** | x64 (Intel / AMD) |
| **RAM** | 4 GB minimum, 8 GB recommended |
| **Storage** | ~50 MB free space |
| **Graphics** | Any modern integrated GPU |
| **Internet** | None required (offline use fully supported) |

---

## Installation & Quick Start

### Step 1: Download
Get `FO76CampPlanner.exe` from the [latest release](https://github.com/knoksen/Fallout76CAMPplanner/releases).

### Step 2: Run
Double-click the `.exe` file. Windows may show a security prompt – allow it.

### Step 3: Grant Permissions (if needed)
- **Windows Defender SmartScreen**: Click "More info" → "Run anyway"
- **Antivirus software**: Allow the app to run (it's a legitimate application)

### Step 4: Start Building
1. Open a **sample project** from the File menu, or
2. Create a **new project**, or
3. Open an **existing project** (`.json` file)

See [INSTALL_WINDOWS.md](INSTALL_WINDOWS.md) for more details.

---

## Keyboard Shortcuts

| Action | Shortcut |
|--------|----------|
| Rotate selected item(s) | `R` |
| Delete selected item(s) | `Delete` or `Backspace` |
| Undo | `Ctrl+Z` |
| Redo | `Ctrl+Y` |
| Zoom in | `+` |
| Zoom out | `-` |
| Move selection (1 cell) | Arrow keys |
| Multi-select add/remove | `Ctrl+Click` |
| Marquee selection | Click and drag |

---

## Getting Help

### First Steps
- Open **sample projects** from the built-in samples
- Read **quick-start tooltips** in the Inspector tab
- Consult [INSTALL_WINDOWS.md](INSTALL_WINDOWS.md) for setup help

### Troubleshooting
See the **Troubleshooting** section in [README.md](README_v1_RELEASE.md).

### Report Issues
Found a bug? File an issue on [GitHub Issues](https://github.com/knoksen/Fallout76CAMPplanner/issues).

### Feature Requests
Have an idea? Post in [GitHub Discussions](https://github.com/knoksen/Fallout76CAMPplanner/discussions).

---

## What's Next?

### v1.1 – Planning Engine Enhancements (Q2 2026)
- Quick-duplicate for zones
- Better group rotation
- Blueprint library per CAMP slot

### v1.2 – Deeper Fallout 76 Logic (Q3 2026)
- Visitor flow visualization
- Trap logic zones
- Budget profiles per playstyle

### v2.0 – 3D & Professional Tooling (Q4 2026 – Q1 2027)
- 3D preview of your layout
- Print-ready PDF exports
- Darker Fallout-inspired UI

See the full [roadmap](ROADMAP_NEXT.md) for more details.

---

## Known Limitations & Workarounds

| Issue | Workaround |
|-------|-----------|
| Large projects (2000+ items) slow down | Disable layer visibility; split project into zones |
| PNG export blurry on dense layouts | Zoom in before exporting; export sections separately |
| Blueprints reset when app closes | Save blueprints as project files; store `.json` files separately |
| No 3D visualization | Coming in v2.0; use the minimap and overlays for now |

---

## License & Attribution

**FO76 CAMP Planner** is licensed under the **MIT License**.

**Fallout 76** is © Bethesda Softworks LLC. This tool is a fan-made utility not affiliated with Bethesda.

---

## Thank You! 🙏

A huge thank you to all the builders, testers, and Fallout 76 enthusiasts who supported development. Your feedback shaped this tool into what it is today.

Enjoy planning your CAMP!

---

**Questions? Issues? Suggestions?**  
👉 [GitHub Repository](https://github.com/knoksen/Fallout76CAMPplanner)

---
