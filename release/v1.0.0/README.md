# Knoksen FO76 CAMP Planner - Windows 10 Planner MVP++

Dette er et viderebygget fundament for et Fallout 76 CAMP-planleggingsverktøy for Windows 10.

## Hva appen gjør nå

- 2D grid-basert CAMP-planlegging
- Verktøy for foundation, wall, door, stairs, roof, workbench, turret, power, light, decor, vendor, resource, display og ally
- CAMP- og Shelter-modus
- Regelprofiler: Strict, Relaxed, Shelter
- Budsjettmåler med placed/stored budget
- Lagre og åpne prosjekter som JSON
- Eksport til PNG
- **Undo / Redo**
- **Layer toggles** for Structure, Utility, Defense, Power, Aesthetic og Commerce
- **Shelter- og surface-presets**
- **Smartere snapping**
  - wall og door snapper mot **foundation-kanter**
  - roof snapper mot **foundation-felt**
  - stairs snapper mot **åpne foundation-kanter**
- **Multi-select**
  - Ctrl-klikk for å bygge utvalg
  - drag-boks for marquee selection
  - gruppedrag og gruppesletting
- **Blueprint-moduler**
  - lagre valgt utvalg som `.blueprint.json`
  - laste blueprint-filer
  - lime inn ferdig modul på canvas
- **Automatiske Shelter-restriksjoner** for kategorier som Vendor, Resource, Display og Ally
- **Approximate analysis overlays**
  - surface CAMP radius overlay
  - turret coverage arcs
- **Layer lock** i tillegg til layer hide


## Nytt i UI/UX-versjonen (v5)

- **Tab-basert høyreside** i stedet for én lang kontrollkolonne
  - Overview
  - Build
  - Library
  - Inspect
- Tydeligere toppheader med prosjektstatus
- Forbedret mørk UI med sterkere kontrast og bedre hierarki
- **Søk og lagfilter** i listen over plasserte items
- Egen visualisering av item-kort i listen med fargeswatch og lock-badge
- **Live placement preview** på canvas
- **Hover-koordinater og snarveishint** direkte i canvas-visningen

## Analysemodus

Denne versjonen har også et enkelt analyse-lag for planlegging, ikke som en eksakt simulator:

- **Approx surface CAMP radius** viser en omtrentlig bygge-/kontrollsone for surface planlegging
- **Approx turret coverage arcs** viser en omtrentlig forsvarskjegle basert på turret-rotasjon
- **Layer lock** lar deg fryse hele lag mens du jobber videre på andre systemer

## Viktig om Fallout 76-logikk

Denne versjonen er fortsatt et planleggingsverktøy, ikke en full 1:1 simulering av alle skjulte plasseringstester i Fallout 76.

Det bevisste regelsettet er:

- **Strict:** ingen overlap utenom foundation-underlag
- **Relaxed:** overlap tillatt for mykere lag som Power og Decor
- **Shelter:** mer kreativ stacking, men fortsatt kontrollert

I tillegg er noen kategorier bevisst blokkert i Shelter-modus for å ligge nærmere faktisk bruk:

- Vendor
- Resource
- Display
- Ally

## Presets som følger med

- Custom Surface CAMP
- Vault Lobby Shelter
- Vault Utility Room
- Missile Silo Shelter
- Nuclear Test Bunker
- The Flatlands Shelter
- Triumph Terrace
- Wrangler Casino
- Nuke Surface CAMP

## Eksempelfiler

Prosjekter:
- `sample-foundation-layout.json`
- `sample-missile-silo-layout.json`
- `sample-defense-layout.json`

Blueprints:
- `sample-blueprints/foundation-ring.blueprint.json`
- `sample-blueprints/nuke-lane.blueprint.json`

## Tastatursnarveier

- `R` = roter valgt objekt / valgt gruppe
- `Delete` = slett valgt objekt / valgt gruppe
- `Ctrl+Z` = undo
- `Ctrl+Y` = redo
- `+ / -` = zoom
- piltaster = flytt valgt objekt / gruppe én celle
- `Ctrl+klikk` = legg til / fjern fra utvalg

## Bygg på Windows 10 til én .exe

Forutsetning: .NET 8 SDK installert.

### PowerShell

```powershell
./build-win10-singlefile.ps1
```

### CMD

```bat
build-win10-singlefile.bat
```

Publisert fil havner normalt i:

```text
bin\Release\net8.0-windows\win-x64\publish\FO76CampPlanner.exe
```

## Viktig om `.exe`

Prosjektet er satt opp for single-file Windows-publisering, men selve `.exe` må bygges på en Windows-maskin med .NET SDK.

## Anbefalt neste steg

1. Hurtigduplisering av valgt modul
2. Visitor flow / ingress-egress-linjer
3. Trap logic-zoner og lure-path overlays
4. Print-ready legend og prosjektkort
5. Blueprint-bibliotek per prosjekt og per CAMP-slot
6. Shelter-spesifikke templates med ferdige romsekvenser
7. Mini-map / oversiktskart
8. Print-/share-ready prosjektpresentasjon

## Start raskt

1. Åpne `sample-foundation-layout.json` eller `sample-missile-silo-layout.json`
2. Test `Ctrl+klikk` og drag-boks i Select-modus
3. Marker et lite oppsett og bruk **Save selection as blueprint**
4. Last blueprint igjen og bruk **Paste loaded blueprint**


## v7 UI/UX and workflow improvements

This round focuses on making the planner feel more like a real design workstation:

- minimap overview with selected-item highlighting, CAMP center marker and hover cell feedback
- dynamic quick-start guidance that recommends the next best workflow step
- contextual inspector for single-item note, X/Y and rotation edits
- explicit duplicate / delete actions in the toolbar and inspector
- fast set-CAMP-center action from current selection
- stronger workflow support for layout → envelope → systems → defense → polish
