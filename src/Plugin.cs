// SPDX-License-Identifier: MPL-2.0

using System;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace BetterGraveCraftingRewards
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "nikich.graveyardkeeper.bettergravecraftingrewards";
        public const string PluginName = "Better Grave Crafting Rewards";
        public const string PluginVersion = "0.1.1";

        internal static ManualLogSource Log;

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;

            try
            {
                BalanceRules.Validate();
                GameApi.Bind();

                _harmony = new Harmony(PluginGuid);

                MethodInfo started = GameApi.RequireMethod(
                    GameApi.MainGameType,
                    "OnGameStartedPlaying",
                    0);

                MethodInfo finished = GameApi.RequireMethod(
                    GameApi.CraftComponentType,
                    "ProcessFinishedCraft",
                    0);

                _harmony.Patch(
                    started,
                    postfix: new HarmonyMethod(
                        typeof(RuntimePatches).GetMethod(
                            nameof(RuntimePatches.OnGameStartedPlayingPostfix),
                            BindingFlags.Static | BindingFlags.Public)));

                _harmony.Patch(
                    finished,
                    prefix: new HarmonyMethod(
                        typeof(RuntimePatches).GetMethod(
                            nameof(RuntimePatches.ProcessFinishedCraftPrefix),
                            BindingFlags.Static | BindingFlags.Public)),
                    postfix: new HarmonyMethod(
                        typeof(RuntimePatches).GetMethod(
                            nameof(RuntimePatches.ProcessFinishedCraftPostfix),
                            BindingFlags.Static | BindingFlags.Public)));

                Log.LogInfo(PluginName + " " + PluginVersion + " loaded.");
            }
            catch (Exception ex)
            {
                Log.LogError(PluginName + " failed to initialize: " + ex);
            }
        }

        private void OnDestroy()
        {
            if (_harmony != null)
                _harmony.UnpatchSelf();
        }
    }

    internal static class RuntimePatches
    {
        public static void OnGameStartedPlayingPostfix()
        {
            try
            {
                MasteryRuntime.OnGameStartedPlaying();
            }
            catch (Exception ex)
            {
                MasteryRuntime.Fail("game-start projection", ex);
            }
        }

        public static void ProcessFinishedCraftPrefix(
            object __instance,
            out string __state)
        {
            __state = null;

            try
            {
                __state = MasteryRuntime.CaptureCompletedDesign(__instance);
            }
            catch (Exception ex)
            {
                MasteryRuntime.Fail("craft completion capture", ex);
            }
        }

        public static void ProcessFinishedCraftPostfix(string __state)
        {
            if (string.IsNullOrEmpty(__state))
                return;

            try
            {
                MasteryRuntime.CommitCompletedDesign(__state);
            }
            catch (Exception ex)
            {
                MasteryRuntime.Fail("craft mastery commit", ex);
            }
        }
    }
}
