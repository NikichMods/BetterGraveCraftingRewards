# BGCR Blue Economy Dump 0.4.0

Research-only read-only probe for **Better Grave Crafting Rewards**.

Purpose: enumerate Graveyard Keeper 1.407 craft-data rows that produce positive blue technology points, so the project can verify non-grave recovery paths before making grave-craft blue finite.

The probe:
- does not patch game methods;
- does not mutate recipes, technology data, inventory, player state, or save state;
- does not recurse material graphs;
- emits only `CraftDefinition` rows whose native `output` contains positive blue points;
- records Survey/one-time/repeatable classification, station, needs/output, visibility/automation flags, and owning technology/DLC metadata;
- dumps once after gameplay starts and then becomes inert.

Runtime evidence required:
1. replace the previous BGCR research DLL with `BGCR-Blue-Economy-Dump-0.4.0.dll`;
2. launch Graveyard Keeper and load any save until normal gameplay is active;
3. return `LogOutput.log`.

No crafting, Study action, inventory setup, or save mutation is required.
