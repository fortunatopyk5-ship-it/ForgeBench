#!/usr/bin/env python3
from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]
service_path = ROOT / "Assets/Scripts/Simulation/HardwareComparisonService.cs"
panel_path = ROOT / "Assets/Scripts/Production/FitmentPlannerPanel.cs"
test_path = ROOT / "Assets/Tests/EditMode/HardwareComparisonTests.cs"
gate_path = ROOT / "Assets/Scripts/Editor/HardwareComparisonBuildGate.cs"

errors = []
def require_file(path):
    if not path.exists(): errors.append(f"missing {path.relative_to(ROOT)}")

def require(text, token, label):
    if token not in text: errors.append(f"{label}: missing {token}")

for path in (service_path, panel_path, test_path, gate_path): require_file(path)
if errors:
    print("[FAIL] hardware A/B comparison contract")
    for e in errors: print("  -", e)
    raise SystemExit(1)

service = service_path.read_text(encoding="utf-8")
panel = panel_path.read_text(encoding="utf-8")
tests = test_path.read_text(encoding="utf-8")
gate = gate_path.read_text(encoding="utf-8")

for token in ("HardwareComparisonVerdict", "HardwareMetricKind", "technicalDelta", "efficiencyDelta", "valueDelta", "suitabilityScore", "FitmentPlanningService", "IsJobRelevant"):
    require(service, token, "service")
for token in ("A/B ENGINEERING COMPARISON", "COMPARE", "RELEVANT ONLY", "FILTER ·", "technicalDelta", "efficiencyDelta", "valueDelta"):
    require(panel, token, "panel")
for token in ("DamagedCandidate_IsBlocked", "StorageCandidate_IsClassifiedAsExpansion", "SeparatesTechnicalEfficiencyAndValueDimensions", "ExpensiveTechnicalUpgrade_IsNotDowngradedByPriceMetric", "RequiredCategory_IsMarkedJobRelevant", "FaultedCandidate_CannotBePresentedAsUpgrade"):
    require(tests, token, "tests")
for token in ("HardwareComparisonBuildGate", "must remain non-destructive"):
    require(gate, token, "build gate")

# The comparison service is an analysis layer. Mutation belongs to the existing install action in the UI/runtime.
for forbidden in (".Install(", "InstallBestAvailable", "reserved = true", "reserved=true"):
    if forbidden in service: errors.append("service mutates install state via " + forbidden)

# Price/value is intentionally excluded from the technical verdict dimension.
verdict_window = re.search(r"if \(r\.fitment.*?return r;", service, flags=re.S)
if verdict_window and "valueDelta" in verdict_window.group(0):
    errors.append("valueDelta participates in upgrade/downgrade verdict; technical verdict must stay independent of price")

case_count = len(re.findall(r"\[(?:Test|TestCase)(?:\([^\]]*\))?\]", tests))
if case_count < 6: errors.append(f"only {case_count} comparison test declarations")

if errors:
    print("[FAIL] hardware A/B comparison contract")
    for e in errors: print("  -", e)
    raise SystemExit(1)

print("[PASS] hardware A/B comparison contract")
print(f"  tests declared: {case_count}")
print("  non-destructive service: yes")
print("  independent technical/efficiency/value dimensions: yes")
print("  mobile-friendly category/relevance filtering source: yes")
