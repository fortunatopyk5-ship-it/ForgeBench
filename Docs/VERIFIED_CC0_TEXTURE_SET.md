# ForgeBench — verified CC0 PBR texture set

Research date: 2026-09-15.

Purpose: establish a legally simple, production-usable baseline texture library for the ForgeBench visual pass. This file does **not** mean the textures are already imported into Unity. Every listed source was checked as CC0 at the source site at research time.

## Licensing baseline

Poly Haven states that all assets on the site are licensed CC0 and may be used commercially and redistributed without attribution requirement:
https://polyhaven.com/license

ambientCG states that all assets are released under CC0 and may be used for commercial work:
https://ambientcg.com/

For ForgeBench, prefer Poly Haven/ambientCG for generic surface materials because this avoids storefront ambiguity and makes redistribution in a public repository much safer than most purchased packs.

## Recommended first-wave texture set

| ForgeBench use | Primary source | Why it fits | Unity/Android import target |
|---|---|---|---|
| Warm off-white workshop walls | Poly Haven — White Plaster 02: https://polyhaven.com/a/white_plaster_02 | Clean matte plaster with fine micro-roughness; matches the bright repair-studio direction without warehouse grunge. | 1K for most walls; 2K only for hero wall closeups. Albedo sRGB; normal/roughness/AO linear. |
| ESD mat / dark anti-slip rubber | Poly Haven — Rubber Tiles: https://polyhaven.com/a/rubber_tiles | Dark matte rubber with subtle wear and readable micro-surface. Use as source material, not literal gym tile layout. Remove/soften tile seam influence for the bench ESD mat variant. | 1K. Re-author a seamless ESD-rubber derivative; roughness around 0.72–0.86. |
| Clean wood / laminate bench accents | Poly Haven — Oak Veneer 01: https://polyhaven.com/a/oak_veneer_01 | Clean furniture-grade grain. Useful for selected bench accents, shelves or neutral cabinet internals. | 1K; 2K only on the main workbench if visible at very close range. |
| Pale premium cabinet/desk variant | Poly Haven — Silver Oak Veneer 01: https://polyhaven.com/a/silver_oak_veneer_01 | Newer pale white-oak material; works with warm off-white/graphite palette without turning the room yellow. | 1K. |
| Neutral grey cabinet/desk variant | Poly Haven — Grey Oak Veneer 01: https://polyhaven.com/a/grey_oak_veneer_01 | Subtle blonde-grey veneer for restrained secondary furniture. | 1K. |
| Varnished workbench donor | Poly Haven — Wood Table 001: https://polyhaven.com/a/wood_table_001 | Clean finished furniture surface with useful roughness/normal information. Color should be normalized toward ForgeBench palette rather than copied literally. | 1K; optional 2K hero bench. |
| Plywood drawer/packing insert donor | Poly Haven — Plywood: https://polyhaven.com/a/plywood | Clean engineered wood with predictable linear grain. | 512–1K. |
| Painted steel cabinet donor | Poly Haven — Blue Metal Plate: https://polyhaven.com/a/blue_metal_plate | Painted steel with scratches and abrasion. Good roughness/normal donor, but the blue albedo should be recolored to ForgeBench graphite/teal and wear reduced. | 1K. Do not keep strong original scratches on every cabinet. |
| Dirty/worn workshop-only metal variant | Poly Haven — Metal Plate 02: https://polyhaven.com/a/metal_plate_02 | Useful as a damage/wear mask donor only. Too rusty/grungy for the default clean studio. | 512 mask source or selective decal; not a default base material. |
| Floor reference / wear-mask donor | Poly Haven — Garage Floor: https://polyhaven.com/a/garage_floor | Provides realistic concrete floor response and wear information. Default ForgeBench floor should be cleaner, so use as a roughness/normal/wear source and reduce cracks/stains. | 1K. Keep damage subtle and localized. |
| Cardboard / parcel prop | Poly Haven — Cardboard Box 01: https://polyhaven.com/a/cardboard_box_01 | CC0 model with PBR maps; useful for packaging/receiving station reference and donor material. | Texture maps 512–1K after review. Use original fictional shipping labels. |

## Materials that should NOT rely on generic downloaded textures

The following are better authored as clean procedural/material-library assets instead of searching endlessly for exact texture scans:

- PCB solder mask and traces — create a controlled PCB material plus board-specific trace/mask atlases. Generic PCB photos will not match sockets or gameplay geometry.
- Brushed/anodized aluminium — use a small directional normal/roughness tile and physically correct metallic response; avoid obvious photographed scratches repeated over every part.
- Powder-coated case steel — author a subtle grain normal and parameterized base color/roughness; do not use a visibly rusty sheet as the base.
- ABS/glossy device plastic — small reusable micro-normal plus scalar roughness is sufficient; specific photographed plastic textures often add unwanted baked wear.
- Tempered glass — shader/material response plus real panel thickness; no photographic texture needed except optional extremely subtle smudge/decal masks.
- Copper/nickel/chrome contacts — clean metallic material with tiny micro-scratch normals; connector geometry and reflection quality matter more than a unique albedo.
- Braided cable sleeving — tileable weave normal aligned to cable UV, not a world-space photographic material.
- Thermal paste — authored scalar/normal material, because gameplay requires controlled appearance and coverage states.

## Unity 6 / URP import rules

1. Start from 1K for environment and instruments, 2K only for held/installed hero surfaces.
2. Do not import the original 8K/16K files at full size into the Android build.
3. Albedo/emission: sRGB on. Normal, roughness, AO, metallic/masks: sRGB off.
4. Convert source roughness to URP smoothness (`smoothness = 1 - roughness`).
5. Do not feed a generic ARM texture directly into stock URP Lit. Pack only after defining the shader's channel contract.
6. Enable mipmaps. Disable Read/Write unless runtime modification actually needs it.
7. Use ASTC 6x6 as the normal Android target; move tiny/background atlases toward 8x8. Reserve 4x4 for exceptional hero normal/contact detail after device profiling.
8. Keep high-resolution masters outside the runtime build for rebaking and regrading.
9. Regrade surfaces into the ForgeBench palette instead of letting donor color identity drive the room.
10. Use downloaded grunge mainly as masks/decals. The target studio is organized and maintained, not abandoned.

## Current recommendation

These CC0 sources are enough to build the **first production material pass** for walls, floor, ESD surfaces, wood/laminate, painted steel and shipping props without buying a texture pack. Spend art budget on mechanically accurate 3D hardware, repair tools, hands/animations and specialist device internals instead of generic surface textures.

Still unresolved as external-source searches: mechanically correct CPU coolers/AIOs, keyed PC power/data connectors, modern electronics microscope, modern digital multimeter/bench instruments, repair-ready laptop/tablet/phone/console/NAS/router shells. These are 3D-asset problems, not texture-library problems.
