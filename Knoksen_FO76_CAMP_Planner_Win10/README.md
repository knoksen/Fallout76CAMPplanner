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
  - lagre og laste blueprint-moduler per aktiv CAMP-slot (Slot1–Slot4)
- **Automatiske Shelter-restriksjoner** for kategorier som Vendor, Resource, Display og Ally
- **Approximate analysis overlays**
  - surface CAMP radius overlay
  - turret coverage arcs
  - visitor flow overlay (fra CAMP center til nøkkelmål)
  - trap zone overlay (defense/trap-markerte områder)
  - persisted ingress / checkpoint / egress markers for ruteplanlegging
  - ordered route-path editor with editable marker labels og sekvens
  - structured trap zones med severity, bounds og egne review-notater
- **Layer lock** i tillegg til layer hide
- **Device Hub / Quick Launch Center**
  - plattformgrupper for PC, Mobile Companion og Console
  - store knapper for hurtigåpning av prosjekt-/release-mapper, GitHub, SourceForge og docs
  - mobil-workflow for kompakt prosjektsammendrag, snapshot-pack (PNG + summary) og QR-ready linkfil
  - konsollsnarveier (Xbox/PlayStation/Generic) med konfigurerbare mål/lenker/notater
  - deaktiverte handlinger med forklarende tooltip ved manglende target
- **Snap mode (Strict / Relaxed / Off)**
  - velg snap-opplevelse direkte i Rules-seksjonen
  - hurtigsyklus i verktøylinjen (`Snap`)
- **Budget profiles per playstyle**
  - Builder
  - Trap CAMP
  - Vendor CAMP
  - Utility CAMP
  - Nuke CAMP
  - Showcase CAMP


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
- **Visitor flow** viser foreslåtte hovedlinjer fra CAMP center til funksjonelle mål som dører, workbench og vendor
- **Trap zones** viser risiko-/forsvarssoner rundt trap-/defense-elementer
- **Defense analysis summary** viser en omtrentlig score (`0-100`), ingress coverage (`covered/total`) og konkrete risikohint for svake vinkler/udekkede ruter
- **Scenario compare (A/B)** lar deg lagre baseline i Analysis-panelet og sammenligne nåværende plan mot baseline (score, budget og ingress coverage)
- **Route planning editor** lar deg:
  - bygge en rekkefølge av ingress → checkpoints → egress
  - gi hvert punkt eget navn og justere rekkefølgen direkte i UI
  - flytte markører direkte på canvas med drag-and-drop
  - lagre forsvars-/trafikkvurderinger i prosjektfilen
- **Trap planning editor** lar deg:
  - opprette soner fra valgt område
  - flytte soner direkte på canvas
  - resize soner med hjørnehåndtak på canvas
  - sette severity (`Low`, `Medium`, `High`, `Critical`)
  - lagre egne review-notater per sone

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

## Device Hub / Quick Launch Center

Device Hub er en praktisk launch- og companion-seksjon i WinForms-appen, ikke en egen app.

Støttede førsteversjonshandlinger:
- PC Tools
  - Open project folder
  - Open release folder
  - Launch `FO76CampPlanner.exe` (hvis publish-fil finnes)
  - Open GitHub repository
  - Open SourceForge page (hvis konfigurert)
  - Open Fallout docs/resources (URL eller lokal filsti)
- Mobile Companion
  - Open mobile export folder
  - Export project summary (mobilvennlig tekst)
  - Generate snapshot pack (PNG + summary)
  - Open compact presentation view
  - Generate QR-ready links file (for videre QR-generering)
- Console Shortcuts
  - Xbox
  - PlayStation
  - Generic Console

Konfigurasjon og lagring:
- Launch targets lagres per prosjekt i JSON (`deviceHub` i `PlannerProject`).
- Targets kan være URL, lokal filsti eller placeholder for fremtidig integrasjon.
- Konsollseksjonen er designet som konfigurerbar handlingspanel siden direkte launch til konsoll ikke alltid er mulig fra desktop.

Begrensninger i v1:
- Ingen direkte innebygd QR-renderer; appen genererer QR-ready linkfil.
- Direkte konsoll-launch er avhengig av eksterne mål/URL-er som brukeren konfigurerer.

Videre utvidelsessti:
1. Egen innstilling for flere profiler (streaming/testing/release).
2. QR-bildegenerering direkte i appen.
3. Mer avansert deling (pakker med metadata + versjonshistorikk).

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

## v1.1/v1.2 progression (in progress)

- Quick-duplicate zones er nå tilgjengelig i både toolbar og quick actions (`Dup ←/→/↑/↓`)
- Trap zones kan også hurtigdupliseres retningsstyrt (`Dup zone ←/→/↑/↓`) i route/trap-seksjonen
- Blueprint library per CAMP-slot er funksjonell for in-memory moduler i prosjektet
- Snap mode finnes både i Rules-seksjonen og som toolbar-knapp (`Strict → Relaxed → Off`)
- Visitor flow og trap zones har egne analyse-overlays med toggles
- Visitor flow kan nå planlegges med egne ingress-, checkpoint- og egress-markører som lagres i prosjektet
- Trap zone tagging finnes i Inspector quick actions
- Inspector støtter nå nudge-kontroller for presis flytting av valgt element
- Placement preview viser tydeligere årsak i footer når plassering blokkeres
- Multi-select rotasjon bruker nå gruppepivot for mer forutsigbar modulspinning
- Overlay review presets gjør det raskere å bytte mellom visitor flow-, trap- og defense-gjennomganger
- Budget profiles per playstyle er koblet direkte til budsjettstyring i UI
- Defense analysis bruker nå profile-aware mål for turrets/defense/ingress/trap-zones
- Route-panel viser tydelig ingress/checkpoint/egress-komposisjon og markører i high/critical trap-zones
- Ved mode-bytte håndheves Shelter vs Surface tydeligere (rule profile, overlay, og inkompatible objekter)
- Visitor-flow visualisering markerer ingress med coverage-status og farger rutesegmenter etter trap-severity
- Release tooling inkluderer nå manifest-generator med SHA256 og filstørrelser (`GENERATE_RELEASE_MANIFEST.ps1`)
- Shelter presets håndhever nå konkrete caps for turrets, route markers, trap zones og maksimal trap severity
- Route/Trap-panelet viser nå shelter-advisory med cap headroom og fallback-anbefalinger før caps nås
- Route/Trap-actions soft-disables nå ved shelter-cap (marker/zone-cap og severity-cap) for å redusere avviste handlinger
- Deaktiverte Route/Trap-actions viser nå forklarende tooltip med konkret shelter-begrensning
- Route/Trap-panelet viser nå en inline cap meter med live bars (`Markers`, `Zones`, `Max severity`) og tydelig warn/error-farge ved cap-pressure


## v7 UI/UX and workflow improvements

This round focuses on making the planner feel more like a real design workstation:

- minimap overview with selected-item highlighting, CAMP center marker and hover cell feedback
- dynamic quick-start guidance that recommends the next best workflow step
- contextual inspector for single-item note, X/Y and rotation edits
- explicit duplicate / delete actions in the toolbar and inspector
- fast set-CAMP-center action from current selection
- stronger workflow support for layout → envelope → systems → defense → polish
