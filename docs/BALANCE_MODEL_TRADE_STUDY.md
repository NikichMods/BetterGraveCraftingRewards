# Balance Model Trade Study

Target: **Graveyard Keeper 1.407**

Status: **design trade study; no values accepted yet**

This document compares materially different ways to rebalance repeatable technology-point rewards from grave-decoration crafting. It uses the accepted 0.1.0 / 0.2.0 / 0.3.0 runtime dataset recorded in `VANILLA_BALANCE_RESEARCH.md`.

## Quantitative baseline

Dataset checks:

- 42 grave-decoration items were identified.
- Sum of current final-craft blue rewards if every identified decoration were crafted exactly once: **266 blue**.
- Sum of one-time Study blue outputs across the 24 grave decorations that expose Survey recipes: **1544 blue**.
- Reconstructed vanilla all-DLC technology-tree blue cost: **3715 blue**.
- Therefore Study is already the dominant blue reward associated with discovering grave-decoration items; repeatable crafting should complement progression rather than duplicate the Study payout.

These totals are **not** a normal-play forecast. They are scale checks for candidate inflation.

### Core grave progression cost

Cumulative blue technology cost along the core grave branch:

| Unlock | Cumulative blue |
|---|---:|
| Stone gravestones | 15 |
| Carved gravestones | 45 |
| Grave monuments | 95 |
| Marble gravestones | 195 |
| Carved marble gravestones | 345 |
| Crypts | 345 |

Relevant material branch:

| Unlock | Cumulative blue |
|---|---:|
| Stone processing | 0 |
| Stone carving | 50 |
| Marble Quarrying | 100 |
| The art of stone | 150 |

Later decoration crafting therefore requires materially more technology investment even before resource cost is considered.

## Recycling / repeat-farm constraint

Several early and mid stone decorations have explicit dismantling crafts.

Important loops:

- `grave_bot_stn_1`: 2 stone blocks -> 5 blue -> dismantle to 1 stone block. Net repeat loss: 1 basic stone block.
- `grave_bot_stn_2`: 1 basic + 1 polished stone -> 5 blue -> dismantle to 1 polished stone. Net repeat loss: 1 basic stone block.
- `grave_top_stella_stn_1`: consumes polished stone + carved stone + details, but dismantling returns carved stone and both details. Net repeat loss: 1 polished stone; making that polished stone itself yields 1 blue.
- `grave_top_sculpt_stn_2`: 2 carved stones -> 10 blue -> dismantle to 1 carved stone. Replacing the lost carved stone requires the polished-stone chain and faith.

This means a model that only increases rewards with tier can simply move the optimal grind to a later recyclable decoration.

A successful model should distinguish:

1. **normal progression reward** — what a player earns while genuinely improving graves;
2. **repeat-loop efficiency** — what remains after dismantling and rebuilding the cheapest recoverable inputs.

## Model A — Grave-quality ladder

### Rule

Set final-craft blue primarily from grave quality, e.g. monotonic quality bands.

### Strengths

- very transparent;
- easy to explain;
- automatically rewards visually/quality-superior decorations;
- works across base game and DLC without large recipe-specific tables.

### Weaknesses

- grave quality is not identical to production cost or progression timing;
- DLC/quest-unlocked items can have high quality without belonging to the same economic path;
- ignores recycling;
- can make a high-quality recyclable recipe the new dominant grind;
- advanced components already award some blue, so quality-only final rewards can double-count sophistication.

### Assessment

Useful as a **sanity constraint** (advanced quality should not generally pay less), but too crude as the sole balance rule.

## Model B — Production-complexity reward

### Rule

Calculate final reward from inputs, processing depth, energy, prerequisite technologies, and/or embedded component cost.

### Strengths

- economically defensible;
- naturally distinguishes cheap stone from carved marble, metal detail and faith-heavy recipes;
- can be tuned against resource/energy efficiency.

### Weaknesses

- substantial hidden complexity;
- difficult for players to predict;
- component crafting already grants blue in several advanced chains:
  - polished stone: +1;
  - raw/polished marble stages: +1;
  - carved marble: +2;
  - steel/gold-detail paths can add blue;
- risks paying twice for the same production depth;
- small vanilla recipe changes/DLC interactions can make the formula less intuitive.

### Assessment

Best used as an **analysis metric**, not as the player-facing rule.

## Model C — Progression redistribution with recycle adjustment

### Rule

Curate final-craft rewards by progression family/tier, but apply an explicit downward adjustment to cheap highly recoverable loops.

Design shape:

- wood / trivial early decorations: little or no blue;
- first stone tier: modest blue, lower than current farm efficiency;
- carved/advanced stone: meaningful but controlled increase;
- grave monuments / sculptures: stronger reward;
- marble and carved marble: clear step upward instead of vanilla zeroes;
- late DLC decorations: continue progression, with a ceiling to prevent runaway inflation;
- recyclable recipes are checked against **net repeat cost**, not gross recipe cost.

Compensate reductions to the basic fence by moving some reward to ordinary progression crafts that players make to improve graves, especially recipes that currently give zero blue despite being later.

### Strengths

- directly solves the observed product problem;
- remains data-only and host-native;
- simple enough to explain as “better decorations generally reward more knowledge”;
- can preserve roughly similar normal-play blue flow while reducing the one cheap farm loop;
- lets the design account for recycling without exposing a complicated formula;
- does not require persistent state.

### Weaknesses

- requires a curated table rather than one mathematical formula;
- needs explicit checks after every candidate table to ensure the optimum grind did not merely move;
- DLC families need deliberate placement on the progression curve.

### Assessment

**Leading model for the first numeric candidate.**

It is the least-complex model that addresses both progression and the recycle exploit while respecting the fact that Study already supplies a large one-time reward.

## Model D — First-craft / discovery bonus

### Rule

Pay a strong bonus only the first time each grave decoration is crafted, with much smaller repeat rewards.

### Strengths

- directly eliminates repetitive crafting as the intended progression route;
- strongly encourages trying new decorations.

### Weaknesses

- duplicates the existing Study/discovery system;
- requires persistent per-recipe state and save compatibility;
- no longer a native data-only rebalance;
- more UI/communication burden;
- changes vanilla character more than necessary.

### Assessment

Reject for the initial mod unless simpler data-only models fail acceptance.

## Leading design direction

Proceed with **Model C: progression redistribution with recycle adjustment**.

Do **not** choose final numbers from grave quality alone.

For the first numeric candidate:

1. reduce the repeat-loop efficiency of `grave_bot_stn_1` and `grave_bot_stn_2`;
2. compensate normal progression by adding/raising blue on nearby ordinary grave upgrades that are not equally cheap loops;
3. preserve the existing roughly 10-blue role of the advanced stone sculptures unless the full candidate curve justifies a small change;
4. remove the conspicuous zero-blue holes in marble progression;
5. continue progression into DLC families but cap rewards so late crafts do not become absurdly superior farms;
6. leave Study rewards unchanged;
7. leave component-craft rewards unchanged;
8. leave red/green rewards unchanged unless a specific inconsistency is independently justified.

## Candidate evaluation scenarios

Every numeric table must be checked against all of these:

### A. Early repeat farm

Repeated craft/dismantle of the cheapest available stone decorations.

Measure:
- blue per net basic material loss;
- blue per energy;
- unlock cost/stage.

### B. Advanced recyclable farm

Stone stella/cross/sculpture loops after Stone Carving / Grave Monuments.

Goal:
- later progression may be more rewarding;
- it must not become an overwhelmingly efficient replacement exploit.

### C. Normal graveyard development

Use transparent scenario blocks rather than pretending to know one exact average player:

- 10 early stone upgrades;
- 10 advanced-stone upgrades;
- 10 marble/upgraded-marble crafts.

Report vanilla vs candidate repeatable blue for each block.

### D. Full-table scale check

Compare:
- sum of one craft of each affected recipe;
- delta versus vanilla 266-blue grave-craft checksum;
- delta as a percentage of the 3715-blue technology-tree denominator;
- Study remains unchanged at the 1544-blue checksum.

### E. DLC progression

Check Game of Crone and Better Save Soul families separately so free/quest unlocks and Soul Gratitude progression do not distort the base-game curve.

## Current decision state

- Vanilla-data research: **complete**.
- Production mechanism: host-native `CraftDefinition.output` mutation remains the leading seam.
- Balance model family: trade study favors Model C.
- Exact reward table: **not yet accepted**.
- Production evidence gate: remains **BLOCKED** until the first numeric table is selected and its affected recipe set/invariants are made reviewable.


## First numeric candidate — C1 (proposal, not accepted)

Purpose: test the leading Model C shape with conservative inflation and a 15-blue soft ceiling.

### Core / base progression

| Recipe | Vanilla B | C1 B |
|---|---:|---:|
| grave_bot_wd_1 | 0 | 0 |
| grave_top_wd_tab_1 | 0 | 0 |
| grave_top_wd_cross_1 | 0 | 0 |
| grave_top_stn_plate_1 | 0 | 2 |
| grave_bot_stn_1 | 5 | 3 |
| grave_bot_stn_2 | 5 | 3 |
| grave_top_stn_cross_1 | 5 | 4 |
| grave_top_stn_plate_2 | 5 | 4 |
| grave_top_stella_stn_1 | 5 | 5 |
| grave_top_stn_cross_2 | 5 | 5 |
| grave_top_sculpt_stn_1 | 10 | 10 |
| grave_top_sculpt_stn_2 | 10 | 10 |
| grave_bot_mrb_1 | 0 | 6 |
| grave_top_mrb_cross_1 | 0 | 6 |
| grave_bot_mrb_2 | 0 | 8 |
| grave_top_mrb_cross_2 | 0 | 8 |
| grave_top_stella_mrb_1 | 15 | 10 |
| grave_top_sculpt_mrb_1 | 15 | 15 |
| grave_top_sculpt_mrb_2 | 15 | 15 |

Note: `grave_top_stella_mrb_1` is intentionally lower than its vanilla 15 in this first candidate because its quality/tier neighbors are zero-reward recipes and it is not necessary to preserve an isolated outlier while smoothing the curve. This point should be revisited before acceptance; avoiding unnecessary nerfs may justify leaving it at 15.

### Game of Crone / refugee families

| Recipe | Vanilla B | C1 B |
|---|---:|---:|
| grave_bot_stn_3 | 0 | 4 |
| grave_bot_stn_4 | 0 | 5 |
| grave_top_memorial_stn_1 | 6 | 6 |
| grave_bot_stn_5 | 2 | 7 |
| grave_bot_mrb_3 | 0 | 7 |
| grave_top_memorial_mrb_1 | 6 | 9 |
| grave_top_womansaver_stn_1 | 15 | 15 |
| grave_top_highangel_stn_1 | 15 | 15 |
| grave_bot_mrb_4 | 0 | 8 |
| grave_bot_mrb_5 | 2 | 10 |
| grave_top_womansaver_mrb_1 | 15 | 15 |
| grave_top_highangel_mrb_1 | 15 | 15 |

### Better Save Soul families

| Recipe | Vanilla B | C1 B |
|---|---:|---:|
| grave_bot_stn_6 | 0 | 8 |
| grave_bot_stn_7 | 5 | 10 |
| grave_bot_stn_8 | 10 | 12 |
| grave_bot_mrb_6 | 0 | 10 |
| grave_bot_mrb_7 | 5 | 12 |
| grave_bot_mrb_8 | 15 | 15 |
| grave_top_sculpt_stn_4 | 15 | 15 |
| grave_top_sculpt_stn_5 | 15 | 15 |
| grave_top_sculpt_mrb_4 | 15 | 15 |
| grave_top_sculpt_mrb_5 | 15 | 15 |

### Excluded from C1 mutation

`grave_top_sarcofag_mrb_1` remains at vanilla 0 in C1.

Reason: it has no technology owner in the current dump and its exact quest/availability position has not been needed to establish the main balance problem. Leaving it unchanged avoids broadening the first behavior change without evidence that it participates in the problematic incentive curve.

### Scale checks

- one-of-each final-craft checksum: vanilla **266 blue** -> C1 **357 blue**;
- delta: **+91 blue**;
- +91 is about **2.45%** of the reconstructed 3715-blue technology-tree cost;
- Study checksum remains **1544 blue**;
- component-craft rewards remain unchanged.

This checksum deliberately overweights rare recipes relative to normal play, but it is a useful inflation bound/check.

### Normal-play block checks

Representative early pair:
- `grave_bot_stn_1 + grave_top_stn_plate_1`;
- vanilla: 5 blue;
- C1: 5 blue.

For ten graves, this remains 50 blue total. C1 therefore reduces the basic fence's individual farming payout without reducing this simple normal early-grave upgrade block.

Representative advanced-stone pair:
- `grave_bot_stn_2 + grave_top_sculpt_stn_2`;
- vanilla: 15 blue;
- C1: 13 blue.

The small reduction is intentional anti-loop pressure; the sculpture remains a strong advanced reward.

Representative first-marble pair:
- `grave_bot_mrb_1 + grave_top_mrb_cross_1`;
- vanilla: 0 blue;
- C1: 12 blue.

For ten graves, this adds 120 blue, about 3.23% of total vanilla blue technology cost. This is the most material normal-play inflation point in C1 and should be the main pacing decision for user review.

### Repeat-loop check

C1 changes the cheapest stone-fence loops:

- `grave_bot_stn_1`: 5 -> 3 blue, net repeat loss remains one basic stone block;
- `grave_bot_stn_2`: 5 -> 3 blue, net repeat loss remains one basic stone block.

This avoids simply moving the same cheap one-block loop from the first fence to the second fence.

Later recyclable items remain stronger but require more advanced components and/or faith. In particular the vanilla 10-blue stone sculptures are preserved rather than increased, so C1 does not create a new higher reward there.

### Open product decisions before C1 can become accepted

1. Is 6 blue each for the first marble fence/cross an acceptable normal-play acceleration, or should the first marble step be 5/5?
2. Should `grave_top_stella_mrb_1` keep its vanilla 15 instead of being smoothed to 10?
3. Should late DLC recipes remain capped at 15 for vanilla character, or should the very highest tiers rise above 15?

Until these are decided, C1 is a comparison candidate only and production remains BLOCKED.
