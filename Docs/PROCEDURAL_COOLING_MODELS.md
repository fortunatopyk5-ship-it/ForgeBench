# ForgeBench procedural cooling models

Status: implemented as an original runtime visual fallback on `art/visual-production-plan-20260915`.

## Why this exists

The art audit found useful third-party donors, but no fully cleared, mechanically consistent AIO/fan set for every ForgeBench cooling family. Rather than block the game on marketplace assets, ForgeBench now generates its own brand-neutral cooling geometry at runtime. No third-party mesh or texture is copied into the repository.

The system is presentation-only: `MachineState`, inventory, compatibility, sockets, save data and interaction colliders remain authoritative. The adapter hides only the primitive renderer from the current production assembly view and leaves gameplay objects intact.

## Implemented geometry

- 120 mm and 140 mm case-fan families.
- Square frame rails, circular shroud, corner bosses and hub.
- Nine swept, volumetric fan blades generated from one reusable mesh.
- Optional emissive RGB ring driven by the machine customization color.
- Air-cooler tower with a single combined fin-stack mesh, four heatpipes, base plate and fan.
- AIO 240 / 280 / 360 visual families.
- Pump/block with cold plate and cap.
- Radiator perimeter frame plus one combined fin-pack mesh.
- Two 120 mm fans for 240, two 140 mm fans for 280 and three 120 mm fans for 360.
- Two smooth Bezier hoses, each generated as one tube mesh rather than dozens of physics segments.
- Generic GPU fan upgrade using the same original fan family.

## Catalog correction rules

Art does not rewrite gameplay data. The visual resolver handles two known catalog-normalization conflicts locally:

1. `rad240` / `240` and `rad280` / `280` naming wins over a fallback `radiatorSupportMm` value that may have been normalized to 360.
2. `140mm` / `140` naming wins over the historical 120 mm fan fallback.

These rules are covered by EditMode source tests in `ProceduralCoolingVisualTests.cs`.

## Performance design

The generator avoids cloth physics and per-fin GameObjects. Radiator fins and tower fins are combined meshes; AIO hoses are low-sided continuous tube meshes; fan blades share one cached mesh; gameplay colliders remain the pre-existing coarse colliders. This is intended as a substantially better visual baseline while retaining an Android-friendly object count.

No claim is made that the current budgets are final. Unity compilation, RenderDoc/Profiler measurements, Android draw calls, thermal throttling behavior and device GPU time still require real execution.

## External reference search

During the follow-up search, downloadable CC Attribution fan references were found on Sketchfab, including a ~3.1k-triangle simple fan and a ~4.6k-triangle low-poly animated fan. They are useful reference/donor candidates, but they are not imported here. Any external asset must keep its license/attribution trail and pass mechanical/scale review before replacing the original fallback.

## Replacement path

Authored production prefabs can later replace these models through the visual-profile/prefab resolver. Until then, the procedural fallback gives every supported cooling family a consistent, original, editable visual representation instead of waiting on uncertain external assets.
