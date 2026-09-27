# Grave Decoration Tech Point Rebalance

We are researching and developing a Graveyard Keeper mod that rebalances technology-point rewards from crafting grave decorations.

Target: Graveyard Keeper 1.407.
Repository: `NikichMods/BetterGraveCraftingRewards`.
Global contract: `NikichMods/DevRules`.
Shared host research: `NikichMods/GraveyardKeeperResearch`.

## Purpose

The product problem is the progression and incentive structure of technology-point rewards from gravestones, grave fences, crosses, sculptures and related higher-tier grave decorations.

Blue points are the main motivation, but red/green/blue rewards may be investigated together when they belong to the same recipes or affect the same balance.

Goal: make advancement through increasingly sophisticated grave decoration feel meaningful and reduce the incentive to repeatedly craft the cheapest early-game item purely because it is the most efficient technology-point farm.

Do not interpret the goal as simply “increase blue points”. Prefer coherent progression and normal graveyard development over repetitive low-tier grind while preserving vanilla character and pacing.

Out of scope: late-game sinks for surplus technology points, repeatable post-tech-tree spending, or unrelated economy redesign. Treat those as separate projects.

## Startup / recovery

Before substantive technical work:

1. inspect the current repository and relevant history/evidence;
2. read the current DevRules contract, especially `ENGINEERING_RULES.md`, `CI_POLICY.md`, `GIT_WORKFLOW.md`, `PROJECT_BOOTSTRAP.md`, and `RUNTIME_TEST_HARNESS.md` when relevant;
3. read this repository's `AGENTS.md` and relevant canonical docs;
4. search `NikichMods/GraveyardKeeperResearch`, starting from `docs/RESEARCH_INDEX.md`, before fresh host/runtime research;
5. inspect accepted evidence before creating a new probe.

Repository evidence outranks chat memory.

## Research scope

Establish the actual Graveyard Keeper 1.407 progression for grave-decoration crafting, including as relevant:

- technology-point rewards;
- recipe tier and prerequisite technology;
- materials and processing depth;
- energy cost;
- recycling returns and repeatable loops;
- grave quality;
- Study rewards;
- early/mid/late-game availability and pacing.

Community discussions are useful evidence for player incentives, dominant strategies and frustration, but are not authoritative for exact game values or internals.

Keep distinct:
- verified vanilla facts;
- community observations;
- balance interpretation;
- hypotheses;
- proposed values/formulas;
- accepted final behavior.

Promote reusable host/runtime facts into `NikichMods/GraveyardKeeperResearch`. Keep project-specific balance decisions, candidates and acceptance state in this repository.

## Design discipline

Do not make the initial `1 -> 2 -> 3` tier-progression idea the requirement.

The requirement is the player-facing outcome: meaningful progression, less incentive for low-tier farming, coherent reward for more advanced grave decoration, and acceptable total technology-point pacing.

Before selecting a balance model, compare materially different useful approaches. Evaluate incentive structure, total technology-point generation, resource/energy efficiency and recycling loops, interaction with Study rewards, pacing, whether the optimal grind merely moves elsewhere, transparency and vanilla compatibility.

Prefer the least-complex model that solves the established problem. Re-open the choice if evidence shows inflation, a new dominant grind loop, damaged pacing or another material failure.

Do not freeze changing balance numbers or formulas into these Project Instructions.

## Implementation discipline

Before the first production-source mutation for each materially independent behavior change, make the DevRules evidence gate reviewable as `READY` or `BLOCKED`, covering observable property, canonical owner/data path, final consumer/commit point where applicable, blast radius, preserved invariants and acceptance evidence.

`BLOCKED` means research/probe only.

Use the host-native-first rule. If Graveyard Keeper already stores these rewards as native recipe/balance data consumed by the normal crafting path, prefer changing the verified native input rather than replacing the reward algorithm.

Keep probes/diagnostics separate from production behavior.

Evidence gates are per change; builds/candidates are handoff units. Several READY changes may share one coherent candidate when combined acceptance remains attributable. Never bundle BLOCKED or independent unverified mechanisms merely to reduce test cycles.

## Project knowledge

GitHub is the canonical store of mutable state.

Keep verified vanilla/balance data, design rationale, accepted balance model, test/candidate records and release state here. Do not store current SHAs, branches, candidate versions, temporary hypotheses, open bugs or changing balance values in Project Instructions.

New chats should start directly with the actual task and recover current state from GitHub and shared research.

## User-operation boundary

Use GitHub, inspection, CI and research tools directly for mechanical technical work.

Ask the user primarily for product/balance decisions and runtime or perceptual evidence that genuinely requires their installed game.

Before creating a new probe/harness, first check whether accepted evidence, direct inspection, an existing exact artifact or a short deterministic runtime action answers the question with fewer assumptions.

## Iteration report

After a substantial iteration, report briefly what was unknown, what is now verified or excluded, how that changes design direction, what remains open, and what exact runtime evidence, if any, the user must provide.
