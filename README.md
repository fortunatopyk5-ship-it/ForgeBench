# ForgeBench — Unity 6 Android PC & Electronics Workshop Simulator

ForgeBench is an original offline-first 3D workshop simulator source project created from the supplied 520-requirement specification. It is **not** a copy of PC Building Simulator or any other commercial game: brands, UI copy, hardware names, world layout, sounds and procedural geometry in this project are original/place-independent.

## What is actually implemented in this source snapshot

A playable source-first vertical slice exists end-to-end in code:

1. Start a new workshop with cash, tools and consumables.
2. Accept a procedurally generated customer contract.
3. Browse a data-driven fictional hardware store (79 definitions) and order unique item instances.
4. Advance the calendar, pay recurring costs, track shipments, receive delivered boxes into inventory.
5. Put the contract machine on the workshop bench and install compatible case/board/CPU/RAM/GPU/storage/PSU/cooler/fans.
6. Apply thermal compound, clean dust and route power/data/front-panel/fan cables.
7. Power on and receive real domain POST results/codes for missing RAM, power, video, fan, thermals and critical faults.
8. Enter the tablet BIOS page to enable the memory performance profile and choose fan behavior.
9. Install ForgeOS after POST, then install chipset/GPU/network drivers.
10. Run a deterministic benchmark/stress simulation driven by actual installed component data, PSU headroom, dust, cooling, BIOS settings and cable-management score.
11. Validate the job centrally against mandatory target score/stability/OS/drivers/cleanliness and optional noise/deadline goals.
12. Submit the completed job, receive money/reputation/XP, unlock workshop upgrades, and continue.
13. Autosave during important state transitions; manually save/load slot 1; save format is versioned and uses temp-file + backup recovery.
14. Walk through a procedurally built 3D workshop using touch joystick/look controls, or keyboard/mouse; gamepad movement/right-stick look is also wired through legacy Unity axes.

The source also includes diagnostic scans, component wear/fault fields, dust, thermal/power simulation, customer-owned/reserved item state, multiple device categories in the domain, workshop progression, EN/UK localization data, accessibility settings, haptic/audio feedback, crash logging, EditMode tests and a 520-item traceability matrix.

## Important verification status

This environment did **not** contain Unity Editor, Unity Hub, Android SDK/NDK or a C# compiler. Therefore this delivery does **not** claim a successful Unity compile, Play Mode run, real-device run or APK build. The project is configured to self-complete Unity/URP/Android settings when opened in the correct editor, but that step still needs to be run and verified on a machine with Unity installed.

The supplied specification explicitly says not to invent a successful build when the toolchain is absent. `Docs/requirements_index.json` and `Docs/PROMPT_ANALYSIS.md` distinguish implemented source work from partial, data-only, simulated and unverified requirements. In particular, a number of the 520 requirements (for example full screw-by-screw 3D fastener gameplay, production-grade laptop/phone repair scenes, full RGB customization, final Android performance/device acceptance) are not dishonestly marked complete.

## Required Unity version

- Unity **6000.3.15f1** (Unity 6.3 LTS family)
- Android Build Support module
- Android SDK & NDK Tools module
- OpenJDK module
- URP package (declared in `Packages/manifest.json`)
- Target: Android, ARM64, IL2CPP, landscape

## Open and run

1. Install Unity 6000.3.15f1 with Android Build Support, SDK/NDK and OpenJDK.
2. Open the folder `ForgeBench_Unity6_Android` as a Unity project.
3. Let Unity resolve packages and reimport assets.
4. `ForgeBenchEditorSetup` runs automatically once. You can also run **ForgeBench → Configure Project**.
5. Open `Assets/Scenes/Workshop.unity` if Unity did not open it automatically.
6. Enter Play Mode.

If the hand-authored bootstrap scene does not import on your Unity patch, the editor configurator detects the invalid/missing SceneAsset and recreates a clean scene containing the bootstrap anchor; all gameplay/world/UI content is created by runtime bootstrap code.

## Build an APK

Use **ForgeBench → Build Android APK**. The output path is:

`Builds/Android/ForgeBench.apk`

Equivalent batch-mode command (adjust the Unity executable path):

```bash
Unity -batchmode -quit -projectPath "/path/to/ForgeBench_Unity6_Android" \
  -executeMethod ForgeBench.EditorTools.ForgeBenchEditorSetup.BuildAndroid \
  -logFile "Builds/android-build.log"
```

Run EditMode tests from **Window → General → Test Runner**, or in batch mode:

```bash
Unity -batchmode -quit -projectPath "/path/to/ForgeBench_Unity6_Android" \
  -runTests -testPlatform EditMode \
  -testResults "Builds/editmode-results.xml" \
  -logFile "Builds/editmode.log"
```

## Controls

### Android/touch
- Left virtual joystick: move
- Drag right side: camera
- INTERACT: use focused workshop station/object
- Hold PRECISION: slower movement for close work
- Tablet tabs: contracts, store, inventory, bench, BIOS, OS, diagnostics, progression, settings

### Keyboard/mouse
- WASD / arrows: move
- Mouse: look (click game view to lock pointer)
- E: interact
- Left Ctrl: precision movement
- Left Shift: faster movement
- Esc: release pointer

### Gamepad
- Left stick: move
- Right stick: look

## Project map

- `Assets/Scripts/Core/` — serializable data models, catalog, event bus, localization
- `Assets/Scripts/Simulation/` — compatibility, power/thermal, POST, benchmark, economy, shipping, jobs, diagnostics
- `Assets/Scripts/Runtime/` — lifecycle, save/load, feedback, crash logs
- `Assets/Scripts/World/` — first-person controls, interaction raycast, procedural workshop and active PC visualization
- `Assets/Scripts/UI/` — runtime tablet/HUD/touch UI
- `Assets/Scripts/Editor/` — Unity/URP/Android configuration and APK build command
- `Assets/Resources/Data/hardware.json` — fictional data-driven hardware database
- `Assets/Resources/Localization/` — English/Ukrainian strings
- `Assets/Tests/EditMode/` — core domain tests
- `Docs/requirements_index.json` — all 520 original requirement titles + implementation/status mapping
- `Docs/PROMPT_ANALYSIS.md` — section-by-section prompt analysis and traceability notes
- `Tools/validate_project.py` — static project/content integrity gate runnable without Unity
- `Tools/smoke_balance_test.py` — brute-force early-game compatibility/budget/benchmark solvability check

## Known source-level limitations in this snapshot

The project intentionally reports its current limits instead of replacing them with fake UI. Some specification areas are represented in the domain/data but are not yet production-complete interactive 3D systems, especially detailed fastener/panel manipulation, custom liquid-loop building, full board microsoldering gameplay, specialized laptop/phone/tablet/console/NAS repair scenes, large-scale store virtualization, full RGB/cosmetic editing, production audio/art assets, occlusion/LOD authoring, and real-device profiling/thermal acceptance.

Those items are visible in `Docs/requirements_index.json`; they should be treated as remaining work until Unity/device tests prove them.

## License / originality note

All invented hardware names and generated source/data in this delivery are intended as original project material. No third-party branded hardware models, commercial-game UI, protected music, models, missions or copied assets are included.
