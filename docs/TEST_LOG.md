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
- Status: **runtime-complete; accepted with a scoped follow-up gap**.
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


### Runtime result — BGCR Material Dump 0.2.0

User-returned log: `LogOutput(20260927-013608).log`.

Observed:
- Graveyard Keeper 1.407 runtime;
- exact `BGCR Material Dump 0.2.0` loaded;
- no `BGCR_MATERIAL_ERROR`;
- terminal marker: `BGCR_MATERIAL_DONE|materials=67|producer_crafts=959|roots=11|max_depth=3`;
- direct grave-material set: 16 items.

Probe result is accepted for the material recipes it resolved. It also exposed two design limits that require one narrower follow-up rather than deeper recursion:

1. `marble_plate_3:1/:2/:3` were reported as roots even though they are craftable; static inspection confirms these are multi-quality outputs and exact matching on `Item.id` misses the host's `multiquality_items` representation.
2. 882 of 959 producer rows were generic `MixedCraft` alchemy combinations reached through water/oil/faith-adjacent dependencies. They are valid host recipes but irrelevant to grave-decoration balance and demonstrate that unrestricted recursive producer traversal is too broad.

Accepted 0.2.0 conclusions therefore remain limited to the resolved ordinary production paths; the next probe must target the missing multi-quality producer plus progression/economy metadata directly rather than recurse further.


## BGCR Scope Completion Dump 0.3.0

- Purpose: close the final carved-marble, progression-graph, workstation-availability and aggregate tech-cost gaps.
- Status: **runtime-complete; accepted as final scope-completion evidence**.
- Research branch: `research/vanilla-balance-dump`.
- Exact built source state: `04c0afcb31f825879032f5bc6db089caa37e100d`.
- CI run: `36286375097`.
- GitHub Actions artifact: `10919804008` (`BGCR-Scope-Completion-Dump-0.3.0`).
- Handed DLL: `BGCR-Scope-Completion-Dump-0.3.0.dll`.
- DLL SHA-256: `06a81967f10a4dba286970f69385301565f74341bc31d24aec41775dae98bbee`.
- Artifact ZIP digest reported by GitHub: `sha256:5f28e7ee240e9566d920abccfb1667a3a05c6673e74e04e7a21c730ce7e456f9`.
- Build result: success.
- Safety: read-only, no Harmony patches, no game/save mutation.
- Evidence target:
  1. no `BGCR_SCOPE_ERROR`;
  2. one complete dump ending in `BGCR_SCOPE_DONE`;
  3. at least one `BGCR_MQ_CRAFT` covering `marble_plate_3:1/:2/:3`;
  4. complete `BGCR_TECH` parent/price rows and `BGCR_TECH_TOTALS`;
  5. relevant `BGCR_BLUEPRINT` rows sufficient to map the observed crafting stations to their unlock path.


### Runtime result — BGCR Scope Completion Dump 0.3.0

User-returned log: `LogOutput(20260927-113229).log`.

Observed:
- exact `BGCR Scope Completion Dump 0.3.0` loaded on Graveyard Keeper 1.407;
- no `BGCR_SCOPE_ERROR`;
- `BGCR_MQ_CRAFT` resolved `marble_plate_3` as the native multi-quality producer for `marble_plate_3:1/:2/:3`;
- 187 technology definitions with parents/prices were dumped;
- 26 relevant blueprint rows were dumped;
- terminal marker: `BGCR_SCOPE_DONE|multiquality_crafts=1|techs=187|blueprints=26`.

The final evidence target is satisfied. No further broad balance-data probe is planned.

Environment correction:
- Better Save Soul Rebalance 1.1.1 was active before the dump and changed selected Soul technology prices.
- The dumped aggregate was `b=4615`.
- Accepted Better Save Soul Rebalance source records the exact stock baseline: the mod adds 150 + 200 + 250 + 300 = 900 blue to four Soul technology prices while leaving `soul_writing_additions` at 10 blue.
- Therefore the reconstructed vanilla Graveyard Keeper 1.407 aggregate technology-tree blue cost for this all-DLC dataset is **3715 blue**.


## BGCR Blue Economy Dump 0.4.0

- Purpose: enumerate all positive-blue native `CraftDefinition.output` sources needed to verify recovery paths after finite grave-craft mastery.
- Status: **CI-built; runtime evidence pending**.
- Research branch: `research/vanilla-balance-dump`.
- Exact built source state: `c37f40e8aa19e6358e47c493d459a2bd39d279ec`.
- CI run: `36340169556`.
- GitHub Actions artifact: `10938014022` (`BGCR-Blue-Economy-Dump-0.4.0`).
- Handed DLL: `BGCR-Blue-Economy-Dump-0.4.0.dll`.
- DLL SHA-256: `a8b7e645e471978d0e1949071bbbd679318fc4dad7d20731a65a190809ededd7`.
- Artifact ZIP digest: `sha256:38006eb2b4248d0d05582b53e5bab93d02c49f60f2e23d9ff324847ad805862c`.
- Build result: success.
- Safety: read-only, no Harmony patches, no game/save mutation.
- Evidence target:
  1. no `BGCR_BLUE_ECON_ERROR`;
  2. one complete dump ending in `BGCR_BLUE_ECON_DONE`;
  3. all logged rows have positive native blue output;
  4. rows contain enough classification data to distinguish Survey/one-time, ordinary visible repeatable, hidden/scripted and automatic sources;
  5. non-grave repeatable fallback paths can be identified without community-value reconstruction.
