using System;
using System.Collections;
using System.Collections.Generic;

namespace BetterGraveCraftingRewards
{
    internal sealed class VariantState
    {
        internal object Craft;
        internal string CraftId;
        internal BalanceRule Rule;
        internal int OriginalRed;
        internal int OriginalBlue;
    }

    internal static class MasteryRuntime
    {
        private static readonly Dictionary<string, BalanceRule> RuleByDesign =
            new Dictionary<string, BalanceRule>(StringComparer.Ordinal);

        private static readonly Dictionary<string, BalanceRule> RuleByCraft =
            new Dictionary<string, BalanceRule>(StringComparer.Ordinal);

        private static readonly Dictionary<string, List<VariantState>> VariantsByDesign =
            new Dictionary<string, List<VariantState>>(StringComparer.Ordinal);

        private static readonly List<VariantState> AllVariants =
            new List<VariantState>();

        private static bool _initialized;
        private static bool _runtimeFailed;
        private static int _studyMutations;

        internal static void OnGameStartedPlaying()
        {
            if (_runtimeFailed)
                return;

            if (!_initialized)
                Initialize();

            ProjectAllFromCurrentSave();

            Plugin.Log.LogInfo(
                "G2 active | designs=" + RuleByDesign.Count
                + " | craftVariants=" + AllVariants.Count
                + " | studies=" + _studyMutations
                + " | savePolicy=missing-counter-means-zero");
        }

        private static void Initialize()
        {
            IList crafts = GameApi.CraftData;
            if (crafts == null)
                throw new InvalidOperationException(
                    "GameBalance.craft_data is unavailable.");

            RuleByDesign.Clear();
            RuleByCraft.Clear();
            VariantsByDesign.Clear();
            AllVariants.Clear();
            _studyMutations = 0;

            foreach (BalanceRule rule in BalanceRules.All)
                TryActivateRule(rule, crafts);

            ApplyStudyRewards(crafts);

            if (RuleByDesign.Count == 0)
                throw new InvalidOperationException(
                    "No G2 grave designs passed baseline validation.");

            _initialized = true;
        }

        private static void TryActivateRule(BalanceRule rule, IList crafts)
        {
            List<object> matches = new List<object>();

            foreach (object craft in crafts)
            {
                if (craft == null)
                    continue;

                string craftId = GameApi.Id(craft);
                if (!IsManufacturingId(rule.DesignId, craftId))
                    continue;

                IList output = GameApi.Get(craft, "output") as IList;
                if (!GameApi.OutputContains(output, rule.DesignId))
                    continue;

                matches.Add(craft);
            }

            if (matches.Count == 0)
            {
                Plugin.Log.LogWarning(
                    "G2 skipped design with no manufacturing recipe: "
                    + rule.DesignId);
                return;
            }

            foreach (object craft in matches)
            {
                string craftId = GameApi.Id(craft);
                IList output = GameApi.Get(craft, "output") as IList;

                int actualRed = GameApi.PointTotal(output, "r");
                int actualBlue = GameApi.PointTotal(output, "b");
                int expectedRed = rule.ExpectedVanillaRed(craftId);

                if (actualRed != expectedRed
                    || actualBlue != rule.VanillaBlue)
                {
                    Plugin.Log.LogWarning(
                        "G2 skipped design because another mod or host mismatch "
                        + "changed its manufacturing reward | design="
                        + rule.DesignId
                        + " | craft=" + craftId
                        + " | expected=" + expectedRed + "R/"
                        + rule.VanillaBlue + "B"
                        + " | actual=" + actualRed + "R/"
                        + actualBlue + "B");
                    return;
                }
            }

            List<VariantState> states = new List<VariantState>();

            foreach (object craft in matches)
            {
                string craftId = GameApi.Id(craft);
                IList output = GameApi.Get(craft, "output") as IList;

                VariantState state = new VariantState
                {
                    Craft = craft,
                    CraftId = craftId,
                    Rule = rule,
                    OriginalRed = GameApi.PointTotal(output, "r"),
                    OriginalBlue = GameApi.PointTotal(output, "b")
                };

                GameApi.EnsurePointEntry(output, "r");
                GameApi.EnsurePointEntry(output, "b");

                states.Add(state);
                AllVariants.Add(state);
                RuleByCraft[craftId] = rule;
            }

            RuleByDesign[rule.DesignId] = rule;
            VariantsByDesign[rule.DesignId] = states;
        }

        private static void ApplyStudyRewards(IList crafts)
        {
            foreach (BalanceRule rule in BalanceRules.All)
            {
                if (rule.TargetStudyBlue <= 0)
                    continue;

                if (!RuleByDesign.ContainsKey(rule.DesignId))
                    continue;

                string studyId = "surv:" + rule.DesignId;
                object study = null;

                foreach (object craft in crafts)
                {
                    if (craft != null
                        && string.Equals(
                            GameApi.Id(craft),
                            studyId,
                            StringComparison.Ordinal))
                    {
                        study = craft;
                        break;
                    }
                }

                if (study == null)
                {
                    Plugin.Log.LogWarning(
                        "G2 Study mutation skipped; Survey craft missing: "
                        + studyId);
                    continue;
                }

                IList output = GameApi.Get(study, "output") as IList;
                int actual = GameApi.PointTotal(output, "b");

                if (actual != rule.VanillaStudyBlue)
                {
                    Plugin.Log.LogWarning(
                        "G2 Study mutation skipped because another mod or host "
                        + "mismatch changed blue output | craft=" + studyId
                        + " | expected=" + rule.VanillaStudyBlue
                        + " | actual=" + actual);
                    continue;
                }

                GameApi.IncreasePointTotalPreservingShape(
                    output,
                    "b",
                    rule.VanillaStudyBlue,
                    rule.TargetStudyBlue);

                _studyMutations++;
            }
        }

        private static bool IsManufacturingId(
            string designId,
            string craftId)
        {
            if (string.Equals(designId, craftId, StringComparison.Ordinal))
                return true;

            return craftId.StartsWith(
                designId + "_",
                StringComparison.Ordinal);
        }

        internal static string CaptureCompletedDesign(object craftComponent)
        {
            if (_runtimeFailed || !_initialized || craftComponent == null)
                return null;

            object other = GameApi.Get(craftComponent, "other_obj");
            object craft = GameApi.Get(craftComponent, "current_craft");
            if (craft == null)
                return null;

            if (!GameApi.CompletionAwardsTechPointsToPlayer(
                craftComponent,
                craft,
                other))
            {
                return null;
            }

            BalanceRule rule;
            return RuleByCraft.TryGetValue(GameApi.Id(craft), out rule)
                ? rule.DesignId
                : null;
        }

        internal static void CommitCompletedDesign(string designId)
        {
            if (_runtimeFailed || !_initialized)
                return;

            BalanceRule rule;
            if (!RuleByDesign.TryGetValue(designId, out rule))
                return;

            int count = ReadCompletedCount(rule);
            if (count < rule.MaxCompletedCount)
            {
                count++;
                GameApi.SetPlayerInt(rule.CounterParam, count);
            }

            ProjectRule(rule, count);
        }

        private static void ProjectAllFromCurrentSave()
        {
            foreach (BalanceRule rule in RuleByDesign.Values)
                ProjectRule(rule, ReadCompletedCount(rule));
        }

        private static int ReadCompletedCount(BalanceRule rule)
        {
            int count = GameApi.GetPlayerInt(rule.CounterParam, 0);

            if (count < 0)
                count = 0;
            if (count > rule.MaxCompletedCount)
                count = rule.MaxCompletedCount;

            return count;
        }

        private static void ProjectRule(BalanceRule rule, int completedCount)
        {
            List<VariantState> states;
            if (!VariantsByDesign.TryGetValue(rule.DesignId, out states))
                return;

            int red = rule.RewardAfterCompleted(
                rule.RedStart,
                completedCount);

            int blue = rule.RewardAfterCompleted(
                rule.BlueStart,
                completedCount);

            foreach (VariantState state in states)
            {
                IList output = GameApi.Get(state.Craft, "output") as IList;
                GameApi.SetPointTotal(output, "r", red);
                GameApi.SetPointTotal(output, "b", blue);
            }
        }

        internal static void Fail(string stage, Exception ex)
        {
            if (_runtimeFailed)
                return;

            _runtimeFailed = true;

            try
            {
                RestoreManufacturingBaselines();
            }
            catch (Exception restoreEx)
            {
                Plugin.Log.LogError(
                    "G2 baseline restoration also failed: " + restoreEx);
            }

            Plugin.Log.LogError(
                "Better Grave Crafting Rewards disabled dynamic mastery after "
                + stage + " failure: "
                + ex.GetType().Name + ": " + ex.Message);
        }

        private static void RestoreManufacturingBaselines()
        {
            foreach (VariantState state in AllVariants)
            {
                IList output = GameApi.Get(state.Craft, "output") as IList;
                if (output == null)
                    continue;

                GameApi.SetPointTotal(
                    output,
                    "r",
                    state.OriginalRed);

                GameApi.SetPointTotal(
                    output,
                    "b",
                    state.OriginalBlue);
            }
        }
    }
}
