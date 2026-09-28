# Release Readiness Audit

Target: **Better Grave Crafting Rewards / Graveyard Keeper 1.407**

## Accepted gameplay baseline

Changes A+B (G2 Study rebalance + finite manufacturing mastery) are runtime accepted.

The handed 0.1.0 source was `3c8b9c744ee77f51ebfb9b60bbf57b6acc2e24a9`. All commits after that source on `dev/0.1.0` were documentation-only, so the accepted gameplay source did not drift before release hardening.

## Release hardening decision

The accepted release-hardening candidate was **0.1.1** because production bytes changed after the numbered 0.1.0 handoff. Before any public release, the project was declared feature-complete for its intended scope, so the first public stable version is **1.0.0**. The 0.1.1 artifact remains an immutable internal candidate and is not renamed.

The only gameplay-adjacent production change is removal of `BGCR_MASTERY` per-craft Info logging. That logging was acceptance telemetry: it could emit up to 780 shared-log lines across the full G2 mastery pool and mostly repeated already-accepted deterministic reward arithmetic.

Permanent diagnostics retained:
- plugin load line;
- one G2 activation summary per save load;
- baseline/host mismatch warnings;
- fail-closed runtime errors.

No reward calculation, counter mutation, save format, recipe projection, Study mutation, worker routing or Soul Gratitude routing is changed by the logging cleanup.

## Repository / release checklist

- [x] Production project contains only `src/Plugin.cs`, `src/GameApi.cs`, `src/BalanceRules.cs`, and `src/MasteryRuntime.cs`.
- [x] Research-only BGCR Test Console is not present in the production branch/project.
- [x] Historical balance-dump research remains isolated under `research/` and is not compiled into production.
- [x] Acceptance-only `BGCR_MASTERY` logging removed for stable.
- [x] README updated from research-phase wording to current product behavior.
- [x] CHANGELOG added.
- [x] GitHub release notes added.
- [x] Nexus-facing description prepared.
- [x] MPL-2.0 root license and `LICENSING.md` present.
- [x] SPDX headers added to production C# source.
- [x] Version advanced to 1.0.0 for the first public stable release; prior 0.1.0/0.1.1 artifacts remain immutable.
- [x] Candidate workflow derives artifact version from project metadata instead of hardcoding 0.1.0.
- [x] Release workflow promotes an exact accepted CI artifact by run/source/hash instead of rebuilding it.
- [x] Clean 0.1.1 release-hardening CI build recorded.
- [x] Exact 0.1.1 release-hardening artifact identity/hash recorded.
- [ ] Stable source promoted to `main`.
- [ ] GitHub Release `v1.0.0` published from the exact accepted artifact.

## Acceptance requirement for stable 1.0.0

No new Graveyard Keeper runtime regression pass is required. The 0.1.1 release-hardening candidate already proved the only gameplay-adjacent cleanup. The 1.0.0 production diff from accepted 0.1.1 is limited to version metadata and release documentation. The earlier accepted 0.1.1 production diff was limited to:
- removing the acceptance-only log call and now-unused local calculations;
- version metadata;
- source-license comments.

A clean 1.0.0 Release build plus static diff against the technically accepted 0.1.1 production source is sufficient for stable release.


## 0.1.1 candidate identity

- Source SHA: `bad22b61450efdcacaaaf74b4ee305b9904c6b8b`
- CI run: `36360595338`
- Artifact ID: `10945591816`
- Artifact name: `BetterGraveCraftingRewards-0.1.1-bad22b61450efdcacaaaf74b4ee305b9904c6b8b`
- Handoff DLL: `BetterGraveCraftingRewards-0.1.1.dll`
- Installed DLL identity: `BetterGraveCraftingRewards.dll`
- DLL SHA-256: `a8bbce8237e627fe2413eac97548d9f02b2b76280dbd065e65130c96a4e9f950`
- Artifact ZIP digest: `sha256:62d2a3788c080b04951a07f495205d011d61b9700de8c64e98f0dd9b3e12b863`
- Build result: **success, 0 warnings, 0 errors**
- Technical acceptance: **PASS**
- Additional installed-runtime evidence required: **none**


## Stable-version decision

The project is complete for its declared BGCR scope and has no unresolved design, balance, implementation, runtime-acceptance, licensing, or release-readiness blocker.

Therefore:
- **1.0.0** is the first public stable version;
- **0.1.0** remains the immutable runtime-acceptance handoff;
- **0.1.1** remains the immutable release-hardening candidate;
- no public 0.x release is created.


## 1.0.0 accepted stable artifact

- Exact source SHA: `0a971830a62f29f78738a0219610299ebe32428b`
- CI run: `36361332183`
- Artifact ID: `10945069959`
- Artifact name: `BetterGraveCraftingRewards-1.0.0-0a971830a62f29f78738a0219610299ebe32428b`
- Handoff DLL: `BetterGraveCraftingRewards-1.0.0.dll`
- Installed DLL identity: `BetterGraveCraftingRewards.dll`
- DLL SHA-256: `804f3aec98e070c5f41babd5c5343e87fe2bab37b30f5f2ae7571097b988edba`
- Artifact ZIP digest: `sha256:a2ac25413160e8d7380cdfb76cd764f6d2c8295078f777751922a88e30713427`
- Build result: **success, 0 warnings, 0 errors**
- Production diff from technically accepted 0.1.1: version metadata only.
- Stable artifact status: **ACCEPTED**.
- Additional installed-runtime evidence required: **none**.
