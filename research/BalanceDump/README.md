# BGCR Red Economy Dump 0.5.0

Research-only read-only probe for **Better Grave Crafting Rewards**.

Purpose: inventory the repeatable red technology-point economy relevant to grave-decoration balancing in Graveyard Keeper 1.407.

The probe:
- does not patch game methods;
- does not mutate recipes, technology data, inventory, player state, or save state;
- emits positive-red `CraftDefinition.output` rows;
- emits positive-red `WorkDefinition.reward` rows;
- emits object rows whose linked work reward, `drop_items`, or `add_player_param_after_hp_0` contains red points;
- records enough station, recipe, technology and object metadata to classify grave grind against wider repeatable red sources;
- dumps once after gameplay starts and then becomes inert.

Runtime evidence required:
1. replace the previous BGCR research DLL with `BGCR-Red-Economy-Dump-0.5.0.dll`;
2. launch Graveyard Keeper and load any save until normal gameplay is active;
3. return `LogOutput.log`.

No crafting, gathering, Study action, inventory setup, or save mutation is required.
