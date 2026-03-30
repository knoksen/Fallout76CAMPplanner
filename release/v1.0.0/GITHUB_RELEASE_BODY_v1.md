# FO76 CAMP Planner v1.0.0

## 🎉 Official Release v1.0.0 – Production Ready!

This is the first official public release of **FO76 CAMP Planner** – a professional tool for designing Fallout 76 CAMPs and Shelters. After months of development and refinement, the planner is production-ready with hundreds of features to streamline your building workflow.

### ✨ What's New

**FO76 CAMP Planner v1.0.0** delivers a complete, feature-rich MVP:

#### Grid-Based Design
- 2D precision grid for CAMP and Shelter layouts
- 14 building categories: Foundation, Wall, Door, Stairs, Roof, Workbench, Turret, Power, Light, Decor, Vendor, Resource, Display, Ally
- Dual mode support: Surface CAMP and Interior Shelter

#### Smart Placement & Validation
- **Foundation-aware snapping**: Walls snap to edges, roofs snap to tiles, stairs snap to open edges
- **Rule profiles**: Strict (no overlap), Relaxed (soft overlaps), Shelter (generous stacking)
- **Layer lock** to freeze completed sections

#### Multi-Select & Editing
- Multi-select via `Ctrl+Click` or drag-box selection
- Group move, duplicate, and delete
- Contextual Inspector for notes, X/Y, rotation
- Keyboard shortcuts (R=Rotate, Delete, Ctrl+Z/Y, etc.)

#### Reusable Blueprints
- Save selections as `.blueprint.json` modules
- Load and paste blueprints into projects
- Build libraries of reusable components

#### Analysis & Visualization
- Approximate surface CAMP radius overlay
- Turret coverage arc visualization
- Minimap panel with selected-item highlighting
- Layer visibility toggles (Structure, Utility, Defense, Power, Aesthetic, Commerce)
- Focus presets (All, Structure, Systems, Defense, Presentation)

#### Workflow Support
- **Build stages guide**: Layout → Envelope → Systems → Defense → Polish
- **Dynamic quick-start recommendations**
- **Workflow status dashboard** showing progress

#### Data Management
- Save/load projects as JSON
- Export to PNG for sharing
- 9 built-in presets (Vault, Bunker, Casino, etc.)
- 3 sample projects and 2 sample blueprints included

#### Professional Polish
- Dark, high-contrast UI
- Undo/Redo (up to ~50 actions)
- Live placement preview
- Hover coordinates and shortcut hints

---

## 🎯 Quick Start

1. **Download** `FO76CampPlanner.exe` from this release
2. **Run** the executable (no installation required)
3. **Open** a sample project or start creating
4. See **[INSTALL_WINDOWS.md](https://github.com/knoksen/Fallout76CAMPplanner/blob/main/Knoksen_FO76_CAMP_Planner_Win10/INSTALL_WINDOWS.md)** for detailed instructions

---

## 📋 System Requirements

- **OS**: Windows 10 Build 1909+ or Windows 11 (64-bit)
- **RAM**: 4 GB minimum, 8 GB recommended
- **Storage**: ~50 MB free space
- **GPU**: Integrated graphics sufficient
- **Runtime**: Bundled (no separate .NET installation needed)

---

## 📦 Release Assets

| Asset | Description |
|-------|-------------|
| **FO76CampPlanner.exe** | Single-file, self-contained Windows executable (~70 MB) |
| **INSTALL_WINDOWS.md** | Installation and setup guide |
| **RELEASE_NOTES_v1.md** | Detailed v1.0.0 highlights |
| **README_v1_RELEASE.md** | Full feature documentation |
| **CHANGELOG.md** | Complete version history |
| **BUILD.md** | Developer build instructions |

---

## 🔧 Known Limitations

- No 3D visualization (coming in v2.0)
- Blueprints are session-local (library sync planned for v1.1)
- Large projects (2000+ items) may slow down UI
- PNG export may require scaling for very dense layouts

See **[Roadmap](ROADMAP_NEXT.md)** for planned improvements.

---

## 📚 Documentation

- **[README](https://github.com/knoksen/Fallout76CAMPplanner/blob/main/Knoksen_FO76_CAMP_Planner_Win10/README_v1_RELEASE.md)** – Full overview and features
- **[Installation Guide](https://github.com/knoksen/Fallout76CAMPplanner/blob/main/Knoksen_FO76_CAMP_Planner_Win10/INSTALL_WINDOWS.md)** – Setup instructions
- **[Release Notes](https://github.com/knoksen/Fallout76CAMPplanner/blob/main/Knoksen_FO76_CAMP_Planner_Win10/RELEASE_NOTES_v1.md)** – What's new
- **[Changelog](https://github.com/knoksen/Fallout76CAMPplanner/blob/main/Knoksen_FO76_CAMP_Planner_Win10/CHANGELOG.md)** – Version history
- **[Build Guide](https://github.com/knoksen/Fallout76CAMPplanner/blob/main/Knoksen_FO76_CAMP_Planner_Win10/BUILD.md)** – For developers

---

## ⌨️ Keyboard Shortcuts

| Action | Shortcut |
|--------|----------|
| Rotate | `R` |
| Delete | `Delete` |
| Undo | `Ctrl+Z` |
| Redo | `Ctrl+Y` |
| Zoom in/out | `+` / `-` |
| Move (1 cell) | Arrow keys |
| Multi-select | `Ctrl+Click` |
| Marquee select | Click & drag |
| Save | `Ctrl+S` |

---

## 🐛 Reporting Issues

Found a bug or have a feature request? Open an [issue](https://github.com/knoksen/Fallout76CAMPplanner/issues) on GitHub.

---

## 🗺️ Roadmap

### v1.1 – Planning Engine (Q2 2026)
- Quick-duplicate zones
- Blueprint library per CAMP slot
- Snap toggle

### v1.2 – Fallout 76 Logic (Q3 2026)
- Visitor flow visualization
- Trap logic zones
- Budget profiles per playstyle

### v2.0 – 3D & Professional Tooling (Q4 2026)
- 3D preview
- Print-ready PDF exports
- Darker Fallout-inspired UI

---

## 🙏 Credits

**Created by**: Knoksen  
**Built with**: .NET 8, Windows Forms  
**License**: MIT

Fallout 76 is © Bethesda Softworks LLC. This tool is a fan-made utility not affiliated with Bethesda.

---

## 📝 License

FO76 CAMP Planner is licensed under the **MIT License** – free for personal and commercial use.

See [LICENSE.md](LICENSE.md) for details.

---

**Enjoy planning your CAMP! 🏗️**

**Download v1.0.0 now and start designing!**
