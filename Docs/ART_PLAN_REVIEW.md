# Art plan integration review

This branch is intentionally non-runtime. It adds the visual production brief, stable hardware visual profiles for the current 79-part catalog, and an offline validator.

Integration rules:

- Do not mark a profile `ready` until a real prefab exists under `Assets/` and review blockers are cleared.
- Do not treat marketplace listings as cleared production assets until license, technical data, and redistribution limits are verified.
- Preserve gameplay-authoritative state; visual profiles must not become a second hardware/compatibility database.
- Resolve cooler/fan normalization mismatches before signing off mechanically exact replacement art.
- Validate the manifest in CI together with the main project validator.

The art profile validator is structural only. It does not certify mesh quality, licensing, Unity import, Android performance, animation quality, or device ergonomics.
