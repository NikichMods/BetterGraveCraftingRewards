# BGCR Material Dump 0.2.0

Research-only read-only probe for **Better Grave Crafting Rewards**.

Purpose: trace the loaded Graveyard Keeper 1.407 production chain behind the direct materials used by grave decorations.

The probe:
- does not patch game methods;
- does not mutate recipes, rewards, inventory, player state, or save state;
- waits until normal gameplay has started;
- finds each grave decoration's primary craft and collects its direct inputs;
- walks backward through native producer recipes for those inputs until terminal/root resources are reached;
- logs producer inputs, outputs, red/green/blue rewards, energy/time, workstation and prerequisite technologies;
- emits localized material names where available;
- dumps once and then becomes inert.

This specifically closes the remaining balance questions around material processing depth, component-crafting tech-point generation and repeatable-loop efficiency.

Runtime evidence required:
1. remove/replace the old `BGCR Balance Dump 0.1.0.dll`;
2. install `BGCR Material Dump 0.2.0.dll` in the normal BepInEx plugins folder;
3. launch Graveyard Keeper and load any save until normal gameplay is active;
4. return the generated `LogOutput.log`.

No crafting, Study action, grave interaction, inventory setup, or special save state is required.
