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
- Status: **runtime-complete; accepted**.
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


### Runtime result — BGCR Blue Economy Dump 0.4.0

User-returned log: `LogOutput(20260927-201825).log`.

Observed:
- Graveyard Keeper 1.407;
- exact `BGCR Blue Economy Dump 0.4.0` loaded;
- no `BGCR_BLUE_ECON_ERROR`;
- terminal marker:
  `BGCR_BLUE_ECON_DONE|sources=355|surveys=194|one_time=194|hidden=2|ordinary_repeatable_visible=151|one_pass_blue_sum=3571`.

Parsed source inventory:
- Survey rows: 194, one-pass blue sum 2630;
- grave-decoration Survey rows: 24, blue sum 1544;
- non-grave Survey rows: 170, blue sum 1086;
- non-Survey positive-blue rows: 161, raw one-pass blue sum 941;
- non-grave non-Survey rows: 133, raw one-pass blue sum 670.

The non-Survey sums are **not** a progression forecast because alternate-station duplicates and DLC rows coexist. They are inventory checks only.

Base-game recovery evidence includes positive-blue repeatable recipes outside graves in multiple independent systems:
- corpse handling/extraction;
- glass/metal/material processing;
- writing -> notes/chapter/books;
- paper/printing;
- candles/church crafts;
- embalming fluid production;
- prayer/sermon item production.

Several no-`needs_unlock` non-grave rows also exist in the loaded balance, so grave finite mastery is not the sole recovery path.

Conclusion:
**Recovery-path evidence is sufficient. No further broad runtime probe is required before selecting the numeric balance model.**


## BGCR Red Economy Dump 0.5.0

- Purpose: inventory positive-red production/work/object reward paths needed to evaluate red grind and red progression.
- Status: **runtime-complete; accepted with one known mod-added craft excluded from vanilla analysis**.
- Research branch: `research/vanilla-balance-dump`.
- Exact built source state: `9e7251b14a7640bf53af9046efc23c96915dc43d`.
- CI run: `36351939021`.
- GitHub Actions artifact: `10942581753` (`BGCR-Red-Economy-Dump-0.5.0`).
- Handed DLL: `BGCR-Red-Economy-Dump-0.5.0.dll`.
- DLL SHA-256: `e4914d8cb8377f2af0209b9c100258842da64d3d8d5f6b6f820975ee2847fd22`.
- Artifact ZIP digest: `sha256:9b3ac95f24bb2fe893a517c00f1a3f774dfefdb3e39672a50edc44c2a4e185b9`.
- Build result: success.
- Safety: read-only, no Harmony patches, no game/save mutation.
- Evidence target:
  1. no `BGCR_RED_ECON_ERROR`;
  2. one complete dump ending in `BGCR_RED_ECON_DONE`;
  3. positive-red craft rows include grave and non-grave repeatable production;
  4. positive-red work rewards and their linked objects are enumerated;
  5. object-level red drops/direct rewards are enumerated where present;
  6. returned data is sufficient to compare grave red grind against wider red recovery/progression sources.


### Runtime result — BGCR Red Economy Dump 0.5.0

User-returned log: `LogOutput(20260927-213311).log`.

Observed:
- Graveyard Keeper 1.407;
- exact `BGCR Red Economy Dump 0.5.0` loaded;
- no `BGCR_RED_ECON_ERROR`;
- terminal marker:
  `BGCR_RED_ECON_DONE|craft_sources=315|craft_surveys=69|craft_repeatable_visible=209|craft_one_pass_red_sum=3550|work_sources=2|work_one_pass_red_sum=3|object_sources=51|object_work_links=0|object_drop_sources=51|object_direct_param_sources=0`.

Known environment correction:
- `I Neeeed Sticks! 1.6.12` added `wooden_stick` to loaded `GameBalance` before the dump;
- that added recipe contributes 1 red and is one visible repeatable craft;
- excluding it gives 314 positive-red craft rows, 208 visible repeatable rows, and a 3549 raw one-pass craft-red checksum for the otherwise accepted loaded dataset.
- Better Save Soul Rebalance changes grave inputs/Soul Gratitude and Soul tech prices but not the grave recipes' native r/g/b output rewards.
- Queue Everything reported `converted=0 halved=0 fireAdjusted=0`; its forced-multicraft UI behavior does not change the logged reward values.

WorkDefinition note:
- only `shovel_1` (+1R/+1G) and `pickaxe_1` (+2R/+1G) have positive red `WorkDefinition.reward`;
- no loaded `ObjectDefinition.work` points to those IDs, so they are not relied upon as proven active recovery sources.
- Static `WorldGameObject.RewardForWork()` confirms a work reward is consumed only through an object's `obj_def.work` link.

Independent gathering evidence is complete through object drops:
- 51 object definitions contain red in `drop_items`;
- 49 drop 1 red, and the iron/coal quarry nodes drop 2 red;
- examples include trees/stumps, stone nodes, iron ore, coal/iron quarry nodes and breakable scenery.

Probe acceptance target is satisfied for the project decision. No follow-up runtime probe is required.


## Production candidate 0.1.0 — G2 mechanics

- Status: **CI-built; runtime acceptance pending**.
- Branch: `dev/0.1.0`.
- Exact handed source: `3c8b9c744ee77f51ebfb9b60bbf57b6acc2e24a9`.
- CI run: `36355295391`.
- Artifact ID: `10944090446`.
- GitHub artifact: `BetterGraveCraftingRewards-0.1.0-3c8b9c744ee77f51ebfb9b60bbf57b6acc2e24a9`.
- Handoff DLL: `BetterGraveCraftingRewards-0.1.0.dll`.
- Installed assembly identity: `BetterGraveCraftingRewards.dll`.
- DLL SHA-256: `8b5a7f48fcef478bb8638e0a91c223d13f28d57fa1b2ea180d3cae2cd7ebced3`.
- Artifact ZIP digest: `sha256:64c1a8de8e3f9ad8fe9f2a54c8df3b09af20eabd1842bb0a2b9b29a6976b4ba3`.
- Build result: success.
- Earlier internal build from source `1eb7145...` was superseded before user handoff after static review found the Soul Gratitude mastery bypass; it is not an accepted/handed candidate.

### Included behavior

- accepted G2 table for 41 active grave-decoration designs;
- Study-blue changes for 23 existing Survey recipes;
- paired per-design red/blue manufacturing mastery;
- save-native mastery counters;
- equivalent manufacturing variants share one design counter;
- baseline guards against conflicting R/B reward mods;
- player, auto, and Soul Gratitude completions that natively award tech points consume mastery;
- ordinary zombie/worker crafts that strip tech-point output do not consume mastery;
- bounded `BGCR_MASTERY` diagnostics record completed count, awarded R/B and next R/B until mastery exhaustion;
- zero-reward endpoint uses stable native r/b output entries;
- Marble sarcophagus excluded;
- qualitative Study-value tooltip cue **not included**.

### Runtime acceptance result — Phase 1

User-returned log: `LogOutput(20260927-224127).log`.

Accepted observations:
- Graveyard Keeper 1.407 loaded Better Grave Crafting Rewards 0.1.0;
- startup reported `G2 active | designs=41 | craftVariants=47 | studies=23`;
- no Better Grave Crafting Rewards warning/error was emitted;
- `grave_bot_stn_1` completed sequence was:
  - #1: awarded 5R/5B, next 5R/5B;
  - #2: awarded 5R/5B, next 4R/4B;
  - #3: awarded 4R/4B, next 4R/4B;
  - #4: awarded 4R/4B, next 3R/3B;
- crafts #2-#4 were requested as one three-item multicraft/queue and decayed per completed item, proving queue behavior;
- the game was saved, returned to menu, and the same save was reloaded;
- startup re-projected G2 for the loaded save;
- #5 then awarded 3R/3B, proving save persistence and non-reset after reload.

**Phase 1 status: PASS.**

### Research-method checkpoint before Phase 2

The remaining acceptance states are not all guaranteed to be cheap or naturally reachable on the user's ordinary save:
- formerly zero-blue Marble recipe;
- controlled Study verification;
- alternate manufacturing variant identity;
- near-exhausted mastery;
- ordinary worker exclusion;
- Soul Gratitude completion.

Do not ask the user to grind materials, exhaust a long mastery sequence, edit saves or rely on unavailable cheats/developer console.

Before those checks, create a **separate test-only user-operated harness** following `NikichMods/DevRules/RUNTIME_TEST_HARNESS.md`. It may prepare the minimum inputs/state required for the scenario, but each assertion must still exercise the actual production/native path. Production `BetterGraveCraftingRewards.dll` remains unchanged by test UI/tools.

### Original Phase 1 procedure — retained for provenance

Phase 1 — one early design, minimal proof:
1. install only this candidate in place of BGCR research probes;
2. load a save where `grave_bot_stn_1` can be crafted;
3. craft four copies as the player;
4. expected manufacturing sequence: `5R/5B, 5R/5B, 4R/4B, 4R/4B`;
5. save/reload;
6. next copy should continue at `3R/3B`, not reset to 5/5;
7. return `LogOutput.log`; expected diagnostic rows are `BGCR_MASTERY|design=grave_bot_stn_1|completed=1..4`, with awarded sequence `5/5, 5/5, 4/4, 4/4` and next reward after the fourth craft `3/3`.

Phase 2 — after Phase 1 passes:
- verify one formerly zero-blue Marble recipe starts at its G2 value;
- verify one Study reward;
- verify an alternate workstation variant shares mastery where convenient;
- verify mastery exhaustion/worker exclusion with the cheapest deterministic setup available.

Do not request broad all-recipe manual testing unless a concrete mismatch appears.


## Cross-save projection check — PASS

User-returned log: `LogOutput(20260927-230717).log`.

Observed in one process:
- first loaded save: `grave_bot_stn_1 completed=8`, both manufacturing variants projected **1R/1B**, physical output x1;
- the user returned to the main menu and loaded a different save;
- `OnGameStartedPlaying` ran again and production reprojected G2 for the newly loaded save;
- second loaded save: `grave_bot_stn_1 completed=0`, both variants projected **5R/5B**, physical output x1.

This is the required contrasting-value proof that the global `CraftDefinition.output` projection is refreshed from each save's own BGCR counter state and does not leak the previous save's mastery projection.

**Cross-save isolation: PASS.**

### Production mechanics acceptance

All runtime acceptance items for Changes A+B are now satisfied:
- player craft mastery sequence;
- multicraft/queue per-item decay;
- save persistence;
- cross-save isolation;
- formerly-zero-blue Marble projection;
- representative Study mutations;
- equivalent workstation variants sharing one design counter;
- zero-reward endpoint preserving physical output;
- ordinary worker exclusion;
- Soul Gratitude routing.

**Better Grave Crafting Rewards 0.1.0 G2 mechanics: RUNTIME ACCEPTED.**

The research-only BGCR Test Console is no longer required for ordinary use and must not ship with production.


## Production release-hardening candidate 0.1.1

Purpose: remove acceptance-only per-craft mastery logging and prepare the already runtime-accepted G2 implementation for stable distribution without reusing the handed 0.1.0 version.

- Branch: `dev/0.1.1`
- Exact source: `bad22b61450efdcacaaaf74b4ee305b9904c6b8b`
- CI run: `36360595338`
- Artifact ID: `10945591816`
- Artifact: `BetterGraveCraftingRewards-0.1.1-bad22b61450efdcacaaaf74b4ee305b9904c6b8b`
- Handoff DLL: `BetterGraveCraftingRewards-0.1.1.dll`
- DLL SHA-256: `a8bbce8237e627fe2413eac97548d9f02b2b76280dbd065e65130c96a4e9f950`
- Artifact ZIP digest: `sha256:62d2a3788c080b04951a07f495205d011d61b9700de8c64e98f0dd9b3e12b863`
- CI result: **success, 0 warnings, 0 errors**

Static production diff from runtime-accepted 0.1.0:
- version metadata `0.1.0 -> 0.1.1`;
- MPL-2.0 SPDX comments added to source files;
- `BGCR_MASTERY` Info logging and the two locals used only to format that log removed;
- mastery counter mutation, save persistence, reward projection, Study mutation, worker routing and Soul Gratitude routing unchanged.

Acceptance method followed the approved Change D gate: static diff + clean Release build. No repeated installed-runtime G2 pass is required because executable gameplay behavior other than logging is unchanged.

**0.1.1 release-hardening candidate: TECHNICALLY ACCEPTED.**


## Stable version selection — 1.0.0

After the 0.1.1 release-hardening candidate passed technical acceptance, the project scope was reviewed as complete with no unresolved design or implementation questions.

The first public stable release is therefore version **1.0.0**.

The already-handed/recorded 0.1.0 and 0.1.1 binaries remain immutable evidence artifacts. They are not renamed or republished as 1.0.0. Version 1.0.0 receives a fresh deterministic CI build whose executable change from 0.1.1 is version metadata only.


## Stable candidate 1.0.0 — ACCEPTED

- Exact source: `0a971830a62f29f78738a0219610299ebe32428b`
- CI run: `36361332183`
- Artifact ID: `10945069959`
- Artifact: `BetterGraveCraftingRewards-1.0.0-0a971830a62f29f78738a0219610299ebe32428b`
- Handoff DLL: `BetterGraveCraftingRewards-1.0.0.dll`
- DLL SHA-256: `804f3aec98e070c5f41babd5c5343e87fe2bab37b30f5f2ae7571097b988edba`
- Artifact ZIP digest: `sha256:a2ac25413160e8d7380cdfb76cd764f6d2c8295078f777751922a88e30713427`
- CI result: **success, 0 warnings, 0 errors**

Static comparison against the technically accepted 0.1.1 production state shows no G2 behavior change. Production differences are version metadata only.

The 1.0.0 artifact is therefore accepted for stable promotion without another installed-runtime pass.


## Stable publication — v1.0.0

Stable promotion and publication completed.

- `main` promotion state: `d1a1612676efdf46b504736a9cae48461ea5f5f3`
- Release workflow run: `36361478396` — **success**
- GitHub Release ID: `397867204`
- Tag: `v1.0.0`
- Tag/release target: accepted source `0a971830a62f29f78738a0219610299ebe32428b`
- Release asset ID: `594058668`
- Asset filename: `BetterGraveCraftingRewards.dll`
- Asset SHA-256: `804f3aec98e070c5f41babd5c5343e87fe2bab37b30f5f2ae7571097b988edba`
- Asset digest matches the accepted stable candidate exactly.
- Release state: public, stable, not a prerelease.

A separate automatic build of the promoted `main` state (run `36361478243`) succeeded with 0 warnings and 0 errors but produced DLL SHA-256 `31bad71b2138dcb594e5830573158d41c92213443453ac4fceb9404cbf23c6b2`. It is a noncanonical CI artifact from a later documentation-bearing source commit and was never released or handed out.

The build workflow was subsequently hardened so `main` remains compile-checked but no longer uploads candidate artifacts. This preserves a single unambiguous downloadable identity per accepted release.


## Final main CI hygiene check

Run `36361649029` validated the post-release workflow policy:
- build succeeded;
- candidate preparation skipped on `main`;
- candidate upload skipped on `main`;
- no workflow artifact was created.

**Repository release workflow hygiene: PASS.**
