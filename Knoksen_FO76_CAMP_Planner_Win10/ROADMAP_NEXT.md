# Roadmap - neste byggetrinn

## Fase 1.3 - sterkere planmotor
- Hurtigduplisering av utvalgte soner
- Bedre gruppespinning rundt valgt pivot
- Valgfri snap-av/på-toggle
- Blueprint-bibliotek per prosjekt og per CAMP-slot

Status (2026-03-30):
- Hurtigduplisering av soner: ferdig.
- Bedre gruppespinning rundt pivot: ferdig (multi-select roterer rundt gruppepivot med validering/rollback).
- Snap-kontroll: ferdig som `Strict/Relaxed/Off`.
- Blueprint-bibliotek per CAMP-slot: ferdig.

## Fase 1.3b - creator/release tools
- `BUILD_AND_PUBLISH_v10.ps1` wrapper: ferdig.
- `GENERATE_RELEASE_MANIFEST.ps1` for SHA256/size-manifest: ferdig.

## Fase 1.4 - bedre Fallout 76-logikk
- Visitor flow / ingress-egress-linjer
- Trap logic-zoner og lure-path overlays
- Budsjettprofiler for fast travel hub, nuke camp, vendor camp og stealth camp
- Shelter-regler per konkret sheltertype

Status (2026-03-30):
- Visitor flow / ingress-egress-linjer: ferdig.
- Trap logic-zoner og lure-path overlays: ferdig.
- Budsjettprofiler: ferdig.
- Shelter-regler per konkret sheltertype: delvis ferdig (preset-baserte caps for turrets/markers/zones + severity-cap og UI-risikohint).

## Fase 2 - produksjon og design
- Mørkere Fallout-inspirert UI skin
- Mini-map / oversiktskart
- Fargekode per lag og per funksjon
- Symboler/ikoner for turrets, strøm, benches, ingress/egress
- Print-ready PNG med legend og prosjektinfo
- Eksport av prosjektkort per build

## Fase 2.0 - Professional Tooling (safe groundwork)

### Arkitekturvalg (forberedt nå)
- 3D preview-path: behold WinForms-kjerne, eksporter plannerdata til et adapter-lag (`IThreeDPreviewAdapter`) i stedet for å bygge 3D direkte i UI-laget.
- PDF/print-path: bruk eget eksport-adapter (`IPrintExportAdapter`) som tar ferdig `CanvasSnapshot` + metadata og skriver dokument uten å endre planmotor.
- Theme-path: innfør prosjektnivå `ThemeProfile` (lagret i prosjektdata) uten å aktivere runtime-temabytte enda.

### Hva er gjort nå
- Lagt til stabile extension points i kode for fremtidig 3D preview/PDF-export (adapter interfaces + payload/document records).
- Lagt til `ThemeProfile` i prosjektmodell for trygg, bakoverkompatibel lagring.
- Dokumentert beslutning om feature flags for høy-risiko funksjoner (`FutureFeatureFlags` holder alt av).

### Anbefalt implementasjonsrekkefølge etter v1.1/v1.2
1. Ferdigstill v1.x stabilisering og testdekning.
2. Implementer PDF-export først (lavere risiko, ingen runtime-render loop).
3. Implementer theme engine som gradvis refaktor av farger/fonts til palett-objekter.
4. Implementer 3D preview via ekstern adapter (f.eks. sidecar/webview), ikke i MainForm render-loop.

## Fase 3 - full Knoksen edition
- Shelter templates: Vault Lobby, Utility Room, Missile Silo, Test Bunker, Flatlands, Triumph Terrace, Wrangler Casino
- Prosjektbibliotek for 4 CAMP-spor
- Blueprint packs og variantgenerator
- Plan / snitt / soner for trap logic, defense arcs og visitor flow
- “Mystisk/fangende” design-kit med ferdige layoutfamilier
