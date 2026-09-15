# ForgeBench — art asset and interaction production plan

Research date: 2026-09-15. Target: Unity 6000.3.15f1, URP 17.3.0, Android ARM64/IL2CPP, landscape.

## Decision and delivery status

Choose **a modern realistic repair studio: “Precision, made tangible.”** Warm off-white architectural surfaces, graphite cabinets, brushed aluminium instruments, desaturated teal ESD mats, restrained amber status indicators. The bench is bright enough to read a black connector against a black PCB. This is an original working electronics studio, not a warehouse, showroom, spacecraft, or imitation of another game's art.

| Status | This delivery |
|---|---|
| IMPLEMENTED | Repository inspection; this production brief; explicit authoring profiles for all 79 catalog definitions; an offline profile validator |
| RECOMMENDED | Hybrid hands; original modular hardware; selective purchased prop donors; baked room lighting; Bezier tube cables; gameplay-authoritative presentation adapters |
| REQUIRES ASSET | All replacement meshes, PBR textures, rigged hands and authored audio; no external assets were downloaded or introduced |
| REQUIRES ARTIST | Mechanical fit, retopology, UVs, material normalization, fictional branding, rig cleanup, repair grips and interaction clips |
| REQUIRES DEVICE TEST | Every performance number below; Unity import, rendering, IL2CPP compilation and Android runtime behavior have not been tested here |

There are useful verified listings, but **this is not a fully cleared shopping cart with two suitable assets for every requested category**. Some storefronts expose neither their selected license nor technical data; several specialist categories have no verified suitable pair. Those gaps are named below rather than filled with invented products. “Best” means the strongest candidate found for the stated role, not an exhaustive marketplace ranking. None of these listings proves premium in-game quality by itself.

## 1. Existing project audit

Inspected source at `main` commit `5e5e1be1b08a7825dc3c228676a3c8f611093590`. `integration/physical-assembly-v3` points to the same commit. Also checked the tree of `integration/physical-assembly-review` at `65d128fce6ebc63f436a1af58d80de61b547ba6b`. Source was fetched through GitHub into a local review snapshot; this was not a complete Unity checkout or a Unity Editor session.

**Branch mismatch:** none of these trees contains `PhysicalMachineBinder`, `InstallationSocket`, `FastenerPoint`, or `CableEndpoint`. The current code uses `AssemblySnapPoint`, `PickupPartProxy`, `FastenerState`, and aggregate cable flags. Do not write adapters against the missing classes until their actual branch is available. The endpoint proposal below includes a route for the inspected implementation.

| Existing file/system | Observed behavior | Art integration consequence |
|---|---|---|
| `Assets/Scripts/Production/PhysicalAssemblyController.cs` | Creates cubes/cylinders and new materials for installed hardware; rebuild destroys the entire visual root. Hardcoded DIMM/storage/fan layouts; GPUs share a triple-fan silhouette. Straight boxes represent cables. | A shared prefab resolver should replace visual children incrementally. Stable interaction roots must outlive clips and state refreshes. Preserve existing commands. |
| Same controller, fasteners | Visual screw callbacks invoke global `LoosenFastener()` / `TightenFastener()`, which operate on the next eligible fastener. | Do not animate the clicked screw as if it were necessarily the one changed. Resolve the actual next fastener or wait for an explicit targeted command API. |
| Same controller, motion/materials | Constant `SpinVisual` rotations include a PSU grille. “Glass” is an opaque material without a transparent surface setup. | Separate stationary frames/grilles from rotors; drive motion from machine state. Author real glass material and thickness. |
| `ObjectHandlingController.cs` | Camera-follow pickup, rotation, category/radius snap search, compatibility validation, then `game.Install(instanceId)`; checks reservation to determine success. Tray uses separate miniature procedural visuals. | Installed and held forms need the same visual profile and consistent scale; preserve pickup root, home pose, collider and snap semantics. |
| `WorkshopWorld.cs` | Procedural room, benches and older `ActiveMachine3D` representation. The production renderer hides the old machine. | Preserve station layout and actions. Eventually stop constructing duplicate hidden machine geometry; do not merely pile art on top. |
| `WorkshopAtmosphereDirector.cs` | Adds procedural fittings, task lights, animated fans and runtime reflection probes. Mobile uses one low-resolution scripted probe render. | Author room geometry before baking. A runtime-generated room cannot simply receive an editor light bake that never contained it. |
| `DistanceDetailCuller.cs` | Periodic renderer discovery and name-based `Renderer.enabled` changes. | Exclude art-managed LOD hierarchies explicitly; otherwise it can re-enable hidden renderers or fight `LODGroup`. Never cull gameplay markers/colliders with decorative detail. |
| `RuntimeQualityController.cs` | Desktop quality controller; disables itself on mobile. | Extend the existing quality path, not a third competing governor. |
| `MobilePlatformController.cs` | Adaptive four-tier quality, FPS/battery/low-memory handling and autosave on focus/pause. Sets legacy `QualitySettings` values. | Map its tiers to real URP assets/renderers. `pixelLightCount` is not a sufficient URP light budget implementation. Cancel/reconcile presentation before pause saves. |
| `ProductionAudio.cs` | Synthesized workshop/PC hum. | Retain ambience ownership; add local authored connector, screw, latch and tool transients through existing feedback paths. |
| `SpecialistDeviceVisuals.cs` | Rebuilds portable devices, repair PCB, NAS/server/router and liquid-loop visuals from persisted specialist state. Portable types share coarse primitives; network devices share a chassis. | Extend this renderer by `DeviceCategory`; preserve `Portable*`, `Board*`, `Network*`, `Liquid*` commands. Animate visual children, not a second repair simulation. |
| `Assets/Scripts/Runtime/GameRuntime.cs` | `Install`/`Remove` validate and mutate state, autosave/refresh; install returns void. Cable connection is an aggregate operation. | Presentation must revalidate at commit and verify command postconditions. Do not assume a void call succeeded. |
| `Assets/Scripts/Core/DomainModels.cs`, `HardwareCatalog.cs` | Separate hardware and device categories; catalog normalizes absent extended specifications at load. | Read normalized definitions in Unity. Do not infer actual dimensions from every field containing “Mm”. |

### Catalog facts that affect art

There are 79 definitions: 5 cases, 6 motherboards, 8 CPUs, 6 RAM, 6 GPUs, 7 storage, 6 PSUs, 8 coolers, 5 fans, 3 network cards, 3 batteries, 3 displays, 3 controller modules, 5 consumables and 5 tools. Laptop/phone/tablet/console/NAS/server/router are **device categories**, not 3 copies each in this hardware file. Current boards are ATX/mATX; ITX is a future base mesh, not an existing catalog board.

Resolve these specification issues with the gameplay owner before signing off exact-fit art:

- `case.lengthMm` and `heightMm` represent component clearance constraints, not a measured case exterior bounding box. Current visual case/board scales are substantially enlarged compared with physical PC dimensions. Preserve interaction tolerances until a coordinated scale migration is approved.
- With absent explicit values, cooler normalization assigns all three current AIOs 360 mm because their performance exceeds the threshold, despite `rad240` / `rad280` labels. AIO fan size defaults to 120 mm even for the named 280.
- Fan normalization defaults to 120 mm, including the two definitions tagged `140mm`.
- Actual DIMM/M.2/PCIe/header counts come from normalized specs; don't keep rendering a fixed decorative layout. RAM slot indices must match placement.

`Docs/hardware_visual_profiles.json` records **art intent** and review blockers separately. It does not silently override catalog mechanics. All profiles remain `requires_asset`; none is a fabricated prefab binding.

## 2. Asset procurement register

Prices are observed USD excluding tax where the page exposes them; unknown is not free. Technical descriptions are seller claims, not measurements from downloaded files. **U = not disclosed or not verified**. In every row, missing LOD/collider/animation information remains U, not “included.” Native source means editable DCC source, not merely FBX. No purchased source belongs in this public repository unless the specific license permits that redistribution.

### Hardware and repair donors

| ID / exact asset and direct source | Price, license, commercial game status | Geometry / textures / maps | Rig, animation, LOD, colliders, source | Unity/mobile assessment |
|---|---|---|---|---|
| H1 [Computer Parts ( Built PC )](https://sketchfab.com/3d-models/computer-parts-built-pc-bd6fb0ed93f3475b890a099fedecf351) — Daniel Cardona, Sketchfab | Price U; **LICENSE MUST BE VERIFIED**; commercial use U. Historical listing, current acquisition unconfirmed. | Indexed 574.6k tris for the collection. CPU, M.2, 2.5 SSD, RAM, GPU, PSU, cooling and 120/140 fans. Radiator described as 260 mm. Texture size/maps U. | Rig/animation/LOD/colliders/source U. | Detailed donor lead only; no Unity6 proof; needs significant optimization and mechanical review. Do not treat 260 as 240/280. |
| H2 [Computer Components](https://sketchfab.com/3d-models/computer-components-4daeb72925d140809bdfd51634a1908e) — crimsonfalcon, Sketchfab | Indexed free, CC Attribution; exact version/download terms **LICENSE MUST BE VERIFIED**. Commercial use conditional on verified CC-BY and attribution. | Indexed 36.8k tris total; Blender Internal, textured version referenced on BlendSwap. Resolution/PBR U. | Blender origin; current downloadable source/rig/animation/LOD/colliders U. | Low-cost donor lead; older rendering materials need PBR reauthoring. Exact per-part inventory not verified. |
| H3 [Modern PC Gaming Motherboard PBR](https://www.renderhub.com/locus-models/modern-pc-gaming-motherboard) — Locus Models, RenderHub | Current price U (conflicting indexed prices); **LICENSE MUST BE VERIFIED**, including commercial game use. | PBR advertised; triangles/resolution/map list U. | Rig/animation/LOD/colliders/native source U. | Quality motherboard candidate, not proven modular. Check socket frame, latches, underside and rear IO before purchase. |
| H4 [Motherboard Chips and Components](https://www.cgtrader.com/3d-models/electronics/computer/computer-chips-circuit-motherboard) — colonelduck, CGTrader | $12 observed; Royalty Free (no AI) listing. Embedded game use subject to [CGTrader terms](https://www.cgtrader.com/pages/terms-and-conditions); source redistribution not assumed. | Triangles/resolution U. V-Ray native materials; exchange files use standard materials; PBR map set U. | MA/MB source, OBJ/FBX/STL/VRSCENE listed. Rig/animation/LOD/colliders U. | Useful IC/detail donor after material baking, not a finished motherboard or mobile-ready guarantee. |
| H5 [Low Poly Computer Parts 01](https://www.fab.com/listings/dfcebc27-4afe-4bba-9936-2bbe56596dbf) — Reckera Studios, Fab | Price U; **LICENSE MUST BE VERIFIED**; commercial use U. | Cases, board, CPU, GPU, fans, RAM, PSU, monitors, panels, HDD/SSDs. Triangles/resolution/PBR maps U. | UE and FBX; Blueprint builds are not Unity animations. Rig/LOD/colliders/native DCC source U. | Coherent modular alternate lead; stylized direction requires substantial art rework. Affordability not established. |
| H6 [PC Hard Drive - 3D Model with Separate Cable](https://www.fab.com/listings/7f7338ce-c40e-4456-8466-8b356eb8fae1) — GAMICO, Fab | Price U; **LICENSE MUST BE VERIFIED**; commercial use U. | Separate cable advertised. Triangles/resolution/PBR maps U. | FBX; native source/rig/animation/LOD/colliders U. | Focused HDD donor; seller mentions Unity, but no Unity6 or Android test. Replace supplied wire with endpoint renderer. |
| H7 [PSU Power Supply Unit](https://sketchfab.com/3d-models/psu-power-supply-unit-69ccd1be3a77497cb2acc9e39e7c52b3) — Sketchfab; author not recovered | Indexed free CC Attribution; exact version **LICENSE MUST BE VERIFIED**, commercial use conditional. | Triangles/resolution/maps U. | Rig/animation/LOD/colliders/source U. | Budget PSU lead only; modular ports and internals not established. |
| H8 [MotherBoard + Components](https://sketchfab.com/3d-models/motherboard-components-3bc94057328243d4b341a55f59160f8a) — Sketchfab; author not recovered | Indexed downloadable/free; **LICENSE MUST BE VERIFIED**; commercial use U. | Listing names motherboard, CPU, RAM and M.2. Triangles/resolution/maps U. | Rig/animation/LOD/colliders/source U. | Budget board-family lead; don't mistake a classroom render for mechanically complete assembly art. |
| D1 [Electronics store - devices and furniture](https://assetstore.unity.com/packages/3d/props/interior/electronics-store-devices-and-furniture-184870) — Mixall, Unity Asset Store | $29.99; Standard Unity Asset Store EULA. Commercial embedded use allowed subject to correct license/seats. | 64.2 MB package; triangles/resolution/map inventory U. Listing identifies PC/phone/laptop/store use; exact SKU inventory needs package inspection. | Unity package; native DCC source, rig/animation/LOD/colliders U. | Explicit URP compatibility for 2021.3.15f1. Best practical exterior/furniture donor here; Unity6 untested. Not verified to contain repair internals. |
| D2 [Electronics Mega Pack](https://assetstore.unity.com/packages/3d/props/electronics/electronics-mega-pack-114936) — 3DiZ-ART, Unity Asset Store | $19.99; Standard Unity Asset Store EULA; conditional commercial embedded use. | 880.9 MB; PBR keyword, exact maps/resolution/triangles U. | Unity2017.3.1 package; native source/rig/animation/LOD/colliders U. | Cheaper exterior donor alternative. Old package, broad appliances; substantial irrelevant content. URP conversion and inventory inspection required. |
| D3 [Rackmount Server 3D Model](https://www.fab.com/listings/9b7cd204-2a33-4b49-9884-2b89bf5c739c) — SMContent, Fab | Price U; **LICENSE MUST BE VERIFIED**; commercial use U. | 4096 PBR advertised; individual maps and triangles U. | FBX, converted GLB/glTF/USDZ; native source/rig/animation/LOD/colliders U. | Quality server exterior candidate, explicitly intended for high-end systems. Rework to 1K/2K, create removable drive trays; no mobile proof. |

### Workshop and instrument donors

| ID / exact asset and direct source | Price, license, commercial game status | Geometry / textures / maps | Rig, animation, LOD, colliders, source | Unity/mobile assessment |
|---|---|---|---|---|
| W1 [Crafting Table - Workshop Basement Workbench - Makers Bench](https://www.fab.com/listings/a092ad28-50d0-44fc-8ed1-c1286729ee49) — Studio Fjuna, Fab | Price U; **LICENSE MUST BE VERIFIED**; commercial use U. | 285 meshes, 149 textures, 6 master materials/103 instances; triangle/resolution/map inventory U. | UE Nanite project; articulated props/Blueprint behavior, not Unity clips. Native DCC/FBX availability, conventional LODs and colliders U. | Strong detailed bench/tool donor, expensive conversion risk. Includes drawers, task lamp, screwdriver variants, tweezers, soldering iron, pliers, toolbox, containers, shelves, stool, cartons, mug/paperwork/clock. Reauthor clean finishes; do not import basement identity. |
| W2 [Screwdriver](https://polyhaven.com/a/screwdriver) — PierreB3D, Poly Haven | Free CC0; commercial use and redistribution allowed. | ~3k tris (rounded listing); 1K/2K/4K; diffuse, metal, roughness, AO, DX/GL normals, packed ARM. | Blend/FBX/glTF/USD. Rig/animation/LODs/colliders U. | Best free handle/material donor. Flathead and worn: custom PH/Torx precision bits and cleaner finish required; use 1K. |
| W3 [Tongue & Groove Pliers](https://polyhaven.com/a/tongue_groove_pliers) — BKS, Poly Haven | Free CC0; commercial use and redistribution allowed. | ~3k tris; 1K/2K/4K; diffuse, metal, roughness, AO, DX/GL normals, ARM. | Blend/FBX/glTF/USD; rig/animation/LODs/colliders U. | Good free maintenance prop; not precision electronics pliers. Use selectively, resurface. |
| W4 [Retro Multimeter](https://polyhaven.com/a/retro_multimeter) — elli moeller, Poly Haven | Free CC0; commercial use and redistribution allowed. | ~13k tris; 1K/2K/4K; diffuse, metal, roughness, AO, alpha, DX/GL normals, ARM. | Blend/FBX/glTF/USD; rig/animation/LODs/colliders U. | Strong free secondary analog instrument. Retopo to 4–6k/1K for room use. Modern digital hero meter should be custom. |
| W5 [Oscilloscope - Radio Game Ready Low-poly 3D model](https://www.fab.com/listings/8bbc952e-d76b-4458-8d0c-9cd7e4cc02c7?lang=en) — Radio World, Fab | Price U; **LICENSE MUST BE VERIFIED**; commercial use U. | 6,746 tris; 256 through4096 maps: base color, GL normal, roughness, metallic, AO, opacity. | FBX/OBJ; Blender/Substance authoring mentioned, native source inclusion U. Rig/animation/LODs/colliders U. | Best focused instrument candidate. Use1K/2K, custom screen driven by diagnostics. URP material setup required. |
| W6 [Soldering station CD 8898 v.1.2](https://www.blendkit.com/asset-gallery-detail/3b63c95c-4a67-4b61-b6cc-d16e4d42ea31/) — Joachim Bornemann, Blendkit/BlenderKit | Free; Royalty Free. Commercial games allowed under [licensing FAQ](https://www.blenderkit.com/docs/licenses/licensing-faq/) with extraction restrictions. Do not publish source. | 47,402 polygons, **not stated triangles**; 50.9 MiB; texture resolution/PBR map list U. | Blender model; movable tools with following cables advertised, not baked Unity animation. LOD/colliders U. | Best free solder/hot-air donor. Bake materials, remove real branding, retopo station to6–10k and tools2–4k. |
| W7 [Cardboard Box 01](https://polyhaven.com/a/cardboard_box_01) — Rahul Chaudhary, Poly Haven | Free CC0; commercial use and redistribution allowed. | ~17k tris; 1K/2K/4K; diffuse, roughness, AO, DX/GL normals, ARM. | Blend/FBX/glTF/USD; rig/animation/LODs/colliders U. | Realistic but worn parcel donor; simplify to0.5–2k and512. Opening flaps need reconstruction; don't spend17k on every parcel. |
| W8 [Furniture Kit](https://kenney.nl/assets/furniture-kit) — Kenney | Free CC0; commercial use allowed. | 140 files; triangles/resolution/PBR U. | Native source/rig/animation/LODs/colliders U. | Cheap furniture base/blockout only. Not approved as final premium art without rebuilding silhouettes/materials. |
| M1 [Cardboard 004](https://ambientcg.com/view?id=Cardboard004) — ambientCG | Free CC0; commercial use allowed. | PBR texture set1K–8K; individual archive maps uninspected. Geometry/rig/LOD/collider N/A; editable generator source U. | Raster maps, not a model. | Clean packaging material candidate; use512/1K and authored flap geometry. |

### Hands

| ID / exact asset and direct source | Price, license, commercial game status | Technical evidence | Fit and required work |
|---|---|---|---|
| A1 [FPS Arms 01 (Rigged)](https://superhivemarket.com/products/fps-arms-01-rigged-) — Davlet, Superhive | $15; Royalty Free listed. **LICENSE MUST BE VERIFIED** against the applicable full terms before procurement/public source use; embedded commercial permission must be recorded. | 13,292 tris; 4096 base color, normalGL, roughness, AO, height/displacement and others. FBX plus Blender file with two rigs; forearm twist bones. First-person male arms, finger correction described. LOD/collider/bone count/repair clip inventory U. | Strongest documented quality hand candidate. Import Generic rig,2K textures, one skin material; strip unused full-skeleton bones only after skin validation. No Unity6 proof or Android test. |
| A2 [VR Hands and FP Arms Pack](https://assetstore.unity.com/packages/3d/characters/humanoids/vr-hands-and-fp-arms-pack-77815) — NatureManufacture | $10; Standard Unity Asset Store EULA; commercial embedded use subject to appropriate seats. | 422.6MB, Unity2020.3.22, updated2022. Listing marks Built-in compatible, URP incompatible. Triangles/resolution/map list/bone layout/native source/LODs/colliders and exact clips U. | Best cheap Unity-oriented rig candidate, pending finger/grip inspection and URP conversion. No SSS shader requirement on mobile. |
| A3 [Realistic FPS Hands](https://assetstore.unity.com/packages/3d/characters/realistic-fps-hands-107409) — Knife Entertainment | $14.99; Standard Unity Asset Store EULA; conditional commercial embedded use. | 487.2MB, Unity5.5.1,2018. Triangles/maps/resolution/rig details/animation inventory/LODs/colliders/native source U. | Backup only; old package with less exposed technical evidence than A1. |

Unity listings above expose an “Extension Asset” label alongside entity selectors. Confirm correct seat/entity licensing at checkout instead of assuming a single entity purchase covers all collaborators. [Unity EULA](https://unity.com/legal/as-terms). Fab's Standard License supports commercial embedded use and compatible engines, but forbids standalone redistribution; **a generic Fab license page does not prove an individual listing uses it**. [Fab license summary](https://www.fab.com/eula). CC0 Poly Haven sources permit redistribution; keep provenance even when attribution is optional. [Poly Haven license](https://polyhaven.com/license).

No externally sourced files were introduced, so `Docs/THIRD_PARTY_ASSETS.md` is not created. At acquisition, record asset, author, exact source, license/version/date, receipt reference, permitted distribution, local file/hash, usage and modifications. Keep paid FBX/textures and raw DCC files in access-controlled art storage, with only lawful deliverables in this public repository.

### Evaluated but not approved

- [Ultimate Electronics Repair Shop Low-Poly Pack](https://www.fab.com/listings/110f04a5-7194-4517-81ae-dc63eca74bb9): attractive category name, but UE/Nanite-only evidence, no exposed exact mechanical inventory, AI-generated declaration and unclear license. Not an adequate hero hardware shortcut.
- [Workbench](https://www.fab.com/listings/5a9cacb2-c55f-4ea7-bcf0-172bf004e0fb): free FBX, creator notes rough early textures; not final premium bench art. License must be verified.
- [Computer Case Basic](https://www.cgtrader.com/free-3d-models/electronics/computer/computer-case-basic): free Royalty Free listing, but beginner model and texture issues reported; internals/separable panel completeness not established. Not approved for assembly.

## 3. Category coverage and make/buy decisions

The table supplies a quality route and economical route for every requested group. IDs refer to exact listings above. **GAP means no suitable verified two-product pair was established.** Custom routes are briefs, not invented marketplace assets or “free” labor. A donor named for evaluation is not guaranteed to contain the required subassembly.

### PC hardware

| Category and variants | A: quality route | B: economical route | Required original geometry / decision |
|---|---|---|---|
| ATX, mATX, MiniITX cases; glass, mesh, budget, premium | Custom Aster chassis family; evaluate H1 for donor quality | H5 modular pack lead; H2 inventory audit; price/license gates remain | 4 chassis envelopes including existing EATX,5 profiles. Removable left/right panels, feet, front IO, tray, PSU bay, vents, rear brackets, screw bosses. GAP: two cleared premium case products. |
| ATX/mATX/ITX boards | H3 donor plus original mechanical parts | H8/H2 donor leads; H4 reusable chip kit | 3 board outlines; socket families; DIMM/PCIe/M.2/header/rear-IO markers. Board underside and fastener holes. ITX requires a future catalog definition. |
| CPU package/contact pads | H1 donor lead | H8 or H2 donor lead | Square PGA, square LGA and rectangular LGA envelopes with socket-specific contacts/lid detail. Current AM4 needs underside pins, not contact pads. Socket cover/frame/lever belong to motherboard, not CPU prefab. |
| Basic/gaming/RGB DIMM | H1 donor lead | H8 donor lead | One accurate card/connector per memory geometry,3 heatspreader styles; latch clearance. RGB diffuser separate, not a lamp per LED. |
| Compact/dual/triple/large GPU | H1 detail donor; custom Helix shrouds | H5 alternate, price U; H2 inventory U | 5 silhouettes for6 IDs; calibrated length/slot width, PCIe edge connector, power ports, removable bracket; no shroud scaling that stretches fan circles. GAP: cleared cheap GPU set. |
| ATX fixed/modular PSU | H1 donor lead | H7 free lead | Two shells,port insert variants, stationary grille/separate fan, keyed modular sockets. Labels cannot imply arbitrary PSU cable compatibility. |
| 2.5 SATA SSD | H1 | H5 or H2 inventory audit | One body+label atlas; expose SATA data/power mouths. |
| 3.5 HDD | H6 | H2 inventory audit | One closed shell and separate service hero interior only if needed. NAS tray is separate. Don't animate internal exposed platters during normal handling. |
| M.2 NVMe | H1 | H8 | 2280 base, label/heatsink variations, edge connector, screw notch; compatible mount positions. |
| Tower/dual tower | H1 donor cooling inventory review | H5 fan donor; tower coverage U | Two towers, one fin normal/opacity-free distant surface, heatpipes, fan clips, bracket family. GAP: cleared tower pair. |
| AIO240/280/360, pump, block, radiator | Custom Frostline family, H1 donor review | Reuse original fan and radiator modules | Three radiators,2/2/3 fans sized120/140/120; custom hoses/block. Resolve catalog conflict first. H1's260mm is not a matching variant. GAP. |
| Fans120/140/RGB | H1 | H5 alternate, cheap status U | Two frames/rotors, diffuser insert. Author hole spacing, local rotor axis and airflow marking. Resolve fan normalization. |
| ATX24, EPS8, PCIe8,12V-2x6 | Custom connector kit | Same small original kit with fewer near detail loops | Geometry for keying/latches/contact mouths; EPS and PCIe cannot share an indistinguishable head. 12V-2x6 includes sense-contact silhouette. No cleared exact external pair found. |
| SATA power/data, front panel, fan connectors | Custom connector kit | Shared housing/contact atlas and procedural cables | Distinct L-key SATA shapes, slim data cable, bundled front-panel leads, keyed fan plug. Geometry sockets match profile axes. GAP: exact external pair. |

### Specialist repairs

| Category | A: quality route | B: economical route | Artist requirement / gap |
|---|---|---|---|
| Laptop | D1 exterior donor audit | D2 exterior donor audit | Original bottom cover, hinge, mainboard, pouch battery, ribbon connectors, fan/heatpipe. Neither pack verified repair-ready. |
| Smartphone | D1 inventory audit | Original generic shell using common material kit | No cleared second phone model. Display, midframe, battery pull tabs, charge port and shields must match existing states. |
| Tablet | D1 inventory audit, exact inclusion U | Share custom phone assembly modules, larger shell | GAP: verified tablet pair; don't imply phone meshes become valid just by uniform scaling. |
| Console | Custom fictional console with removable covers | Reuse original PSU/fan/PCB modules | GAP: no verified pair; don't import branded console replicas or ripped game props. |
| Controller/handheld | Custom AxisWorks exterior + replaceable stick module | Shared custom buttons/triggers/PCB family | GAP: verified pair. Differentiate handheld display from controller; current generic portable proxy does not establish correct geometry. |
| NAS | Custom2/4/8-bay enclosure, D3 rack donor review | Reuse HDD/PSU/board modules | GAP: two verified NAS products. Map each animated tray to the disk state actually changed. |
| Server | D3 | Original rack shell + H6/H2 drive donors | GAP: verified cheap server. Expose service lid, drive sleds, power modules; not all need physics. |
| Router | Custom Linkforge case/ports/antennas | Reuse network PCB/contact kit | GAP: verified pair. RJ45 latch and link LEDs should reflect network state. |
| Hard drives | H6 | H2 inventory audit | Separate drive from NAS caddy; no unsupported repairable internal mechanism. |
| PCB, loose components, ICs | H4 as donor; custom repair board | H2/H8 component donors | Build original resistor/capacitor/inductor/MOSFET/QFP/BGA/shield kit. Near solder joints geometric, distant joints normal-map. |
| Connectors | Original keyed kit | Shared contact atlas and simplified housings | GAP: verified pair. Add FFC/ZIF,board-to-board,JST-like,RJ45,USB-C shapes without brand marks. |

Specialist visual binding: `DeviceCategory -> device profile`; each profile exposes cover, screw, battery, display, port, probe, rework and drive-tray markers. The hardware profile manifest covers replacement parts separately. Appearance must not claim a repair completed before the existing command succeeds.

### Workshop coverage

| Required group | A: quality route | B: economical route | Normalization / custom requirement |
|---|---|---|---|
| Workbench, drawers, shelves, rack, pegboard, part bins | W1 selective donors + original graphite cabinet fronts | W8 base furniture with original surfaces | Pegboard/ESD bench top and electronics rack dimensions custom; exact W1 pegboard/rack coverage U. Keep workstation positions. |
| Tools, toolbox, screwdrivers/Torx, pliers, tweezers | W1 donors | W2/W3 plus custom precision tips/tweezers | Custom driver bit mounts and tool-tip/grip markers; W3 is maintenance pliers. Torx tips not verified in W1. |
| Multimeter | Custom digital BenchPro instrument | W4 analog secondary meter | GAP: external modern-meter pair. Movable probes and readable diagnosis screen mandatory. |
| Oscilloscope | W5 | Reuse original BenchPro instrument enclosure/screen family | GAP: verified free scope. No generic static screen can substitute for diagnostic feedback. |
| Solder station, hot-air station | W6 after retopo, or W1 iron donor for premium custom station | W6 free donor | Distinct iron/hot-air tools, hose endpoints, tip-rest pivots; supply trace for current license. |
| Microscope | Custom binocular boom microscope | Simplified original stand/head with diagnostic UI inset | GAP: no appropriate cleared electronics microscope pair. Avoid medical microscope shapes. |
| Bench power supply, PSU tester, cable tester, thermal camera | Custom unified instrument family | Reuse screen/knob/port atlas across four housings | GAP: verified pairs. Different port arrangements, screen content and grips; don't simply recolor one meter. |
| ESD mat/bags, thermal paste, screw boxes | Original BenchPro consumables kit | Same kit with fewer bag folds/one atlas | GAP: exact external pairs. Mat grounding cord, syringe plunger, opaque ESD bag folds with limited transparency. |
| Air blower, compressed air, brush | Original compact blower/can/brush | Common handle/nozzle parts and label atlas | GAP: exact cleared pairs. Never make a can a flame/smoke emitter; short air/dust feedback. |
| Parcels, packaging, shipping labels | W1 carton donors | W7 + M1 clean cardboard | Original fictional shipping labels, no personal addresses; four flap pivots, insert/foam, small torn tape decals. |
| Chair/stool, desk, lamps, ceiling lights | W1 stool/lighting donors and D1 fixture audit | W8 furniture bases | Rolling chair needs original casters; only articulate parts affecting access. |
| PC terminals, monitors, keyboard, mouse | D1 | D2 | Confirm exact package inventory; author ForgeOS UI/screen emission and monitor arm pivots. |
| Vents, AC, extinguisher, safety signs, trash | Original architectural service-prop kit | Extend existing procedural dimensions, bake authored meshes/materials | GAP: verified pairs; replace prototype finish rather than calling cubes final art. Safety signage original, legible. |
| Coffee cup, paperwork, clock | W1 | W8 inventory audit + original simple props | One contained personal corner, original paper/clock artwork; no pile of decorative clutter. |
| Wall panels, workshop walls, industrial floor | Original room kit with trim sheet | Same kit using fewer modules/shared materials | Clean warm walls, sealed grey floor, subtle wear under chair only; no warehouse grunge. GAP: cleared external pair. |

Procurement sequence: first request A1's full license and export details; evaluate A2 if that fails. W2/W3/W4/W7 are cleared CC0 donors but should still pass visual review. W6 offers real specialist value at no license fee. Evaluate D1 before the larger, older D2. Request W5's selected license and W1's neutral export inventory before spending. Do not buy H1/H3/H5 merely to fill the spreadsheet: custom mechanical hardware is the safer primary production route.

## 4. Original identity and material library

Architectural palette: warm off-white `#D9D8D1` about55% of room surfaces; graphite `#252C31`25%; warm grey/aluminium15%; teal `#3F7776` and amber `#E5A74B` accents5%. These are art swatches, not measured reflectance. Keep main assembly surface clear with a parts tray, driver and current job. Use organized vertical storage and one coffee/paperwork corner. No random industrial clutter.

Hardware preserves existing fictional brands: Aster cases, Novera boards, Vanta CPUs, Quanta memory, Helix GPUs, VoltEdge PSUs, Frostline cooling, Aeroform fans, ArcDisk storage, Linkforge networking, Cellera batteries, Luma displays, AxisWorks controls and BenchPro tools. Shared bevel language and fastener finishes; differentiated vents/shrouds and label typography. No NVIDIA/AMD/Intel/Apple/Samsung marks. Removing a logo alone does not establish rights to a protected product design; prefer original shells.

ForgeOS/UI should use the same teal/amber status logic, restrained outlines and clear type. An interactable highlight is temporary and thin, not a permanently glowing machine. Color is backed by text/icon/shape. Calm ventilation and precise clicks support a bright, quiet working atmosphere.

All values below are **authoring starting ranges**, evaluated under the same neutral light. Base color examples are sRGB; metallic/roughness/AO are linear data. URP smoothness =1−roughness. Coatings are dielectric; bare-metal islands use a mask, not a uniform0.5 metallic compromise. Do not bake lighting into base color.

| Material | Base-color starting point | Metallic | Roughness | Normal / AO policy |
|---|---|---:|---:|---|
| Brushed aluminium | light neutral silver `#C4C7CA` | 1 | .28–.42 | Fine directional brush normal; no deep scratches; AO only seams |
| Anodized aluminium | charcoal/teal tinted metal, reference-calibrated | .8–1 | .30–.48 | Fine brush, broad controlled roughness; simplify layered response on mobile |
| Painted steel | room/brand palette | 0 paint,1 chips | .35–.55 | Very subtle orange peel; chips rare |
| Black powder coat | `#292D30` | 0 | .58–.75 | Fine grain, restrained AO |
| ABS | `#33383C` | 0 | .38–.55 | Mold texture, seam detail |
| Matte plastic | `#484C50` | 0 | .62–.80 | Fine grain, no glossy edges baked into color |
| Glossy plastic | `#25292C` | 0 | .16–.28 | Minimal normal; geometry gives edge highlights |
| PCB fiberglass/solder mask | green `#24483A`, near-black variants | 0 | .42–.62 | Trace height extremely shallow; metal exposed pads separate |
| Copper | copper tinted metal `#D9946A` | 1 | .20–.40 | Micro scratches; oxide patches dielectric if present |
| Nickel | silver `#BABDC1` | 1 | .20–.34 | Subtle machining |
| Chrome | pale neutral silver | 1 | .08–.18 | Very restrained normal; reflection probe essential |
| Rubber | `#292C2D` | 0 | .70–.88 | Soft grain; avoid crushed black |
| Braided sleeving | graphite/teal | 0 | .60–.78 | Tile weave normal along cable UV; silhouette strands only luxury |
| Tempered glass | nearly neutral, slight edge tint | 0 | .05–.14 | Real thin edge geometry; no dirt wallpaper; restrained transparency |
| Frosted plastic | soft white/grey | 0 | .42–.62 | Opaque approximation for RGB diffuser on lower tiers |
| Cardboard | muted brown `#A5815C` | 0 | .75–.92 | Paper grain, folds/edges; AO not baked across movable flaps |
| Paper | off-white `#DCDAD2` | 0 | .65–.88 | Minimal normal; text atlas |
| ESD rubber | desaturated teal | 0 | .72–.86 | Fine matte grain; grounding stud separate metal |
| Laminate/wood bench | warm neutral laminate or light wood | 0 | .42–.62 | Subtle directional grain; no exaggerated grooves |

AO strength usually .3–.7 on baked cavity maps, white on exposed flats; validate in shader because ambient occlusion should not darken direct lighting indiscriminately. Use geometry bevels on hero silhouettes, normal bevels for background props. Bake repair damage masks separately and derive enabled damage from gameplay, never randomize fault visuals independently.

### Texture strategy

| Use | Max source size in Android import | Suggested ASTC | Notes |
|---|---:|---|---|
| Held/installed hero case, board, GPU, hands | 2048 | 6×6;4×4 only if contact/normal detail needs it | One shared2K set per family where possible; hands one2K skin set |
| Bench, normal instruments, environment | 1024 | 6×6 or8×8 | Trim sheets and tiling surfaces; avoid unique giant room textures |
| Small props/cables | 512 | 6×6/8×8 | Atlas label variants and shared metal/plastic trims |
| Tiny props | 256 or atlas region | 8×8 | Pins/screws use shared materials, usually no unique maps |

Hero density target2K–4K texels/metre at0.25–0.6m viewing; room512–1024texels/metre. Small printed labels can use a dedicated512/1K atlas to remain legible. Inspect at intended screen pixels, not only DCC zoom. Albedo/emission are sRGB; masks/normals linear. Pack URP metallic in R and smoothness in alpha; use a separate occlusion texture with G for URP Lit, or an explicitly documented custom packed-mask shader. **Do not feed generic ARM directly to stock URP Lit.** Disable Read/Write unless runtime mesh/texture mutation truly requires it; enable mipmaps and streaming where supported and appropriate. Keep4K masters privately for rebaking, not everywhere in the Android build.

At ASTC6×6 a2K texture is approximately1.8MiB base level,2.4MiB with mips; three maps cost about7.1MiB, before CPU copies/render targets. Track resident memory rather than compressed download size. Atlas moving subparts only when material response matches; do not create a single enormous always-resident atlas containing the whole game.

## 5. Modular profiles and prefab contract

`Docs/hardware_visual_profiles.json` assigns each existing definition a stable `fb.hw.<definitionId>.v1` ID and explicit family/variant. It is an authoring manifest, outside Resources, with no runtime consumer yet. Future `HardwareVisualProfile` ScriptableObjects can carry the same IDs. Save files retain existing definition and item IDs; no save migration is required for a visual profile addition.

| Family | Mesh basis and variation |
|---|---|
| Cases | mATX budget, ATX shared, EATX premium, MiniITX; interchangeable front mesh/glass, side panels, feet, paint/accents. Five catalog profiles. |
| Boards | ATX/mATX current, ITX future; socket modules, DIMM/PCIe/header layouts and VRM heatsinks. Match normalized specs; do not randomly remove slots. |
| CPUs | Three envelope bases, with distinct AM4/AM5/LGA1700/LGA1851 contact/lid variants. AM4 underside pins must remain distinct from LGA pads. |
| RAM | Basic, heatspreader, RGB; geometry keyed by memory family, capacity label varies. |
| GPU | Compact, dual-small, dual-large, triple, premium-triple; six profiles reuse one dual family. Move fan mounts and shroud segments rather than stretching fans. |
| PSU | Fixed cable and modular bases; wattage label, grille and port insert; preserve normalized connector availability. |
| Cooling | Single/dual tower;240/280/360 radiators; common pump housing, hose connectors, fan family. Block disputed sizes until specs agree. |
| Fans |120/140 frames with optional diffuser; same rotors/materials across catalog variations. |
| Storage |2.5 SSD,3.5 HDD, M.2 bare/heatsink. Labels/PCB masks distinguish definitions. |
| Specialist replacement parts | NIC card family, pouch battery sizes, display module family, analog-stick module; distinct from whole-device shells. |

Suggested future prefab hierarchy: stable `InteractionRoot` with current collider/proxy/state identity; child `VisualRoot` containing renderers/LODGroup and movable panel/latch/rotor parts; sibling `Markers` with install pose, grips, socket mouths, fastener axes and clearance bounds. Model roots use1unit=1metre,+Yup,+Zforward,+Xright,positive uniform scale. Board authored flat with components+Y; assembly pose rotates the whole board. Connector marker+Z points in insertion direction, with a named up vector for keying. These are new authoring conventions, not assumptions about the old primitives.

Marker names: `InstallPose`, `ApproachPose`, `Grip_R_Primary`, `Grip_L_Support`, `ToolTip`, `Fastener_<stableId>`, `Socket_<semanticId>`, `Latch_<semanticId>`, `CableExit_<id>`, `PanelRest`, `RotorAxis`. Numeric slot IDs must be stable across LODs and not based on child order. A missing marker is an import validation error, not a runtime guess. LOD meshes share pivots and marker parents.

Resolver integration sequence:

1. Load registry once, build an ordinal lookup, and verify complete definition coverage.
2. `PhysicalAssemblyController` and `ObjectHandlingController.BuildProxyVisual` call the same resolver for visual children. Keep existing interactions/collider roots. Unknown profile uses the existing procedural fallback with a development warning.
3. Diff item/state bindings instead of destroying all visuals on every refresh. Replace only changed parts; cancel presentation first when active machine changes. Pool only after correctness is established.
4. Extend `SpecialistDeviceVisuals` similarly using a separate device registry and persisted specialist fields.
5. Add marker bindings to the actual socket implementation available at integration time. If the missing binder/endpoint branch arrives, review and adapt to it instead of introducing duplicates.
6. Shared materials keyed by preset/variant; avoid per-chip `new Material`, per-frame LINQ and repeated `Find`. Prefer shared-material variants/vertex data. MaterialPropertyBlock use must be benchmarked against SRP batching; it is not automatically a batching optimization. [Unity SRP Batcher guidance](https://docs.unity3d.com/6000.3/Documentation/Manual/urp/shaders-in-universalrp-srp-batcher.html).

## 6. First-person interaction and IK

Recommend **Option C, hybrid**: components remain directly readable and responsive during free pickup; hands blend in for supported close-up grips, insertion and tool work. Hide arms during inventory/UI, obstructed reach, unsupported poses, or a user-selected hands-off mode. Use short sleeve cuffs, neutral adult appearance, no jewelry or character identity. Gloves are an optional texture/mesh variant, not required for every task.

| Option | Benefit | Cost | Decision |
|---|---|---|---|
| A floating tools/components | Clear view; lowest rig/clipping cost | Less physical presence | Keep as fallback and accessibility option |
| B always-visible arms | Strong presence | Requires grips for every pose and reach; occludes tiny work on phone screens | Too costly as initial default |
| C hybrid | Presence at meaningful contacts with lower coverage burden | Requires smooth entry/exit and clear reach limits | Adopt |

A1 is the documented quality candidate, A2 the inexpensive alternative, both gated by the register. Require independent fingers, good thumb opposition, forearm twist, a palm that closes around a20–30mm driver handle without collapsed knuckles, and nonuniform scale-free export. Avoid a full-body movement system. Target one two-arm skinned mesh,40–60 deform bones,one Animator,one RigBuilder,up to two TwoBoneIK constraints,4weights/vertex maximum. Hands plus cuffs target12–18k tris near,6–9k fallback.

Unity lists **Animation Rigging1.4.1 released for6000.3**. It is appropriate for the two arms, not every screw/cable. The current manifest does not include it; add only when a real hand rig is ready. [Package compatibility](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.animation.rigging.html). TwoBoneIK supports target position/rotation, elbow hint and weights. [Constraint documentation](https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.4/manual/constraints/TwoBoneIKConstraint.html).

Rig flow: authored reach/grip pose → item/tool grip targets in world space → right/left hand IK → small authored finger curl correction. Targets live outside animated bone chains. Elbow hints stay camera-relative with authored side offsets and reach clamping. Never place the hint exactly collinear with the arm. Blend IK weights in80–150ms and out100–200ms; no camera bob or forced head movement.

Grip authoring: GPU primary at shroud edge, secondary under safe support area; motherboard left/right edges, never socket pins; case two side grips; RAM pinch on heatspreader/top edges; CPU edge pinch; PSU primary side/secondary underneath; panel two edge grips; cable grip behind connector housing, never dragging by a wire tip; driver palm target plus tool-tip target. One object pose drives both hands. The second hand follows the support marker; two hand solvers must not independently move the same item. If a target is unreachable, stop/reposition the presentation rather than stretching bones.

## 7. Authoritative animation architecture

Names below are proposed extensions, **not implemented classes**:

- `PhysicalInteractionAnimator`: persistent presentation coordinator attached beside existing production systems, not under a root destroyed by `Rebuild`.
- `HandIKController`: grip poses, reach limits, left/right support, accessibility.
- `ToolAnimationController`: tool pose, tip contact, local feedback.
- `SocketAnimationProfile`: approach/insertion poses, local axis, clearance, contact distance and animation timings.
- `FastenerAnimationDriver`, `CableAnimationDriver`, `PanelAnimationDriver`: move only visual children from normalized presentation progress.
- A small adapter around **existing** `GameRuntime` commands handles validation, exactly-once commit and postcondition checks. It does not reproduce compatibility/economy/save logic.

Transaction phases: validate action → capture machine/item/target identity and precondition snapshot → begin animation → reach physical threshold → revalidate → invoke existing authoritative command once → verify resulting state → complete presentation. Events may request a threshold check; they are not the only route to commit. Coordinator detects threshold crossing even when frames skip and uses an action token/committed flag. A timeout cancels before commit or reconciles after commit; it never declares success based on elapsed time alone.

Existing `Install` is void: after invoking it, verify the item is reserved **and assigned to the expected machine/slot**, not merely that some reservation exists. The command can refresh/destroy visual roots synchronously; hold stable IDs and reacquire visual handles after commit. A presentation lock prevents duplicate gestures, but every authoritative path still needs validation because UI actions can change state while a clip is playing.

| Interruption | Required result |
|---|---|
| Cancel before contact, focus loss, item removed by UI | Cancel token, release presentation lock, restore/rebuild visuals from current state; no gameplay mutation |
| Pause/save after commit | Finish or snap visuals to authoritative result; save sees committed state, not an intermediate transform |
| Machine/job switched or root destroyed | Stop old presentation; never commit against the newly active machine; reacquire by original ID or cancel |
| Duplicate animation event or low-FPS threshold skip | Commit at most once; detect threshold crossing in coordinator |
| Command rejects or postcondition fails | Failure feedback and state reconciliation; no success click or item disappearance |
| App killed | Only existing authoritative saved state restores; transient hand/tool poses are reconstructed, not serialized |

Current main has **no per-endpoint cable commit** and **no targeted screw API**. Initially animate the aggregate cable operation as a short presentation after its real result, or defer individual plug interaction to the actual endpoint branch. Do not fake independent unplugging that never changes diagnostics. Similarly animate the fastener selected by `LoosenNext`, not any arbitrary clicked screw. New mechanics require an explicit separate scope; this plan does not smuggle them in through art.

## 8. Exact original animation production list

No verified off-the-shelf PC-assembly clip pack was found that matches these mechanisms. Do not buy weapon reload animations as repair animations. **The following are exact proposed ForgeBench clip/action names and artist specifications**, not claims of purchased clips. Mechanical motions are mostly curves on rigid transforms; hands use an authored reusable grip library. Timing is a starting target and must stay skippable/reduced-motion compatible.

| Clip/action names | Timing target | Contact and commit behavior |
|---|---:|---|
| `FB_Reach`, `FB_Grab`, `FB_Release` | .25–.45s each | Reach palm first, close fingers at grip marker, attach visual only at grasp; release at stable rest pose |
| `FB_Grip_Precision`, `FB_Grip_Driver`, `FB_Grip_Tweezers`, `FB_Grip_Cable`, `FB_Grip_TwoHand` | pose assets +.1s blend | Authored finger shapes, wrist offsets per tool; no gameplay changes |
| `FB_Screw_Align`, `FB_Screw_Turn`, `FB_Screw_Torque`, `FB_Screw_StripReact` | .2s +.5–1.2s turn | Align tip to axis; screw travel=thread pitch×turns. Rotate driver/screw together while engaged; torque stop1–2° settle. Stripped reaction brief slip, not a successful tighten |
| `FB_RAM_Align`, `FB_RAM_InsertPress`, `FB_RAM_LatchClose`, `FB_RAM_LatchOpen`, `FB_RAM_Remove` | .7–1.1s install | Key/notch alignment, straight insert, short press, latch closure. Commit seated state at validated insertion threshold. Opening latch separate from removing stick |
| `FB_CPU_LeverOpen`, `FB_CPU_FrameOpen`, `FB_CPU_Place`, `FB_CPU_FrameClose`, `FB_CPU_LeverLock` | .2–.45s each | Lever unloads then lifts; frame pivots independently; CPU descends without scraping contacts. CPU install commit at supported seat; latch persistence only if gameplay supports it |
| `FB_GPU_Align`, `FB_GPU_Insert`, `FB_GPU_Latch`, `FB_GPU_BracketScrew`, `FB_GPU_Remove` | .8–1.2s +screw | Support underside, align bracket and PCIe, insert, latch then fastening. Remove after actual unlocking constraints; no clip bypasses compatibility |
| `FB_M2_AngledInsert`, `FB_M2_PressFlat`, `FB_M2_Screw`, `FB_M2_Remove` | .8–1.2s | Author around25–30° approach adjusted to connector; rotate about inserted edge, not center. Seat/retention commits follow available state model |
| `FB_PSU_Slide`, `FB_PSU_Fasten`, `FB_PSU_Plug` | .5–.8s +screws | Bay guide axis, rear alignment, mounting contacts; cable not visible as connected before actual cable result |
| `FB_Paste_Apply`, `FB_Cooler_Align`, `FB_Cooler_Press`, `FB_Cooler_CrossTighten`, `FB_Cooler_FanClip` | .4–.8s per phase | Syringe plunger and small paste mask, mount compression, alternating screw order, fan clip flex. Paste consumption/thermal result stays authoritative |
| `FB_Panel_Slide`, `FB_Panel_Lift`, `FB_Panel_Remove`, `FB_Panel_BenchPlace`, `FB_Panel_Attach`, `FB_Panel_Fasten` | .6–1.0s removal/place | Release rail then lift; glass stays rigid, hands on edges; stable bench rest pose. Panel flag changes only at validated removal/attachment |
| `FB_Cable_Pickup`, `FB_Cable_Align`, `FB_Cable_Insert`, `FB_Cable_Click`, `FB_Cable_LatchRelease`, `FB_Cable_Unplug` | .35–.65s plug | Orient keyed housing, final2–4mm visual compression; click only after success. Pull straight before bending; do not stretch connector mesh |
| `FB_Tool_Driver`, `FB_Tool_AirBurst`, `FB_Tool_Brush`, `FB_Tool_Paste` | .2–.8s repeated actions | Reuse grips; nozzle/tip aligns with affected surface, minimal particles |
| `FB_Tool_ProbePair`, `FB_Tool_Solder`, `FB_Tool_HotAir`, `FB_Tool_TweezerLift` | .5–1.5s presented work | Probe tips land on test markers; iron tip contact, small hot-air orbit, tweezers close before lift. Effects reflect accepted repair actions |
| `FB_Portable_Cover`, `FB_Portable_BatteryUnplug`, `FB_Portable_DisplayLift`, `FB_NAS_TrayRelease`, `FB_NAS_TraySlide` | .4–1.0s | Device profile poses; ribbon slack and caddy guides. Keep current specialist state sequence |

Source deliverables from animator: editable Blender/Maya scene, neutral bind pose, FBX bones/clips, pose library, named contact markers, clip timing manifest, mirror review and contact videos in Unity. Commission includes commercial game use and agreed source ownership; no unnamed mocap provenance. Avoid root motion moving the player. Author at60fps, export compressed curves after contact-error review; sampling rate is not gameplay timing.

## 9. Cable rendering

Choose **custom piecewise cubic Bezier curves with pooled procedural tube meshes**. Unity Splines is useful for editor route authoring if its dependency is justified later; it is unnecessary for a few controlled cable spans. No cloth/rope physics. The existing `ConnectCables` flags initially drive presentation; real endpoint IDs can replace that adapter when available.

| Technique | Evaluation |
|---|---|
| LineRenderer | Cheapest distant/Low fallback; camera-facing ribbon and endcap shading are weak at close contact |
| Procedural tube | Best cross-section, UV and material control; bounded CPU updates; selected |
| Unity Splines | Helpful authoring tooling; still needs extrusion/LOD/update ownership; not itself realistic cable physics |
| Custom Bezier | Small dependency-free curve evaluator, explicit exit tangents/anchors; selected centerline |
| Skinned cable | Predictable authored paths but extra bones/skin cost per cable; useful only for a special fixed hose |

Each route has source/destination marker IDs, cable type, radius or ribbon dimensions, rest length, endpoint exit tangents, optional management anchors, minimum bend radius and material preset. Endpoints are exactly at connector mouths; short stiff exit sections preserve strain relief. Length must exceed route chord/anchor path. Use a small bounded arc-length solve for slack; if no legal route exists, retain the last valid route or show an unconnected cable, not an intersecting S-curve.

Sample curvature and enforce the authored bend limit by moving control handles/adding an anchor. Start with bend radius5–8×bundle diameter as a visual heuristic, then author connector-specific limits from reference. Preserve especially gentle exit at GPU power. Route around coarse case/board keep-out volumes, never run full physics collision per vertex. Artist-defined management paths beat a general-purpose cable solver here.

Near hero:8radial sides×16–24segments, about256–384side triangles/cable. Medium6×12≈144; far4×6≈48 or strip; caps/connectors separate. Parallel-transport frames avoid twist flips; accumulated arc-length UVs keep braid scale constant. Use ribbon cross-section for SATA data/FFC, round bundle for sleeved power; do not render24individual simulated wires except a very short static breakout on Ultra. Preallocate vertex/index buffers; update only dirty routes at20–30Hz while manipulating, interpolate endpoints/frame display smoothly; freeze at rest. Target0B/frame managed allocation after warmup. Pool by size class and update bounds without rebuilding indices each frame.

Cable relaxation: one critically damped control-point offset settling100–250ms, bounded to avoid geometry intersections. No perpetual wobble, no overshoot at socket. Reduced motion removes settle oscillation. Detach visuals only when the authoritative unplug exists/succeeds.

## 10. Fans, micro-animation and workshop motion

Fan profile separates frame, rotor, hub, RGB diffuser, airflow axis and blur disc. Drive target RPM from current power/fan/cooling state with start/stop envelopes; no spinning PSU grille. Low RPM uses visible blades; normal RPM crossfades between rotor and a prefiltered blurred disc; high RPM predominantly disc/material motion. Avoid relying on exact high-speed rotation at30/60fps, which aliases or reverses. Crossfade thresholds are artistic and device-tested, not a physical RPM measurement.

Start ramps .3–.8s, stop coasts .8–2s unless the state indicates an abrupt fault. Blur disc is one tightly fitted surface with minimal transparent overdraw; Low uses an opaque stylized disc. Bound geometry rotation for visual stability. RGB modes: static color; breathing2–4s period; slow rainbow phase; synchronized phase clock shared across components. Emission does not create a real light per LED. Reduced motion freezes color cycling/breathing and uses a stable blur representation while retaining a clear running/stopped indicator.

| Micro-motion | Limit / trigger |
|---|---|
| Connector compression, latch flex |1–3mm/1–3° around contact only; return to exact seat |
| Screw torque wobble |1–2° single settle; no continuous shake |
| Cable relaxation |Short damped settle only after movement |
| HDD spin vibration |Mostly sound; optional subpixel case vibration, never moving the whole bench |
| PSU switch, power button |Measured pivot/travel, crisp state-driven release |
| Glass panel |Rigid slide/lift with subtle seating contact, no rubber bending |
| RGB, monitor startup |Short controlled luminance fade; no flashing or automatic exposure pumping |
| Drawer damping |Ease-out last20% travel; motion only when opening needed storage |
| Chair wheels |Rotate only if chair actually moves; avoid background rolling physics |
| Parcel flaps, antistatic bag |2–4rigid flap pivots or1–2shape keys; one interaction-triggered crumple, no cloth |

Workshop actions worth shipping: drawer/tool cabinet opens to retrieve tools, storage bin opens for inventory, parcel opens during receiving, lamp/monitor arm repositions if it improves sightlines, door opens for actual room access. Stool adjustment/rolling chair is optional if it solves access. Decorative cabinets, lights and chairs otherwise remain static. Use a shared curve driver, no Animator/Rigidbody on every prop. Existing station UI remains the task interface.

## 11. URP lighting tiers

Build one authored room prefab in Editor from the current station arrangement, then bake static indirect/diffuse lighting and reflection captures. Preserve runtime actions via marker binding. Use broad4000–4500K overhead fixtures, slightly cooler task illumination, subtle monitor spill. Bake with neutral materials and final dimensions. Dynamic hands/parts use probes plus the task/key light; probes must cover both the held-item volume and bench. No realtime GI, planar reflections, or reflection-camera-per-panel system.

These are proposed URP configuration targets, **not settings applied in this delivery**. Use Forward initially; benchmark Forward+ only if measured light demand warrants it. Map0/1/2/3 to Low/Medium/High/Ultra in the existing mobile governor. Explicit URP assets set render scale, MSAA, main/additional shadow maps, additional-light modes and renderer features. [Unity URP optimization guidance](https://docs.unity3d.com/6000.3/Documentation/Manual/urp/optimize-for-better-performance.html).

| Setting | LOW | MEDIUM | HIGH | ULTRA |
|---|---|---|---|---|
| Intended frame target |30fps |30/45fps |60fps sustained flagship |60fps opt-in after thermal test |
| Render scale / AA |.70–.80 /2×MSAA or FXAA |.85 /2× |.9–1 /2× |1 /4× only if budget permits |
| Static overheads |Baked |Baked |Baked |Baked |
| Main dynamic key |1, shadows off |1, hard shadows |1, low-cost soft shadows |1,soft shadows |
| Additional realtime lights total / per object |0 /0 |1 /1 |2 /2 |4 /4 |
| Task light shadows |None |None |At most1 spot if main shadow budget reduced |At most1 spot |
| Shadow distance |0m |8m |12m |18m |
| Main atlas / cascades |Off |1024 /1 |2048 /2 |2048 /2 |
| Additional shadow atlas |Off |Off |1024 if enabled |1024 |
| Baked reflection captures |1×64 |2×128 |3×128 |3×256 |
| Realtime probe captures |0 |0 |0 normal play |Explicit rare refresh only; never continuous |
| Reflection blending |Off |Off |Up to2 nearby probes/object |Up to2; box projection where beneficial |
| SSAO |Off |Off |Off default, optional half-res |Half-res low samples if GPU budget permits |
| Bloom |Off |Off or restrained |Low-quality, subtle |Subtle, not a glow blanket |
| Other post |Neutral grading |Neutral grading |Grading, optional tonemap |Same; no motion blur,DOF,chromatic aberration |

Baked counts describe room captures, not simultaneously sampled probes. Monitor/RGB emission illuminates neighboring surfaces through the bake or one carefully limited spill light, not automatically at runtime. Glass uses one transparent surface with edge geometry; low tier simplifies reflections, preserves readable hardware and avoids multiple overlapping alpha layers. Avoid opaque-texture refraction unless a measured hero case needs it. Shadow improvements concentrate on tool/hand/component contact, not long-distance room detail.

## 12. LOD and frame budgets

Targets are **triangles**, not source polygons. Preserve connector silhouettes, holes and contact planes before removing decorative fin/chip detail. Bounds, pivots and markers stay consistent. Hero LOD0 is reserved for the active bench/held item, not every box on a shelf.

| Asset | LOD0 | LOD1 | LOD2 | Starting view-distance policy |
|---|---:|---:|---:|---|
| Hero case including panels, excluding contents |20–35k |9–16k |3–6k |0–1.5m /1.5–4m /4–12m; major silhouette never culled while visible in room |
| Motherboard |25–40k |10–16k |3–5k |0–.9m /.9–2.5m /2.5–5m; internal detail cull beyond6m or opaque enclosure occlusion |
| GPU |18–30k |8–14k |2.5–5k |0–1m /1–3m /3–7m; omit unseen interior fins |
| RAM stick |1.5–3k |.6–1.2k |.15–.35k |0–.7m /.7–2m /2–4m; tiny chips baked beyond near |
| Screw |160–400 |48–96 |12–24 |0–.45m /.45–1.2m /1.2–2m; render cull beyond2–3m, retain interaction |
| Held driver/tool |2–6k |.8–2k |.2–.6k |0–.8m /.8–2.5m /2.5–6m; background cull8–12m |
| Workbench |10–18k |4–8k |1.5–3k |0–2m /2–6m /6–15m; keep room silhouette |
| Normal room prop |.5–5k |.2–2k |.05–.6k |Small0–1.5m /1.5–4m /4–8m; cull8–15m by size |
| PSU / cooler |5–10k /10–20k |2–4k /4–8k |.6–1.2k /1–2k |Similar GPU distances; bake fins/grilles at distance |
| Fan each |1–2k |.4–.8k |.1–.2k |Use blur system for motion; simplify hidden blades |

These distances assume eventual physical scale; current enlarged primitives require calibration. Configure `LODGroup` using projected screen height (start .35/.12/.035; per-asset adjust) and use distances as review aids. In precision mode pin the selected part to a suitable LOD and limit displayed background detail. Use hysteresis; hard switches on Low, short dither only where it doesn't shimmer. Don't fade tiny screws through transparency. Material slots must also fall with LOD; triangle reduction alone doesn't fix draw calls.

| Runtime budget (starting envelope) | Mid-range fallback | Flagship High |
|---|---:|---:|
| Main-view visible geometry |150–300k tris |350–650k tris; <800k brief peak |
| GPU-submitted geometry incl. shadows |<500k |<1.2m; inspect actual passes |
| Draw calls incl. shadows/UI |100–180 |180–300 |
| SetPass / shader-state changes |<60 |<100 |
| Visible unique materials / slots on active PC |25–40 /≤10–12 |40–60 /≤16 |
| Realtime lights |1 main+0–1additional |1 main+≤2additional |
| Reflection capture updates in play |0 |0 |
| Resident texture memory incl. lightmaps/probes |128–220MiB |256–384MiB |
| Render targets additional budget |40–80MiB |80–140MiB; screen-size dependent |
| Total application memory planning cap |~700MiB |~1.2GiB; measure Android PSS/graphics allocation |
| Active skinned meshes / deform bones |0–1 /≤45 |1–2 /≤60 total |
| Animators / RigBuilders |≤1 /≤1 |≤2 /≤1 |
| Simulating rigidbodies |≤8 |≤16; sleeping/kinematic displayed props separate |
| Interactive/physics colliders in active area |≤80 |≤150; no collider on every chip |
| Cable+animation managed allocation after warmup |0B/frame |0B/frame |
| Main thread / GPU target |<20ms /<25ms at30fps |<10ms /<12ms at60fps, leaving thermal margin |

Budgets are coupled: a max-detail populated PC plus two hands can consume150–230k triangles before room geometry; only one assembly should receive this allocation. Don't sum every catalog LOD0 into the frame. Physics uses compound primitive colliders for held objects; installed hardware becomes kinematic/non-simulated. Fine screw hit targets can be larger invisible interaction volumes without increasing physical collision complexity. Keep collision layers for hands/visual cables from colliding with every chip. SRP batching reduces CPU material setup cost, not necessarily draw-call count.

## 13. Import pipeline and acceptance

1. **License verification:** exact item/author/license version, commercial embedded use, collaborator access and public-source redistribution. No editorial/NC/ripped material. If unclear, record `LICENSE MUST BE VERIFIED` and stop acquisition.
2. **Source backup:** receipt, license text, source archive/hash in permitted private art storage; derived FBX/textures tracked with provenance. No unlicensed archive committed to public GitHub.
3. **Units:** metres, positive uniform scale; compare a ruler asset and known component dimensions. Don't normalize cases from clearance fields.
4. **Orientation:** root+Yup/+Zforward; apply DCC transforms; validate tangent handedness/normal green channel.
5. **Pivots:** panel rails/hinges, latch fulcrums, rotor centers, screw axes, tool tip; preserve installation reference pose.
6. **Mesh cleanup:** remove invisible duplicate geometry, fix normals/nonmanifold surfaces, controlled triangulation, bevels at useful screen size. No static geometry joined across moving parts.
7. **URP materials:** convert/bake source renderer graphs, remove unsupported shader dependencies, author shared material presets. Verify metal/roughness channel conversion.
8. **Textures:** Android max size/ASTC/mips/linear flags; replace real logos, labels and baked shadows. Measure memory after import.
9. **Colliders:** primitive/compound, one interaction root; no automatic nonconvex dynamic mesh collider. Keep presentation collider separate from generous touch target.
10. **LODs:** artist-reviewed, keep contact planes/UV seams; cut material slots and hidden geometry, not just decimate. Validate no pivot/size jump.
11. **Prefab:** stable root, visual child, correct shared materials, no source scripts/UE behavior copied into runtime.
12. **Socket markers:** key direction, insertion axis, physical contact plane, dimensions and keep-out volume.
13. **Grip markers:** primary/support palm poses, safe fingers, side handedness; review all major camera angles.
14. **Animation markers:** contact, seat, lock, tool-tip and rest poses; map to existing command results.
15. **Validation:** license gate, scale/tri/material/texture report, missing marker checks, preview under common lighting, assembled-fit test, Android profiling. Only then set an asset binding ready.

Future Editor utilities worth implementing after first real assets: selected-prefab audit (mesh triangles, material slots, shader names, Read/Write and texture importer overrides); marker gizmos/required-marker validator; neutral lighting preview scene; contact-pose scrubber; catalog coverage validator reading **normalized** `HardwareCatalog`; LOD/culler conflict checker. These tools should report issues, not silently resize models or rewrite import settings across the project.

The supplied offline `Tools/validate_art_profiles.py` checks unique IDs, coverage, category correspondence, fingerprint drift, family definitions and LOD budgets. It intentionally does not claim to inspect Unity meshes, licenses or normalized runtime specs. Run `python Tools/validate_art_profiles.py` from the repository root. `--require-ready` is a production gate expected to fail until actual assets and spec reviews exist.

Validation performed:79 profiles across42 reusable mesh families pass the structural check; all79 await assets,39 have explicit specification/fit review blockers. In-memory negative checks reject missing assignments, duplicate IDs, wrong categories, catalog fingerprint drift, increasing LOD budgets and a falsely ready profile. The readiness gate correctly rejects the unfinished catalog. These are offline checks, not Unity tests.

## 14. Work order and completion gates

| Priority | Work | Definition of done |
|---|---|---|
| P0a | Resolve source-branch/spec discrepancies; consistent scale/material test scene; one original ATX case, board, GPU, RAM, PSU, tower, screws/driver; core cable connectors; main bench/room | One fully populated machine, same held/installed model, all existing commands still correct; no root rebuild invalidates an interaction |
| P0b | Hybrid hands and three signature interactions: screw, RAM, cable; task/key lighting; quality mappings | Contact quality reviewed at phone screen size; abort/pause/save tests pass;60fps flagship or documented lower target sustained |
| P1 | Expand modular catalog; CPU lever, GPU latch,M.2,panel,AIO; specialist phone/laptop/NAS board workspace; packaged tools | All79 profiles covered by actual assets; form factors/ports match authoritative specs; repair states legible |
| P2 | Damage masks, fan blur/RGB, restrained bloom, parcel/drawer animation, authored contact sounds, probe/solder work | Distinct failure/repair feedback with accessibility parity; no meaningful GPU regression |
| P3 | Bag crumple, extra PCB variants, fine cable breakout, interior HDD service detail, cosmetic chair/monitor-arm motion | Added only when screen impact justifies cost; no new gameplay dependency |

Minimum vertical slice capture set: closed case, opened case, underside GPU/PCIe contact, RAM latch, screwdriver screw engagement, power connector mouth, two-hand panel on bench, wide room, phone battery disconnect, board probe contact. Compare in the same lighting with neutral exposure. Obtain artist sign-off on these views before mass-producing variants.

Artist/animator still needed: mechanical hardware kit, specialist device internals, clean room/cabinet kit, exact connector heads, tool family, grip/clip library, retopo/UV/LOD work, fictional label graphics, PBR authoring and contact sound recording. A rendering system or registry alone cannot supply this quality.

Android validation still needed: Unity6000.3.15f1 import and ARM64/IL2CPP builds; Vulkan and GLES3 fallback if supported; at least one modern Adreno and one Mali/Immortalis flagship plus a mid-range device;20–30minute thermal soak; active populated case with glass and hands; inventory open/close and repeated100install/remove cycles; save/pause/low-memory interruption during every action phase; duplicate taps and low-FPS skipped-frame commits; orientation/safe-area/touch occlusion; shader/ASTC fallback; LOD shimmer, cable bend/contact and fan aliasing; reduced-motion/hands-off parity. Capture CPU/GPU timings, memory/PSS, allocations, draw calls and submitted triangles. None of these tests was run in this session.

## 15. Delivered files and limits

- `Docs/ART_ASSET_PLAN.md` — this researched plan, license-aware shortlist, explicit sourcing gaps and integration specification.
- `Docs/hardware_visual_profiles.json` —79 stable art-intent assignments, family budgets and source fingerprint; no external assets or runtime bindings.
- `Tools/validate_art_profiles.py` — offline authoring manifest checks and readiness gate.

**Unity implementation completed: none.** No C# gameplay, prefabs, scenes, hardware definitions or packages were changed. This is deliberate: the inspected branch lacks four named integration classes, and there is no imported asset or Unity runtime here against which to verify a new visual adapter. The concrete implementation delivered is the manifest/validator, not a claim that the proposed Unity architecture exists.

Sourcing remains incomplete for exact two-asset pairs in several categories, especially connectors, AIO sizes, modern specialist instruments and repairable device interiors. Those are explicit custom-production jobs, not approved downloads. The next purchase decision should be based on the small vertical slice and supplier technical/license evidence, not a large collection of incompatible assets.
