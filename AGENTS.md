# Better Grave Crafting Rewards — Working Contract

This repository follows the canonical global development rules in `NikichMods/DevRules`.

Read before substantive technical work:
- `ENGINEERING_RULES.md`
- `CI_POLICY.md`
- `GIT_WORKFLOW.md`
- `PROJECT_BOOTSTRAP.md`
- `LICENSE_POLICY.md`
- `RUNTIME_TEST_HARNESS.md` when installed-runtime evidence is relevant

This file contains only project-specific additions and constraints.

## Project identity

- Project: **Better Grave Crafting Rewards**
- Repository: `NikichMods/BetterGraveCraftingRewards`
- Target: **Graveyard Keeper 1.407**
- Planned installed DLL: `BetterGraveCraftingRewards.dll`
- Purpose: rebalance technology-point rewards from crafting grave decorations so progression through more advanced decorations is meaningful and low-tier repetitive farming is less dominant.

## Scope

Primary scope:
- gravestones, grave fences, crosses, sculptures and related grave decorations;
- red/green/blue technology-point rewards where they belong to the same crafting economy;
- recipe tier, prerequisite technology, materials, processing depth, energy, recycling, grave quality, Study rewards and progression timing where relevant to balance.

Out of scope:
- late-game sinks for surplus technology points;
- repeatable post-tech-tree spending;
- unrelated economy redesign.

Do not interpret the product goal as simply increasing blue-point income. Preserve vanilla character and pacing while improving progression incentives.

## Mandatory project-specific start-of-work checks

Before substantive work:
1. inspect current repository state and relevant history/evidence;
2. read this `AGENTS.md` and relevant project docs;
3. check `NikichMods/GraveyardKeeperResearch/docs/RESEARCH_INDEX.md` and linked canonical research before fresh host/runtime investigation;
4. distinguish verified vanilla facts, community observations, balance interpretation, hypotheses, proposals and accepted behavior;
5. do not rely on wiki/community values as authoritative when Graveyard Keeper 1.407 data can be established directly.

Repository evidence outranks chat memory.

## Shared host/runtime research

Canonical shared source: `NikichMods/GraveyardKeeperResearch`.

Reusable Graveyard Keeper 1.407 host/runtime facts belong there once accepted. Project-specific balance analysis, candidate values, design rationale and release state belong here.

Before a new probe:
`project docs -> shared research index -> accepted history/evidence -> direct inspection -> narrow probe only if still needed`.

## Design requirements

The requirement is the player-facing outcome, not a predetermined `1 -> 2 -> 3` reward formula.

Before choosing final balance values, compare materially different useful models and evaluate:
- normal-play progression and incentives;
- total technology-point generation;
- resource/energy efficiency and recycling loops;
- interaction with one-time Study rewards;
- early/mid/late-game pacing;
- whether the optimal grind merely moves to another recipe;
- transparency and vanilla compatibility.

Prefer the least-complex model that solves the established problem. Re-open the design if evidence shows inflation, a new dominant farm, pacing damage or another material failure.

## Implementation constraints

Use the DevRules host-native-first gate.

If rewards are verified host-owned recipe/balance data consumed by the normal crafting path, prefer changing those native inputs rather than replacing reward algorithms.

Before the first production-source mutation for each materially independent behavior change, make the evidence gate reviewable as **READY** or **BLOCKED**, covering:
- observable property;
- canonical owner/data path;
- final consumer/commit point where applicable;
- blast radius;
- preserved invariants;
- acceptance evidence.

**BLOCKED means research/probe only.**

Keep probes and diagnostics separate from production behavior.

## Runtime-test ergonomics

The user's installed game is a normal play save. Do not assume a developer console, cheats, arbitrary item spawning, unlocked recipes, abundant rare materials or convenient resettable progression.

Default acceptance design:
- use a short natural gameplay action when the required state is already easy to reach on the user's save;
- when setup would require rare materials, many repeated crafts, unavailable progression, artificial counters or destructive save manipulation, build a separate test-only harness first;
- prefer a small user-operated panel/console with named scenario actions over typed commands or manual save editing;
- the harness may prepare inputs or synthetic edge state, but the production/native path under test must produce the result;
- classify any harness mutation as nonpersistent/session-persistent/save-persistent and provide obvious cleanup for persistent state;
- never bundle the harness into the production DLL.

PrayerClarity's Rebalanced Test Console is the accepted precedent for this workflow.

## Git / version / acceptance

- `main` is the stable line.
- Use `research/<topic>` for substantial unresolved research/probes.
- Use `dev/<version>` or a semantically clear feature/fix branch for production development.
- Numbered handed artifacts are immutable.
- No production version is consumed for research-only work.
- Runtime behavior reaches stable only after the applicable acceptance gate is satisfied.
- Stable installed filename is expected to be `BetterGraveCraftingRewards.dll`.

## Licensing

Default original software-source license is MPL-2.0 under `NikichMods/DevRules/LICENSE_POLICY.md`. Game-owned data/assets/binaries/decompiled payloads are research inputs only and must not be relicensed or committed in bulk.

## Long-lived sources of truth

- `AGENTS.md`
- `docs/VANILLA_BALANCE_RESEARCH.md`
- `docs/CHATGPT_PROJECT_INSTRUCTIONS.md`
- future design/test/release docs when they become necessary
- `README.md`

When a reusable host fact is accepted, promote it into `NikichMods/GraveyardKeeperResearch`.
