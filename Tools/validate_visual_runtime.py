#!/usr/bin/env python3
from __future__ import annotations
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors: list[str] = []
checks: list[str] = []


def text(rel: str) -> str:
    p = ROOT / rel
    if not p.is_file():
        errors.append(f"missing {rel}")
        return ""
    return p.read_text(encoding="utf-8")


required = [
    "Assets/Scripts/Production/ProceduralCoolingVisuals.cs",
    "Assets/Scripts/Production/CoolingVisualUpgradeDirector.cs",
    "Assets/Scripts/Production/ProceduralGeneratedMeshOwner.cs",
    "Assets/Scripts/Production/ProceduralSurfaceLibrary.cs",
    "Assets/Scripts/Production/HardwareSurfaceUpgradeDirector.cs",
    "Assets/Scripts/Production/HardwarePresentationLayout.cs",
    "Assets/Scripts/Editor/CoolingVisualBuildGate.cs",
    "Assets/Tests/EditMode/ProceduralCoolingVisualTests.cs",
    "Assets/Tests/EditMode/ProceduralSurfaceLibraryTests.cs",
    "Docs/hardware_visual_profiles.json",
    "Tools/validate_art_profiles.py",
]
for rel in required:
    if not (ROOT / rel).is_file(): errors.append(f"missing {rel}")
if not errors: checks.append(f"{len(required)} visual runtime/build/test files present")

cooling = text("Assets/Scripts/Production/CoolingVisualUpgradeDirector.cs")
layout = text("Assets/Scripts/Production/HardwarePresentationLayout.cs")
surfaces = text("Assets/Scripts/Production/ProceduralSurfaceLibrary.cs")
surface_director = text("Assets/Scripts/Production/HardwareSurfaceUpgradeDirector.cs")
mesh_owner = text("Assets/Scripts/Production/ProceduralGeneratedMeshOwner.cs")
gate = text("Assets/Scripts/Editor/CoolingVisualBuildGate.cs")

# Current PhysicalAssemblyController naming contract. This prevents a stale visual branch from
# silently hiding no renderers after the runtime presentation evolves.
for token in [
    '"ProductionMachine3D"', '"AIOPumpBlock"', '"AIORadiator"', '"AIOFan_"',
    '"CaseFanFrame_"', '"CaseFanRotor_"', '"GPUFan_"', '"PSUFan"'
]:
    if token not in cooling: errors.append(f"CoolingVisualUpgradeDirector missing current runtime binding {token}")
if not any("current runtime binding" in e for e in errors):
    checks.append("cooling upgrade uses current physical-renderer naming contract")

for token in ["RuntimeRenderBudget.Changed", "RuntimeRenderBudget.CurrentTier", "ClearGenerated(true)",
              "renderer.enabled = true", "ProceduralGeneratedMeshOwner", "DistanceCullHint"]:
    if token not in cooling: errors.append(f"cooling upgrade missing lifecycle token {token}")
if not any("lifecycle token" in e for e in errors):
    checks.append("cooling upgrade has quality/lifecycle restoration hooks")

for token in ["ResolveRadiatorMm", "ResolveFanMm", "AioFanCount", "StringComparison.OrdinalIgnoreCase"]:
    if token not in layout: errors.append(f"presentation layout missing {token}")
if not any("presentation layout missing" in e for e in errors):
    checks.append("presentation layout has authored-size identity resolution")

if "UnityEngine.Random" in surfaces or "Random.Range" in surfaces:
    errors.append("procedural surfaces must remain deterministic; Unity random source found")
for token in ["TextureWrapMode.Repeat", "HideFlags.DontSave", "Apply(true, true)", "public static Color32 Sample"]:
    if token not in surfaces: errors.append(f"procedural surface library missing {token}")
if not any("procedural surface library missing" in e for e in errors):
    checks.append("procedural surfaces are deterministic, tileable and non-persistent")

for token in ["Restore()", "renderer.sharedMaterial = upgraded", "pair.Key.sharedMaterial = pair.Value",
              "renderQueue", "sharedMaterials.Length != 1"]:
    if token not in surface_director: errors.append(f"surface upgrade missing non-destructive guard {token}")
if not any("non-destructive guard" in e for e in errors):
    checks.append("surface upgrade preserves/restores authoritative materials")

for mesh_name in ["ForgeBench_RadiatorFins", "ForgeBench_TowerFins", "ForgeBench_AioTube"]:
    if mesh_name not in mesh_owner: errors.append(f"generated mesh lifetime guard missing {mesh_name}")
for shared_name in ["ForgeBench_FanBlade", "ForgeBench_FanShroud"]:
    if shared_name in mesh_owner: errors.append(f"generated mesh owner must not destroy cached shared mesh {shared_name}")
if not any("mesh" in e.lower() for e in errors):
    checks.append("unique procedural meshes have explicit lifetime ownership")

for rel in [
    "Assets/Scripts/Production/ProceduralSurfaceLibrary.cs.meta",
    "Assets/Scripts/Production/HardwareSurfaceUpgradeDirector.cs.meta",
    "Assets/Scripts/Production/ProceduralGeneratedMeshOwner.cs.meta",
    "Assets/Tests/EditMode/ProceduralSurfaceLibraryTests.cs.meta",
]:
    meta = text(rel)
    if not re.search(r"(?m)^guid: [0-9a-f]{32}$", meta): errors.append(f"invalid Unity meta guid: {rel}")

# Detect duplicate GUIDs repository-wide; duplicated script GUIDs can silently remap serialized refs.
guids: dict[str, str] = {}
for p in ROOT.glob("Assets/**/*.meta"):
    m = re.search(r"(?m)^guid: ([0-9a-f]{32})$", p.read_text(encoding="utf-8", errors="replace"))
    if not m: continue
    guid = m.group(1)
    rel = str(p.relative_to(ROOT))
    if guid in guids: errors.append(f"duplicate Unity GUID {guid}: {guids[guid]} and {rel}")
    else: guids[guid] = rel
if not any("duplicate Unity GUID" in e for e in errors): checks.append(f"{len(guids)} Unity asset GUIDs unique")

for token in ["ProceduralSurfaceLibrary.cs", "HardwareSurfaceUpgradeDirector.cs", "ProceduralGeneratedMeshOwner.cs"]:
    if token not in gate: errors.append(f"production visual build gate does not require {token}")
if not any("production visual build gate" in e for e in errors): checks.append("production build gate covers visual runtime fallback stack")

print("ForgeBench visual runtime validation")
for c in checks: print("PASS:", c)
for e in errors: print("FAIL:", e)
if errors:
    print(f"RESULT: FAIL ({len(errors)} issue(s))")
    sys.exit(1)
print(f"RESULT: PASS ({len(checks)} checks)")
