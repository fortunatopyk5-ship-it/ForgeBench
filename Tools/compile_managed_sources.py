#!/usr/bin/env python3
"""Compile source against supplied installed Unity assemblies, without opening Unity.

This is a C# audit only: it does not import assets, execute Unity tests, validate
the target editor version, or produce an Android player. Use the required editor
and actual package assemblies for release evidence.
"""
import argparse
from pathlib import Path
import subprocess
import sys


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--unity-data", required=True, type=Path)
    parser.add_argument("--package-assemblies", required=True, type=Path)
    parser.add_argument("--output-dir", required=True, type=Path)
    args = parser.parse_args()
    root = Path(__file__).resolve().parents[1]
    data = args.unity_data.resolve()
    packages = args.package_assemblies.resolve()
    output = args.output_dir.resolve()
    mono = data / "MonoBleedingEdge/bin/mono.exe"
    compiler = data / "MonoBleedingEdge/lib/mono/4.5/csc.exe"
    netstandard = data / "MonoBleedingEdge/lib/mono/4.5/Facades/netstandard.dll"
    for path in (mono, compiler, netstandard, packages / "UnityEngine.UI.dll"):
        if not path.is_file():
            parser.error("Required compiler/reference missing: " + str(path))
    output.mkdir(parents=True, exist_ok=True)
    references = sorted((data / "Managed/UnityEngine").glob("*.dll"))
    references += [p for p in sorted(packages.glob("*.dll"))
                   if "Editor" not in p.name and p.name.startswith(
                       ("UnityEngine.UI", "Unity.RenderPipelines", "Unity.TextMeshPro"))]
    references.append(netstandard)
    runtime = output / "ForgeBench.Runtime.audit.dll"
    groups = [
        ("runtime", [p for p in sorted((root / "Assets/Scripts").rglob("*.cs")) if "Editor" not in p.parts],
         references, runtime, []),
        ("editor", sorted((root / "Assets/Scripts/Editor").glob("*.cs")),
         references + sorted((data / "Managed").glob("UnityEditor.*Module.dll")) + [runtime],
         output / "ForgeBench.Editor.audit.dll", ["/define:UNITY_EDITOR"]),
    ]
    print("Reference editor data:", data)
    print("Package assemblies:", packages)
    for label, sources, refs, target, defines in groups:
        arguments = ["/nologo", "/target:library", '/out:"' + str(target) + '"'] + defines
        arguments += ['/r:"' + str(p) + '"' for p in refs]
        arguments += ['"' + str(p) + '"' for p in sources]
        response = output / (label + ".rsp")
        response.write_text("\n".join(arguments), encoding="utf-8")
        result = subprocess.run([str(mono), str(compiler), "@" + str(response)],
                                cwd=root, capture_output=True, text=True)
        log = result.stdout + result.stderr
        (output / (label + ".log")).write_text(log, encoding="utf-8")
        print(log, end="")
        print(f"{label}: {len(sources)} sources; compiler exit {result.returncode}")
        if result.returncode:
            return result.returncode
    print("C# audit passed. Unity import, runtime, tests and Android build remain unverified.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
