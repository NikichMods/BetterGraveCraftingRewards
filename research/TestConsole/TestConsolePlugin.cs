using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using BepInEx;
using UnityEngine;

namespace BetterGraveCraftingRewards.TestConsole
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(
        "nikich.graveyardkeeper.bettergravecraftingrewards",
        BepInDependency.DependencyFlags.HardDependency)]
    public sealed class TestConsolePlugin : BaseUnityPlugin
    {
        public const string PluginGuid =
            "nikich.graveyardkeeper.bgcrtestconsole";

        public const string PluginName =
            "BGCR Test Console";

        public const string PluginVersion = "0.1.0";

        private const BindingFlags Inst =
            BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic;

        private const BindingFlags Stat =
            BindingFlags.Static
            | BindingFlags.Public
            | BindingFlags.NonPublic;

        private const string StoneFence = "grave_bot_stn_1";
        private const string MarbleFence = "grave_bot_mrb_1";
        private const string SoulStoneFence = "grave_bot_stn_6";

        private Assembly _gameAssembly;
        private Assembly _productionAssembly;
        private Type _gameBalanceType;
        private Type _mainGameType;
        private Type _masteryRuntimeType;
        private MethodInfo _projectCurrentSave;
        private MethodInfo _captureCompletedDesign;

        private bool _visible;
        private Rect _window = new Rect(40f, 40f, 620f, 350f);
        private string _status =
            "F6 toggles this window. Run the three primary probes, then return LogOutput.log.";

        private void Awake()
        {
            try
            {
                Bind();
                Logger.LogInfo(
                    "BGCR Test Console 0.1.0 loaded. Research-only; F6 toggles UI.");
            }
            catch (Exception ex)
            {
                Logger.LogError(
                    "BGCR_TEST_BIND_ERROR|" + OneLine(ex.ToString()));
                enabled = false;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
                _visible = !_visible;
        }

        private void OnGUI()
        {
            if (!_visible)
                return;

            _window = GUI.Window(
                GetInstanceID(),
                _window,
                DrawWindow,
                "BGCR Test Console 0.1.0");
        }

        private void DrawWindow(int id)
        {
            GUILayout.Label(
                "Research-only acceptance harness. It does not spawn items or unlock technologies.");

            GUILayout.Space(5f);

            if (GUILayout.Button("Validate G2 state"))
                Run("validate", ValidateG2State);

            if (GUILayout.Button("Probe mastery endpoint"))
                Run("endpoint", ProbeMasteryEndpoint);

            if (GUILayout.Button("Probe worker / Soul routing"))
                Run("routing", ProbeRouting);

            GUILayout.Space(8f);

            if (GUILayout.Button("Snapshot current save"))
                Run("save_snapshot", SnapshotCurrentSave);

            GUILayout.Space(8f);
            GUILayout.Label("Status:");
            GUILayout.TextArea(_status, GUILayout.Height(90f));

            GUILayout.FlexibleSpace();
            GUILayout.Label("Close with F6. No test action intentionally leaves synthetic state behind.");

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
        }

        private void Run(string action, Action probe)
        {
            try
            {
                Logger.LogInfo("BGCR_TEST_BEGIN|action=" + action);
                probe();
            }
            catch (Exception ex)
            {
                _status = "FAIL: " + action + " — "
                    + ex.GetType().Name + ": " + ex.Message;

                Logger.LogError(
                    "BGCR_TEST_ERROR|action=" + action
                    + "|error=" + OneLine(ex.ToString()));
            }
        }

        private void Bind()
        {
            _gameAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a =>
                    string.Equals(
                        a.GetName().Name,
                        "Assembly-CSharp",
                        StringComparison.Ordinal));

            if (_gameAssembly == null)
                throw new InvalidOperationException(
                    "Assembly-CSharp was not found.");

            _productionAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a =>
                    string.Equals(
                        a.GetName().Name,
                        "BetterGraveCraftingRewards",
                        StringComparison.Ordinal));

            if (_productionAssembly == null)
                throw new InvalidOperationException(
                    "BetterGraveCraftingRewards production assembly was not found.");

            _gameBalanceType = RequireGameType("GameBalance");
            _mainGameType = RequireGameType("MainGame");

            _masteryRuntimeType = _productionAssembly.GetType(
                "BetterGraveCraftingRewards.MasteryRuntime",
                true);

            _projectCurrentSave = _masteryRuntimeType.GetMethod(
                "OnGameStartedPlaying",
                Stat);

            _captureCompletedDesign = _masteryRuntimeType.GetMethod(
                "CaptureCompletedDesign",
                Stat);

            if (_projectCurrentSave == null)
                throw new MissingMethodException(
                    "MasteryRuntime.OnGameStartedPlaying");

            if (_captureCompletedDesign == null)
                throw new MissingMethodException(
                    "MasteryRuntime.CaptureCompletedDesign");

            Version version = _productionAssembly.GetName().Version;
            Logger.LogInfo(
                "BGCR_TEST_BIND"
                + "|production_assembly="
                + _productionAssembly.GetName().Name
                + "|production_version="
                + (version == null ? "<null>" : version.ToString()));
        }

        private void ValidateG2State()
        {
            IList crafts = CraftData;
            if (crafts == null)
                throw new InvalidOperationException(
                    "GameBalance.craft_data is unavailable.");

            bool marblePass = false;
            string marbleObserved = "";

            int originalMarble = GetCounter(MarbleFence);

            if (originalMarble == 0)
            {
                ProjectCurrentSave();
                marblePass = ValidateAllVariants(
                    MarbleFence,
                    8,
                    8,
                    out marbleObserved);
            }
            else
            {
                WithTemporaryCounter(
                    MarbleFence,
                    0,
                    delegate
                    {
                        marblePass = ValidateAllVariants(
                            MarbleFence,
                            8,
                            8,
                            out marbleObserved);
                    });
            }

            bool variantsPass = false;
            string variantsObserved = "";

            WithTemporaryCounter(
                StoneFence,
                2,
                delegate
                {
                    variantsPass = ValidateAllVariants(
                        StoneFence,
                        4,
                        4,
                        out variantsObserved);
                });

            string stoneStudyDetails;
            bool stoneStudyPass = ValidateStudy(
                "surv:grave_bot_stn_1",
                40,
                true,
                "faith",
                3,
                out stoneStudyDetails);

            string marbleStudyDetails;
            bool marbleStudyPass = ValidateStudy(
                "surv:grave_bot_mrb_1",
                90,
                true,
                null,
                0,
                out marbleStudyDetails);

            string soulStudyDetails;
            bool soulStudyPass = ValidateStudy(
                "surv:grave_top_sculpt_stn_5",
                160,
                true,
                null,
                0,
                out soulStudyDetails);

            bool pass =
                marblePass
                && variantsPass
                && stoneStudyPass
                && marbleStudyPass
                && soulStudyPass;

            Logger.LogInfo(
                "BGCR_TEST_VALIDATE"
                + "|pass=" + Lower(pass)
                + "|marble_count_before=" + originalMarble
                + "|marble=" + OneLine(marbleObserved)
                + "|stone_variants=" + OneLine(variantsObserved)
                + "|study_stone=" + OneLine(stoneStudyDetails)
                + "|study_marble=" + OneLine(marbleStudyDetails)
                + "|study_soul=" + OneLine(soulStudyDetails));

            _status = (pass ? "PASS" : "FAIL")
                + ": live G2 state. "
                + "See BGCR_TEST_VALIDATE in the log.";
        }

        private void ProbeMasteryEndpoint()
        {
            int original = GetCounter(StoneFence);
            bool endpointPass = false;
            string endpointObserved = "";

            try
            {
                SetCounter(StoneFence, 10);
                ProjectCurrentSave();

                endpointPass = ValidateAllVariants(
                    StoneFence,
                    0,
                    0,
                    out endpointObserved);
            }
            finally
            {
                SetCounter(StoneFence, original);
                ProjectCurrentSave();
            }

            string restored;
            bool restoredShape = DescribeVariants(
                StoneFence,
                out restored);

            bool pass = endpointPass && restoredShape;

            Logger.LogInfo(
                "BGCR_TEST_ENDPOINT"
                + "|pass=" + Lower(pass)
                + "|original_count=" + original
                + "|endpoint=" + OneLine(endpointObserved)
                + "|restored=" + OneLine(restored));

            _status = (pass ? "PASS" : "FAIL")
                + ": mastery endpoint restored to the real save state. "
                + "See BGCR_TEST_ENDPOINT.";
        }

        private void ProbeRouting()
        {
            object craft = FindCraft(SoulStoneFence);
            if (craft == null)
                throw new InvalidOperationException(
                    "Required Soul craft is missing: " + SoulStoneFence);

            FakeCraftComponent playerContext =
                new FakeCraftComponent
                {
                    current_craft = craft,
                    other_obj = new FakeActor { is_player = true },
                    wgo = new FakeStation
                    {
                        is_current_craft_gratitude = false
                    }
                };

            FakeCraftComponent workerContext =
                new FakeCraftComponent
                {
                    current_craft = craft,
                    other_obj = new FakeActor { is_player = false },
                    wgo = new FakeStation
                    {
                        is_current_craft_gratitude = false
                    }
                };

            FakeCraftComponent gratitudeContext =
                new FakeCraftComponent
                {
                    current_craft = craft,
                    other_obj = new FakeActor { is_player = false },
                    wgo = new FakeStation
                    {
                        is_current_craft_gratitude = true
                    }
                };

            string player = Capture(playerContext);
            string worker = Capture(workerContext);
            string gratitude = Capture(gratitudeContext);

            bool pass =
                string.Equals(player, SoulStoneFence, StringComparison.Ordinal)
                && worker == null
                && string.Equals(
                    gratitude,
                    SoulStoneFence,
                    StringComparison.Ordinal);

            Logger.LogInfo(
                "BGCR_TEST_ROUTING"
                + "|pass=" + Lower(pass)
                + "|design=" + SoulStoneFence
                + "|player=" + NullText(player)
                + "|worker=" + NullText(worker)
                + "|gratitude=" + NullText(gratitude));

            _status = (pass ? "PASS" : "FAIL")
                + ": player / worker / Soul Gratitude routing. "
                + "No mastery state was changed.";
        }

        private void SnapshotCurrentSave()
        {
            int count = GetCounter(StoneFence);
            string variants;
            bool found = DescribeVariants(StoneFence, out variants);

            Logger.LogInfo(
                "BGCR_TEST_SAVE"
                + "|found=" + Lower(found)
                + "|design=" + StoneFence
                + "|completed=" + count
                + "|variants=" + OneLine(variants));

            _status = "Snapshot written: completed=" + count
                + ". Use this after loading another save for the cross-save check.";
        }

        private bool ValidateStudy(
            string craftId,
            int expectedBlue,
            bool expectedOneTime,
            string requiredNeedId,
            int requiredNeedValue,
            out string details)
        {
            object craft = FindCraft(craftId);
            if (craft == null)
            {
                details = craftId + ":missing";
                return false;
            }

            IList output = Get(craft, "output") as IList;
            IList needs = Get(craft, "needs") as IList;

            int blue = PointTotal(output, "b");
            bool oneTime = ToBool(Get(craft, "one_time_craft"));

            bool needPass = true;
            int needValue = 0;

            if (!string.IsNullOrEmpty(requiredNeedId))
            {
                needValue = ItemTotal(needs, requiredNeedId);
                needPass = needValue == requiredNeedValue;
            }

            bool pass =
                blue == expectedBlue
                && oneTime == expectedOneTime
                && needPass;

            details =
                craftId
                + ":b=" + blue
                + ",one_time=" + Lower(oneTime)
                + (string.IsNullOrEmpty(requiredNeedId)
                    ? ""
                    : "," + requiredNeedId + "=" + needValue);

            return pass;
        }

        private bool ValidateAllVariants(
            string designId,
            int expectedRed,
            int expectedBlue,
            out string observed)
        {
            List<object> variants = FindManufacturingVariants(designId);
            if (variants.Count == 0)
            {
                observed = designId + ":no_variants";
                return false;
            }

            bool pass = true;
            List<string> rows = new List<string>();

            foreach (object craft in variants)
            {
                IList output = Get(craft, "output") as IList;

                int red = PointTotal(output, "r");
                int blue = PointTotal(output, "b");
                int physical = ItemTotal(output, designId);

                if (red != expectedRed
                    || blue != expectedBlue
                    || physical != 1)
                {
                    pass = false;
                }

                rows.Add(
                    Id(craft)
                    + "=" + red + "R/" + blue + "B"
                    + "/physical:" + physical);
            }

            observed = string.Join(";", rows.ToArray());
            return pass;
        }

        private bool DescribeVariants(
            string designId,
            out string observed)
        {
            List<object> variants = FindManufacturingVariants(designId);
            if (variants.Count == 0)
            {
                observed = designId + ":no_variants";
                return false;
            }

            List<string> rows = new List<string>();

            foreach (object craft in variants)
            {
                IList output = Get(craft, "output") as IList;
                rows.Add(
                    Id(craft)
                    + "="
                    + PointTotal(output, "r") + "R/"
                    + PointTotal(output, "b") + "B"
                    + "/physical:" + ItemTotal(output, designId));
            }

            observed = string.Join(";", rows.ToArray());
            return true;
        }

        private List<object> FindManufacturingVariants(string designId)
        {
            List<object> result = new List<object>();

            foreach (object craft in CraftData)
            {
                if (craft == null)
                    continue;

                string id = Id(craft);
                bool idMatch =
                    string.Equals(id, designId, StringComparison.Ordinal)
                    || id.StartsWith(
                        designId + "_",
                        StringComparison.Ordinal);

                if (!idMatch)
                    continue;

                IList output = Get(craft, "output") as IList;
                if (ItemTotal(output, designId) > 0)
                    result.Add(craft);
            }

            return result;
        }

        private void WithTemporaryCounter(
            string designId,
            int temporaryValue,
            Action body)
        {
            int original = GetCounter(designId);

            try
            {
                if (original != temporaryValue)
                {
                    SetCounter(designId, temporaryValue);
                    ProjectCurrentSave();
                }
                else
                {
                    ProjectCurrentSave();
                }

                body();
            }
            finally
            {
                if (original != temporaryValue)
                    SetCounter(designId, original);

                ProjectCurrentSave();
            }
        }

        private void ProjectCurrentSave()
        {
            _projectCurrentSave.Invoke(null, null);
        }

        private string Capture(object fakeCraftComponent)
        {
            object value = _captureCompletedDesign.Invoke(
                null,
                new[] { fakeCraftComponent });

            return value as string;
        }

        private int GetCounter(string designId)
        {
            return GetPlayerInt(
                "nikich_bgcr_m_" + designId,
                0);
        }

        private void SetCounter(string designId, int value)
        {
            SetPlayerInt(
                "nikich_bgcr_m_" + designId,
                value);
        }

        private object FindCraft(string craftId)
        {
            IList crafts = CraftData;
            if (crafts == null)
                return null;

            foreach (object craft in crafts)
            {
                if (craft != null
                    && string.Equals(
                        Id(craft),
                        craftId,
                        StringComparison.Ordinal))
                {
                    return craft;
                }
            }

            return null;
        }

        private IList CraftData
        {
            get
            {
                object balance = GetStatic(_gameBalanceType, "me");
                return Get(balance, "craft_data") as IList;
            }
        }

        private object Player
        {
            get
            {
                object mainGame = GetStatic(_mainGameType, "me");
                return Get(mainGame, "player");
            }
        }

        private int GetPlayerInt(string name, int fallback)
        {
            object player = Player;
            if (player == null)
                throw new InvalidOperationException(
                    "MainGame player is unavailable.");

            MethodInfo method = player.GetType().GetMethod(
                "GetParam",
                Inst,
                null,
                new[] { typeof(string), typeof(float) },
                null);

            if (method == null)
                throw new MissingMethodException(
                    player.GetType().FullName,
                    "GetParam(string,float)");

            object value = method.Invoke(
                player,
                new object[] { name, (float)fallback });

            return (int)Math.Round(
                Convert.ToSingle(value, CultureInfo.InvariantCulture));
        }

        private void SetPlayerInt(string name, int value)
        {
            object player = Player;
            if (player == null)
                throw new InvalidOperationException(
                    "MainGame player is unavailable.");

            MethodInfo method = player.GetType().GetMethod(
                "SetParam",
                Inst,
                null,
                new[] { typeof(string), typeof(float) },
                null);

            if (method == null)
                throw new MissingMethodException(
                    player.GetType().FullName,
                    "SetParam(string,float)");

            method.Invoke(
                player,
                new object[] { name, (float)value });
        }

        private Type RequireGameType(string name)
        {
            Type direct = _gameAssembly.GetType(name, false);
            if (direct != null)
                return direct;

            try
            {
                Type found = _gameAssembly.GetTypes()
                    .FirstOrDefault(t => t != null && t.Name == name);

                if (found != null)
                    return found;
            }
            catch (ReflectionTypeLoadException ex)
            {
                Type found = ex.Types
                    .FirstOrDefault(t => t != null && t.Name == name);

                if (found != null)
                    return found;
            }

            throw new TypeLoadException(name);
        }

        private static object Get(object obj, string name)
        {
            if (obj == null)
                return null;

            for (Type t = obj.GetType(); t != null; t = t.BaseType)
            {
                FieldInfo field = t.GetField(name, Inst);
                if (field != null)
                    return field.GetValue(obj);

                PropertyInfo property = t.GetProperty(name, Inst);
                if (property != null && property.CanRead)
                    return property.GetValue(obj, null);
            }

            return null;
        }

        private static object GetStatic(Type type, string name)
        {
            if (type == null)
                return null;

            for (Type t = type; t != null; t = t.BaseType)
            {
                FieldInfo field = t.GetField(name, Stat);
                if (field != null)
                    return field.GetValue(null);

                PropertyInfo property = t.GetProperty(name, Stat);
                if (property != null && property.CanRead)
                    return property.GetValue(null, null);
            }

            return null;
        }

        private static string Id(object obj)
        {
            return Get(obj, "id") as string ?? string.Empty;
        }

        private static int PointTotal(IList list, string pointId)
        {
            return ItemTotal(list, pointId);
        }

        private static int ItemTotal(IList list, string itemId)
        {
            if (list == null)
                return 0;

            int total = 0;

            foreach (object item in list)
            {
                if (item == null)
                    continue;

                if (!string.Equals(
                    Id(item),
                    itemId,
                    StringComparison.Ordinal))
                {
                    continue;
                }

                total += Convert.ToInt32(
                    Get(item, "value"),
                    CultureInfo.InvariantCulture);
            }

            return total;
        }

        private static bool ToBool(object value)
        {
            if (value == null)
                return false;

            return Convert.ToBoolean(
                value,
                CultureInfo.InvariantCulture);
        }

        private static string Lower(bool value)
        {
            return value ? "true" : "false";
        }

        private static string NullText(string value)
        {
            return value ?? "<null>";
        }

        private static string OneLine(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return value
                .Replace("|", "/")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }

        private sealed class FakeCraftComponent
        {
            public object current_craft;
            public object other_obj;
            public object wgo;
        }

        private sealed class FakeActor
        {
            public bool is_player;
        }

        private sealed class FakeStation
        {
            public bool is_current_craft_gratitude;
        }
    }
}
