# Residual red manufacturing rewards — 1.1.0

Status: **product model adopted; candidate implementation; runtime acceptance pending**.

## Adopted behavior

Keep G2 starting rewards and paired decay, but stop red at a per-design floor.
Blue still reaches zero. The user selected this scale after reviewing vanilla:

| Designs | Count | Red floor |
| --- | ---: | ---: |
| Core wooden marker, fence and cross | 3 | 1 |
| Core stone plates, fences, crosses and stella | 7 | 2 |
| Core stone sculptures | 2 | 3 |
| All core marble decorations, including stella and sculptures | 7 | 4 |
| All supported Game of Crone and Better Save Soul decorations | 22 | 5 |

The DLC rule takes precedence over material: a DLC stone fence gets 5, not 2.
The unused marble sarcophagus remains excluded. All 41 designs and their
validated equivalent workstation variants remain supported.

For completed count c, starting reward S and floor F:

- red: `max(S - floor(max(c, 0) / 2), F)`;
- blue: unchanged `max(S - floor(max(c, 0) / 2), 0)` where enabled;
- early designs without manufacturing blue still pay zero blue.

Example, early stone design S=5/F=2:

- red: `5,5,4,4,3,3,2,2,2...`;
- blue: `5,5,4,4,3,3,2,2,1,1,0...`.

The floor replaces the lower end of decay; it is not added to the start.
Starting rewards and every reward already at or above the floor are unchanged.
Red becomes a repeatable practical-work reward. Blue manufacturing mastery
remains finite. Red's cumulative total is deliberately no longer finite.

## Compatibility and preserved behavior

No counter reset or save migration. Existing exhausted designs immediately
receive the floor after normal save-load projection. Counter keys, caps,
successful-completion ordering and shared variant identity are unchanged.
No new hooks, save fields, configuration or runtime diagnostics are needed.

Blue starts/decay, Study, green, materials, energy/time, physical output,
installation (+1 red), removal/repair, ordinary workers and Soul Gratitude
routing remain as in accepted 1.0.0. Runtime failure still restores vanilla
manufacturing outputs through the existing restoration path.

## Evidence and acceptance

Baseline: stable 1.0.0, recovered repository HEAD `82a8de1`.
Vanilla facts: `VANILLA_BALANCE_RESEARCH.md`, accepted 0.5.0 red findings.
The earlier all-design floor of 1 was proposed and not adopted; the table above
is the user's selected replacement. The accepted 1.0.0 G2 tables remain the
source of unchanged starts and Study rewards in `UNIFIED_BALANCE_CANDIDATE.md`.

Change E in `PRODUCTION_EVIDENCE_GATE.md` establishes the implementation gate.
`tests/BalanceRules.Tests.csproj` executes actual production rule code for all
41 designs against product groups, paired boundaries, exhausted/large counts,
zero-blue early designs and the unchanged counter schema.

Before stable promotion, a representative native exhausted craft must pay its
red floor, zero blue and normal physical output. Check reload and equivalent
workstation projection. Prior 1.0.0 acceptance remains evidence for unchanged
completion and worker-routing mechanisms. Compilation/arithmetic checks alone
do not establish installed-game acceptance.
