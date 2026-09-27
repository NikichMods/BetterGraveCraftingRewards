using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterGraveCraftingRewards
{
    internal sealed class BalanceRule
    {
        internal BalanceRule(
            string designId,
            int vanillaRed,
            int vanillaBlue,
            int start,
            bool blueMastery,
            int vanillaStudyBlue = 0,
            int targetStudyBlue = 0)
        {
            DesignId = designId;
            VanillaRed = vanillaRed;
            VanillaBlue = vanillaBlue;
            RedStart = start;
            BlueStart = blueMastery ? start : 0;
            VanillaStudyBlue = vanillaStudyBlue;
            TargetStudyBlue = targetStudyBlue;
        }

        internal string DesignId { get; private set; }
        internal int VanillaRed { get; private set; }
        internal int VanillaBlue { get; private set; }
        internal int RedStart { get; private set; }
        internal int BlueStart { get; private set; }
        internal int VanillaStudyBlue { get; private set; }
        internal int TargetStudyBlue { get; private set; }

        internal string CounterParam
        {
            get { return "nikich_bgcr_m_" + DesignId; }
        }

        internal int MaxCompletedCount
        {
            get { return 2 * Math.Max(RedStart, BlueStart); }
        }

        internal int RewardAfterCompleted(int start, int completedCount)
        {
            if (start <= 0)
                return 0;

            int safeCount = Math.Max(0, completedCount);
            return Math.Max(start - safeCount / 2, 0);
        }

        internal int ExpectedVanillaRed(string craftId)
        {
            if (string.Equals(
                craftId,
                "grave_top_wd_tab_1_3",
                StringComparison.Ordinal))
            {
                return 1;
            }

            return VanillaRed;
        }
    }

    internal static class BalanceRules
    {
        internal static readonly List<BalanceRule> All =
            new List<BalanceRule>
            {
                R("grave_top_wd_tab_1",       3,  0,  3, false,  11,  20),
                R("grave_bot_wd_1",           2,  0,  4, false,  21,  30),
                R("grave_top_stn_plate_1",    5,  0,  4, false,  21,  30),
                R("grave_top_wd_cross_1",     5,  0,  4, false,  21,  30),

                R("grave_bot_stn_1",          2,  5,  5, true,   31,  40),
                R("grave_top_stn_cross_1",    5,  5,  5, true,   31,  40),
                R("grave_top_stn_plate_2",    5,  5,  5, true,   31,  40),

                R("grave_bot_stn_2",          2,  5,  6, true,   31,  50),
                R("grave_top_stn_cross_2",    5,  5,  6, true,   31,  50),
                R("grave_top_stella_stn_1",   5,  5,  7, true,   31,  60),

                R("grave_bot_mrb_1",          5,  0,  8, true,   81,  90),
                R("grave_top_mrb_cross_1",    5,  0,  8, true,   81,  90),

                R("grave_bot_mrb_2",          5,  0,  9, true,   91, 100),
                R("grave_top_sculpt_stn_1",   5, 10,  9, true,   51,  80),
                R("grave_top_sculpt_stn_2",   5, 10,  9, true,   51,  80),
                R("grave_top_mrb_cross_2",    5,  0,  9, true,   91, 100),
                R("grave_top_stella_mrb_1",   5, 15, 10, true,   91, 100),

                R("grave_top_sculpt_mrb_1",   5, 15, 12, true,  101, 110),
                R("grave_top_sculpt_mrb_2",   5, 15, 12, true,  101, 110),

                R("grave_bot_stn_3",          5,  0,  6, true),
                R("grave_bot_stn_4",          5,  0,  7, true),
                R("grave_top_memorial_stn_1",10,  6,  8, true),
                R("grave_bot_mrb_3",          5,  0,  8, true),
                R("grave_bot_stn_5",          5,  2,  8, true),
                R("grave_bot_mrb_4",          5,  0,  9, true),
                R("grave_top_memorial_mrb_1",10,  6, 10, true),
                R("grave_bot_mrb_5",          5,  2, 10, true),
                R("grave_top_womansaver_stn_1",10,15,11,true),
                R("grave_top_highangel_stn_1",15,15,12,true),
                R("grave_top_womansaver_mrb_1",10,15,13,true),
                R("grave_top_highangel_mrb_1",15,15,14,true),

                R("grave_bot_stn_6",          5,  0,  9, true),
                R("grave_bot_stn_7",         10,  5, 10, true),
                R("grave_bot_stn_8",         15, 10, 11, true, 101, 110),
                R("grave_bot_mrb_6",          7,  0, 12, true),
                R("grave_bot_mrb_7",          9,  5, 13, true),
                R("grave_bot_mrb_8",         10, 15, 14, true, 121, 130),
                R("grave_top_sculpt_stn_4",  15, 15, 15, true),
                R("grave_top_sculpt_stn_5",  17, 15, 17, true, 101, 160),
                R("grave_top_sculpt_mrb_4",  15, 15, 18, true),
                R("grave_top_sculpt_mrb_5",  17, 15, 20, true, 121, 190)
            };

        private static BalanceRule R(
            string id,
            int vanillaRed,
            int vanillaBlue,
            int start,
            bool blueMastery,
            int vanillaStudyBlue = 0,
            int targetStudyBlue = 0)
        {
            return new BalanceRule(
                id,
                vanillaRed,
                vanillaBlue,
                start,
                blueMastery,
                vanillaStudyBlue,
                targetStudyBlue);
        }

        internal static void Validate()
        {
            if (All.Count != 41)
                throw new InvalidOperationException(
                    "G2 must contain exactly 41 active grave designs.");

            if (All.Select(x => x.DesignId).Distinct().Count() != All.Count)
                throw new InvalidOperationException(
                    "G2 contains duplicate grave design IDs.");

            if (All.Any(x =>
                string.Equals(
                    x.DesignId,
                    "grave_top_sarcofag_mrb_1",
                    StringComparison.Ordinal)))
            {
                throw new InvalidOperationException(
                    "The unused Marble sarcophagus must stay outside G2.");
            }

            if (All.Count(x => x.TargetStudyBlue > 0) != 23)
                throw new InvalidOperationException(
                    "G2 must contain exactly 23 active Study mutations.");

            foreach (BalanceRule rule in All)
            {
                if (rule.RedStart <= 0)
                    throw new InvalidOperationException(
                        "Invalid red mastery start for " + rule.DesignId);

                if (rule.TargetStudyBlue < rule.VanillaStudyBlue)
                    throw new InvalidOperationException(
                        "G2 Study reward must not decrease for " + rule.DesignId);

                if (rule.TargetStudyBlue > 0 && rule.VanillaStudyBlue <= 0)
                    throw new InvalidOperationException(
                        "G2 must not create a new Study recipe for " + rule.DesignId);
            }

            BalanceRule probe = All.First(x => x.RedStart == 5);
            if (probe.RewardAfterCompleted(5, 0) != 5
                || probe.RewardAfterCompleted(5, 1) != 5
                || probe.RewardAfterCompleted(5, 2) != 4
                || probe.RewardAfterCompleted(5, 8) != 1
                || probe.RewardAfterCompleted(5, 10) != 0)
            {
                throw new InvalidOperationException(
                    "Paired mastery formula self-check failed.");
            }
        }
    }
}
