# Test / Research Artifact Log

## BGCR Balance Dump 0.1.0

- Purpose: read-only dump of Graveyard Keeper 1.407 grave-decoration balance data.
- Status: **runtime-complete; accepted as research evidence**.
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


### Runtime result — 2026-09-27

User-returned log: `LogOutput(20260927-011148).log`.

Observed:
- target runtime identified itself as Graveyard Keeper 1.407;
- exact probe `BGCR Balance Dump 0.1.0` loaded;
- dump began once normal gameplay was active;
- no `BGCR_DATA_ERROR` occurred;
- terminal marker: `BGCR_DATA_DONE|decorations=42|producers=132|consumers=55|studies=24`.

Probe acceptance target is satisfied.

Important environment note:
- Better Save Soul Rebalance 1.1.1 was active before the dump and had already modified ten Better Save Soul grave recipes plus the related Soul technology prices.
- Inspection of `NikichMods/BetterSaveSoulRebalance@0772e4484fd3e9a91bdee386a802e05909cbce93` proves those mutations affect recipe `needs`, Soul Gratitude craft cost, and selected Soul technology prices; they do **not** modify grave-craft `r/g/b` output rewards.
- That mod uses guarded stock-baseline checks before mutation. Therefore the vanilla stock material lists and vanilla Soul technology-price baselines for the touched entries can be reconstructed from its accepted source instead of requiring a second clean runtime launch.

Research conclusion:
- the 0.1.0 dump is authoritative for grave item identities, qualities, craft reward outputs, Study outputs, energy/time, recipe consumers/recycling, and unaffected prerequisite technologies;
- for Better Save Soul recipes, use the stock baselines recorded by Better Save Soul Rebalance for material inputs / Soul Gratitude / modified technology-price fields.


## BGCR Material Dump 0.2.0

- Purpose: recursively enumerate native producer chains behind direct grave-decoration crafting materials.
- Status: **CI-built; runtime evidence pending**.
- Research branch: `research/vanilla-balance-dump`.
- Exact built source state: `caf3d66435567b299397e3ecb71b1bf9479d1f13`.
- CI run: `36285157007`.
- GitHub Actions artifact: `10920272311` (`BGCR-Material-Dump-0.2.0`).
- Handed DLL: `BGCR Material Dump 0.2.0.dll`.
- DLL SHA-256: `b35ccf013197d04a2f0b39f50ab65901592d457b5a9cd003a4981750ec383537`.
- Artifact ZIP digest reported by GitHub: `sha256:b77c9319150dc542ca2babfa71b058d19680fddf19a9a64bcc392874dae4e509`.
- Build result: success.
- Safety: read-only, no Harmony patches, no save mutation.
- Evidence target:
  1. no `BGCR_MATERIAL_ERROR`;
  2. one complete dump ending in `BGCR_MATERIAL_DONE`;
  3. producer chains sufficient to derive material processing depth, component-craft r/g/b generation, energy/time and terminal resources for grave-decoration inputs.
