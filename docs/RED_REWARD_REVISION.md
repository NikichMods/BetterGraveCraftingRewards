# Red manufacturing reward revision

Status: **product choice pending; no production change**.

## Requested outcome

Keep a small repeatable red technology-point reward for manufacturing grave
decorations after design mastery. The user reopened the accepted G2 red-to-zero
decision because fully removing the final-craft red reward feels undesirable.

Baseline: stable 1.0.0; recovered repository HEAD
`82a8de1`. Current source applies `RewardAfterCompleted` to both colors in
`MasteryRuntime.ProjectRule`; both reach zero. The per-design save counter is
capped at twice the maximum starting reward.

## Proposed minimal adjustment

Recommended, **not yet adopted**: floor red manufacturing reward at 1 for all
41 supported designs; keep the existing paired decay above that floor. Blue
still reaches zero. Example for S=5:

- red: `5,5,4,4,3,3,2,2,1,1,1...`;
- blue: `5,5,4,4,3,3,2,2,1,1,0...` where blue mastery is enabled.

This preserves the learning incentive while leaving a small reward for routine
work. It deliberately makes cumulative red manufacturing income unbounded;
the finite-pool statement would remain true only for blue and the red reward
above the floor. Every craft whose old red reward is at least 1 is unchanged.
Each later craft adds exactly 1 red compared with stable 1.0.0.

Alternative requiring a product choice: a residual red ladder by decoration
complexity. This would preserve a tier incentive after mastery but needs an
agreed scale and additional economy comparison before implementation.

## Preserved scope and migration

Keep starting rewards, paired decay, blue/Study/green, materials, energy,
physical outputs, installation rewards, worker/Soul Gratitude routing and
shared workstation identity unchanged. Reuse existing counters without reset:
an exhausted design would immediately pay 1 red on its next eligible craft
under the recommended proposal. Save projection already runs on load.

## Next step and verification

The user requested inspecting vanilla red rewards before selecting a floor.
The proposed uniform 1 is not accepted. Canonical vanilla evidence is in
`VANILLA_BALANCE_RESEARCH.md` (accepted 0.5.0 findings), with per-design baseline
values in `BalanceRules.All` and the table in `UNIFIED_BALANCE_CANDIDATE.md`.
Vanilla final-craft red is repeatable without mastery decay: early wooden
decorations pay 2/3/5, early stone fences 2, most core stone/marble headstones,
crosses and sculptures 5. Later DLC designs reach 7/9/10/15/17. The wooden
marker variant `grave_top_wd_tab_1_3` pays 1 instead of the usual 3.
Component manufacture and installation (+1) are separate red sources.

Await a product decision after the vanilla comparison.
Before production mutation, record the new gate in
`PRODUCTION_EVIDENCE_GATE.md` with the exact adopted semantics. The existing
native output projection/completion path can be reused; no new runtime hook,
counter or save schema is currently justified.

Verification should cover the paired transition, exhaustion and over-cap
counts for all designs; unchanged blue rewards; existing exhausted-save
projection and shared workstation variants. A candidate must pass the existing
build gate and a representative native completion must demonstrate 1 red,
0 blue and the normal physical output after exhaustion. Stable promotion
requires the applicable runtime acceptance.
