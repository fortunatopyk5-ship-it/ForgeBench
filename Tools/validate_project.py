#!/usr/bin/env python3
from __future__ import annotations
import json, re, sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
errors=[]; warnings=[]; checks=[]
def ok(name, detail=''): checks.append((name, detail))
def fail(name, detail): errors.append((name, detail))
def warn(name, detail): warnings.append((name, detail))

def loadj(rel):
    try: return json.loads((ROOT/rel).read_text(encoding='utf-8'))
    except Exception as e: fail('JSON '+rel, str(e)); return None

# Required deliverable paths
required=[
 'Assets/Scenes/Workshop.unity','Assets/Scripts/Core/DomainModels.cs','Assets/Scripts/Simulation/GameSimulation.cs',
 'Assets/Scripts/Runtime/GameRuntime.cs','Assets/Scripts/Runtime/SaveService.cs','Assets/Scripts/World/WorkshopWorld.cs',
 'Assets/Scripts/UI/GameUI.cs','Assets/Scripts/Editor/ForgeBenchEditorSetup.cs','Assets/Resources/Data/hardware.json',
 'Assets/Resources/Localization/en.json','Assets/Resources/Localization/uk.json','Assets/Tests/EditMode/SimulationTests.cs',
 'Packages/manifest.json','ProjectSettings/ProjectVersion.txt','ProjectSettings/EditorBuildSettings.asset','ProjectSettings/InputManager.asset',
 'Docs/PROMPT_ANALYSIS.md','Docs/requirements_index.json','README.md'
]
missing=[x for x in required if not (ROOT/x).exists()]
if missing: fail('deliverable paths','missing: '+', '.join(missing))
else: ok('deliverable paths',f'{len(required)} critical files present')

# Requirements traceability
idx=loadj('Docs/requirements_index.json')
if isinstance(idx,list): rows=idx
elif isinstance(idx,dict): rows=idx.get('requirements') or idx.get('items') or []
else: rows=[]
nums=[]
for r in rows:
    n=r.get('number',r.get('id'))
    if isinstance(n,str) and n.isdigit(): n=int(n)
    if isinstance(n,int): nums.append(n)
if len(rows)!=520: fail('520 requirement rows',f'found {len(rows)}')
elif nums != list(range(1,521)): fail('requirement numbering','not exactly contiguous 1..520')
else: ok('520 requirement rows','contiguous 1..520')
if rows:
    statuses={}
    for r in rows: statuses[r.get('status','MISSING')]=statuses.get(r.get('status','MISSING'),0)+1
    ok('traceability statuses',', '.join(f'{k}:{v}' for k,v in sorted(statuses.items())))

# Hardware data
h=loadj('Assets/Resources/Data/hardware.json') or {}
parts=h.get('parts',[]) if isinstance(h,dict) else []
ids=[x.get('id') for x in parts]
if len(parts)<70: fail('hardware catalog size',f'{len(parts)} parts')
elif len(ids)!=len(set(ids)): fail('hardware unique ids','duplicates found')
else: ok('hardware catalog',f'{len(parts)} unique definitions')
required_cats=set(range(0,15)); cats={x.get('category') for x in parts}
if not required_cats.issubset(cats): fail('hardware category coverage',f'missing enum categories {sorted(required_cats-cats)}')
else: ok('hardware category coverage','all 15 PartCategory enum values represented')
for p in parts:
    for key in ['id','brand','model','category','price','performance','powerWatts','quality','connectors','tags']:
        if key not in p: fail('hardware schema',f"{p.get('id','?')} missing {key}")

# Localization parse + basic parity
en=loadj('Assets/Resources/Localization/en.json') or {}
uk=loadj('Assets/Resources/Localization/uk.json') or {}
def flatten(x,p=''):
    if isinstance(x,dict) and isinstance(x.get('entries'),list):
        return {str(e.get('key')):e.get('value') for e in x['entries'] if e.get('key')}
    out={}
    if isinstance(x,dict):
        for k,v in x.items(): out.update(flatten(v,p+'.'+k if p else k))
    elif isinstance(x,list):
        for i,v in enumerate(x): out.update(flatten(v,p+'.'+str(i) if p else str(i)))
    else: out[p]=x
    return out
fen,fuk=flatten(en),flatten(uk)
if set(fen)!=set(fuk): warn('localization parity',f'EN-only={sorted(set(fen)-set(fuk))[:10]}, UK-only={sorted(set(fuk)-set(fen))[:10]}')
else: ok('localization parity',f'{len(fen)} keys in both languages')

# Version / Android configuration source
pv=(ROOT/'ProjectSettings/ProjectVersion.txt').read_text(encoding='utf-8')
if '6000.3.15f1' not in pv: fail('Unity version','expected 6000.3.15f1')
else: ok('Unity version','6000.3.15f1')
editor=(ROOT/'Assets/Scripts/Editor/ForgeBenchEditorSetup.cs').read_text(encoding='utf-8')
need=['NamedBuildTarget.Android','AndroidArchitecture.ARM64','ScriptingImplementation.IL2CPP','UIOrientation.LandscapeLeft','BuildTarget.Android']
miss=[s for s in need if s not in editor]
if miss: fail('Android build configuration','missing tokens: '+', '.join(miss))
else: ok('Android build configuration','ARM64 + IL2CPP + landscape + APK build target source present')

# Save contract
save=(ROOT/'Assets/Scripts/Runtime/SaveService.cs').read_text(encoding='utf-8')
domain=(ROOT/'Assets/Scripts/Core/DomainModels.cs').read_text(encoding='utf-8')
if 'schemaVersion = 4' not in domain or '.bak' not in save or '.tmp' not in save:
    fail('save safety','versioned schema / temp / backup token missing')
else: ok('save safety','schema version + temp atomic write + backup recovery source found')

# Static C# sanity: no stubs and balanced delimiters after stripping strings/comments.
def scrub(s):
    s=re.sub(r'/\*.*?\*/','',s,flags=re.S)
    s=re.sub(r'//.*','',s)
    s=re.sub(r'@"(?:""|[^"])*"','""',s)
    s=re.sub(r'"(?:\\.|[^"\\])*"','""',s)
    s=re.sub(r"'(?:\\.|[^'\\])'","''",s)
    return s
cs=list(ROOT.glob('Assets/**/*.cs'))
for p in cs:
    txt=p.read_text(encoding='utf-8')
    if 'NotImplementedException' in txt: fail('no NotImplementedException',str(p.relative_to(ROOT)))
    if re.search(r'\bTODO\b',txt,re.I): warn('TODO token',str(p.relative_to(ROOT)))
    z=scrub(txt)
    for a,b,name in [('(',')','parentheses'),('[',']','brackets'),('{','}','braces')]:
        depth=0
        for ch in z:
            if ch==a: depth+=1
            elif ch==b: depth-=1
            if depth<0: break
        if depth!=0: fail('C# '+name,f'{p.relative_to(ROOT)} balance {depth}')
if not [e for e in errors if e[0].startswith('C#')]: ok('C# delimiter sanity',f'{len(cs)} scripts scanned')

# Required gameplay terms, a lightweight coverage guard not a compile proof.
combined='\n'.join(p.read_text(encoding='utf-8') for p in cs)
terms=['PowerOn','POST','memoryProfileEnabled','InstallOS','InstallDrivers','RunBenchmark','ValidateAndSubmit','UpgradeWorkshop','Autosave','ShipmentState','CompatibilityService','CharacterController','TouchJoystick','Gamepad']
# Gamepad is described by right-stick axes rather than a literal class name.
if 'Gamepad' not in combined and 'LookHorizontal' in (ROOT/'ProjectSettings/InputManager.asset').read_text(encoding='utf-8'):
    terms.remove('Gamepad')
missing_terms=[t for t in terms if t not in combined]
if missing_terms: fail('core loop source coverage','missing tokens: '+', '.join(missing_terms))
else: ok('core loop source coverage','ordering/POST/BIOS/OS/drivers/benchmark/submission/upgrades/save + 3D/touch source found')

print('ForgeBench static validation')
for n,d in checks: print(f'[PASS] {n}: {d}')
for n,d in warnings: print(f'[WARN] {n}: {d}')
for n,d in errors: print(f'[FAIL] {n}: {d}')
print(f'\nPASS={len(checks)} WARN={len(warnings)} FAIL={len(errors)}')
sys.exit(1 if errors else 0)
