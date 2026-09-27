# Base-Game Stage Economy Analysis

Target: **Graveyard Keeper 1.407**

Status: **quantitative design analysis; no candidate accepted**

Scope: base game without DLC. This document uses the accepted 0.1.0 / 0.2.0 / 0.3.0 runtime evidence and deliberately separates:
- direct grave-decoration craft rewards;
- one-time Study rewards;
- unchanged blue generated while manually producing required stone/marble components;
- Faith spent on Study;
- Faith consumed by advanced component/final recipes.

## Base-game technology demand

From the accepted 0.3.0 technology dump, excluding:
- Better Save Soul branch 8;
- technologies explicitly gated by Breaking Dead;
- zero-cost Refugees/Game of Crone hidden rows;

the base-game/no-DLC technology tree costs:

- **5780 red**
- **3117 green**
- **3540 blue**

This is the denominator for progression-scale checks.

## Representative core grave progression

The analysis uses one coherent representative path rather than pretending every player chooses the same cosmetic variant.

| Stage | Representative newly-built recipe(s) | Final-craft blue per batch unit | Study blue | Study Faith | Manual component blue | Production Faith | Next relevant grave/material unlock |
|---|---|---:|---:|---:|---:|---:|---|
| Stone gravestones | `grave_bot_stn_1` + `grave_top_stn_plate_2` | 10 | 62 | 6 | 0 | 0 | Carved gravestones + Stone carving: 80 B / 250 R |
| Carved stone | `grave_bot_stn_2` + `grave_top_stn_cross_2` | 10 | 62 | 6 | 4 per batch unit | 0 | Grave monuments: 50 B / 150 R |
| Grave monuments | `grave_top_sculpt_stn_1` | 10 | 51 | 5 | 2 per item | 3 per item | Marble gravestones + Marble Quarrying: 150 B / 450 R |
| Marble gravestones | `grave_bot_mrb_1` + `grave_top_mrb_cross_1` | 0 | 162 | 14 | `ceil(4N/3)+N` | 0 | Carved marble gravestones + The art of stone: 200 B / 600 R |
| Carved marble | `grave_bot_mrb_2` + `grave_top_mrb_cross_2` | 0 | 182 | 16 | `ceil(4N/3)+5N` | 5 per batch unit | Crypts sibling unlock: 150 B / 300 R |
| Crypts | `grave_top_sculpt_mrb_1` | 15 | 101 | 10 | `ceil(N/3)+3N` | 10 per item | end of core base grave branch |

Notes:
- `N` is the number of copies of each representative newly-built recipe in the stage.
- Manual component blue assumes the ordinary manual material path from accepted 0.2/0.3 data. Zombie/alternate production can change this background contribution, so it is a sensitivity layer, not the invariant balance budget.
- The selected stone headstone path avoids requiring Stone carving before that material technology is intentionally part of the next stage.
- `grave_top_stella_mrb_1` is an explicit vanilla outlier at 15 final-craft blue inside Carved marble; it is not used as the representative path and must be normalized separately in a final table.
- Study and production Faith are separate pressures. Higher tiers already consume substantial Faith during production itself, especially carved marble and marble sculptures.

## Vanilla output at natural-batch scenarios

### Direct grave craft + Study only

| Copies N | Stone | Carved stone | Monuments | Marble | Carved marble | Crypts | Total |
|---:|---:|---:|---:|---:|---:|---:|---:|
| 5 | 112 | 112 | 101 | 162 | 182 | 176 | **845** |
| 10 | 162 | 162 | 151 | 162 | 182 | 251 | **1070** |
| 15 | 212 | 212 | 201 | 162 | 182 | 326 | **1295** |
| 20 | 262 | 262 | 251 | 162 | 182 | 401 | **1520** |

### Including unchanged manual component blue

| Copies N | Vanilla grave-path blue | Share of 3540 base-game blue demand |
|---:|---:|---:|
| 5 | **936** | 26.4% |
| 10 | **1252** | 35.4% |
| 15 | **1565** | 44.2% |
| 20 | **1881** | 53.1% |

Interpretation:
- the user's concern is quantitatively correct: a 2–3 point reduction per repeat craft can remove tens or hundreds of blue during ordinary graveyard expansion;
- grave progression is not a small side source. At natural batches around 10–20, it can represent a very large fraction of total base-game blue demand;
- therefore anti-grind changes must be calibrated as stage budgets, not one-craft deltas;
- component blue remains an unchanged fallback/background source and prevents the final-decoration reward from being the only repeatable knowledge path.

## Faith pressure

Study Faith is fixed per unique item, while advanced production Faith scales with copies.

Representative totals:

| Stage | Study Faith | Production Faith at N=5 | N=10 | N=15 | N=20 |
|---|---:|---:|---:|---:|---:|
| Stone | 6 | 0 | 0 | 0 | 0 |
| Carved stone | 6 | 0 | 0 | 0 | 0 |
| Monuments | 5 | 15 | 30 | 45 | 60 |
| Marble | 14 | 0 | 0 | 0 | 0 |
| Carved marble | 16 | 25 | 50 | 75 | 100 |
| Crypts | 10 | 50 | 100 | 150 | 200 |

This is important for late-tier grind risk: sophisticated marble crafting is already heavily constrained by Faith even before any Study redistribution.

## Diminishing-mastery stress-test ladder

The following start values are **test parameters, not accepted balance values**:

- Stone: 5 blue;
- Carved stone: 6;
- Monuments: 7;
- Marble: 8;
- Carved marble: 9;
- Crypts: 10.

For stages with a fence + marker, each recipe uses the stage start value.

Three shape families are compared:

### F1 — linear

`S, S-1, S-2 ... 1, 0`

Strongest anti-grind / shortest mastery window.

### F2 — three-copy plateau

`S, S, S, S-1, S-2 ... 1, 0`

Preserves several full-value practical crafts before mastery begins to decay.

### F3 — paired steps

`S, S, S-1, S-1, S-2, S-2 ... 1, 1, 0`

Longest finite mastery window of the three.

For comparison, each model shifts enough of the **currently blue-paying** stage's missing 10-copy craft budget into that stage's Study pool to preserve its vanilla N=10 budget. Existing zero-blue Marble / Carved-marble Study is never reduced just to fund a new craft reward.

Required Study-pool shift on the representative path:

| Stage | F1 linear | F2 plateau | F3 paired |
|---|---:|---:|---:|
| Stone | +70 | +50 | +40 |
| Carved stone | +58 | +34 | +20 |
| Monuments | +72 | +58 | +50 |
| Marble | +0 | +0 | +0 |
| Carved marble | +0 | +0 | +0 |
| Crypts | +95 | +78 | +70 |
| **Total** | **+295** | **+220** | **+180** |

These are stage pools, not final per-item Study values. A final allocation must preserve a coherent monotonic Study hierarchy rather than blindly split each pool in half.

## Candidate totals — direct grave craft + Study

| N | Vanilla | F1 linear | F2 plateau | F3 paired |
|---:|---:|---:|---:|---:|
| 5 | 845 | 1180 | 1175 | 1125 |
| 10 | 1070 | 1232 | 1298 | 1330 |
| 15 | 1295 | 1232 | 1303 | 1415 |
| 20 | 1520 | 1232 | 1303 | 1434 |

The apparent N=10 inflation is caused by deliberately repairing the vanilla zero-blue Marble and Carved-marble tiers while also conserving the earlier blue-paying stage budgets.

### With unchanged manual component blue

| N | Vanilla | F1 linear | F2 plateau | F3 paired |
|---:|---:|---:|---:|---:|
| 5 | 936 | 1271 | 1266 | 1216 |
| 10 | 1252 | 1414 | 1480 | 1512 |
| 15 | 1565 | 1502 | 1573 | 1685 |
| 20 | 1881 | 1593 | 1664 | 1795 |

Because component blue is unchanged in every model, the candidate-vs-vanilla delta is identical to the direct table.

## What the matrix says

### F1

Pros:
- strongest removal of long-run grave grind;
- moves the largest share into intentional Study;
- simple and easy to explain.

Cons:
- very front-loaded;
- mastery expires after a small number of items;
- likely too aggressive if 10–15 copies are genuinely normal rather than exceptional.

### F2

Pros:
- gives a short full-value practical batch before decay;
- near vanilla around N=15 on the representative full progression;
- still materially caps N=20+ grind.

Cons:
- requires a large Study shift;
- the three-copy plateau is mechanically arbitrary unless player-use evidence supports it.

### F3

Pros:
- best preserves useful repeat crafting through roughly the 10–15 range;
- still produces a finite lifetime reward;
- moves a meaningful amount into Study while avoiding the sharpest cliff;
- at N=20 remains below vanilla despite repairing late zero-blue tiers.

Cons:
- longest remaining grind tail;
- more total blue is retained in craft than the user's Study-heavy intuition may ultimately prefer;
- exact late-tier start values need downward tuning to avoid excessive Marble-stage inflation.

**Current quantitative leader: F3's long finite-mastery shape, but with lower/tuned start values for the currently zero-blue Marble tiers and a separately smoothed Study allocation.**

This is not yet an accepted balance model.

## Red-point audit

For the representative final grave recipes, direct red income per batch unit is:

- Stone: 7 red;
- Carved stone: 7;
- Monuments: 5;
- Marble: 10;
- Carved marble: 10;
- Crypts: 5.

Thus base-game final grave crafts do **not** provide a clean red progression either; most individual advanced recipes are still flat at 5 red.

However red differs semantically and economically from blue:
- red is broadly generated by component/manual work;
- base-game technology demand is 5780 red, higher than 3540 blue;
- practical repeated work remains a coherent source of red.

Therefore red should receive a separate smoothing pass, not the finite-to-zero mastery rule. Exact red values remain open.

## Remaining whole-game supply question

The current accepted evidence proves:
- total base-game technology demand;
- grave Study/craft income;
- relevant component income.

It does **not** yet enumerate every non-grave repeatable and one-time blue source in loaded 1.407.

That missing table is now decision-relevant because finite grave mastery must preserve recovery after a bad technology purchase.

Before accepting any numeric curve, obtain one narrow read-only inventory of all positive-blue `CraftDefinition.output` sources, classified by:
- Survey/one-time vs repeatable;
- craft type/station;
- needs/output;
- technology unlock where present.

No production change is justified until this recovery-path evidence is closed.


## Recovery-path evidence update — accepted 0.4.0

The whole-game blue inventory is now accepted.

Key result:
- finite grave mastery does **not** remove the wider repeatable blue economy;
- base-game repeatable positive-blue recipes exist in writing/books, anatomy, glass/material production, candles, embalming, sermons/prayers and other systems;
- therefore grave recipes do not need an infinite tail merely to serve as an emergency recovery exploit.

This materially strengthens the case for a true zero-after-mastery endpoint.

## Strict monotonic start ladder

Per the design correction, local vanilla zeroes are not anchors.

For quantitative comparison, retain the paired-step F3 shape and test the strictly increasing start ladder:

`5 -> 6 -> 7 -> 8 -> 9 -> 10`

for:
1. Stone;
2. Carved stone;
3. Grave monuments;
4. Marble;
5. Carved marble;
6. Crypts.

Per-recipe lifetime blue mastery pool under paired steps is:

`S(S+1)`

because each positive reward value is paid twice.

Thus:
- start 5 -> 30 lifetime blue per recipe;
- 6 -> 42;
- 7 -> 56;
- 8 -> 72;
- 9 -> 90;
- 10 -> 110.

For the representative stage path with two recipes in Stone/Carved/Marble/Carved-marble and one in Monuments/Crypts, total finite craft mastery across the entire core progression is **634 blue**.

That total cannot grow further no matter how many decorations are mass-produced.

## Study-redistribution variants with strict mastery

Representative vanilla Study pools across the six core stages total **620 blue**.

Three coherent Study ladders are compared. Values shown are per representative item in each stage; two-item stages therefore contribute twice.

### S1 — restrained

`40 -> 45 -> 60 -> 85 -> 95 -> 105`

Representative Study total: **695** (+75 versus vanilla representative Study).

### S2 — balanced

`40 -> 50 -> 70 -> 90 -> 100 -> 110`

Representative Study total: **740** (+120).

### S3 — Study-heavy

`45 -> 55 -> 80 -> 100 -> 110 -> 120`

Representative Study total: **820** (+200).

These are design test ladders, not accepted exact Survey outputs.

## Combined result — strict F3 mastery + Study ladder

Direct grave-craft + Study only:

| Copies N | Vanilla | S1 restrained | S2 balanced | S3 Study-heavy |
|---:|---:|---:|---:|---:|
| 5 | 845 | 1020 | 1065 | 1145 |
| 10 | 1070 | 1225 | 1270 | 1350 |
| 15 | 1295 | 1310 | 1355 | 1435 |
| 20 | 1520 | 1329 | 1374 | 1454 |
| 30 | 1970 | 1329 | 1374 | 1454 |

Unchanged component blue adds equally to vanilla/candidates for the same production path and therefore does not change the delta.

### Interpretation

All three variants fully repair the zero-blue Marble discontinuity while retaining a strictly increasing mastery start value.

Even **S3 Study-heavy**:
- is more generous than vanilla through ordinary 5–15-copy development;
- is already below vanilla by 20 copies;
- reaches a hard lifetime ceiling after mastery instead of scaling forever.

This is a strong result for the product goal.

The system can therefore afford to move a meaningful amount of blue into Study **without** preserving the infinite craft tail.

### Current qualitative preference

S1 is probably too restrained relative to the desired Study-centric identity.

S2 and S3 are both viable for user/product review:
- **S2** minimizes pacing acceleration while making the intended system legible;
- **S3** more decisively makes Study the primary knowledge event and relies on Faith/Science/logistics as its natural gate.

Because Study is optional, costs Faith/Science, requires the item to be carried to the table, and is now planned to receive a qualitative value cue, S3's apparent front-loading is less automatic than the raw totals suggest.

No exact Study ladder is accepted yet.


## Red finite-mastery analysis — after accepted 0.5.0

### Why red should join mastery

The project no longer treats red as an unlimited "hands-on work" reward.

The same incentive defect exists for red:
- early grave decoration can be mass-produced for technology points;
- the classic stone-fence loop simultaneously yields red and blue;
- advanced base-game grave recipes often remain stuck at 5 red despite much higher input sophistication;
- red technology demand is high enough that players can rationally grind these loops.

At the same time, 0.5.0 proves the wider game has abundant independent red channels, so removing the infinite grave-manufacturing tail does not make graves the sole recovery source.

### Natural-use protection already present in vanilla

Two red streams remain linear with genuine graveyard development even if grave **manufacturing** mastery becomes finite:

1. required component production continues to emit red;
2. installing each decoration on a grave emits +1 red.

This is a useful distinction:
- a grinder who repeatedly manufactures/dismantles one fence loses the final craft reward after mastery;
- a player who actually decorates 10–20 graves still receives component-work red plus installation red for every useful copy.

### Shared paired mastery hypothesis

For the first red quantitative comparison, use the same paired diminishing shape already favored for blue:

`S, S, S-1, S-1, ... 1, 1, 0`.

Test the same strictly increasing core-tier start ladder:

`5 -> 6 -> 7 -> 8 -> 9 -> 10`

for:
1. Stone;
2. Carved stone;
3. Grave monuments;
4. Marble;
5. Carved marble;
6. Crypts.

This is intentionally simple and legible: first-craft mastery becomes more valuable as the technology becomes more sophisticated.

### Final grave-craft red only

Representative vanilla final-craft red per natural batch unit remains:

`7 -> 7 -> 5 -> 10 -> 10 -> 5`

because pair stages contain fence + marker while Monument/Crypt stages use one representative marker.

Comparison:

| Copies N | Vanilla final-craft red | Paired mastery red | Delta |
|---:|---:|---:|---:|
| 5 | 220 | 325 | +105 |
| 10 | 440 | 530 | +90 |
| 15 | 660 | 615 | -45 |
| 20 | 880 | 634 | -246 |
| 30 | 1320 | 634 | -686 |

Against the 5780-red base-game technology denominator:
- N=5 delta: +1.82%;
- N=10: +1.56%;
- N=15: -0.78%;
- N=20: -4.26%.

This is an unusually good shape for the product goal:
- normal early/mid useful batches become slightly more rewarding;
- around 10–15 copies the system stays close to vanilla;
- long-run production diverges sharply downward;
- the lifetime final-craft red pool is finite at **634 red** on this representative core path.

### Unchanged red streams

The table above excludes component red and grave-installation red because they are unchanged by this candidate and therefore cancel in candidate-vs-vanilla deltas.

Installation alone contributes on the representative path:

- pair stages: +2 red per batch unit;
- single-marker stages: +1 red per batch unit;
- total across the six stages: **+10 red per N**.

Thus N=10 normal use receives another 100 red simply for actually installing the decorations.

Component production adds substantially more red and generally scales with real material throughput.

### Red Study compensation

The quantitative result means **red Study compensation is not required by default**.

Unlike blue, where redistribution into Study is central to the product concept, the shared red mastery ladder already preserves stage-scale income around the target natural batch while improving late-tier reward coherence.

Adding red to grave Study remains a valid host-native option because vanilla Study routinely gives 50/100/150 red for advanced practical items. However doing so should be justified by a specific stage deficit, not added automatically.

Current preferred first candidate:
- Blue: Study-heavy + finite paired mastery.
- Red: finite paired mastery with coherent tier starts; no new grave-Study red yet.
- Installation +1 red: unchanged.
- Component/gathering red: unchanged.

### Wooden-tier requirement

Red mastery must also cover wooden grave decorations.

Leaving wood unlimited would preserve a cheap red-only farm even after stone/marble recipes are fixed.

Exact wooden starts are still open because the three early recipes have different complexity and rewards (2/3/5 red). They should form a short lower ladder that flows cleanly into the first stone start of 5 without creating a new red bottleneck.

This is the remaining red-number design task, not a research-data gap.
