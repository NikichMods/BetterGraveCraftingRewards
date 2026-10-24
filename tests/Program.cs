// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using BetterGraveCraftingRewards;

internal static class Program
{
    private static void Main()
    {
        BalanceRules.Validate();
        // Independent product groups: DLC floors take precedence over material.
        string[] groups =
        {
            "grave_top_wd_tab_1 grave_bot_wd_1 grave_top_wd_cross_1",
            "grave_top_stn_plate_1 grave_bot_stn_1 grave_top_stn_cross_1 grave_top_stn_plate_2 grave_bot_stn_2 grave_top_stn_cross_2 grave_top_stella_stn_1",
            "grave_top_sculpt_stn_1 grave_top_sculpt_stn_2",
            "grave_bot_mrb_1 grave_top_mrb_cross_1 grave_bot_mrb_2 grave_top_mrb_cross_2 grave_top_stella_mrb_1 grave_top_sculpt_mrb_1 grave_top_sculpt_mrb_2",
            "grave_bot_stn_3 grave_bot_stn_4 grave_top_memorial_stn_1 grave_bot_mrb_3 grave_bot_stn_5 grave_bot_mrb_4 grave_top_memorial_mrb_1 grave_bot_mrb_5 grave_top_womansaver_stn_1 grave_top_highangel_stn_1 grave_top_womansaver_mrb_1 grave_top_highangel_mrb_1 grave_bot_stn_6 grave_bot_stn_7 grave_bot_stn_8 grave_bot_mrb_6 grave_bot_mrb_7 grave_bot_mrb_8 grave_top_sculpt_stn_4 grave_top_sculpt_stn_5 grave_top_sculpt_mrb_4 grave_top_sculpt_mrb_5"
        };
        var expected = new Dictionary<string, int>();
        for (int i = 0; i < groups.Length; i++)
            foreach (string id in groups[i].Split(' '))
                expected.Add(id, i + 1);

        Require(expected.Count == 41, "Product fixture must cover 41 designs");
        foreach (BalanceRule rule in BalanceRules.All)
        {
            Require(expected[rule.DesignId] == rule.RedFloor, "Floor: " + rule.DesignId);
            Require(rule.MaxCompletedCount == 2 * Math.Max(rule.RedStart, rule.BlueStart),
                "Save cap changed: " + rule.DesignId);
            Require(rule.CounterParam == "nikich_bgcr_m_" + rule.DesignId,
                "Save key changed: " + rule.DesignId);
            // Include paired boundaries, exhausted saves, negative input and int maximum.
            var counts = Enumerable.Range(-2, rule.MaxCompletedCount + 8)
                .Concat(new[] { 1000, int.MaxValue });
            foreach (int count in counts)
            {
                int oldRed = Math.Max(0, rule.RedStart - Math.Max(0, count) / 2);
                int red = rule.RedRewardAfterCompleted(count);
                Require(red == Math.Max(expected[rule.DesignId], oldRed),
                    "Red transition: " + rule.DesignId + " at " + count);
                Require(red <= rule.RedStart, "Starting reward increased: " + rule.DesignId);
                if (oldRed >= rule.RedFloor)
                    Require(red == oldRed, "Above-floor reward changed: " + rule.DesignId);
                int blue = rule.RewardAfterCompleted(rule.BlueStart, count);
                int expectedBlue = Math.Max(0, rule.BlueStart - Math.Max(0, count) / 2);
                Require(blue == expectedBlue, "Blue changed: " + rule.DesignId);
            }
            Require(rule.RedRewardAfterCompleted(rule.MaxCompletedCount) == rule.RedFloor,
                "Exhausted save: " + rule.DesignId);
            Require(rule.RewardAfterCompleted(rule.BlueStart, rule.MaxCompletedCount) == 0,
                "Blue must exhaust: " + rule.DesignId);
        }
        Console.WriteLine("PASS: 41 designs, floors 1/2/3/4/5, paired boundaries, exhausted saves, unchanged blue and counter schema.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
