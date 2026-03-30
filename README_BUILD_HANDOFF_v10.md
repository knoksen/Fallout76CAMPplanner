# Build handoff v10

Dette er en ren build-handoff-runde for Windows / VS Code / Blackbox AI.

## Lagt til i v10
- `.vscode/tasks.json` for clean / restore / build / publish / run
- `BUILD_AND_PUBLISH_v10.ps1` med logging til `build_logs/`
- `BUILD_AND_PUBLISH_v10.bat`
- `BLACKBOX_DIRECT_PROMPT_v10.md`
- `BLACKBOX_QUICK_COMMANDS_v10.md`

## Merk
Denne pakken er gjort mer byggklar, men er fortsatt **ikke lokalt kompilert i Linux-miljøet** der den ble forberedt.
Selve EXE-en må fortsatt bygges på Windows-maskinen din.
