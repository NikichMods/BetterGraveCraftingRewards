# Runtime Acceptance Harness 0.1.0

Target production candidate: **Better Grave Crafting Rewards 0.1.0**

Harness: **BGCR Test Console 0.1.0**

Status: **research-only test harness; not production**

## Research-method checkpoint

The remaining runtime acceptance questions after Phase 1 are:

1. does a formerly zero-blue Marble recipe receive the G2 start value through the live `CraftDefinition.output` representation?
2. are representative G2 Study blue rewards present in the live native Survey rows while Study remains one-time?
3. do equivalent manufacturing variants of one grave design share the same projected reward?
4. is the mastery endpoint represented as `0R/0B` while the physical grave-decoration output remains intact?
5. does production routing distinguish direct player craft, ordinary worker craft and Soul Gratitude craft correctly?
6. can a second save be inspected without requiring manual arithmetic or hidden state?

Accepted evidence already proves:
- the real player craft completion path consumes dynamic G2 outputs;
- a three-item multicraft/queue decrements mastery per completed item;
- mastery state persists across save/reload;
- `WorldGameObject.DropItems()` consumes native r/g/b output items and ignores a zero aggregate;
- `CraftComponent.ProcessFinishedCraft()` strips tech-point output from ordinary worker crafts but preserves it in the Soul Gratitude branch.

Manual gameplay is not the cheapest evidence for the remaining states because it could require rare materials, unavailable recipes, long mastery exhaustion, a worker setup or DLC-specific progression.

Selected method: a **separate user-operated runtime harness** that:
- reads the live host data produced by the real production plugin;
- temporarily projects controlled mastery counters only where a boundary value is required;
- invokes the real production routing method for synthetic player/worker/gratitude contexts;
- restores any temporary counter immediately in a `finally` block;
- never spawns items, unlocks technologies, changes recipe inputs or modifies production code.

This is narrower and safer than a general cheat/debug console.

## Test matrix

### Action: Validate G2 state

Mutation class: **nonpersistent read-only**, except for two immediately restored temporary mastery projections.

Checks:
- `grave_bot_mrb_1` at synthetic count 0 projects **8R/8B**, proving the zero-blue Marble representation;
- all manufacturing variants of `grave_bot_stn_1` at synthetic count 2 project **4R/4B** and keep physical output `grave_bot_stn_1 x1`;
- `surv:grave_bot_stn_1` is **40B**, one-time and still requires `faith x3`;
- `surv:grave_bot_mrb_1` is **90B** and one-time;
- `surv:grave_top_sculpt_stn_5` is **160B** and one-time.

Each temporary mastery counter is restored immediately and production reprojects the real current-save state before the action returns.

### Action: Probe mastery endpoint

Mutation class: **session-only temporary projection; immediately restored**.

Checks:
- temporarily sets `grave_bot_stn_1` completed count to 10;
- invokes the real production save projection;
- verifies every manufacturing variant has **0R/0B**;
- verifies every variant still outputs `grave_bot_stn_1 x1`;
- restores the original counter and real projection in `finally`.

The user should not save during the button action. The action is synchronous and restoration happens before control returns.

### Action: Probe worker / Soul routing

Mutation class: **nonpersistent**.

Uses one real live `grave_bot_stn_6` `CraftDefinition` and three synthetic completion contexts passed to the real production `MasteryRuntime.CaptureCompletedDesign()`:

- player context -> must capture `grave_bot_stn_6`;
- ordinary worker context -> must return null;
- Soul Gratitude context -> must capture `grave_bot_stn_6`.

The harness does not call the commit method and does not change mastery state. Phase 1 already proved that a captured design is committed correctly after successful completion.

### Action: Snapshot current save

Mutation class: **read-only**.

Logs:
- current `grave_bot_stn_1` mastery counter;
- all live manufacturing-variant R/B outputs.

This action exists for the later cross-save leakage check: load another save normally, press the same button, and compare the per-save projection.

## Harness UX

- Toggle with **F6**.
- No typed commands.
- Three primary probe buttons plus one read-only current-save snapshot.
- Every action writes one concise `BGCR_TEST_...` diagnostic result and an on-screen PASS/FAIL status.
- Fail closed if the production assembly, required recipe or required production method is missing.

## Save safety

The harness never intentionally leaves synthetic state behind.

The two actions that temporarily change a BGCR player parameter:
- snapshot the original value;
- restore it in `finally`;
- invoke the real production projection after restoration.

If the game process crashes during the tiny temporary window, do **not** save before restarting. Normal restart reloads the last saved state.

## Acceptance

The harness is not merged into production.

A successful returned log should establish:
- live Marble 0B -> G2 representation;
- representative Study mutations;
- shared variant projection;
- zero-reward endpoint preserving physical output;
- player/worker/Soul routing;
- later, per-save projection across a second save.

After acceptance, production diagnostics can be reduced separately; this test DLL remains research-only.
