# FO76 CAMP Planner v1.0.0

**A professional tool for planning Fallout 76 CAMP and Shelter layouts with precision grid-based design, budget tracking, and intelligent placement rules.**

![FO76 CAMP Planner](https://github.com/knoksen/Fallout76CAMPplanner/raw/main/docs/screenshot-placeholder.png)

## Overview

FO76 CAMP Planner is a Windows desktop application that empowers Fallout 76 players with a sophisticated, productivity-focused tool for designing CAMPs and Shelters. It combines intuitive drag-and-drop placement with intelligent rule validation, making it easy to prototype, iterate, and visualize complex layouts before building them in-game.

### For Whom?

- **CAMP Architects & Builders**: Design intricate layouts with confidence
- **Shelter Designers**: Plan vault interiors with specialized shelter rules
- **Defense Strategists**: Visualize turret coverage and defensive perimeters
- **Creative Builders**: Discover new building concepts using blueprints and presets
- **Anyone** who wants to prototype outside Fallout 76 without exhausting budget caps

### Why Use FO76 CAMP Planner?

✅ **Iterate Faster** – Build in the app instantly; save your design before spending budget in-game  
✅ **Smart Placement** – Foundation-aware snapping, rule validation, and collision detection  
✅ **Budget Tracking** – Real-time placed vs. stored budget measurement  
✅ **Reusable Blueprints** – Save modules and paste them into new projects  
✅ **Advanced Visualization** – Turret arcs, CAMP radius, layer visibility controls  
✅ **Multiple Presets** – Start from surface CAMPs or specialized shelter templates  
✅ **Export & Share** – Save designs as JSON, export to PNG, share with friends  

---

## Features

### Core CAMP & Shelter Building

- **2D Grid-Based Canvas** – Precise placement with pixel-perfect snapping
- **Building Tools**: Foundation, Wall, Door, Stairs, Roof, and more
- **Placement Categories**: Structure, Utility, Defense, Power, Aesthetic, Commerce
- **Dual Modes**: CAMP (surface) and Shelter (interior)
- **Rule Profiles**:
  - **Strict** – No overlap except for foundations
  - **Relaxed** – Allows soft overlaps (Power, Decor)
  - **Shelter** – Generous stacking with category restrictions (Vendor, Resource, Display, Ally)

### Smart Snapping & Validation

- **Foundation-Aware Snapping**:
  - Walls and doors align to foundation edges
  - Roofs snap to foundation tiles
  - Stairs snap to open foundation edges
- **Smart Collision Detection** – Prevents overlaps based on active rule profile
- **Layer Lock** – Freeze entire layers while working on others

### Advanced Selection & Editing

- **Multi-Select**:
  - Hold `Ctrl` + click to build selections
  - Drag-box (marquee) selection for quick area grabs
  - Group moves and group deletions
- **Contextual Inspector**:
  - Edit item notes, X/Y coordinates, rotation
  - Quick actions: duplicate, delete, set CAMP center
- **Keyboard Shortcuts**:
  - `R` = rotate
  - `Delete` = delete
  - `Ctrl+Z` / `Ctrl+Y` = undo/redo
  - `+` / `-` = zoom
  - Arrow keys = move selection by 1 cell
  - `Ctrl+Click` = multi-select

### Blueprint System

- **Save Selections** – Capture favourite modules as `.blueprint.json` files
- **Load & Paste** – Import blueprints and place them on the canvas
- **Build Reusable Libraries** – Store common patterns for quick reuse

### Analysis & Visualization

- **Surface CAMP Radius** – Approximate build/control zone overlay
- **Turret Coverage Arcs** – Visual representation of turret defensive coverage
- **Minimap Panel** – Overview of entire grid with selected-item highlighting
- **Layer Toggles** – Show/hide Structure, Utility, Defense, Power, Aesthetic, Commerce
- **Focus Presets** – Quick filters for Structure, Systems, Defense, and Presentation views

### Data Management

- **Save Projects** – Store designs as human-readable JSON
- **Export to PNG** – Snapshot your layout for sharing
- **Preset Surfaces** – Start from built-in surface CAMP templates
- **Preset Shelters** – Specialized interior templates (Vault, Missile Silo, etc.)
- **Sample Blueprints** – Learn by example with included modules

### Workflow Support

- **Build Stages Guide**: Layout → Envelope → Systems → Defense → Polish
- **Quick-Start Recommendations** – Dynamic suggestions for your next action
- **Workflow Status Dashboard** – Track progress across all build phases

---

## Getting Started

### Installation

1. Download **FO76CampPlanner.exe** from the [latest release](https://github.com/knoksen/Fallout76CAMPplanner/releases)
2. Run the executable (no installation required – single-file app)
3. Grant Windows security prompts if prompted

See [INSTALL_WINDOWS.md](INSTALL_WINDOWS.md) for detailed instructions.

### First Run

1. Launch the app
2. Open a **sample project**:
   - `sample-foundation-layout.json` – Standard surface CAMP
   - `sample-missile-silo-layout.json` – Shelter design
   - `sample-defense-layout.json` – Defensive layout example
3. Explore the toolbar and test features

### Quick Workflow

1. **Select Mode** – Click items on the canvas to select them
2. **Build Mode** – Choose a tool from the sidebar and place items
3. **Multi-Select** – Hold `Ctrl` and click multiple items
4. **Copy Designs** – Use **Save Selection as Blueprint** to capture reusable modules
5. **Export** – Save your project or export to PNG

For more guidance, see [INSTALL_WINDOWS.md](INSTALL_WINDOWS.md).

---

## Documentation

- **[INSTALL_WINDOWS.md](INSTALL_WINDOWS.md)** – Step-by-step installation and first-run guide
- **[BUILD.md](BUILD.md)** – Developer build instructions
- **[RELEASE_NOTES_v1.md](RELEASE_NOTES_v1.md)** – What's new in v1.0.0
- **[CHANGELOG.md](CHANGELOG.md)** – Full version history
- **[API & Developer Reference](#API)** – For contributors

---

## Current Features & Limitations

### ✅ Supported

- 2D CAMP and Shelter design
- Foundation snapping
- Real-time budget tracking
- Multi-select and group operations
- Undo/Redo up to ~50 actions per session
- JSON save/load
- PNG export
- Blueprint save/load
- Analysis overlays (turret, radius)

### ⏳ Limitations (Planned for Future)

- No 3D visualization (planned for v2.x)
- Blueprints are session-local (library sync coming)
- No collaborative/multiplayer editing
- Limited preset variety (expanding roadmap)

### 🔧 Known Issues

- Very large projects (2000+ items) may experience slowdowns
- PNG export may require manual scaling for very dense layouts
- Shelter presets assume standard room sizes (customization coming)

See [ROADMAP.md](ROADMAP_NEXT.md) for planned improvements.

---

## Downloads & Release Assets

### v1.0.0 Release Package

| Asset | Description |
|-------|-------------|
| **FO76CampPlanner.exe** | Single-file Windows 10/11 executable |
| **INSTALL_WINDOWS.md** | Installation and setup guide |
| **RELEASE_NOTES_v1.md** | v1.0.0 highlights and changes |
| **README.md** | This file |
| **CHANGELOG.md** | Full version history |

**System Requirements**:
- Windows 10 or later (64-bit)
- ~50 MB free disk space
- No external dependencies – fully self-contained

---

## Troubleshooting

### The app won't start

- **Ensure Windows Defender or antivirus allows it**: First run may trigger security warnings
- **Check Windows version**: Must be Windows 10 or later (64-bit)
- **Check for error log**: Look in `%AppData%\FO76CampPlanner_Error.log`

### Slow performance with large projects

- Disable unnecessary layer visibility (toggle Structure, Utility, etc.)
- Disable analysis overlays (turret arcs, CAMP radius)
- If freezing persists, reduce project complexity

### Projects won't save

- Verify folder write permissions
- Try saving to Desktop or Documents
- Check available disk space

### Can't place items

- Verify the correct tool is selected
- Check rule profile (Strict vs. Relaxed vs. Shelter)
- Ensure no layer lock is preventing placement

For more support, see [Issues](https://github.com/knoksen/Fallout76CAMPplanner/issues) on GitHub.

---

## Roadmap & Future Milestones

### v1.1 – Enhanced Planning Engine (Q2 2026)
- Quick-duplicate for selected zones
- Improved group rotation around pivot
- Blueprint library per project and per CAMP slot
- Snap toggle on/off

### v1.2 – Fallout 76 Logic Depth
- Visitor flow and ingress/egress lines
- Trap logic zones and lure-path overlays
- Budget profiles (fast travel hub, nuke camp, vendor camp, stealth camp)

### v2.0 – Professional Tooling
- 3D visualization (preview in-game perspective)
- Print-ready PNG with legend and project info
- Darker Fallout-inspired UI skin
- Colour coding per layer and function

### v3.0 – Complete Edition
- Shelter templates (Vault Lobby, Utility, Missile Silo, Test Bunker, etc.)
- 4-slot CAMP project manager
- Advanced blueprint variants and generators
- Plan/section/zone visualization for trap logic and defense

See [ROADMAP_NEXT.md](ROADMAP_NEXT.md) for full details.

---

## License

Licensed under the **MIT License**. See [LICENSE.md](LICENSE.md) for details.

**Copyright © 2026 Knoksen**

---

## Contributing & Support

### Report Bugs
File an issue on [GitHub Issues](https://github.com/knoksen/Fallout76CAMPplanner/issues).

### Feature Requests
Vote on or propose new features in [Discussions](https://github.com/knoksen/Fallout76CAMPplanner/discussions).

### Build from Source
See [BUILD.md](BUILD.md) for developer setup instructions.

---

## Credits

**Created by**: Knoksen  
**Built with**: .NET 8, Windows Forms  
**Fallout 76** is a trademark of Bethesda Softworks LLC. Not affiliated with Bethesda.

---

## Contact

- **GitHub**: [knoksen/Fallout76CAMPplanner](https://github.com/knoksen/Fallout76CAMPplanner)
- **Issues & Support**: [GitHub Issues](https://github.com/knoksen/Fallout76CAMPplanner/issues)

---

**Enjoy planning your CAMP! 🏗️**
