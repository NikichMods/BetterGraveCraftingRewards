# BGCR Scope Completion Dump 0.3.0

Final research-only read-only probe for **Better Grave Crafting Rewards**.

Purpose: close the remaining evidence gaps before balance-model trade study.

The probe:
- does not patch game methods;
- does not mutate recipes, technology data, inventory, player state, or save state;
- does not recurse material production;
- logs the native multi-quality carved-marble craft;
- logs technology prices and parent links so cumulative progression cost can be derived;
- logs blueprint/unlock data only for workstations already observed in accepted grave/material evidence;
- logs aggregate technology-tree point cost;
- dumps once and becomes inert.

Runtime evidence required:
1. replace the old BGCR research DLL with `BGCR Scope Completion Dump 0.3.0.dll`;
2. launch Graveyard Keeper and load any save until normal gameplay is active;
3. return `LogOutput.log`.

No crafting, Study action, grave interaction, inventory setup, or special save state is required.
