# BGCR Test Console 0.1.0

Research-only runtime acceptance harness for **Better Grave Crafting Rewards 0.1.0**.

Requires the exact production candidate to be installed:
- `BetterGraveCraftingRewards-0.1.0.dll`
- production SHA-256: `8b5a7f48fcef478bb8638e0a91c223d13f28d57fa1b2ea180d3cae2cd7ebced3`

## Use

1. Put `BGCR-Test-Console-0.1.0.dll` next to the production mod in `BepInEx/plugins`.
2. Load the same normal gameplay save.
3. Press **F6**.
4. Press, in order:
   - **Validate G2 state**
   - **Probe mastery endpoint**
   - **Probe worker / Soul routing**
5. Close with F6.
6. Return `LogOutput.log`.

No crafting, item spawning, technology unlocking, rare materials or save editing is required.

The endpoint/state probes temporarily touch a BGCR mastery player parameter and restore it synchronously before the button action returns. Do not deliberately save during the instant a probe is running.

The optional **Snapshot current save** button is reserved for a later cross-save check.

Do not ship this DLL with the mod.
