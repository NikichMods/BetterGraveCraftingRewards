# Test / Research Artifact Log

## BGCR Balance Dump 0.1.0

- Purpose: read-only dump of Graveyard Keeper 1.407 grave-decoration balance data.
- Status: **CI-built; runtime evidence pending**.
- Research branch: `research/vanilla-balance-dump`.
- Exact built source state: `bc209de8baec41d65768b77d7d267f0ef27df660`.
- CI run: `36284029640`.
- GitHub Actions artifact: `10919494051` (`BGCR-Balance-Dump-0.1.0`).
- Handed DLL: `BGCR Balance Dump 0.1.0.dll`.
- DLL SHA-256: `b194c72a7fdb55fb9fb6ce0f1999146b04240c9eece73510d48cc3c14a727e23`.
- Artifact ZIP digest reported by GitHub: `sha256:4f3d776d76505f0286abd3c3b3a721ac82bdc41b7718a5d04f155688fb59a089`.
- Build result: success.
- Runtime action: install DLL under BepInEx plugins, launch GK 1.407, load any save until normal gameplay is active, then return `LogOutput.log`.
- Expected evidence markers: `BGCR_DATA_BEGIN`, `BGCR_ITEM`, `BGCR_PRODUCER`, `BGCR_CONSUMER`, `BGCR_STUDY`, `BGCR_DATA_DONE`.
- No crafting, Study action, grave interaction, or save mutation is required.

Acceptance target for the probe itself:
1. no `BGCR_DATA_ERROR`;
2. one complete dump ending in `BGCR_DATA_DONE`;
3. enough native rows to identify the complete grave-decoration set, production recipes, repeatable consumers/recycling, Study rewards, technology unlocks, energy/time expressions, and r/g/b outputs.
