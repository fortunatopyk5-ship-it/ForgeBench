# ForgeBench hardware visual fidelity pass — 2026-09-15

This pass turns the reviewed art direction into more truthful runtime procedural presentation without changing gameplay authority or claiming final production assets.

Implemented in source:

- motherboard DIMM/PCIe/M.2 detail counts are derived from normalized hardware definitions instead of one fixed decorative board;
- RAM is presented from persisted `ramSlotIndices`, preserving the dual-channel 1/3 layout;
- GPU length, slot thickness and fan count vary by actual hardware characteristics;
- air coolers and AIOs have different silhouettes; AIO radiator fan count follows the normalized 240/280/360 mm specification;
- NVMe drives are shown on motherboard M.2 locations while SATA drives remain in drive bays;
- 120/140 mm case fan presentation differs and installed fan visuals are no longer hard-limited to the original three-front-fan silhouette;
- modular and non-modular PSU rear presentation differs; the PSU grille is stationary while the fan behind it is state-driven;
- connected cables use low-segment deterministic quadratic Bezier routing rather than one rigid straight cuboid; no cloth/rope physics was introduced;
- machine rotors stop when the machine is off and respond to POST, temperature and BIOS fan profile;
- supported glass-tagged cases use a transparent panel material rather than an opaque pseudo-glass slab;
- repeated RAM chip details share the existing material instead of allocating a new material for every chip.

Verification boundary:

- EditMode regression source was added for layout rules.
- GitHub static validation is expected to run on the review PR.
- Unity compilation, Unity Test Runner, Android IL2CPP build, render profiling and real-device visual inspection remain required before this is called production-ready.
- No external mesh, texture, animation, audio or copyrighted game asset is included by this pass.
