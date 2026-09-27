# Study Tooltip UX Trade Study

Target: **Graveyard Keeper 1.407**

Status: **product-design candidate; no production source mutation**

Accepted mechanical baseline:
- G2 manufacturing mastery and Study blue values are runtime accepted in production candidate 0.1.0.
- The native item tooltip already exposes whether an item is unstudied and which technology-point colors Study can yield.
- The native tooltip deliberately omits exact Study quantities.

## Product goal

Before committing Faith/Science at the Study Table, the player should be able to tell whether studying a grave decoration is a relatively small, medium, large, or exceptional source of blue knowledge.

The cue must:
- preserve uncertainty about the exact blue-point amount;
- remain visually close to vanilla;
- avoid implying a new currency or item-quality system;
- disappear once Study is completed;
- affect only G2 grave decorations with an existing Study recipe;
- remain truthful if another modifier changes the effective Survey output at runtime.

## Existing native presentation

For an unstudied item, `ItemDefinition.GetTooltipData(Item, full_detail)`:
1. resolves the native Survey craft via `GetSurveyCraft()`;
2. checks `completed_one_time_crafts`;
3. processes the Survey output through `ResModificator.ProcessItemsListBeforeDrop(...)` during gameplay;
4. collects technology-point **types**, not quantities;
5. renders the standard row equivalent to:

`Study: Not studied (blue-point icon)`

After Study is completed, the same owner renders:

`Study: Complete`

This is the correct host-owned surface to extend.

## Alternatives

### A. Exact numeric reward

Example:
`Study: Not studied (blue icon) — 90`

Rejected.

It solves discoverability but removes the intended uncertainty and turns Study into a directly solved efficiency calculation before the player reaches the Study Table.

### B. Repeated blue-point icons / pips

Example:
`Study: Not studied (blue icon)(blue icon)(blue icon)`

Rejected.

Advantages:
- language-independent;
- compact;
- no localization files.

Problems:
- repeated tech-point icons look like an exact quantity rather than a qualitative band;
- the same icon already denotes the **type** of technology point in vanilla;
- the encoding is not self-explanatory without documentation;
- 4–5 repeated icons add visual noise to a small tooltip.

### C. Stars, rarity words or quality glyphs

Rejected.

Grave decorations already have grave quality, and Graveyard Keeper uses quality/star semantics elsewhere. Reusing that visual language for Study yield would create a false relationship between grave quality and Study value.

### D. Color/intensity-only cue

Rejected.

Examples would be tinting the blue icon or changing text saturation.

Problems:
- low discoverability;
- accessibility/color-vision issues;
- harder to distinguish on different UI backgrounds;
- materially more presentation complexity for less semantic clarity.

### E. Qualitative text band on the native Study row

**Preferred.**

Keep the native Study row and extend the same native text widget with one short second line:

English:
`Study: Not studied (blue icon)`
`Reward: high`

Russian:
`Исследование: не изучено (blue icon)`
`Награда: высокая`

No exact amount is shown.

The second line is preferred over forcing the phrase onto the first line because it avoids letting one long localization determine the width of the whole tooltip. It adds only one compact text line and no new separator/widget concept.

Advantages:
- immediately understandable without a manual;
- preserves the native Study status and point-color icon;
- no new widget, panel, icon or layout concept;
- accessible independently of color;
- easy to localize;
- trivial to remove after Study because the native completion state already owns that branch.

## Recommended band model

Use **four** bands.

The accepted G2 grave Study values naturally cluster across a wide 20–190 blue range, so three bands would make the upper half too coarse while five bands would add precision that starts to undermine the intended uncertainty.

Bands are based on the **effective total blue Survey output after native runtime modifiers**, not on a duplicated G2 table:

| Effective Study blue | Band | RU |
|---:|---|---|
| 1–40 | Small | небольшая |
| 41–80 | Medium | средняя |
| 81–120 | High | высокая |
| 121+ | Very high | очень высокая |

For the currently accepted G2 values this groups the 23 active Study recipes as:
- Low: 20 / 30 / 40;
- Medium: 50 / 60 / 80;
- High: 90 / 100 / 110;
- Very high: 130 / 160 / 190.

The thresholds intentionally sit in the natural gaps of the G2 value set rather than pretending that the four labels are equal mathematical quartiles.

## Runtime semantics

The qualitative label belongs to the **effective Study reward**, not permanently to an item ID.

At tooltip construction time:
1. use the native `GetSurveyCraft()`;
2. use the same processed Survey output path that vanilla already uses in `GetTooltipData()`;
3. sum effective `b` output;
4. map that value to the four bands;
5. append the localized band phrase only if:
   - the item belongs to the 23 active G2 Study targets;
   - the Survey is not completed in the active save;
   - effective blue output is positive.

Consequences:
- if another compatible modifier changes Study blue, the cue follows the real effective value;
- if a conflicting mod causes BGCR to skip a Study mutation, the cue does not falsely advertise the G2 target number;
- no duplicate balance table is required for tooltip thresholds.

## Completed-state behavior

After Study is complete, preserve the vanilla completed line exactly.

Do **not** show:
- the former band;
- the exact points received;
- historical Study yield.

The cue exists to support a pending decision. Once no decision remains, it is noise.

## Scope

In scope:
- the 23 active G2 grave decorations with existing Study recipes.

Out of scope:
- all other Survey-able game items;
- creating Study recipes for decorations that do not have one;
- exact numeric Study previews;
- Study Table redesign;
- manufacturing mastery display;
- red/green Study-value labels.

This narrow scope avoids turning a grave-rebalance mod into a global Study-information overhaul.

## Localization

Use project-owned localization keys with English fallback.

Target the same 11 language codes already proven in PrayerClarity:
- en
- fr
- de
- es
- pt_br
- ko
- ja
- ru
- it
- pl
- zh_cn

Recommended semantic keys:
- `study.reward.low`
- `study.reward.medium`
- `study.reward.high`
- `study.reward.very_high`

The code should append a localized newline plus `Reward: <band>` rather than reconstructing the native `Study: Not studied` text.

This keeps native wording owned by the game and minimizes localization blast radius.

## Presentation seam

Preferred implementation family:
- Harmony postfix on `ItemDefinition.GetTooltipData(Item,bool)`;
- return immediately unless `full_detail`, item and active save state are valid and the item is one of the 23 G2 Study targets;
- leave the native list structure intact;
- find the already-created native Survey text row and append the short localized phrase to its `BubbleWidgetTextData.text`.

Important:
- do not replace `WidgetsBubbleGUI`;
- do not create a parallel tooltip;
- do not duplicate the native completed/not-completed decision;
- do not change alignment, style, separators or width policy unless runtime evidence proves the appended text needs a separate layout fix.

## Acceptance

A small visual/runtime candidate should demonstrate three representative unstudied items:
- Low (for example 40B);
- High (for example 90–110B);
- Very high (for example 160B).

Acceptance questions:
1. Is the phrase immediately understandable without knowing the mod formula?
2. Does it still feel like native Graveyard Keeper UI?
3. Does the longest supported-language phrase fit without damaging tooltip layout?
4. Does a studied item revert to the unchanged vanilla completed row?
5. Does the band correspond to the effective live Survey blue value?

This property is inherently visual/perceptual, so screenshots/user observation are stronger evidence than a purely synthetic harness for final acceptance.

## Current decision state

Recommended product direction:
- **four localized text bands added as a second line inside the existing native Study text widget**;
- no exact numbers;
- no pips/stars/color-only encoding;
- band derived from effective live Survey blue;
- only while Study is incomplete.

Production gate remains **BLOCKED** until the user accepts this UX contract and the longest-language layout/row-identification seam is verified narrowly.
