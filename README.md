# Better Grave Crafting Rewards

A Graveyard Keeper 1.407 mod that makes grave-decoration progression more rewarding and reduces the incentive to farm the same cheap early recipe for technology points.

## What it changes

Each grave-decoration design has its own finite mastery pool.

For a design with starting reward `S`, successful player manufacturing follows:

`S, S, S-1, S-1, ... 1, 1, 0...`

That means new and more advanced decorations teach the Keeper more, while repeating the same design eventually stops being a technology-point farm.

- **Red points:** mastery applies to all 41 active supported grave-decoration designs.
- **Blue points:** manufacturing mastery starts with Stone gravestones and later designs.
- **Grave plate and Simple gravestones:** manufacturing blue remains 0; their knowledge route stays Study.
- **Study:** 23 existing grave-decoration Study rewards are rebalanced upward where needed. Study remains one-time, and Faith/Science costs are unchanged.
- Equivalent workstation recipes for the same decoration share one mastery counter.
- Normal zombie/worker production does not consume player mastery. Soul Gratitude production does when the native game path actually awards those technology points to the player.

Recipe materials, craft energy/time, grave quality, physical outputs, green-point rewards, installation rewards and unrelated crafts are not changed.

The unused Marble sarcophagus data is intentionally excluded.

## Save behavior

Mastery is stored in native per-save player parameters. The mod creates no separate save file.

On an existing save, a design with no BGCR counter starts at 0 completed crafts, so the save receives that design's full finite mastery pool from installation onward.

Removing the mod leaves those namespaced parameters inert. Reinstalling the mod later resumes the previous mastery counts.

## Installation

1. Install BepInEx 5 for Graveyard Keeper.
2. Copy `BetterGraveCraftingRewards.dll` into `Graveyard Keeper/BepInEx/plugins`.
3. Start the game.

Do not install the research-only BGCR Test Console for normal play.

## Compatibility

Tested on **Graveyard Keeper 1.407 (Steam, Windows)** with all DLC installed.

Other game versions, storefront builds, operating systems and DLC configurations are currently untested.

## Research and design evidence

- Accepted balance model: [docs/UNIFIED_BALANCE_CANDIDATE.md](docs/UNIFIED_BALANCE_CANDIDATE.md)
- Production evidence gates: [docs/PRODUCTION_EVIDENCE_GATE.md](docs/PRODUCTION_EVIDENCE_GATE.md)
- Runtime acceptance history: [docs/TEST_LOG.md](docs/TEST_LOG.md)
- Shared Graveyard Keeper research: `NikichMods/GraveyardKeeperResearch`

## License

Original project source is licensed under the Mozilla Public License 2.0. See [LICENSE](LICENSE) and [LICENSING.md](LICENSING.md).
