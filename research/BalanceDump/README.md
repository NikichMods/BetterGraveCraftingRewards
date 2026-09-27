# BGCR Balance Dump 0.1.0

Research-only read-only probe for **Better Grave Crafting Rewards**.

Purpose: dump the loaded Graveyard Keeper 1.407 native balance data needed to build the grave-decoration technology-point progression table.

The probe:
- does not patch game methods;
- does not change recipes, rewards, inventory, player state, or save state;
- waits until normal gameplay has started;
- logs the dataset once using `BGCR_*` lines and then becomes inert.

Runtime evidence required:
1. install `BGCR Balance Dump 0.1.0.dll` into the normal BepInEx plugins folder;
2. launch Graveyard Keeper and load any save until normal gameplay is active;
3. exit or return the generated `LogOutput.log`.

No crafting, Study action, grave interaction, or special save state is required.
