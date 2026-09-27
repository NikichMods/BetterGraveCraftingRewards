# Production Evidence Gate

Target: **Better Grave Crafting Rewards / Graveyard Keeper 1.407**

Accepted product model: **G2**, documented in `UNIFIED_BALANCE_CANDIDATE.md`.

This record applies the DevRules per-change evidence gate before any production-source mutation.

## Change A — static grave Study blue rebalance

**Status: ACCEPTED**

### Observable property

Existing grave-decoration Study recipes pay the accepted G2 blue reward while preserving their existing one-time Study interaction, Faith/Science costs and non-blue outputs.

### Canonical owner/data path

- `GameBalance.craft_data`
- existing `CraftDefinition` with ID `surv:<grave item>`
- `CraftDefinition.output` blue entries

No new Survey recipe is created.

### Final consumer / commit point

`ItemDefinition.GetSurveyCraft()` resolves the native Survey craft.

Actual completion follows the normal Survey/craft path and ultimately consumes `CraftDefinition.output` through `CraftComponent.ProcessFinishedCraft()`.

The standard item tooltip also derives the unstudied point-color hint from the same Survey output.

### Mutation rule

For each existing in-scope Survey:

`target total blue = max(round current vanilla total blue up to next 10, 10 × (S - 1))`.

Preserve the existing output-list shape where possible:
- if multiple blue entries exist (for example `b:1;b:30`), retain the small existing entries and apply the required delta to the final/largest blue entry;
- preserve story/physical/red/green outputs unchanged.

### Blast radius

Only the existing Study reward of the 23 active studyable grave decorations in G2.

The dead-data Marble sarcophagus is excluded.

### Preserved invariants

- Study remains one-time.
- Faith and Science requirements unchanged.
- Red/green Study rewards unchanged.
- Physical/story output unchanged.
- No new Study recipes.
- Non-grave Study unchanged.
- Exact Study value remains a native output value; qualitative tooltip UX is a separate gate.

### Acceptance evidence

Candidate runtime must verify at least:
- one early Study row;
- one mid-game stone row;
- one marble row;
- one late BSS Study row;
- unchanged Faith/Science needs and one-time completion behavior.

## Change B — finite red/blue manufacturing mastery

**Status: ACCEPTED**

### Observable property

Successful **player manufacturing** of an in-scope grave-decoration design pays the G2 paired mastery sequence:

`S, S, S-1, S-1, ... 1, 1, 0...`

for red and, where enabled by G2, blue.

Equivalent workstation recipes producing the same grave decoration share one mastery count.

### Canonical owners/data path

Reward owner:
- `GameBalance.craft_data`
- native manufacturing `CraftDefinition.output`.

Per-save mastery state:
- native player `WorldGameObject` parameters through `GetParam(string,float)` / `SetParam(string,float)`;
- same persistence mechanism already accepted in PrayerClarity.

Mastery identity:
- produced grave-decoration item/design ID;
- not `CraftDefinition.id`.

### Final consumer / commit point

`CraftComponent.ProcessFinishedCraft()` is the final successful manufacturing output path.

For player craft:
1. it processes the current `CraftDefinition.output`;
2. `WorldGameObject.DropItems()` aggregates native r/g/b output and spawns tech-point drops;
3. near the end it calls `GameSave.OnFinishedCraft(current_craft)`;
4. then `CraftComponent.End()` advances/clears the current craft.

Worker path strips technology-point items before inventory placement.

### Counter commit strategy

Patch `CraftComponent.ProcessFinishedCraft()`.

Prefix:
- capture the design ID only when `current_craft` is an explicitly recognized manufacturing variant for an in-scope design **and** the native completion path awards its tech-point output to the active player:
  - direct player craft (`other_obj.is_player`); or
  - native auto completion, which uses the tech-point-dropping player/auto branch; or
  - Soul Gratitude completion (`wgo.is_current_craft_gratitude`), whose worker/gratitude branch explicitly preserves and drops tech-point items.

Ordinary linked-worker/zombie production without Soul Gratitude is excluded because the native path removes tech-point items before inventory placement.

Postfix:
- increment that design's player parameter;
- project the reward for the **next** successful manufacture into every equivalent manufacturing `CraftDefinition.output` for that design.

This ordering is intentional:
- the just-completed craft consumes the pre-existing current reward;
- only after successful completion is the count advanced;
- a multi-craft amount/queue therefore decays per produced unit.

Removal, installation, repair and dismantling crafts are outside the manufacturing-variant map and never increment mastery.

### Manufacturing-variant identity rule

Start from the explicit accepted 41-design G2 target set.

A craft is an equivalent manufacturing variant only when:
- its output contains that target design as the physical grave-decoration item; and
- its ID is exactly the design ID or starts with `<design id>_`.

This captures verified station variants such as `grave_bot_stn_1_2` while excluding:
- `set_...`;
- `rem_...`;
- `fix_...`;
- `destroy_...`.

Fail closed if a target design has no recognized manufacturing craft.

### Dynamic output representation

Keep a stable native r/b entry in every affected manufacturing output.

If vanilla lacks one color (notably zero-blue Marble recipes), add a native `Item("b", 0)` entry once during initialization.

Projection changes only `Item.value`.

Zero is safe:
- `ResModificator.ProcessItemsListBeforeDrop()` carries the native item forward;
- `WorldGameObject.DropItems()` sums r/g/b values and calls `TechPointsDrop.Drop` only if the aggregate is greater than zero.

Thus mastery exhaustion requires no repeated list removal/insertion.

### Save/load lifecycle

`CraftDefinition` objects are global GameBalance data; mastery is per-save.

Patch `MainGame.OnGameStartedPlaying()` postfix:
- player/save state is already restored;
- read every BGCR design counter from the active player's saved params;
- project the correct next r/b values into all affected global manufacturing definitions.

This prevents reward state leaking between save slots in the same process.

### Old-save policy

If a BGCR mastery parameter is absent, its count is **0**.

Do not infer historical counts from:
- `GameSave.completed_one_time_crafts`, which records only whether a craft was ever completed;
- current graveyard contents;
- inventory;
- technology unlocks.

Those sources cannot establish historical repeat count.

Consequence:
- installing the mod on an old save grants the finite G2 mastery pool from the beginning for each design;
- no player is retroactively penalized by guessed history;
- after the first BGCR-tracked craft, state is exact and save-native.

Uninstalling the mod leaves inert BGCR player params in the save; reinstalling resumes the previous counters rather than resetting them.

### Blast radius

- 41 active G2 grave-decoration designs;
- all verified equivalent manufacturing variants for those designs;
- only r/b values in their native outputs;
- player save gains one namespaced numeric count per design.

### Preserved invariants

- recipe inputs/material costs unchanged;
- energy/time unchanged;
- physical grave-decoration output unchanged;
- green unchanged;
- component crafting rewards unchanged;
- +1 red installation reward unchanged;
- repair/removal/dismantling rewards unchanged;
- ordinary zombie/worker crafts do not consume mastery and continue receiving no r/g/b manufacturing reward;
- Soul Gratitude manufacturing does consume mastery when its native completion path awards the tech points to the player;
- unrelated crafts unchanged;
- one design cannot be re-mastered by switching workstation recipe variants.

### Acceptance evidence

Candidate runtime must prove:
1. one early stone design follows at least `S,S,S-1,S-1`;
2. red and blue advance from the same per-design count;
3. an alternate workstation variant shares the count;
4. save/reload preserves the count and projects the next reward;
5. switching to another save does not inherit the first save's projected reward;
6. installation/removal/repair/dismantling do not advance the count;
7. ordinary worker/zombie production does not advance the count;
8. one Soul Gratitude grave craft advances the shared count if that path awards its native r/b output;
9. after mastery reaches zero, physical output still crafts normally and no r/b tech-point drop is emitted.

## Change C — qualitative Study-value cue in item tooltip

**Status: BLOCKED**

The host presentation seam is known:
`ItemDefinition.GetTooltipData()` already renders the incomplete Survey line using the Survey output's technology-point colors while deliberately hiding numeric quantities.

Still unresolved:
- exact user-facing visual language;
- threshold bands across the whole Study reward distribution;
- localization strategy.

This is an independent UX behavior and must not be bundled into the first mastery candidate merely to reduce test cycles.

## Integration decision

Changes A and B are both **READY** and may share the first production candidate because:
- both operate on the accepted G2 data model;
- their owners/consumers are independently established;
- combined runtime evidence remains attributable;
- Change C remains excluded while BLOCKED.

No further runtime research probe is required before the first production implementation of A+B.


## Runtime acceptance closure

Changes A and B are **runtime accepted** for production candidate 0.1.0.

Accepted evidence covers:
- Study reward mutation;
- finite paired red/blue mastery;
- queue behavior;
- save persistence;
- cross-save isolation;
- zero-blue Marble activation;
- shared workstation variants;
- zero-reward endpoint;
- worker exclusion;
- Soul Gratitude routing.

Change C (qualitative Study-value tooltip cue) remains **BLOCKED** as a separate UX behavior.
