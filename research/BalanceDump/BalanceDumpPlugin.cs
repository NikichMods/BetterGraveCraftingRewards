// SPDX-License-Identifier: MPL-2.0

using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NikichMods.BGCRBalanceDump
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class BalanceDumpPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "nikich.graveyardkeeper.bgcrbalancedump";
        public const string PluginName = "BGCR Red Economy Dump";
        public const string PluginVersion = "0.5.0";

        private static readonly BindingFlags Inst = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly BindingFlags Stat = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly HashSet<string> TechPointIds = new HashSet<string>(StringComparer.Ordinal)
        {
            "r", "g", "b", "v", "gratitude_points"
        };

        private Assembly _gameAssembly;
        private bool _dumped;

        private void Awake()
        {
            Logger.LogInfo("BGCR Red Economy Dump 0.5.0 loaded. Read-only research probe.");
            StartCoroutine(DumpWhenReady());
        }

        private IEnumerator DumpWhenReady()
        {
            while (!_dumped)
            {
                if (TryBindGameAssembly())
                {
                    Type mainGameType = GameType("MainGame");
                    Type balanceType = GameType("GameBalance");

                    if (ToBool(GetStatic(mainGameType, "game_started")))
                    {
                        object balance = GetStatic(balanceType, "me");
                        if (balance != null)
                        {
                            try
                            {
                                Dump(balance);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("BGCR_RED_ECON_ERROR|" + Escape(ex.ToString()));
                            }

                            _dumped = true;
                            yield break;
                        }
                    }
                }

                yield return null;
            }
        }

        private void Dump(object balance)
        {
            IList crafts = Get(balance, "craft_data") as IList;
            IList techs = Get(balance, "techs_data") as IList;
            IList works = Get(balance, "works_data") as IList;
            IList objects = Get(balance, "objs_data") as IList;

            if (crafts == null || techs == null || works == null || objects == null)
                throw new InvalidOperationException("Required GameBalance collections are not available.");

            Logger.LogInfo("BGCR_RED_ECON_BEGIN|probe=0.5.0|target=GraveyardKeeper-1.407|source=loaded-GameBalance");

            int craftSources = 0;
            int surveys = 0;
            int oneTime = 0;
            int hidden = 0;
            int ordinaryRepeatableVisible = 0;
            double craftOnePassRed = 0d;

            foreach (object craft in crafts)
            {
                if (craft == null) continue;

                IList outputs = Get(craft, "output") as IList;
                double red = Reward(outputs, "r");
                if (red <= 0d) continue;

                craftSources++;
                craftOnePassRed += red;

                string craftType = Convert.ToString(Get(craft, "craft_type"), CultureInfo.InvariantCulture) ?? "";
                bool isSurvey = string.Equals(craftType, "Survey", StringComparison.Ordinal);
                bool isOneTime = ToBool(Get(craft, "one_time_craft"));
                bool isHidden = ToBool(Get(craft, "hidden"));

                if (isSurvey) surveys++;
                if (isOneTime) oneTime++;
                if (isHidden) hidden++;

                if (!isSurvey && !isOneTime && !isHidden && !ToBool(Get(craft, "is_auto")))
                    ordinaryRepeatableVisible++;

                string craftId = Id(craft);
                object physical = FirstPhysicalOutput(outputs);

                Logger.LogInfo(
                    "BGCR_RED_CRAFT" +
                    "|id=" + Escape(craftId) +
                    "|craft_type=" + Escape(craftType) +
                    "|sub_type=" + Escape(Convert.ToString(Get(craft, "sub_type"), CultureInfo.InvariantCulture)) +
                    "|red=" + Format(red) +
                    "|green=" + Format(Reward(outputs, "g")) +
                    "|blue=" + Format(Reward(outputs, "b")) +
                    "|needs=" + Escape(ItemList(Get(craft, "needs") as IList)) +
                    "|outputs=" + Escape(ItemList(outputs)) +
                    "|physical_output=" + Escape(Id(physical)) +
                    "|physical_type=" + Escape(ItemType(physical)) +
                    "|craft_in=" + Escape(StringList(Get(craft, "craft_in") as IEnumerable)) +
                    "|energy_expr=" + Escape(ExpressionRaw(Get(craft, "energy"))) +
                    "|time_expr=" + Escape(ExpressionRaw(Get(craft, "craft_time"))) +
                    "|condition_expr=" + Escape(ExpressionRaw(Get(craft, "condition"))) +
                    "|hidden=" + BoolString(Get(craft, "hidden")) +
                    "|one_time=" + BoolString(Get(craft, "one_time_craft")) +
                    "|is_auto=" + BoolString(Get(craft, "is_auto")) +
                    "|needs_unlock=" + BoolString(Get(craft, "needs_unlock")) +
                    "|can_craft_always=" + BoolString(Get(craft, "can_craft_always")) +
                    "|techs=" + Escape(TechIdsForCraft(techs, craftId)) +
                    "|tech_meta=" + Escape(TechMetaForCraft(techs, craftId)));
            }

            int workSources = 0;
            double workOnePassRed = 0d;

            foreach (object work in works)
            {
                if (work == null) continue;

                object reward = Get(work, "reward");
                double red = GameResGet(reward, "r");
                if (red <= 0d) continue;

                workSources++;
                workOnePassRed += red;

                Logger.LogInfo(
                    "BGCR_RED_WORK" +
                    "|id=" + Escape(Id(work)) +
                    "|red=" + Format(red) +
                    "|green=" + Format(GameResGet(reward, "g")) +
                    "|blue=" + Format(GameResGet(reward, "b")));
            }

            int objectSources = 0;
            int workLinkedObjects = 0;
            int dropObjects = 0;
            int addParamObjects = 0;

            foreach (object obj in objects)
            {
                if (obj == null) continue;

                string workId = Convert.ToString(Get(obj, "work"), CultureInfo.InvariantCulture) ?? "";
                object linkedWork = FindById(works, workId);
                object workReward = Get(linkedWork, "reward");

                double workRed = GameResGet(workReward, "r");
                IList dropItems = Get(obj, "drop_items") as IList;
                double dropRed = Reward(dropItems, "r");
                object addPlayer = Get(obj, "add_player_param_after_hp_0");
                double addRed = GameResGet(addPlayer, "r");

                if (workRed <= 0d && dropRed <= 0d && addRed <= 0d) continue;

                objectSources++;
                if (workRed > 0d) workLinkedObjects++;
                if (dropRed > 0d) dropObjects++;
                if (addRed > 0d) addParamObjects++;

                Logger.LogInfo(
                    "BGCR_RED_OBJECT" +
                    "|id=" + Escape(Id(obj)) +
                    "|type=" + Escape(Convert.ToString(Get(obj, "type"), CultureInfo.InvariantCulture)) +
                    "|work=" + Escape(workId) +
                    "|work_red=" + Format(workRed) +
                    "|work_green=" + Format(GameResGet(workReward, "g")) +
                    "|work_blue=" + Format(GameResGet(workReward, "b")) +
                    "|drop_red=" + Format(dropRed) +
                    "|drop_items=" + Escape(ItemList(dropItems)) +
                    "|add_player_red=" + Format(addRed) +
                    "|add_player_green=" + Format(GameResGet(addPlayer, "g")) +
                    "|add_player_blue=" + Format(GameResGet(addPlayer, "b")) +
                    "|hp_expr=" + Escape(ExpressionRaw(Get(obj, "hp"))) +
                    "|player_cant_work=" + BoolString(Get(obj, "player_cant_work")) +
                    "|need_unlock_work=" + BoolString(Get(obj, "need_unlock_work")));
            }

            Logger.LogInfo(
                "BGCR_RED_ECON_DONE" +
                "|craft_sources=" + craftSources.ToString(CultureInfo.InvariantCulture) +
                "|surveys=" + surveys.ToString(CultureInfo.InvariantCulture) +
                "|one_time=" + oneTime.ToString(CultureInfo.InvariantCulture) +
                "|hidden=" + hidden.ToString(CultureInfo.InvariantCulture) +
                "|ordinary_repeatable_visible=" + ordinaryRepeatableVisible.ToString(CultureInfo.InvariantCulture) +
                "|craft_one_pass_red_sum=" + Format(craftOnePassRed) +
                "|work_sources=" + workSources.ToString(CultureInfo.InvariantCulture) +
                "|work_one_pass_red_sum=" + Format(workOnePassRed) +
                "|object_sources=" + objectSources.ToString(CultureInfo.InvariantCulture) +
                "|work_linked_objects=" + workLinkedObjects.ToString(CultureInfo.InvariantCulture) +
                "|drop_objects=" + dropObjects.ToString(CultureInfo.InvariantCulture) +
                "|add_param_objects=" + addParamObjects.ToString(CultureInfo.InvariantCulture));
        }

        private static object FindById(IList list, string id)
        {
            if (list == null || string.IsNullOrEmpty(id)) return null;
            foreach (object item in list)
            {
                if (item != null && string.Equals(Id(item), id, StringComparison.Ordinal))
                    return item;
            }
            return null;
        }

        private static object FirstPhysicalOutput(IList outputs)
        {
            if (outputs == null) return null;

            foreach (object output in outputs)
            {
                if (output == null) continue;
                string id = Id(output);
                if (!TechPointIds.Contains(id))
                    return output;
            }

            return null;
        }

        private static string ItemType(object item)
        {
            if (item == null) return "";
            object def = Get(item, "definition");
            return Convert.ToString(Get(def, "type"), CultureInfo.InvariantCulture) ?? "";
        }

        private static string TechIdsForCraft(IList techs, string craftId)
        {
            List<string> ids = new List<string>();

            foreach (object tech in techs)
            {
                if (tech == null) continue;
                if (ContainsString(Get(tech, "crafts") as IList, craftId))
                    ids.Add(Id(tech));
            }

            return string.Join(";", ids.ToArray());
        }

        private static string TechMetaForCraft(IList techs, string craftId)
        {
            List<string> values = new List<string>();

            foreach (object tech in techs)
            {
                if (tech == null) continue;
                if (!ContainsString(Get(tech, "crafts") as IList, craftId)) continue;

                object price = Get(tech, "price");
                values.Add(
                    Id(tech) +
                    "[branch=" + Convert.ToString(Get(tech, "branch_type"), CultureInfo.InvariantCulture) +
                    ",dlc=" + Convert.ToString(Get(tech, "requires_dlc"), CultureInfo.InvariantCulture) +
                    ",r=" + Format(GameResGet(price, "r")) +
                    ",g=" + Format(GameResGet(price, "g")) +
                    ",b=" + Format(GameResGet(price, "b")) +
                    "]");
            }

            return string.Join(";", values.ToArray());
        }

        private bool TryBindGameAssembly()
        {
            if (_gameAssembly != null) return true;

            _gameAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => string.Equals(a.GetName().Name, "Assembly-CSharp", StringComparison.Ordinal));

            return _gameAssembly != null;
        }

        private Type GameType(string name)
        {
            if (_gameAssembly == null) return null;

            Type direct = _gameAssembly.GetType(name, false);
            if (direct != null) return direct;

            try
            {
                return _gameAssembly.GetTypes().FirstOrDefault(t => t != null && t.Name == name);
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.FirstOrDefault(t => t != null && t.Name == name);
            }
        }

        private static object Get(object obj, string name)
        {
            if (obj == null) return null;

            for (Type t = obj.GetType(); t != null; t = t.BaseType)
            {
                FieldInfo field = t.GetField(name, Inst);
                if (field != null) return field.GetValue(obj);

                PropertyInfo property = t.GetProperty(name, Inst);
                if (property != null && property.CanRead) return property.GetValue(obj, null);
            }

            return null;
        }

        private static object GetStatic(Type type, string name)
        {
            if (type == null) return null;

            for (Type t = type; t != null; t = t.BaseType)
            {
                FieldInfo field = t.GetField(name, Stat);
                if (field != null) return field.GetValue(null);

                PropertyInfo property = t.GetProperty(name, Stat);
                if (property != null && property.CanRead) return property.GetValue(null, null);
            }

            return null;
        }

        private static string Id(object obj)
        {
            return Get(obj, "id") as string ?? "";
        }

        private static double Reward(IList output, string id)
        {
            if (output == null) return 0d;

            double sum = 0d;
            foreach (object item in output)
            {
                if (!string.Equals(Id(item), id, StringComparison.Ordinal)) continue;
                sum += ToDouble(Get(item, "value"));
            }

            return sum;
        }

        private static string ItemList(IList list)
        {
            if (list == null) return "";

            List<string> values = new List<string>();
            foreach (object item in list)
            {
                if (item == null) continue;
                values.Add(Id(item) + ":" + Format(ToDouble(Get(item, "value"))));
            }

            return string.Join(";", values.ToArray());
        }

        private static string StringList(IEnumerable values)
        {
            if (values == null) return "";

            List<string> result = new List<string>();
            foreach (object value in values)
                result.Add(Convert.ToString(value, CultureInfo.InvariantCulture) ?? "");

            return string.Join(";", result.ToArray());
        }

        private static bool ContainsString(IList list, string value)
        {
            if (list == null) return false;

            foreach (object item in list)
            {
                if (string.Equals(Convert.ToString(item, CultureInfo.InvariantCulture), value, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        private static double GameResGet(object gameRes, string key)
        {
            if (gameRes == null) return 0d;

            MethodInfo method = gameRes.GetType().GetMethods(Inst)
                .FirstOrDefault(m =>
                {
                    if (m.Name != "Get") return false;
                    ParameterInfo[] p = m.GetParameters();
                    return p.Length == 2 && p[0].ParameterType == typeof(string);
                });

            if (method == null) return 0d;

            try
            {
                object value = method.Invoke(gameRes, new object[] { key, 0f });
                return value == null ? 0d : Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0d;
            }
        }

        private static string ExpressionRaw(object expression)
        {
            if (expression == null) return "";

            MethodInfo method = expression.GetType().GetMethods(Inst)
                .FirstOrDefault(m => m.Name == "GetRawExpressionString" && m.GetParameters().Length == 0);

            if (method == null) return "";

            object value = method.Invoke(expression, null);
            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
        }

        private static double ToDouble(object value)
        {
            if (value == null) return 0d;
            try { return Convert.ToDouble(value, CultureInfo.InvariantCulture); }
            catch { return 0d; }
        }

        private static bool ToBool(object value)
        {
            if (value == null) return false;
            try { return Convert.ToBoolean(value, CultureInfo.InvariantCulture); }
            catch { return false; }
        }

        private static string BoolString(object value)
        {
            return ToBool(value) ? "true" : "false";
        }

        private static string Format(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("\\", "\\\\").Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }
    }
}
