# Physical interaction polish — 2026-09-15

This pass refines the existing ForgeBench parts-tray workflow without introducing a second inventory, compatibility, or save model.

Implemented in source:

- held components continuously evaluate the nearest category-compatible installation point;
- installation preview distinguishes **move closer**, **incompatible**, **rotate to align**, and **ready to install** states;
- sockets expose configurable snap radius and orientation tolerance instead of accepting a component solely because it is nearby;
- incompatible and misaligned releases keep the part in hand instead of silently installing or returning it;
- installation still commits through `GameRuntime.Install` and `CompatibilityService`;
- closed-panel and case-replacement prerequisites participate in the preview result;
- installation ghosts use non-destructive `MaterialPropertyBlock` highlighting;
- desktop controls now support yaw, roll and pitch while touch users get explicit large rotate/inspect/snap buttons;
- held-component UI shows model-specific CPU/GPU/RAM/storage/PSU/board/cooling/fan details and live distance/orientation feedback;
- tray proxy proportions now respond to form factor, GPU length/slot width, NVMe vs SATA, AIO vs air, and 120/140 mm fans;
- tray materials are reclaimed on rebuild instead of accumulating until controller destruction;
- socket evaluation rules are isolated in `AssemblyInteractionRules` with EditMode regression source.

Verification boundary:

- source/static validation, hardware/art-profile validation, balance smoke and `git diff --check` are run before merge;
- Unity compilation, Unity Test Runner execution, physical reachability, touch ergonomics and Android frame-time still require a Unity-equipped/device run;
- no claim is made that these procedural proxy meshes are final production art.
