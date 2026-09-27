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


## Model E — Discovery-weighted redistribution

### Motivation

Move part of the repeatable blue reward out of grave-decoration crafting and into the existing one-time Survey/Study reward for the same item.

Player-facing intent:

- early repeat crafting becomes a weak fallback rather than the efficient progression route;
- increasingly advanced grave crafts can still pay somewhat more than early crafts;
- the first time a player makes and studies a new decoration remains strongly rewarding;
- repeated dismantle/recraft loops lose most of their advantage;
- no new persistent first-craft state is required because vanilla Study already supplies the one-time state.

### Exact vanilla scale

Accepted runtime data contains:

- 24 grave-decoration Study recipes;
- total Study faith cost: **145 Faith**;
- total grave-decoration Study blue output: **1544 blue**;
- one-of-each final-craft blue checksum across 42 grave decorations: **266 blue**;
- reconstructed all-DLC technology-tree blue cost: **3715 blue**.

Therefore, for the simple first-copy checksum `one craft of every decoration + every available Study`, Study already contributes about **85.3%** of the blue associated with grave decorations.

Average current grave-decoration Study efficiency is about **10.65 blue per Faith**.

Illustrative redistribution without changing Faith cost:

- moving +50 blue into existing Study rewards -> 1594 Study blue, ~10.99 blue/Faith;
- moving +75 -> 1619 Study blue, ~11.17 blue/Faith;
- moving +100 -> 1644 Study blue, ~11.34 blue/Faith.

This is a modest efficiency increase rather than a new Faith-cost burden.

### Important distinction: Faith cost vs Faith dependence

Increasing the `b` output of existing Survey recipes does **not** consume additional Faith if their `needs` entries stay unchanged.

The behavioral change is instead:

- more of the player's blue progression becomes contingent on using the Study table;
- Faith timing therefore matters more;
- players who ignore sermons / spend Faith heavily elsewhere have less access to repeatable grave-craft catch-up.

This distinction matters for evaluating pacing.

### Early-game concentration

The 12 wood/stone Study recipes through the two vanilla stone sculptures consume **37 Faith** and pay **362 blue** in the accepted dataset.

Thus the early/mid game already asks the player to route a meaningful amount of Faith into knowledge progression before marble. Shifting too much additional blue away from crafting could make a missed sermon or competing Faith use feel more punitive even if total Faith cost is unchanged.

### Strong implementation form: first-copy package conservation

For studyable decorations whose repeat craft reward is reduced:

`new Study blue = old Study blue + (old Craft blue - new Craft blue)`

Example:

- Stone grave fence: vanilla first-copy package = 5 craft + 31 Study = 36.
- If repeat craft is reduced to 2, set Study to 34.
- A normal player who crafts one and studies it still receives 36 total.
- A grinder receives only 2 per additional craft instead of 5.

This is a particularly clean anti-grind transformation because it changes repeat incentives without silently taxing the intended `craft one -> study it -> use it` progression.

The same rule does not have to be applied to every advanced item. Later recipes can receive a modest craft increase where progression currently has a zero-reward hole; that becomes an explicit progression bonus rather than compensation.

### Non-studyable DLC constraint

18 of the 42 identified grave-decoration items do not currently expose a Survey recipe in the accepted dataset.

Creating new Survey recipes for them would broaden the mechanism, introduce new Faith/Science costs and require separate host/UI acceptance.

Therefore the first implementation should **not** create new Study recipes merely to preserve a global point budget.

For non-studyable DLC decorations:
- prefer leaving existing craft reward unchanged unless progression evidence justifies a direct adjustment;
- evaluate them separately from the studyable vanilla/core line.

### Community evidence

Community discussion strongly supports both sides of this tradeoff:

- Players repeatedly describe Study as the intended/main source of early blue and advise against crafting items solely for point yield because that loop is boring.
- The Stone grave fence is repeatedly recommended as the cheap repeatable escape valve: two stone -> 5 blue, dismantle -> one stone back.
- Recent 2026 discussions still recommend this route, so the incentive has not disappeared through ordinary player discovery.
- Other players explicitly complain that Study is constrained by weekly Faith and competing Faith uses; some describe the Faith gate itself as frustrating.
- One detailed community criticism compares the high Faith/material effort of studying advanced marble decoration against simply mass-producing Stone grave fence II for equivalent blue. That is almost exactly the failure mode this mod is intended to remove.

Interpretation:
- moving reward toward Study is aligned with the game's thematic/intended knowledge loop;
- eliminating repeatable craft blue entirely would over-correct because the craft path currently acts as a catch-up / escape valve from Faith timing;
- the strongest direction is therefore **Study-heavy, not Study-only**.

### Updated leading direction

Model E is now favored over C1 as the conceptual base, with a small progression curve retained on repeat crafting.

Recommended structure before exact values:

1. keep wood at 0 repeat blue;
2. reduce cheap recyclable early stone crafts substantially;
3. retain a small repeatable floor so players cannot become effectively Faith-gated;
4. allow advanced stone / marble repeat craft rewards to rise gradually, but slower than recipe/resource sophistication;
5. move the removed early/mid repeatable budget into the corresponding existing Study rewards;
6. prefer per-item first-copy package conservation where practical;
7. preserve existing Study Faith/Science costs;
8. do not add Survey recipes to currently non-studyable DLC decorations in the first behavior change;
9. leave component-craft blue and non-grave blue sources untouched.

C1 remains useful as a comparison candidate but is no longer the preferred conceptual model.
