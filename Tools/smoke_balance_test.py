#!/usr/bin/env python3
from __future__ import annotations
import json, itertools, math
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
parts=json.loads((ROOT/'Assets/Resources/Data/hardware.json').read_text(encoding='utf-8'))['parts']
by={c:[p for p in parts if p['category']==c] for c in range(15)}

def case_accepts(c,b):
    if not c or not b: return True
    if c=='EATX': return True
    if c=='ATX': return b in ('ATX','mATX','MiniITX')
    if c=='mATX': return b in ('mATX','MiniITX')
    return c==b

def compatible(case,board,cpu,ram,gpu,storage,psu,cooler):
    if not case_accepts(case['formFactor'],board['formFactor']): return False
    if board['socket']!=cpu['socket'] or board['memoryType']!=ram['memoryType']: return False
    if gpu['lengthMm']>case['lengthMm']: return False
    if 'air' in cooler['tags'] and cooler['heightMm']>case['heightMm']: return False
    if storage['storageInterface']=='NVMe' and not board: return False
    est=18+board['powerWatts']+cpu['powerWatts']+gpu['powerWatts']+cooler['powerWatts']+ram['powerWatts']+storage['powerWatts']
    if psu['psuWattage'] < est*1.18: return False
    return True

def score(cpu,ram,gpu,storage,memory_profile=True,cable_score=.65,stable=True,cpu_temp=80,gpu_temp=78):
    v=cpu['performance']*.45+gpu['performance']*.42+ram['performance']*.08+storage['performance']*.05
    if memory_profile: v*=1.06
    # Mathf.Lerp(.76,1.02,t)
    v*=.76+(1.02-.76)*cable_score
    if cpu_temp>90:v*=.86
    if gpu_temp>88:v*=.89
    if not stable:v*=.82
    return round(v)

def find(target=485,budget=1080):
    best=None
    for combo in itertools.product(by[0],by[1],by[2],by[3],by[4],by[5],by[6],by[7]):
        c,b,cpu,ram,gpu,storage,psu,cooler=combo
        if not compatible(*combo): continue
        price=sum(x['price'] for x in combo)
        s=score(cpu,ram,gpu,storage)
        if price<=budget and s>=target:
            key=(price,-s)
            if best is None or key<best[0]: best=(key,combo,s,price)
    return best

# Tier 1 generated build/upgrade/repair target from JobService: 360 + 1*125 = 485, budget 720 + 1*360 = 1080.
b=find(485,1080)
if b is None:
    raise SystemExit('[FAIL] no compatible early-game build can meet target 485 inside $1080')
_,combo,s,price=b
labels=['Case','Board','CPU','RAM','GPU','Storage','PSU','Cooler']
print('[PASS] early-game solvability')
print(f'  total parts price: ${price:.0f}; modeled score: {s} >= 485')
for lab,p in zip(labels,combo): print(f"  {lab:7s}: {p['brand']} {p['model']} (${p['price']})")
# Verify initial cash can cover parts plus eight normal-speed shipping fees ($8 each).
landed=price+len(combo)*8
if landed>1800: raise SystemExit(f'[FAIL] build landed cost ${landed:.0f} exceeds starting cash $1800')
print(f'[PASS] starting-cash feasibility: landed ${landed:.0f} <= $1800')
# Ensure representative compatibility alternatives exist for every board socket and memory generation.
sockets={p['socket'] for p in by[1]}
for socket in sorted(sockets):
    if not any(c['socket']==socket for c in by[2]): raise SystemExit('[FAIL] no CPU for '+socket)
print('[PASS] every motherboard socket has at least one CPU')
mems={p['memoryType'] for p in by[1]}
for mem in sorted(mems):
    if not any(r['memoryType']==mem for r in by[3]): raise SystemExit('[FAIL] no RAM for '+mem)
print('[PASS] every motherboard memory generation has at least one RAM definition')
