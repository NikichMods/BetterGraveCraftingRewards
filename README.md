# Better Grave Crafting Rewards

A Graveyard Keeper 1.407 mod that makes grave-decoration progression more rewarding and reduces the incentive to farm the same cheap early recipe for technology points.

## What it changes

Each grave-decoration design has its own mastery counter.

The 1.1.0 development candidate retains a small repeatable red reward after
mastery. Stable 1.0.0 still has the original red-to-zero behavior; the new
candidate awaits runtime acceptance.

For blue with starting reward `S`, successful player manufacturing follows:

`S, S, S-1, S-1, ... 1, 1, 0...`

New and more advanced decorations teach the Keeper more, while repeating the
same design eventually stops producing blue technology points.

- **Red points (1.1.0 candidate):** paired decay for all 41 designs stops at 1 for wood, 2 for ordinary core stone decorations, 3 for core stone sculptures, 4 for core marble and 5 for all supported DLC designs.
- **Blue points:** manufacturing mastery starts with Stone gravestones and later designs.
- **Grave plate and Simple gravestones:** manufacturing blue remains 0; their knowledge route stays Study.
- **Study:** 23 existing grave-decoration Study rewards are rebalanced upward where needed. Study remains one-time, and Faith/Science costs are unchanged.
- Equivalent workstation recipes for the same decoration share one mastery counter.
- Normal zombie/worker production does not consume player mastery. Soul Gratitude production does when the native game path actually awards those technology points to the player.

Recipe materials, craft energy/time, grave quality, physical outputs, green-point rewards, installation rewards and unrelated crafts are not changed.

The unused Marble sarcophagus data is intentionally excluded.

## Save behavior

Mastery is stored in native per-save player parameters. The mod creates no separate save file.

On an existing save, a design with no BGCR counter starts at 0 completed crafts,
so mastery begins from the starting reward. Existing counters are preserved;
in the 1.1.0 candidate even exhausted designs receive the residual red reward.

Removing the mod leaves those namespaced parameters inert. Reinstalling the mod later resumes the previous mastery counts.

## Installation

1. Install BepInEx 5 for Graveyard Keeper.
2. Download `BetterGraveCraftingRewards.dll` from the [latest GitHub Release](https://github.com/NikichMods/BetterGraveCraftingRewards/releases/latest).
3. Copy it into `Graveyard Keeper/BepInEx/plugins`.
4. Start the game.

Do not install the research-only BGCR Test Console for normal play.

## Compatibility

Tested on **Graveyard Keeper 1.407 (Steam, Windows)** with all DLC installed.

Other game versions, storefront builds, operating systems and DLC configurations are currently untested.

## Research and design evidence

- Accepted balance model: [docs/UNIFIED_BALANCE_CANDIDATE.md](docs/UNIFIED_BALANCE_CANDIDATE.md)
- Adopted red-floor revision (candidate): [docs/RED_REWARD_REVISION.md](docs/RED_REWARD_REVISION.md)
- Production evidence gates: [docs/PRODUCTION_EVIDENCE_GATE.md](docs/PRODUCTION_EVIDENCE_GATE.md)
- Runtime acceptance history: [docs/TEST_LOG.md](docs/TEST_LOG.md)
- Shared Graveyard Keeper research: `NikichMods/GraveyardKeeperResearch`

## License

Original project source is licensed under the Mozilla Public License 2.0. See [LICENSE](LICENSE) and [LICENSING.md](LICENSING.md).
