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


## Material-chain runtime findings — BGCR Material Dump 0.2.0

**Evidence status:** accepted for resolved ordinary material recipes; multi-quality carved marble remains open.

The 0.2.0 runtime dump found 16 direct grave-crafting inputs and walked 67 material nodes to depth 3.

Important verified component rewards include:

- `stone_plate_1`: 0 blue;
- `stone_plate_2`: 1 blue per craft;
- `stone_plate_3`: 0 blue;
- `marble_plate_1`: 1 blue per craft, producing three pieces per batch;
- `marble_plate_2`: 1 blue per craft;
- `detail_3`: 1 blue per craft;
- `jewelry_detail_gold`: 1 blue per craft;
- ordinary iron/wood/basic resource processing is predominantly red/green rather than blue.

This confirms that final grave-decoration reward comparison must account for blue points already generated during advanced component production. It also means a final-recipe-only comparison would overstate the relative disadvantage of some later decorations.

### Multi-quality gap

`marble_plate_3:1`, `:2`, and `:3` were falsely classified as terminal roots by 0.2.0.

Static 1.407 inspection explains why: `Item.is_multiquality` is represented by a base output item whose `multiquality_items` list contains the concrete quality IDs, and `CraftDefinition.IsMultiqualityOutput()/GetMultiqualityResult()` resolves the produced quality. The 0.2.0 probe matched only exact `output Item.id`, so it could not associate the base multi-quality recipe with the three concrete carved-marble variants.

This is a real evidence gap because carved marble is used by several high-tier grave decorations.

### Recursion boundary

The unrestricted producer walk is now rejected as the evidence method for further depth.

Of 959 producer rows, 882 were generic `MixedCraft` alchemy combinations reached through side resources such as water/oil. These recipes are not needed to evaluate grave-decoration reward progression. Further recursion would add noise rather than decision-relevant evidence.

### Final dataset boundary

Before balance-model trade study, collect only the remaining decision-relevant facts:

1. exact multi-quality carved-marble production recipe, inputs, energy/time, blue output, difficulty/perk/quality mechanism and unlock;
2. prerequisite graph for technologies that unlock grave recipes and their material-production paths;
3. station availability for the workstations those relevant crafts require;
4. aggregate vanilla technology-tree blue cost as the denominator for pacing/inflation analysis.

After these four fields are established, production-economy evidence is considered sufficient. Normal-play scenarios (representative counts of upgraded graves) are analysis cases derived from the verified table and do not require more runtime discovery.


## Research-method checkpoint — final scope-completion probe

### Question

Close only the remaining evidence gaps before balance-model trade study:

- identify the native multi-quality producer for `marble_plate_3:1/:2/:3`;
- recover the prerequisite graph and prices for technologies relevant to grave-decoration progression;
- map availability of the crafting stations observed in accepted grave/material evidence;
- obtain the aggregate technology-tree point cost needed to judge inflation scale.

### Existing path

The accepted 0.1.0 and 0.2.0 dumps already prove grave recipes, rewards, recycling, Study, most component chains and their craft rewards. Static 1.407 inspection proves the multi-quality mechanism and TechDefinition parent/unlock fields, but executable source does not contain the exact loaded balance rows/prices/links.

### Decision

One final read-only scope-completion dump is justified. It must **not** recurse material producers again.

It will:
- emit only the carved-marble multi-quality producer(s);
- emit technology definitions/prices/parents so progression paths and total tree cost can be calculated offline;
- emit only object-blueprint records whose `out_obj` matches workstations already observed in accepted 0.1/0.2 evidence.

After that runtime evidence, no further broad balance-data probe is planned unless the returned data reveals a concrete missing owner/field.


## Final scope-completion findings — BGCR 0.3.0

**Status: accepted. Broad vanilla-data collection is complete for balance-model selection.**

### Carved marble

The native carved-marble producer is one multi-quality craft:

- craft ID: `marble_plate_3`;
- variants: `marble_plate_3:1`, `:2`, `:3`;
- needs: `chisel:chisel_2 x1 + marble_plate_2 x1 + faith x5`;
- outputs: one carved-marble result plus 3 red, 1 green, **2 blue**;
- energy expression: 20;
- time: 3;
- difficulty: 1;
- linked perks: mason, engineer, industriousness;
- workstation: `mf_hammer_1`;
- unlock: `The art of stone` (300 red / 50 green / 50 blue).

This closes the multi-quality gap from 0.2.0 and confirms another embedded blue contribution in late grave-decoration production.

### Relevant progression graph

Core grave-decoration unlock chain:

`Grave plate -> Simple gravestones -> Stone gravestones -> Carved gravestones -> Grave monuments -> Marble gravestones -> (Carved marble gravestones | Crypts)`

Cumulative blue cost along that chain:

- Stone gravestones: 15 blue including ancestors;
- Carved gravestones: 45;
- Grave monuments: 95;
- Marble gravestones: 195;
- Carved marble gravestones: 345;
- Crypts: 345.

Relevant stone/material progression:

`The idea of the stone -> Stone processing -> Stone carving -> Marble Quarrying -> The art of stone`

Cumulative blue cost:
- Stone processing: 0;
- Stone carving: 50;
- Marble Quarrying: 100;
- The art of stone: 150.

Thus later grave recipes are gated by substantially more blue investment in both the grave branch and the material-production branch while their repeatable final-craft blue rewards remain irregular.

### Aggregate blue denominator

The runtime dump reported 4615 blue across 187 technology definitions, but Better Save Soul Rebalance 1.1.1 was active and had already increased four Soul technology prices by a total of 900 blue.

Its accepted source records guarded vanilla baselines of 0 blue for:
- `soul_stone_fences` (+150 in the mod);
- `soul_marble_fences` (+200);
- `soul_stone_statues` (+250);
- `soul_marble_statues` (+300).

Reconstructed vanilla all-DLC technology-tree blue cost:

**3715 blue**.

This is now the pacing denominator for candidate-model inflation analysis.

### Evidence boundary conclusion

The accepted 0.1.0, 0.2.0 and 0.3.0 evidence now covers the decision-relevant dataset:

- grave-decoration identities and qualities;
- final recipe materials, r/g/b rewards, energy/time and unlocks;
- dismantling/recycling paths;
- Study rewards;
- ordinary and multi-quality component production;
- embedded component-craft blue rewards;
- technology parent graph and cumulative unlock cost;
- relevant workstation availability;
- aggregate vanilla blue technology cost.

No additional broad runtime dump is justified before the balance-model trade study. Further runtime research should be opened only for a concrete unresolved mechanism discovered during implementation or acceptance.


## Research-method checkpoint — whole-game blue recovery paths

### Question

Before accepting finite grave-craft mastery, establish whether Graveyard Keeper 1.407 still provides sufficient **non-grave** one-time and repeatable blue sources for a player who spends blue poorly and has exhausted a grave recipe's finite mastery.

### Existing path

Accepted project evidence already proves:
- all grave-decoration craft/Study blue rewards;
- relevant component-craft blue rewards;
- the complete technology-demand denominator.

Shared research proves that blue points are ordinary `CraftDefinition.output` entries.

What is still missing is the loaded 1.407 inventory of **all other positive-blue CraftDefinition outputs**. Community guides can identify examples, but are not authoritative enough to validate a whole-game recovery invariant.

### Justification

One final narrow read-only enumeration is justified.

It will **not** rediscover grave/material progression. It will only emit `craft_data` rows whose native `output` contains positive blue points, with fields needed to classify:
- Survey / one-time research;
- ordinary repeatable craft;
- hidden/scripted/automatic paths;
- station, needs, output and technology owner/DLC gate.

This is cheaper and less error-prone than reconstructing the full blue-source economy from wiki/community tables.

The probe remains research-only, performs no patches and mutates no game/save state.


## Whole-game blue recovery findings — accepted 0.4.0

The 0.4.0 runtime inventory closes the recovery-path question raised by finite grave-craft mastery.

Loaded Graveyard Keeper 1.407 contains:
- 194 one-time Survey rows with positive blue output;
- 161 non-Survey positive-blue craft rows;
- 151 rows classified by the probe as visible, non-hidden, non-one-time, non-auto ordinary repeatable recipes.

The inventory contains many non-grave repeatable blue paths in the base game, including:

- anatomy/corpse operations (5-blue extraction rows and 1-blue insertion rows);
- glass/material processing (typically 1–2 blue);
- stone/marble component processing (1–2 blue);
- writing: notes 3, chapter 5, hard-cover book 15;
- printing/flyers (2–3);
- candles (2);
- embalming-fluid production (3);
- prayer/sermon item production (10–15).

This does not mean every source is available at every instant; individual technologies, stations and materials still gate them. It does establish the system invariant needed by this project:

**finite blue mastery on grave decorations does not remove the game's wider repeatable blue economy.**

A player who spends blue poorly is not dependent on an infinite grave-decoration farm as the only recovery mechanism.

No further broad source enumeration is justified before balance selection.


## Research-method checkpoint — red economy

### Question

Determine whether grave-decoration red-point rewards have the same structural problem as blue rewards:

- cheap early grave recipes act as an attractive red grind;
- later/more expensive grave decorations fail to improve red reward coherently;
- reducing grave red repeatability would risk creating a real progression bottleneck because red demand is high.

### Existing evidence

Accepted 0.1.0 / 0.2.0 / 0.3.0 data already proves:

- base-game grave final-craft red rewards are irregular and largely flat:
  - early stone fence: 2 red;
  - many stone headstones: 5 red;
  - stone sculptures: 5 red;
  - early marble fence/cross: 5 red;
  - carved marble fence/cross: 5 red;
  - marble sculptures: 5 red;
- advanced component production adds additional red:
  - basic stone block: 3 red;
  - polished stone: 3 red;
  - carved stone: 2 red;
  - marble block: 2 red per three produced;
  - polished marble: 2 red;
  - carved marble: 3 red;
  - complex iron/gold detail production adds further red.

Therefore final-recipe reward alone is not enough to judge red progression.

The early recyclable stone-fence loop is already known to generate both colors:
- final `grave_bot_stn_1`: 2 red + 5 blue;
- dismantling returns one of two stone blocks;
- replacing the lost `stone_plate_1` generates another 3 red;
- repeat loop therefore yields roughly **5 red + 5 blue per net basic stone block**, before counting raw-resource gathering rewards.

This makes the grind problem explicitly multi-currency.

### Static source finding

Pinned 1.407 source establishes another native red channel:

- `GameBalance.works_data` stores `WorkDefinition.reward`;
- `WorldGameObject.RewardForWork()` applies that reward to the player;
- `ObjectDefinition` also owns `drop_items` and `add_player_param_after_hp_0`, which can independently contain/apply technology-point rewards.

Therefore a red-economy inventory that reads only `CraftDefinition.output` would be incomplete.

### File-vs-probe decision

`GameBalance.LoadGameBalance()` loads `Resources.Load<GameBalance>("game_data")`.

The exact balance rows are therefore stored as a serialized Unity Resources asset rather than a convenient standalone JSON/CSV. Offline extraction would require Unity asset parsing plus managed type metadata and may require multiple resource files.

For the narrow question above, a read-only runtime enumeration of the already-deserialized `GameBalance` is materially simpler, less assumption-heavy and more attributable.

### Decision

Create one narrow **BGCR Red Economy Dump 0.5.0** that emits only positive-red sources from:

1. `craft_data`;
2. `works_data`;
3. object rows whose `work` links to a positive-red WorkDefinition, whose `drop_items` contain red tech points, or whose `add_player_param_after_hp_0` contains red.

The probe must remain read-only and perform no Harmony patches or mutations.

This evidence will be used to decide whether red should:
- remain fully repeatable;
- use a softer diminishing-mastery curve with a nonzero floor;
- or receive another progression rule.


## Whole-game red economy findings — accepted 0.5.0

### Loaded source inventory

After excluding the one known mod-added `wooden_stick` recipe, the accepted loaded-data inventory contains:

- 314 positive-red craft rows;
- 69 positive-red Survey rows;
- 208 visible non-Survey/non-auto repeatable craft rows;
- 51 object definitions with red tech-point drops.

The raw positive-red Survey pool is **2810 red** in the loaded dataset.

Study is therefore already a major vanilla-native red acquisition mechanism, not a blue-only concept. Representative base-game Survey rewards include:
- 50 red for several advanced materials/tools at about 5 Faith;
- 100 red for advanced tools/components at 5–10 Faith;
- 150 red for top practical items such as carved wood, carved marble, damask sword and advanced armor at 10 Faith.

This means adding red to grave-decoration Study would be host-consistent if stage-budget analysis later justifies it. It is not required merely to make the mechanism feel vanilla.

### Wider repeatable red economy

Red is broadly generated outside grave decoration:

- basic/advanced metalworking;
- wood processing;
- stone/marble processing;
- tool production;
- glass/ceramics;
- mining/gathering object drops;
- quarry resource handling.

Representative manual recipes:
- `wood1_2`: 3 red;
- `stone_plate_1`: 3 red;
- `stone_plate_2`: 3 red;
- `detail_2`: 3 red;
- iron tools: 5 red;
- `carved_wood`: 10 red, with Faith/material gating.

Object drops independently give red for ordinary physical gathering. Trees/stumps and stone nodes commonly drop 1 red; loaded iron/coal quarry nodes drop 2.

Therefore grave decorations are not required to remain an infinite red source for recovery.

### Grave-specific red findings

The core base-game final grave-craft curve is strongly flattened:

- early wooden recipes: roughly 2–5 red;
- first stone fence: 2 red;
- most stone headstones/monuments: 5 red;
- first marble fence/cross: 5 red;
- carved marble fence/cross/sculptures: still commonly 5 red.

Later DLC grave families finally rise to 7/9/10/15/17 red, proving that larger grave-craft red values are already within the host's vocabulary.

The classic early stone-fence recycling loop is multi-currency:

- craft `grave_bot_stn_1`: +2 red +5 blue;
- dismantle: recover one of two basic stone blocks;
- manually replace the lost `stone_plate_1`: +3 red.

Thus the repeat cycle produces roughly **5 red + 5 blue per lost basic stone block**, before raw stone acquisition rewards.

### Installation reward

Every loaded `set_grave_*` installation craft emits **+1 red**.

The accepted earlier dataset also proves the corresponding `rem_grave_*` operation returns the decoration with no point reward.

Therefore intended graveyard use has a separate repeatable practical-work reward even if manufacturing mastery later reaches zero.

A technically infinite install/remove loop exists:
- set decoration: +1 red, 5 energy;
- remove decoration: +0 red, 5 energy, item returned.

At ~1 red per 10 energy before interaction overhead, it is substantially weaker than ordinary productive red sources such as stone/wood processing and is not currently a dominant grind candidate. Keep it unchanged unless acceptance testing demonstrates otherwise.

### Design implication

The earlier assumption that red grave-craft output should stay fully repeatable is rejected.

Red can use finite mastery as well.

Because:
- normal component production continues to generate red;
- actual grave installation continues to generate +1 red;
- ordinary gathering/metal/wood/stone work provides repeatable red;
- Study already provides large one-time red rewards elsewhere;

there is no systemic need for grave-decoration manufacturing itself to retain an infinite red tail.
