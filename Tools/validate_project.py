#!/usr/bin/env python3
from __future__ import annotations
import json, re, sys
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
errors=[];warnings=[];checks=[]
def ok(n,d=''): checks.append((n,d))
def fail(n,d): errors.append((n,d))
def warn(n,d): warnings.append((n,d))
def loadj(rel):
    try:return json.loads((ROOT/rel).read_text(encoding='utf-8'))
    except Exception as e:fail('JSON '+rel,str(e));return None

required=[
'Assets/Scenes/Workshop.unity','Assets/Resources/Data/hardware.json','Assets/Resources/Localization/en.json','Assets/Resources/Localization/uk.json','Assets/link.xml',
'Assets/Scripts/Core/DomainModels.cs','Assets/Scripts/Simulation/GameSimulation.cs','Assets/Scripts/Runtime/GameRuntime.cs','Assets/Scripts/Runtime/SaveService.cs','Assets/Scripts/World/WorkshopWorld.cs','Assets/Scripts/UI/GameUI.cs',
'Assets/Scripts/Simulation/EngineeringSimulationService.cs','Assets/Scripts/Simulation/SpecialistRepairServices.cs','Assets/Scripts/Simulation/SpecialistJobService.cs','Assets/Scripts/Simulation/WorkshopBusinessService.cs','Assets/Scripts/Simulation/SupplyChainService.cs','Assets/Scripts/Simulation/AdvancedJobGeneratorService.cs','Assets/Scripts/Simulation/CustomerRelationsService.cs','Assets/Scripts/Simulation/ProgressionService.cs','Assets/Scripts/Simulation/ReliabilitySimulationService.cs','Assets/Scripts/Simulation/PreflightInspectionService.cs','Assets/Scripts/Simulation/StaffRosterService.cs','Assets/Scripts/Simulation/WarrantyService.cs',
'Assets/Scripts/Runtime/SpecialistRuntimeExtensions.cs','Assets/Scripts/Runtime/EngineeringRuntimeExtensions.cs','Assets/Scripts/Runtime/BusinessRuntimeExtensions.cs','Assets/Scripts/Runtime/SupplyChainRuntimeExtensions.cs','Assets/Scripts/Runtime/MaintenanceRuntimeExtensions.cs',
'Assets/Scripts/Production/ProductionBootstrap.cs','Assets/Scripts/Production/MainMenuController.cs','Assets/Scripts/Production/WorkshopProductionLayer.cs','Assets/Scripts/Production/PhysicalAssemblyController.cs','Assets/Scripts/Production/ObjectHandlingController.cs','Assets/Scripts/Production/MobileInteractionBridge.cs','Assets/Scripts/Production/RuntimeQualityController.cs','Assets/Scripts/Production/TutorialDirector.cs','Assets/Scripts/Production/ProductionAudio.cs',
'Assets/Scripts/Production/SpecialistRepairPanel.cs','Assets/Scripts/Production/SpecialistStationsLayer.cs','Assets/Scripts/Production/SpecialistContractBoard.cs','Assets/Scripts/Production/SpecialistContractTerminalLayer.cs','Assets/Scripts/Production/SpecialistLifecycle.cs','Assets/Scripts/Production/SpecialistDeviceVisuals.cs','Assets/Scripts/Production/EngineeringDiagnosticsPanel.cs','Assets/Scripts/Production/EngineeringTerminalLayer.cs',
'Assets/Scripts/Production/WorkshopBusinessPanel.cs','Assets/Scripts/Production/WorkshopManagementTerminalLayer.cs','Assets/Scripts/Production/WorkshopBusinessLifecycle.cs','Assets/Scripts/Production/SupplyChainPanel.cs','Assets/Scripts/Production/SupplyChainTerminalLayer.cs','Assets/Scripts/Production/AdvancedJobLifecycle.cs',
'Assets/Scripts/Production/CustomerRelationsDirector.cs','Assets/Scripts/Production/CustomerRelationsPanel.cs','Assets/Scripts/Production/CustomerRelationsTerminalLayer.cs','Assets/Scripts/Production/ProgressionDirector.cs','Assets/Scripts/Production/ProgressionPanel.cs','Assets/Scripts/Production/ProgressionTerminalLayer.cs','Assets/Scripts/Production/WorkshopExpansionLayer.cs','Assets/Scripts/Production/SensoryFeedbackDirector.cs','Assets/Scripts/Production/MobilePlatformController.cs','Assets/Scripts/Production/RuntimeLocalizationDirector.cs','Assets/Scripts/Production/AccessibilityRuntimeDirector.cs','Assets/Scripts/Production/WorkshopAtmosphereDirector.cs','Assets/Scripts/Production/DistanceDetailCuller.cs','Assets/Scripts/Production/VirtualizedInventoryPanel.cs','Assets/Scripts/Production/InventoryTerminalLayer.cs','Assets/Scripts/Production/ReliabilityLifecycle.cs','Assets/Scripts/Production/MaintenancePanel.cs','Assets/Scripts/Production/MaintenanceTerminalLayer.cs','Assets/Scripts/Production/PreflightInspectionPanel.cs','Assets/Scripts/Production/PreflightInspectionTerminalLayer.cs','Assets/Scripts/Production/StaffRosterDirector.cs','Assets/Scripts/Production/StaffRosterPanel.cs','Assets/Scripts/Production/StaffRosterTerminalLayer.cs',
'Assets/Scripts/Production/WarrantyDirector.cs','Assets/Scripts/Production/WarrantyPanel.cs','Assets/Scripts/Production/WarrantyTerminalLayer.cs','Assets/Scripts/Production/WarrantySpecialistBridge.cs','Assets/Scripts/Production/OperationsDashboardPanel.cs','Assets/Scripts/Production/OperationsTerminalLayer.cs',
'Assets/Scripts/Editor/ForgeBenchEditorSetup.cs','Assets/Scripts/Editor/ProductionBuildGate.cs','Assets/Tests/EditMode/EngineeringSimulationTests.cs','Assets/Tests/EditMode/SpecialistSimulationTests.cs','Assets/Tests/EditMode/WorkshopBusinessTests.cs','Assets/Tests/EditMode/SupplyChainJobTests.cs','Assets/Tests/EditMode/ProgressionTests.cs','Assets/Tests/EditMode/ReliabilitySimulationTests.cs','Assets/Tests/EditMode/WarrantyServiceTests.cs',
'Packages/manifest.json','ProjectSettings/ProjectVersion.txt','ProjectSettings/EditorBuildSettings.asset','ProjectSettings/InputManager.asset','Docs/PROMPT_ANALYSIS.md','Docs/requirements_index.json','README.md']
missing=[x for x in required if not (ROOT/x).exists()]
if missing:fail('deliverable paths','missing: '+', '.join(missing))
else:ok('deliverable paths',f'{len(required)} critical files present')

idx=loadj('Docs/requirements_index.json');nums=[];statuses={}
if isinstance(idx,dict) and isinstance(idx.get('numbers'),list):nums=idx['numbers'];statuses[idx.get('default_status','UNVERIFIED')]=len(nums)
elif isinstance(idx,list):
    for r in idx:
        n=r.get('number',r.get('id'));n=int(n) if isinstance(n,str) and n.isdigit() else n
        if isinstance(n,int):nums.append(n)
        st=r.get('status','UNVERIFIED');statuses[st]=statuses.get(st,0)+1
elif isinstance(idx,dict):
    for r in idx.get('requirements') or idx.get('items') or []:
        n=r.get('number',r.get('id'));n=int(n) if isinstance(n,str) and n.isdigit() else n
        if isinstance(n,int):nums.append(n)
        st=r.get('status','UNVERIFIED');statuses[st]=statuses.get(st,0)+1
if nums!=list(range(1,521)):fail('520 requirement index',f'expected contiguous 1..520, got {len(nums)} entries')
else:ok('520 requirement index','contiguous 1..520; index presence does not imply completion')
if statuses:ok('traceability status',', '.join(f'{k}:{v}' for k,v in sorted(statuses.items())))
if any(k.upper() in ('DONE','COMPLETE','VERIFIED') for k in statuses) and not (ROOT/'Docs/acceptance_evidence.json').exists():fail('completion evidence','requirements marked complete/verified without Docs/acceptance_evidence.json')

h=loadj('Assets/Resources/Data/hardware.json') or {};parts=h.get('parts',[]) if isinstance(h,dict) else [];ids=[x.get('id') for x in parts]
if len(parts)<70:fail('hardware catalog size',f'{len(parts)} parts')
elif len(ids)!=len(set(ids)):fail('hardware unique ids','duplicates found')
else:ok('hardware catalog',f'{len(parts)} unique definitions')
required_cats=set(range(15));cats={x.get('category') for x in parts}
if not required_cats.issubset(cats):fail('hardware category coverage',f'missing enum categories {sorted(required_cats-cats)}')
else:ok('hardware category coverage','all 15 PartCategory enum values represented')
for p in parts:
    for key in ['id','brand','model','category','price','performance','powerWatts','quality','connectors','tags']:
        if key not in p:fail('hardware schema',f"{p.get('id','?')} missing {key}")

# Localization parse, duplicate-key guard and exact EN/UK parity.
en=loadj('Assets/Resources/Localization/en.json') or {};uk=loadj('Assets/Resources/Localization/uk.json') or {}
def entries(x):
    if not isinstance(x,dict) or not isinstance(x.get('entries'),list):return []
    return x['entries']
def keymap(x):return {str(e.get('key')):e.get('value') for e in entries(x) if e.get('key')}
fen,fuk=keymap(en),keymap(uk);ken=[e.get('key') for e in entries(en) if e.get('key')];kuk=[e.get('key') for e in entries(uk) if e.get('key')]
if len(ken)!=len(set(ken)):fail('EN localization keys','duplicate key found')
if len(kuk)!=len(set(kuk)):fail('UK localization keys','duplicate key found')
if set(fen)!=set(fuk):fail('localization parity',f'EN-only={sorted(set(fen)-set(fuk))[:12]}, UK-only={sorted(set(fuk)-set(fen))[:12]}')
elif len(fen)<70:fail('localization depth',f'only {len(fen)} paired keys')
else:ok('localization parity',f'{len(fen)} matching keys in both languages')
for key in ['menu.continue','crm.title','career.title','inventory.title','specialist.liquid','business.title','engineering.post','operations.title','operations.preflight','warranty.title','warranty.action']:
    if key not in fen:fail('production localization',f'missing {key}')

pv=(ROOT/'ProjectSettings/ProjectVersion.txt').read_text(encoding='utf-8')
if '6000.3.15f1' not in pv:fail('Unity version','expected 6000.3.15f1')
else:ok('Unity version','6000.3.15f1')
editor=(ROOT/'Assets/Scripts/Editor/ForgeBenchEditorSetup.cs').read_text(encoding='utf-8');gate=(ROOT/'Assets/Scripts/Editor/ProductionBuildGate.cs').read_text(encoding='utf-8')
need=['NamedBuildTarget.Android','AndroidArchitecture.ARM64','ScriptingImplementation.IL2CPP','UIOrientation.LandscapeLeft','BuildTarget.Android','com.originalforge.forgebench']
miss=[s for s in need if s not in editor+gate]
if miss:fail('Android build configuration','missing tokens: '+', '.join(miss))
else:ok('Android build configuration','package id + ARM64 + IL2CPP + landscape + Android target source present')

save=(ROOT/'Assets/Scripts/Runtime/SaveService.cs').read_text(encoding='utf-8');domain=(ROOT/'Assets/Scripts/Core/DomainModels.cs').read_text(encoding='utf-8');m=re.search(r'CurrentSchema\s*=\s*(\d+)',save);schema=int(m.group(1)) if m else 0
if schema<7 or '.bak' not in save or '.tmp' not in save or not re.search(r'schemaVersion\s*=\s*[7-9]\d*',domain):fail('save safety',f'schema={schema}; expected schema 7+, temp writes and backup recovery')
else:ok('save safety',f'schema {schema} + temp atomic write + backup recovery')
for token in ['LiquidLoopState','BoardRepairState','PortableDeviceState','NetworkLabState','OsRuntimeState','BenchmarkRunState','ThermalRuntimeState','PowerRuntimeState','MaintenanceState']:
    if token not in domain:fail('persistent simulation state','missing '+token)

# Static C# lexical sanity. This is deliberately not presented as a Unity/C# compiler.
def scrub(s):
    s=re.sub(r'/\*.*?\*/','',s,flags=re.S);s=re.sub(r'//.*','',s);s=re.sub(r'@"(?:""|[^"])*"','""',s);s=re.sub(r'"(?:\\.|[^"\\])*"','""',s);s=re.sub(r"'(?:\\.|[^'\\])'","''",s);return s
cs=list(ROOT.glob('Assets/**/*.cs'))
for p in cs:
    txt=p.read_text(encoding='utf-8');rel=str(p.relative_to(ROOT))
    if 'NotImplementedException' in txt:fail('no NotImplementedException',rel)
    if re.search(r'\bTODO\b',txt,re.I):warn('TODO token',rel)
    # Catches an easy-to-miss malformed bool ternary such as mobile?.38f:.55f while not flagging obj?.Property.
    if re.search(r'\b[A-Za-z_]\w*\?\.\d',txt):fail('suspicious C# ternary',rel)
    z=scrub(txt)
    for a,b,name in [('(',')','parentheses'),('[',']','brackets'),('{','}','braces')]:
        depth=0
        for ch in z:
            if ch==a:depth+=1
            elif ch==b:depth-=1
            if depth<0:break
        if depth!=0:fail('C# '+name,f'{rel} balance {depth}')
if not [e for e in errors if e[0].startswith('C#') or e[0]=='suspicious C# ternary']:ok('C# lexical sanity',f'{len(cs)} scripts scanned; this is not compiler execution')

# Runtime localization uses Dictionary collection initializers; duplicate display strings would throw during static initialization.
loc_runtime=(ROOT/'Assets/Scripts/Production/RuntimeLocalizationDirector.cs').read_text(encoding='utf-8')
loc_pairs=re.findall(r'\{\s*"((?:\\.|[^"\\])*)"\s*,\s*"[^"\\]*"\s*\}',loc_runtime)
dup_display=sorted({x for x in loc_pairs if loc_pairs.count(x)>1})
if dup_display:fail('runtime localization dictionary','duplicate display keys: '+', '.join(dup_display[:12]))
else:ok('runtime localization dictionary',f'{len(loc_pairs)} unique exact-label mappings')

combined='\n'.join(p.read_text(encoding='utf-8') for p in cs)
terms=['PowerOn','TrainMemory','AnalyzePower','ThermalSoak','StorageSmart','BenchmarkSuite','InstallOS','InstallDrivers','ValidateAndSubmit','UpgradeWorkshop','ShipmentState','SupplyChainService','SpecialistJobService','BoardRepairService','PortableRepairService','NetworkLabService','WorkshopBusinessService','AdvancedJobGeneratorService','CustomerRelationsService','ProgressionService','ReliabilitySimulationService','PreflightInspectionService','StaffRosterService','WarrantyService','WarrantyDirector','WarrantySpecialistBridge','OperationsDashboardPanel','MobilePlatformController','VirtualizedInventoryPanel','RuntimeLocalizationDirector','AccessibilityRuntimeDirector','CharacterController','TouchJoystick']
missing_terms=[t for t in terms if t not in combined]
if missing_terms:fail('production source coverage','missing tokens: '+', '.join(missing_terms))
else:ok('production source coverage','core/engineering/specialist/business/CRM/progression/reliability/warranty/operations/preflight/mobile/accessibility/touch source found')

tests=list(ROOT.glob('Assets/Tests/**/*.cs'))
if len(tests)<7:fail('automated test source',f'only {len(tests)} test files')
else:ok('automated test source',f'{len(tests)} C# test files present; Unity Test Runner execution still required')
for t in ['EngineeringSimulationTests.cs','ProgressionTests.cs','ReliabilitySimulationTests.cs','WarrantyServiceTests.cs']:
    if not any(p.name==t for p in tests):fail('critical test source','missing '+t)

print('ForgeBench static validation')
for n,d in checks:print(f'[PASS] {n}: {d}')
for n,d in warnings:print(f'[WARN] {n}: {d}')
for n,d in errors:print(f'[FAIL] {n}: {d}')
print(f'\nPASS={len(checks)} WARN={len(warnings)} FAIL={len(errors)}')
sys.exit(1 if errors else 0)
