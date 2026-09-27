# Vanilla Grave Decoration Balance Research

Target: **Graveyard Keeper 1.407**

Status: **active research**

This document is the canonical project-local record for vanilla grave-decoration balance facts and the analysis used to select the Better Grave Crafting Rewards model.

## Product question

Does vanilla Graveyard Keeper reward increasingly advanced grave-decoration crafting coherently enough that normal graveyard progression competes with repetitive low-tier technology-point farming?

The goal is not to maximize blue points. The goal is to understand and, if necessary, rebalance the incentive curve while preserving overall pacing and vanilla character.

## Evidence classes

Keep entries explicitly separated as:

- **verified vanilla fact** — established from Graveyard Keeper 1.407 data/runtime/source evidence;
- **community observation** — player behavior/opinion, useful for incentives but not exact internals;
- **balance interpretation** — conclusion drawn from verified data;
- **hypothesis** — plausible but not yet proved;
- **proposal** — candidate rebalance value/model;
- **accepted behavior** — final user-accepted mod behavior.

Wiki/community values are discovery aids, not authoritative 1.407 values.

## Required vanilla dataset

For each relevant grave decoration, establish where applicable:

| Field | Required |
|---|---|
| Internal recipe/item ID | yes |
| Display/name mapping | yes |
| Decoration family/type | yes |
| Grave quality | yes |
| Craft station/path | yes |
| Input materials and quantities | yes |
| Material processing depth | derived |
| Energy/craft cost | yes |
| Red tech reward | yes |
| Green tech reward | yes |
| Blue tech reward | yes |
| Recycling recipe/returns | yes |
| Net repeat-loop resource loss | derived |
| Study reward | yes |
| Prerequisite technology | yes |
| Technology cost | yes where material |
| Earliest practical progression stage | derived/evidenced |

## Derived comparisons

Once the base table is verified, calculate at least:

- tech points per craft;
- blue points per craft;
- blue points per unit of non-returned material in repeatable recycle loops;
- blue points relative to energy/craft effort where comparable;
- reward progression versus grave quality;
- reward progression versus material/processing depth;
- effect of prerequisite technology cost;
- total blue generation from representative normal graveyard upgrades;
- optimal repeatable farming recipe at representative progression stages.

The key anti-regression question is:

> After a rebalance, did the optimal grind disappear as a dominant strategy, or did it merely move to another recipe?

## Current established project-level observations

**Community observation / discovery, not yet canonical vanilla-data proof:**
- players commonly use early stone grave decoration crafting as a repeatable source of blue technology points;
- recycling can materially improve the efficiency of that loop;
- community discussion has questioned why more advanced grave decorations do not consistently yield proportionally stronger repeatable rewards.

These observations justify investigation but do not establish exact 1.407 values.

## Research-method checkpoint

### Question

Establish the exact Graveyard Keeper 1.407 data and ownership path for grave-decoration crafting rewards and the adjacent fields needed to evaluate progression.

### Existing evidence

Shared research already verifies that `GameBalance` owns craft collections and that `CraftDefinition` is the common native recipe-data model for many crafting paths. It does **not** yet contain the grave-decoration reward table or prove every adjacent field needed here.

### Preferred method order

1. search existing NikichMods source/history for an accepted data-dump/static-inspection path that already exposes the relevant `GameBalance` data;
2. use direct static inspection if exact 1.407 game data is available to the working environment;
3. only if the dataset cannot be established cleanly that way, create one narrow research-only dumper/probe that reads the native loaded balance data without changing gameplay.

Do not build production behavior until the canonical owner/data path and relevant values are verified.

## Production evidence gate

Current state: **BLOCKED**

Reason: exact 1.407 reward values, complete affected recipe set, canonical reward fields and downstream consumer path are not yet fully established from authoritative evidence.

Allowed work while BLOCKED:
- repository/bootstrap documentation;
- static inspection;
- evidence collection;
- narrow research tooling/probe if justified;
- balance analysis using clearly labelled provisional inputs.

Not allowed:
- production reward mutation;
- final balance values presented as accepted behavior.


## Static ownership findings — 2026-09-27

**Status: verified static facts for Graveyard Keeper 1.407 source reference used by accepted NikichMods research.**

Reference: `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`.

Established:

- `GameBalance.items_data` is the native item-definition collection.
- Grave-decoration item families are represented by native `ItemDefinition.ItemType` values `GraveStone`, `GraveFence`, and `GraveCover`.
- `ItemDefinition.quality` is the base item quality field used by the grave UI path through `Item.GetItemQuality()`.
- `GameBalance.craft_data` owns ordinary craft and Survey definitions.
- A `CraftDefinition` contains native `needs`, `output`, `craft_in`, `energy`, and `craft_time` data.
- Technology points are ordinary craft outputs: `TechDefinition.TECH_POINTS` contains `r`, `g`, `b`, `v`, and `gratitude_points`; `CraftDefinition.GetFirstRealOutput()` explicitly skips these IDs when finding the physical output.
- `CraftComponent.ProcessFinishedCraft()` processes `current_craft.output` through the normal native output/drop path. Therefore repeatable red/green/blue craft rewards are data in the recipe output, not a separate grave-decoration reward algorithm.
- `ItemDefinition.GetSurveyCraft()` resolves the item's Survey recipe from `GameBalance.craft_data`; Survey tech-point rewards are likewise stored in that recipe's `output`.
- `GameBalance.techs_data` owns technology definitions. `TechDefinition.crafts` identifies crafts unlocked by a technology and `TechDefinition.price` is the native technology cost.

### Implementation implication

The leading implementation family is now **host-owned data mutation**: change only the verified `r/g/b` output entries of the selected native grave-decoration craft definitions while leaving `CraftComponent` and the normal reward/drop algorithm intact.

This is not yet READY for production because the exact affected 1.407 recipe IDs/values and complete progression table still need authoritative loaded-data evidence.

## Research probe decision

A new narrow probe is justified.

The missing information is stored in loaded balance data rather than executable method bodies. Reconstructing the complete table manually in-game would require many crafts, Study actions and transcribed values, while a read-only dump can enumerate the canonical collections directly.

Probe contract:

- diagnostic identity: `BGCR Balance Dump 0.1.0`;
- branch: `research/vanilla-balance-dump`;
- read only;
- no Harmony patches;
- no mutation of `GameBalance`, recipes, player state or save state;
- wait until the game has started, dump once, then become inert;
- enumerate native grave-decoration items and their producer/consumer/Survey/technology relationships;
- output machine-readable `BGCR_*` log lines for later analysis.

Expected runtime action: install the diagnostic DLL, load any save until normal gameplay is active, then return `LogOutput.log`. No crafting or other gameplay action should be necessary.


## Runtime grave-decoration dataset — 2026-09-27

**Evidence status:** accepted runtime evidence, with explicit Better Save Soul field reconstruction described below.

The read-only 0.1.0 dump completed successfully on Graveyard Keeper 1.407 and enumerated:

- 42 grave-decoration items;
- 132 producer paths;
- 55 consumer paths;
- 24 Survey/Study definitions.

### Verified reward-shape findings

The repeatable crafting reward curve is not monotonic with grave quality or material sophistication.

Representative native craft outputs:

- wooden decorations: 0 blue;
- early stone decoration crafts: commonly 5 blue;
- quality-5 stone sculptures: 10 blue;
- the first marble fence/cross tiers: 0 blue despite being later and materially more advanced;
- later marble plinth/sculpture recipes reach 15 blue;
- additional late/DLC grave families contain a mixture of 0, 2, 5, 6, 10, and 15 blue rather than a consistent progression.

This verifies the product problem independently of community reports: advancement can lead to an equal or lower repeatable blue reward even while grave quality, prerequisite technology and input sophistication increase.

### Verified recycling structure

The dump also verifies repeatable dismantling for the early wooden/stone family.

Two especially important stone loops are already visible without needing lower-level material-chain assumptions:

- `grave_bot_stn_1`: craft consumes `stone_plate_1 x2`, gives 5 blue; dismantling returns `stone_plate_1 x1`. Net repeat loss: one `stone_plate_1` for 5 blue.
- `grave_bot_stn_2`: craft consumes `stone_plate_1 x1 + stone_plate_2 x1`, gives 5 blue; dismantling returns `stone_plate_2 x1`. Net repeat loss: one `stone_plate_1` for 5 blue.

Thus progression from the first blue-producing stone fence to the later carved-stone fence does not improve blue yield per net basic stone input. At Stone Cutter II both can also use low-energy craft paths, so the early-loop incentive is not naturally displaced by a clearly superior advanced reward curve.

Other recyclable stone decorations either consume more net material for the same 5 blue or move to 10 blue while consuming more advanced material. Exact cross-tier efficiency still requires the lower-level material production chain.

### One-time Study interaction

Study rewards rise strongly with material/tier even where repeatable craft rewards do not. The runtime data contains Study outputs from roughly 11 blue at the earliest wooden grave item through 81/91/101/121-blue late examples.

Interpretation: vanilla already has a strong one-time exploration/progression reward, but that does not repair the repeatable-crafting incentive curve after the item has been studied.

### Better Save Soul runtime contamination and reconstruction

Better Save Soul Rebalance 1.1.1 was active in the evidence session.

Before the BGCR dump it changed ten Better Save Soul grave recipes and related Soul technology prices. Inspection of its accepted source at `0772e4484fd3e9a91bdee386a802e05909cbce93` establishes:

- grave `r/g/b` craft outputs are untouched, so the logged repeatable technology-point rewards remain vanilla;
- mutated grave `needs` and Soul Gratitude costs must not be treated as vanilla directly;
- mutated Soul technology prices must not be treated as vanilla directly;
- the mod's guarded `Stock` / stock-price values are the vanilla baselines it requires before applying its changes, so those affected fields are reconstructable without another clean-game probe.

### Remaining dataset gap

The current evidence is sufficient to prove the reward progression defect and the native mutation seam, but not yet sufficient to compare all recipes on common resource/processing terms.

Still needed:
- producer recipes for the direct grave-crafting materials (stone/marble plate stages, iron details, gold jewelry detail, wood components, etc.);
- their energy/time and prerequisite technologies;
- enough backward material-chain data to calculate comparable processing depth and net repeat-loop cost.

A second read-only material-chain dump is justified because direct inspection / existing NikichMods research does not currently contain those exact loaded 1.407 component recipes, and manual reconstruction would introduce avoidable transcription and arithmetic risk.
