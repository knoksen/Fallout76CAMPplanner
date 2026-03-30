Knoksen FO76 CAMP Planner – CHANGELOG v7

Dato: 2026-03-29

Denne runden prioriterte reelle UI/UX- og arbeidsflytforbedringer fremfor bare visuelle justeringer.

Nytt:
- Minimap-panel med:
  - oversikt over hele gridet
  - fargede itemmarkører
  - markering av valgt seleksjon
  - CAMP center-kryss
  - hover-grid-preview
- Quick Start / Workflow Pulse:
  - dynamisk status over foundations, structure, systems, defense og polish
  - anbefalt neste steg basert på faktisk prosjektstatus
  - raske handlingsknapper for duplicate, delete, set CAMP center og paste blueprint
- Contextual Inspector:
  - rediger note
  - rediger X/Y-posisjon
  - rediger rotasjon
  - ett-klikk “Apply inspector changes”
- Quick Actions i Inspect:
  - rotate selection
  - duplicate selection
  - delete selection
  - set CAMP center from selection
- Verktøylinje:
  - egne Duplicate- og Delete-knapper
- PlannerCanvas:
  - public DuplicateSelection()
  - public DeleteSelection()
  - public SetCampCenterFromSelection()
  - public UpdateSingleSelectedItem(...)
  - HoverGridPoint eksponert for UI/minimap

Effekt:
- Appen føles mer som et faktisk prosjekteringsverktøy
- Mindre friksjon i iterasjon av moduler og kamp-logikk
- Bedre situasjonsforståelse under planlegging
