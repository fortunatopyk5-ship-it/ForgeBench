#!/usr/bin/env python3
from pathlib import Path
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
errors = []
passes = []

def require_file(path):
    p = ROOT / path
    if not p.is_file():
        errors.append(f"missing {path}")
        return ""
    return p.read_text(encoding="utf-8")

def require_tokens(label, text, tokens):
    missing = [token for token in tokens if token not in text]
    if missing:
        errors.append(f"{label}: missing tokens: {', '.join(missing)}")
    else:
        passes.append(label)

physical = require_file("Assets/Scripts/Production/PhysicalAssemblyController.cs")
cooling = require_file("Assets/Scripts/Production/CoolingVisualUpgradeDirector.cs")
geometry = require_file("Assets/Scripts/Production/ProceduralCoolingVisuals.cs")
surfaces = require_file("Assets/Scripts/Production/ProceduralSurfaceLibrary.cs")
material_director = require_file("Assets/Scripts/Production/VisualMaterialUpgradeDirector.cs")
surface_tests = require_file("Assets/Tests/EditMode/ProceduralSurfaceLibraryTests.cs")
cooling_tests = require_file("Assets/Tests/EditMode/ProceduralCoolingVisualTests.cs")
gate = require_file("Assets/Scripts/Editor/VisualPolishBuildGate.cs")
cc0 = require_file("Docs/VERIFIED_CC0_TEXTURE_SET.md")

require_tokens("physical presentation contract", physical, [
    'ProductionMachine3D', 'AIOPumpBlock', 'AIORadiator', 'AIOFan_', 'AIOTube',
    'CoolerHeatsink', 'CoolerFan', 'CaseFanFrame_', 'CaseFanRotor_', 'GPUFan_'
])
require_tokens("cooling director contract", cooling, [
    'ProductionMachine3D', 'AIOPumpBlock', 'AIORadiator', 'CaseFanFrame_',
    'CaseFanRotor_', 'GPUFan_', 'DestroyRuntimeMeshes', 'VisualRevision'
])
require_tokens("procedural geometry contract", geometry, [
    'BuildFan', 'BuildAio', 'BuildAirCooler', 'ResolveRadiatorMm',
    'ResolveFanMm', 'TubeMesh', 'FanBladeMesh', 'TorusMesh'
])
require_tokens("surface fallback contract", surfaces, [
    'GraphitePowderCoat', 'BrushedAluminum', 'EsdTeal', 'PcbGreen', 'Copper',
    'BlackPlastic', 'Rubber', 'WarmWhitePaint', 'Concrete', 'Noise01',
    'TextureFormat.RGBA32', '_BumpMap'
])
require_tokens("material lifecycle contract", material_director, [
    'CoolingVisualUpgradeDirector.VisualRevision', 'ProductionMachine3D',
    'ApplyWorkshopSurfaces', 'ApplyMachineSurfaces', 'ShouldPreserveDynamicMaterial'
])
require_tokens("visual build gate", gate, [
    'ProceduralCoolingVisuals.cs', 'ProceduralSurfaceLibrary.cs',
    'ProceduralSurfaceLibraryTests.cs', 'VERIFIED_CC0_TEXTURE_SET.md'
])

if 'SetActive(false)' in cooling:
    errors.append('cooling director must not disable authoritative gameplay objects with SetActive(false)')
else:
    passes.append('cooling replacement preserves gameplay objects')

if not re.search(r'Application\.isMobilePlatform\s*\?\s*64\s*:\s*128', material_director):
    errors.append('mobile/desktop generated texture budget must remain explicit at 64/128')
else:
    passes.append('generated texture budget')

for dynamic in ('ghost', 'powerbutton', 'thermalpaste', 'rgbring', 'sidepanel'):
    if dynamic not in material_director.lower():
        errors.append(f'dynamic material preservation missing: {dynamic}')
if not any(x.startswith('dynamic material preservation') for x in errors):
    passes.append('dynamic material preservation')

if '[Test' not in surface_tests or 'Noise_IsDeterministicAndBounded' not in surface_tests:
    errors.append('surface tests are missing deterministic noise coverage')
else:
    passes.append('surface test source')
if '[Test' not in cooling_tests or 'ExplicitRadiatorIdentity_WinsOverNormalizedFallback' not in cooling_tests:
    errors.append('cooling tests are missing normalized-radiator regression coverage')
else:
    passes.append('cooling test source')

if 'CC0' not in cc0 or 'This file does **not** mean the textures are already imported into Unity' not in cc0:
    errors.append('CC0 texture notes must preserve licensing scope and imported/not-imported disclaimer')
else:
    passes.append('CC0 licensing scope')

print('ForgeBench visual runtime validation')
for item in passes:
    print('[PASS] ' + item)
for item in errors:
    print('[FAIL] ' + item, file=sys.stderr)
print(f'PASS={len(passes)} FAIL={len(errors)}')
print('This check validates source integration contracts only; it is not Unity compilation or device profiling.')
raise SystemExit(1 if errors else 0)
