# ForgeBench runtime visual polish pass

This pass upgrades the current runtime-authored workshop without changing gameplay authority or save data.

Implemented in this branch:

- higher-detail procedural fan, AIO and tower-cooler fallback geometry;
- current-main-aware cooling replacement that preserves the original colliders and interactables;
- deterministic generated albedo + tangent-space normal fallback textures for graphite powder coat, brushed aluminium, ESD teal, PCB, copper, black plastic, rubber, warm paint, concrete and labels;
- semantic material assignment to the workshop and rebuilt `ProductionMachine3D` hierarchy;
- Android-aware texture resolution (64 px generated fallback maps on mobile, 128 px elsewhere);
- pure EditMode coverage for surface classification/noise plus the cooling resolver tests;
- a build preprocessor gate that refuses a production build if the visual fallback/test/art-manifest files disappear.

The generated textures are original code-generated fallback assets, not replacements for final authored PBR scans. The CC0 research document remains the sourcing reference for later production materials. Runtime generation is intentionally small and shared: it does not allocate textures per component or per frame.

## Integration safety

`CoolingVisualUpgradeDirector` hides only the primitive renderers it supersedes. Existing component objects, colliders, `WorldInteractable` components, machine state, inventory references and save reconstruction remain untouched. The director rebuilds only when the active cooling/GPU/fan signature or `ProductionMachine3D` root changes.

`VisualMaterialUpgradeDirector` applies shared generated materials when a workshop scene appears or when the physical machine root is rebuilt. It explicitly preserves ghost placement material, power-button emission, thermal paste, RGB accents and the existing side-panel material.

## Still requires real Unity/device verification

Static/source review is not a substitute for Unity compilation or an Android device run. Before calling the visuals release-ready, verify:

1. Unity 6000.3.15f1 compilation and EditMode execution.
2. Air cooler and 240/280/360-class AIO visual orientation.
3. Front/rear/top case-fan orientation on all case layouts.
4. GPU fan placement for one/two/three-fan cards.
5. No loss of interaction raycasts after primitive renderers are hidden.
6. Android frame time, memory, thermals and background/resume behavior.
7. Transparent side-panel appearance remains controlled by the existing assembly renderer.

Final AAA art still requires authored meshes, UVs, decals, real PBR maps, hands/animations and device profiling. This pass improves the functional fallback and material coherence; it does not claim GTA-scale asset production.
