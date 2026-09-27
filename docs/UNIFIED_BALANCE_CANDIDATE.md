# Unified Balance Candidate G2

Target: **Graveyard Keeper 1.407**

Status: **product model accepted; production implementation pending evidence gates**

Evidence basis:
- accepted grave-recipe / Study / recycle dump 0.1.0;
- accepted component-chain dump 0.2.0;
- accepted technology / multi-quality / station scope dump 0.3.0;
- accepted whole-game blue economy dump 0.4.0;
- accepted whole-game red economy dump 0.5.0.

## Goal

The candidate must make the reward system coherent:

- a newly unlocked design is worth learning;
- a more advanced design starts more rewarding than a primitive design;
- elaborate monuments/sculptures can be more rewarding than plain decor at a similar progression point;
- several useful copies continue to teach the Keeper;
- repeating one cheap design forever eventually teaches nothing;
- Study is a major one-time knowledge event;
- ordinary component work and actual grave installation remain native background sources.

No production code should be changed until the user accepts the balance direction and the production evidence gate is explicitly READY.

## Concrete model alternatives

### Pure grave-quality formula

Rejected as the sole rule.

Quality is useful inside a family, but base-game technology order is not identical to grave quality. First marble decorations can have lower nominal quality than the immediately preceding stone-monument tier, so a quality-only formula can make a later technology feel like a reward regression.

### Pure technology-band formula

Rejected as the sole rule.

It fixes technology progression cleanly but fails to recognize meaningfully more elaborate items inside a band, especially plinths, memorials, sculptures and statues.

### Selected G2 hybrid

Use a **progression rank** plus a very small **complexity premium**.

This keeps the system explainable while reflecting both progression and obvious production sophistication.

## Mastery shape

For a color with starting value `S`, successful manufacturing of the same decoration design pays:

`S, S, S-1, S-1, S-2, S-2, ... 1, 1, 0, 0...`

Equivalent formula for successful manufacture number `n`:

`reward(n) = max(S - floor((n - 1) / 2), 0)`

Lifetime mastery pool for one color:

`S(S+1)`

The paired shape remains preferred because it preserves a useful 10–15-copy building window much better than a one-step-per-craft decay while still reaching a hard zero.

## Mastery identity

The counter belongs to the **produced grave-decoration item/design**, not to an individual `CraftDefinition.id`.

Accepted 0.1.0 evidence shows equivalent station recipes such as `grave_bot_stn_1` / `grave_bot_stn_1_2` and several wood variants. Separate counters would let the same design be mastered repeatedly by switching stations.

Therefore:
- equivalent manufacturing crafts for the same grave item share one counter;
- removal from a grave does not increment mastery;
- repair/fixing does not increment mastery;
- installing a decoration does not increment manufacturing mastery;
- only a successful intended manufacturing craft does.

This is a required invariant for the production gate.

## Base progression rank

Core base-game technology chain:

| Technology band | Base rank |
|---|---:|
| Grave plate | 3 |
| Simple gravestones | 4 |
| Stone gravestones | 5 |
| Carved gravestones | 6 |
| Grave monuments | 7 |
| Marble gravestones | 8 |
| Carved marble gravestones | 9 |
| Crypts | 10 |

For non-core / DLC grave recipes, the default base rank is:

`grave quality + 3`

This continues the same numerical scale without forcing unrelated DLC unlock trees into the base-game technology chain.

## Complexity premium

Add to the base rank:

- ordinary fence / cross / plate / marker: **+0**;
- stella/plinth or memorial: **+1**;
- sculpture/statue/high-angel/woman-saver family: **+2**.

The result is the candidate starting mastery value `S`.

This premium is deliberately small. It rewards obvious sophistication without turning resource-cost arithmetic into an opaque formula.

## Color rules

### Red

`red start = S` for every in-scope active grave decoration.

Red then follows paired mastery to zero.

Unchanged native red channels:
- component production;
- gathering/object drops;
- +1 red for installing grave decoration;
- all unrelated red sources.

No new red Study reward is added in G2.

### Blue

For Grave plate and Simple gravestones:
- manufacturing blue remains **0**;
- Study remains the knowledge route.

From Stone gravestones onward:

`blue start = S`

Blue follows the same paired mastery counter/shape as red.

Unchanged native blue channels:
- component production;
- unrelated repeatable blue recipes;
- unrelated Study rewards.

## Study rule

Only existing grave-decoration Survey/Study recipes are changed. No new Study recipes are created.

For an existing Study recipe:

`new Study blue = max(round vanilla Study blue up to the next 10, 10 × (S - 1))`

Properties:
- Study reward never decreases;
- odd raw 11/21/31/... values become clean tens;
- more sophisticated decorations naturally study for more;
- Faith and Science costs remain unchanged;
- Study remains one-time and optional;
- red/green Study outputs remain unchanged.

## Full active recipe table

### Core

| Recipe | Q | Vanilla R/B | Start S (R/B) | Study B |
|---|---:|---:|---:|---:|
| `grave_top_wd_tab_1` | 1 | 3/0 | 3/0 | 11→20 |
| `grave_bot_wd_1` | 1 | 2/0 | 4/0 | 21→30 |
| `grave_top_stn_plate_1` | 2 | 5/0 | 4/0 | 21→30 |
| `grave_top_wd_cross_1` | 2 | 5/0 | 4/0 | 21→30 |
| `grave_bot_stn_1` | 2 | 2/5 | 5/5 | 31→40 |
| `grave_top_stn_cross_1` | 3 | 5/5 | 5/5 | 31→40 |
| `grave_top_stn_plate_2` | 3 | 5/5 | 5/5 | 31→40 |
| `grave_bot_stn_2` | 3 | 2/5 | 6/6 | 31→50 |
| `grave_top_stn_cross_2` | 4 | 5/5 | 6/6 | 31→50 |
| `grave_top_stella_stn_1` | 4 | 5/5 | 7/7 | 31→60 |
| `grave_bot_mrb_1` | 4 | 5/0 | 8/8 | 81→90 |
| `grave_top_mrb_cross_1` | 5 | 5/0 | 8/8 | 81→90 |
| `grave_bot_mrb_2` | 5 | 5/0 | 9/9 | 91→100 |
| `grave_top_sculpt_stn_1` | 5 | 5/10 | 9/9 | 51→80 |
| `grave_top_sculpt_stn_2` | 5 | 5/10 | 9/9 | 51→80 |
| `grave_top_mrb_cross_2` | 6 | 5/0 | 9/9 | 91→100 |
| `grave_top_stella_mrb_1` | 6 | 5/15 | 10/10 | 91→100 |
| `grave_top_sculpt_mrb_1` | 7 | 5/15 | 12/12 | 101→110 |
| `grave_top_sculpt_mrb_2` | 7 | 5/15 | 12/12 | 101→110 |

### Game of Crone / refugee grave families

| Recipe | Q | Vanilla R/B | Start S (R/B) | Study B |
|---|---:|---:|---:|---:|
| `grave_bot_stn_3` | 3 | 5/0 | 6/6 | — |
| `grave_bot_stn_4` | 4 | 5/0 | 7/7 | — |
| `grave_top_memorial_stn_1` | 4 | 10/6 | 8/8 | — |
| `grave_bot_mrb_3` | 5 | 5/0 | 8/8 | — |
| `grave_bot_stn_5` | 5 | 5/2 | 8/8 | — |
| `grave_bot_mrb_4` | 6 | 5/0 | 9/9 | — |
| `grave_top_memorial_mrb_1` | 6 | 10/6 | 10/10 | — |
| `grave_bot_mrb_5` | 7 | 5/2 | 10/10 | — |
| `grave_top_womansaver_stn_1` | 6 | 10/15 | 11/11 | — |
| `grave_top_highangel_stn_1` | 7 | 15/15 | 12/12 | — |
| `grave_top_womansaver_mrb_1` | 8 | 10/15 | 13/13 | — |
| `grave_top_highangel_mrb_1` | 9 | 15/15 | 14/14 | — |

### Better Save Soul

| Recipe | Q | Vanilla R/B | Start S (R/B) | Study B |
|---|---:|---:|---:|---:|
| `grave_bot_stn_6` | 6 | 5/0 | 9/9 | — |
| `grave_bot_stn_7` | 7 | 10/5 | 10/10 | — |
| `grave_bot_stn_8` | 8 | 15/10 | 11/11 | 101→110 |
| `grave_bot_mrb_6` | 9 | 7/0 | 12/12 | — |
| `grave_bot_mrb_7` | 10 | 9/5 | 13/13 | — |
| `grave_bot_mrb_8` | 11 | 10/15 | 14/14 | 121→130 |
| `grave_top_sculpt_stn_4` | 10 | 15/15 | 15/15 | — |
| `grave_top_sculpt_stn_5` | 12 | 17/15 | 17/17 | 101→160 |
| `grave_top_sculpt_mrb_4` | 13 | 15/15 | 18/18 | — |
| `grave_top_sculpt_mrb_5` | 15 | 17/15 | 20/20 | 121→190 |

## Marble sarcophagus exclusion

`grave_top_sarcofag_mrb_1` exists in loaded balance data (quality 10, Study 101) but has no technology owner in the accepted runtime dump.

External community documentation also labels the Marble sarcophagus **Not Implemented**.

G2 therefore excludes it from production mutation rather than inventing an availability rule for an inaccessible data row.

If future host evidence shows it becomes player-obtainable in the target build, it should receive its own evidence gate before inclusion.

## Representative base-game path

Using the already accepted representative six-stage path and unchanged component rewards:

### Direct grave manufacturing + Study

| Copies of each representative recipe | Vanilla blue | G2 blue | Delta |
|---:|---:|---:|---:|
| 5 | 845 | 1095 | +250 |
| 10 | 1070 | 1320 | +250 |
| 15 | 1295 | 1425 | +130 |
| 20 | 1520 | 1458 | -62 |
| 30 | 1970 | 1464 | -506 |

Interpretation:
- G2 is deliberately more generous through normal 5–15-copy development;
- it crosses below vanilla around the 20-copy region;
- it then reaches a hard ceiling instead of scaling indefinitely.

Unchanged manual component blue adds equally to vanilla and G2 and does not alter the delta.

### Final grave-manufacturing red

| Copies | Vanilla red | G2 red | Delta |
|---:|---:|---:|---:|
| 5 | 220 | 345 | +125 |
| 10 | 440 | 570 | +130 |
| 15 | 660 | 675 | +15 |
| 20 | 880 | 708 | -172 |
| 30 | 1320 | 714 | -606 |

Component red and +1 installation red remain additional unchanged streams.

This is the intended shape:
- noticeably better reward while learning and performing a useful batch;
- near-vanilla cumulative reward around the 15-copy scale;
- strongly worse as an indefinite grind.

## Whole-family stress checks

These are not claims that a normal player crafts every family recipe N times. They are scale checks.

At N=10 copies of every active recipe in each family:

| Family | Vanilla red | G2 red | Vanilla blue + Study | G2 blue + Study |
|---|---:|---:|---:|---:|
| Core active recipes | 840 | 972 | 1949 | 2150 |
| Game of Crone families | 1000 | 920 | 760 | 920 |
| Better Save Soul families | 1200 | 1190 | 1394 | 1780 |

At N=20 every family is below or near the corresponding vanilla infinite-craft trajectory, and long-run G2 totals are finite.

## Study scale

Across the 23 active studyable grave decorations after excluding the unused sarcophagus row:

- vanilla Study blue checksum: **1443**;
- G2 Study blue checksum: **1840**;
- delta: **+397 blue**.

This does not arrive automatically:
- every reward remains gated by the item's one-time Study action;
- existing Faith and Science requirements are preserved;
- the player must obtain/carry the item and choose to study it.

The planned qualitative Study-value cue becomes more important under G2 because more progression value is intentionally concentrated there.

## Why G2 is stronger than the strict band-only candidate

A strict technology ladder gave clean numbers but treated a basic fence and an elaborate sculpture too similarly.

G2 adds only two small structural concepts:
- +1 for plinth/memorial-style sophistication;
- +2 for sculpture/statue-style sophistication.

That is enough to preserve the desired feeling that expensive visual/production complexity matters without reverting to a hand-tuned reward table.

## Known tradeoff

Some currently 15-blue Game of Crone statues begin below 15 in G2 (11–14) before mastery decays.

This is not an inflation-control requirement; it is a consequence of applying one coherent progression scale.

If product review decides that an advanced decoration must never have a lower **first-craft** reward than vanilla, that becomes a separate invariant and the candidate should be revised before implementation.

Do not silently add a `max(vanilla, G2)` rule: that would reintroduce flat 15-point plateaus and weaken the progression logic.

## Decision state

Research data: **sufficient**.

Balance model G2: **accepted by the user for production design, including scale-over-vanilla precedence**.

Production implementation is governed by `docs/PRODUCTION_EVIDENCE_GATE.md`. The accepted product rule is that the coherent G2 scale outranks conflicting local vanilla rewards; do not clamp first-craft values back to vanilla merely to reduce numerical deviation.

No additional broad balance probe is justified.
