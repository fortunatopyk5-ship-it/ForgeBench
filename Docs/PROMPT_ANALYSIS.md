# ForgeBench — authoritative implementation contract

This project is governed by `MEGA_PROMPT_PC_ELECTRONICS_SIM_520x10_UA.md`: 520 numbered requirements grouped into 52 ten-item sections, followed by a strict definition of done.

## Non-negotiable interpretation

ForgeBench is not considered complete merely because Unity compiles or an APK is produced. A feature is complete only when its gameplay state is real, data-driven where appropriate, connected to the rest of the simulation, persists through save/load, has actionable success/error feedback, works through touch-first Android controls, and has a reproducible validation path or automated test.

No decorative substitute may be counted as implementation. In particular, a button labelled BIOS, benchmark, repair, store, diagnostics, liquid cooling, board repair, OS, staff, networking, or shipping is not proof that the corresponding requirement is complete. The underlying state transition and acceptance rule must exist.

## Product pillars extracted from the prompt

1. Complete source-first Unity 6 project and Android build configuration.
2. Modular data-driven architecture and durable save/load.
3. First-person/touch interaction and physical object handling.
4. Deep PC assembly: cases, boards, CPU, RAM, GPU, storage, PSU, cooling, fans, cables and compatibility.
5. Engineering simulation: POST, UEFI/BIOS, OS/drivers, benchmarks, thermals, power and diagnostics.
6. Maintenance and repair: dust, wear, damage, board-level rework and specialist tools.
7. Multiple electronics categories: laptops, phones, tablets, consoles/controllers, networking, NAS and servers.
8. Workshop simulation: inventory, warehouse, suppliers, shipping, customers, jobs, economy, progression, staff and expansion.
9. Customization, UI/UX, audio/haptics, graphics and Android-specific usability/performance.
10. QA/release: validation, automated tests, smoke tests, build tests, crash diagnostics, documentation and final deliverables.

## Traceability rule

`Docs/requirements_index.json` intentionally records all requirement numbers as `UNVERIFIED` by default. Presence in the index is **not** a claim of completion. Status may only be promoted after the implementation is audited against the exact full wording of that requirement and its persistence/acceptance criteria are exercised.

## Current engineering policy

- Runtime-created geometry is acceptable as an implementation technique, but it is not a substitute for required production art quality; art/rendering acceptance remains separate.
- Simulation values must arise from component/state inputs, not from a fake progress animation.
- Specialist devices must use device-specific workflows rather than pretending every job is a desktop PC.
- Failed validation must explain the blocking state to the player.
- Save migrations must initialize every newly introduced persistent subsystem without destroying old saves.
- Android release builds use a production build gate so missing critical systems fail the build instead of silently producing an incomplete APK.
- Tests committed to source are not reported as passed until Unity Test Runner or an equivalent real execution has run them.
- An APK is not reported as working until it has been built and launched; a successful compile alone is not an end-to-end acceptance result.

## Definition-of-done direction

The final product must support a coherent path from New Game through contracts, purchasing/shipping, physical repair/build work, POST/firmware/OS/diagnostics/benchmarking, job submission, economy/progression and workshop upgrades, then survive a full application restart and continue from persisted state. Specialist repair categories and Android touch operation are part of the same product, not optional mock screens.
