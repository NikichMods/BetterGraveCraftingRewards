# Vanilla Grave Decoration Balance Research

Target: **Graveyard Keeper 1.407**

Status: **active research**

This document is the canonical project-local record for vanilla grave-decoration balance facts and the analysis used to select the Better Grave Crafting Rewards model.

## Product question

Does vanilla Graveyard Keeper reward increasingly advanced grave-decoration crafting coherently enough that normal graveyard progression competes with repetitive low-tier technology-point farming?

The goal is not to maximize blue points. The goal is to understand and, if necessary, rebalance the incentive curve while preserving overall pacing and vanilla character.

## Evidence classes

Keep entries explicitly separated as:

- **verified vanilla fact** — established from Graveyard Keeper 1.407 data/runtime/source evidence;
- **community observation** — player behavior/opinion, useful for incentives but not exact internals;
- **balance interpretation** — conclusion drawn from verified data;
- **hypothesis** — plausible but not yet proved;
- **proposal** — candidate rebalance value/model;
- **accepted behavior** — final user-accepted mod behavior.

Wiki/community values are discovery aids, not authoritative 1.407 values.

## Required vanilla dataset

For each relevant grave decoration, establish where applicable:

| Field | Required |
|---|---|
| Internal recipe/item ID | yes |
| Display/name mapping | yes |
| Decoration family/type | yes |
| Grave quality | yes |
| Craft station/path | yes |
| Input materials and quantities | yes |
| Material processing depth | derived |
| Energy/craft cost | yes |
| Red tech reward | yes |
| Green tech reward | yes |
| Blue tech reward | yes |
| Recycling recipe/returns | yes |
| Net repeat-loop resource loss | derived |
| Study reward | yes |
| Prerequisite technology | yes |
| Technology cost | yes where material |
| Earliest practical progression stage | derived/evidenced |

## Derived comparisons

Once the base table is verified, calculate at least:

- tech points per craft;
- blue points per craft;
- blue points per unit of non-returned material in repeatable recycle loops;
- blue points relative to energy/craft effort where comparable;
- reward progression versus grave quality;
- reward progression versus material/processing depth;
- effect of prerequisite technology cost;
- total blue generation from representative normal graveyard upgrades;
- optimal repeatable farming recipe at representative progression stages.

The key anti-regression question is:

> After a rebalance, did the optimal grind disappear as a dominant strategy, or did it merely move to another recipe?

## Current established project-level observations

**Community observation / discovery, not yet canonical vanilla-data proof:**
- players commonly use early stone grave decoration crafting as a repeatable source of blue technology points;
- recycling can materially improve the efficiency of that loop;
- community discussion has questioned why more advanced grave decorations do not consistently yield proportionally stronger repeatable rewards.

These observations justify investigation but do not establish exact 1.407 values.

## Research-method checkpoint

### Question

Establish the exact Graveyard Keeper 1.407 data and ownership path for grave-decoration crafting rewards and the adjacent fields needed to evaluate progression.

### Existing evidence

Shared research already verifies that `GameBalance` owns craft collections and that `CraftDefinition` is the common native recipe-data model for many crafting paths. It does **not** yet contain the grave-decoration reward table or prove every adjacent field needed here.

### Preferred method order

1. search existing NikichMods source/history for an accepted data-dump/static-inspection path that already exposes the relevant `GameBalance` data;
2. use direct static inspection if exact 1.407 game data is available to the working environment;
3. only if the dataset cannot be established cleanly that way, create one narrow research-only dumper/probe that reads the native loaded balance data without changing gameplay.

Do not build production behavior until the canonical owner/data path and relevant values are verified.

## Production evidence gate

Current state: **BLOCKED**

Reason: exact 1.407 reward values, complete affected recipe set, canonical reward fields and downstream consumer path are not yet fully established from authoritative evidence.

Allowed work while BLOCKED:
- repository/bootstrap documentation;
- static inspection;
- evidence collection;
- narrow research tooling/probe if justified;
- balance analysis using clearly labelled provisional inputs.

Not allowed:
- production reward mutation;
- final balance values presented as accepted behavior.
