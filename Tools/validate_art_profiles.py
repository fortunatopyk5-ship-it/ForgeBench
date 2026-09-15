#!/usr/bin/env python3
"""Validate the offline art manifest; never modify gameplay data or Unity assets.

Run from any directory. --require-ready additionally rejects unresolved profiles.
This checks authoring records, not mesh quality, licensing or Unity runtime behavior.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import sys


def validate(root, manifest, require_ready=False):
    errors = []
    if manifest.get("schemaVersion") != 1:
        return ["Unsupported manifest schemaVersion"], 0, 0
    # Do not let a manifest redirect the check to a different catalog.
    catalog_path = root / "Assets/Resources/Data/hardware.json"
    raw = catalog_path.read_bytes()
    parts = json.loads(raw)["parts"]
    if manifest.get("catalogPath") != "Assets/Resources/Data/hardware.json":
        errors.append("catalogPath must reference the real hardware catalog")
    actual_catalog_sha = hashlib.sha256(raw).hexdigest()
    expected_catalog_sha = manifest.get("catalogSha256")
    if actual_catalog_sha != expected_catalog_sha:
        errors.append(
            "Catalog fingerprint changed: expected " + str(expected_catalog_sha)
            + ", actual " + actual_catalog_sha
            + ". Review visual intent before updating the fingerprint"
        )

    domain = (root / "Assets/Scripts/Core/DomainModels.cs").read_text(encoding="utf-8")
    match = re.search(r"enum\s+PartCategory\s*\{([^}]+)\}", domain)
    if not match:
        return errors + ["Cannot locate PartCategory enum"], 0, 0
    categories = [x.strip() for x in match.group(1).split(",") if x.strip()]
    if any(not re.fullmatch(r"[A-Za-z][A-Za-z0-9_]*", x) for x in categories):
        return errors + ["PartCategory enum syntax changed; review validator"], 0, 0

    catalog = {}
    for part in parts:
        key = part["id"].casefold()
        if key in catalog:
            errors.append("Duplicate catalog ID: " + part["id"])
        catalog[key] = part

    families = manifest.get("families")
    profiles = manifest.get("profiles")
    if not isinstance(families, dict) or not isinstance(profiles, list):
        return errors + ["families must be an object and profiles a list"], 0, 0
    for name, family in families.items():
        if not isinstance(family, dict):
            errors.append(f"{name}: family must be an object")
            continue
        if family.get("category") not in categories:
            errors.append(f"{name}: invalid category")
        budgets = family.get("lodTriangleCeilings")
        if (not isinstance(budgets, list) or len(budgets) != 3
                or any(type(v) is not int or v <= 0 for v in budgets)):
            errors.append(f"{name}: require three positive integer triangle ceilings")
        elif not budgets[0] > budgets[1] > budgets[2]:
            errors.append(f"{name}: LOD ceilings must decrease")
        if family.get("maxTextureSize") not in (256, 512, 1024, 2048):
            errors.append(f"{name}: texture exceeds approved size set")

    seen_defs, seen_profiles = set(), set()
    pending, blocked = 0, 0
    for index, profile in enumerate(profiles):
        if not isinstance(profile, dict):
            errors.append(f"profiles[{index}]: must be an object")
            continue
        definition = profile.get("definitionId", "")
        pid = profile.get("profileId", "")
        if not isinstance(definition, str) or not isinstance(pid, str):
            errors.append(f"profiles[{index}]: IDs must be strings")
            continue
        key = definition.casefold()
        if key in seen_defs:
            errors.append(f"{definition}: duplicate definition assignment")
        if pid.casefold() in seen_profiles:
            errors.append(f"{definition}: duplicate profileId {pid}")
        seen_defs.add(key)
        seen_profiles.add(pid.casefold())
        if not re.fullmatch(r"fb\.hw\.[a-z0-9_]+\.v[1-9][0-9]*", pid):
            errors.append(f"{definition}: invalid stable profileId")
        part = catalog.get(key)
        if part is None:
            errors.append(f"{definition}: not present in hardware catalog")
        else:
            category_index = part["category"]
            if type(category_index) is not int or not 0 <= category_index < len(categories):
                errors.append(f"{definition}: invalid numeric category in hardware catalog")
            elif profile.get("category") != categories[category_index]:
                errors.append(f"{definition}: category disagrees with DomainModels/catalog")
            for field, source_field in (("brand", "brand"), ("label", "model")):
                if profile.get(field) != part.get(source_field):
                    errors.append(f"{definition}: stale {field}")
        family = families.get(profile.get("familyId"))
        if not isinstance(family, dict):
            errors.append(f"{definition}: missing family")
        elif family.get("category") != profile.get("category"):
            errors.append(f"{definition}: family category mismatch")
        for field in ("variantId", "materialPreset"):
            if not isinstance(profile.get(field), str) or not profile[field].strip():
                errors.append(f"{definition}: missing {field}")
        if not isinstance(profile.get("artIntent"), dict):
            errors.append(f"{definition}: artIntent must be an object")
        markers = profile.get("requiredMarkers")
        if (not isinstance(markers, list) or not markers
                or any(not isinstance(x, str) or not x for x in markers)):
            errors.append(f"{definition}: requiredMarkers must be nonempty strings")
        elif len(set(markers)) != len(markers):
            errors.append(f"{definition}: duplicate marker")
        blockers = profile.get("reviewBlockers")
        if not isinstance(blockers, list) or any(not isinstance(x, str) or not x for x in blockers):
            errors.append(f"{definition}: reviewBlockers must be a string list")
        elif blockers:
            blocked += 1
        status = profile.get("assetStatus")
        if status not in ("requires_asset", "in_review", "ready"):
            errors.append(f"{definition}: invalid assetStatus")
        if status != "ready":
            pending += 1
        else:
            if blockers:
                errors.append(f"{definition}: ready profile still has review blockers")
            prefab = profile.get("prefabPath")
            if (not isinstance(prefab, str) or not prefab.startswith("Assets/")
                    or not prefab.endswith(".prefab") or ".." in Path(prefab).parts):
                errors.append(f"{definition}: ready profile requires an Assets/*.prefab path")
            elif not (root / prefab).is_file():
                errors.append(f"{definition}: assigned prefab does not exist")
    for key in sorted(set(catalog) - seen_defs):
        errors.append(f"{key}: missing visual assignment")
    if require_ready and pending:
        errors.append(f"Readiness gate: {pending} profiles still require assets or review")
    return errors, pending, blocked


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--manifest", type=Path, help="Optional manifest for review/CI checks")
    parser.add_argument("--require-ready", action="store_true")
    args = parser.parse_args()
    path = args.manifest or args.root / "Docs/hardware_visual_profiles.json"
    try:
        manifest = json.loads(path.read_text(encoding="utf-8"))
        if not isinstance(manifest, dict):
            raise ValueError("Manifest root must be an object")
        errors, pending, blocked = validate(args.root, manifest, args.require_ready)
    except (OSError, ValueError, KeyError, TypeError, AttributeError) as exc:
        print(f"ERROR: Cannot validate authoring manifest: {exc}", file=sys.stderr)
        return 1
    for error in errors:
        print("ERROR: " + error, file=sys.stderr)
    print(f"{len(manifest.get('profiles', []))} profiles; {pending} pending assets/review; "
          f"{blocked} with specification/fit review blockers; {len(errors)} errors.")
    print("This result does not certify mesh quality, licensing, Unity import or device performance.")
    return 1 if errors else 0


if __name__ == "__main__":
    raise SystemExit(main())
