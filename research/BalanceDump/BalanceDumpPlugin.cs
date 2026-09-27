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
        public const string PluginName = "BGCR Scope Completion Dump";
        public const string PluginVersion = "0.3.0";

        private static readonly BindingFlags Inst = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly BindingFlags Stat = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly HashSet<string> RelevantOutObjects = new HashSet<string>(StringComparer.Ordinal)
        {
            "cooking_table",
            "cooking_table_2",
            "desk",
            "desk_2",
            "mf_anvil_1",
            "mf_anvil_2",
            "mf_anvil_3",
            "mf_beam_gantry_1",
            "mf_chocks_1",
            "mf_furnace_0",
            "mf_furnace_1",
            "mf_furnace_2",
            "mf_hammer_0",
            "mf_hammer_1",
            "mf_jewelry",
            "mf_potter_wheel_1",
            "mf_saw_1",
            "mf_vine_press",
            "mf_workbench_1",
            "mf_workbench_2",
            "mine_zombie_bench",
            "steep_marble_2",
            "steep_stone",
            "tavern_kitchen",
            "zombie_mine_fence_front",
            "zombie_mine_fence_left_front",
            "zombie_sawmill_completed"
        };

        private Assembly _gameAssembly;
        private bool _dumped;

        private void Awake()
        {
            Logger.LogInfo("BGCR Scope Completion Dump 0.3.0 loaded. Read-only research probe.");
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
                                Logger.LogError("BGCR_SCOPE_ERROR|" + Escape(ex.ToString()));
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
            IList objectCrafts = Get(balance, "craft_obj_data") as IList;
            IList techs = Get(balance, "techs_data") as IList;

            if (crafts == null || objectCrafts == null || techs == null)
                throw new InvalidOperationException("GameBalance craft_data/craft_obj_data/techs_data not available.");

            Logger.LogInfo("BGCR_SCOPE_BEGIN|probe=0.3.0|target=GraveyardKeeper-1.407|source=loaded-GameBalance");

            int mqCount = DumpCarvedMarble(crafts, techs);
            int techCount = DumpTechs(techs);
            int blueprintCount = DumpRelevantBlueprints(objectCrafts, techs);

            Logger.LogInfo(
                "BGCR_SCOPE_DONE" +
                "|multiquality_crafts=" + mqCount.ToString(CultureInfo.InvariantCulture) +
                "|techs=" + techCount.ToString(CultureInfo.InvariantCulture) +
                "|blueprints=" + blueprintCount.ToString(CultureInfo.InvariantCulture));
        }

        private int DumpCarvedMarble(IList crafts, IList techs)
        {
            int count = 0;

            foreach (object craft in crafts)
            {
                if (craft == null) continue;
                IList outputs = Get(craft, "output") as IList;
                if (outputs == null) continue;

                foreach (object output in outputs)
                {
                    if (output == null) continue;
                    IList variants = Get(output, "multiquality_items") as IList;
                    if (variants == null || variants.Count == 0) continue;

                    List<string> ids = new List<string>();
                    bool carvedMarble = false;
                    foreach (object variant in variants)
                    {
                        string id = Convert.ToString(variant, CultureInfo.InvariantCulture) ?? "";
                        ids.Add(id);
                        if (id.StartsWith("marble_plate_3:", StringComparison.Ordinal))
                            carvedMarble = true;
                    }

                    if (!carvedMarble) continue;

                    count++;
                    string craftId = Id(craft);

                    Logger.LogInfo(
                        "BGCR_MQ_CRAFT" +
                        "|craft=" + Escape(craftId) +
                        "|out_base=" + Escape(Id(output)) +
                        "|variants=" + Escape(string.Join(";", ids.ToArray())) +
                        "|needs=" + Escape(ItemList(Get(craft, "needs") as IList)) +
                        "|outputs=" + Escape(ItemList(outputs)) +
                        "|red=" + Escape(Reward(outputs, "r")) +
                        "|green=" + Escape(Reward(outputs, "g")) +
                        "|blue=" + Escape(Reward(outputs, "b")) +
                        "|energy_expr=" + Escape(ExpressionRaw(Get(craft, "energy"))) +
                        "|time_expr=" + Escape(ExpressionRaw(Get(craft, "craft_time"))) +
                        "|difficulty=" + Escape(Number(Get(craft, "difficulty"))) +
                        "|linked_perks=" + Escape(StringList(Get(craft, "linked_perks") as IEnumerable)) +
                        "|linked_buffs=" + Escape(StringList(Get(craft, "linked_buffs") as IEnumerable)) +
                        "|craft_in=" + Escape(StringList(Get(craft, "craft_in") as IEnumerable)) +
                        "|techs=" + Escape(TechIdsForCraft(techs, craftId)) +
                        "|tech_prices=" + Escape(TechPricesForCraft(techs, craftId)));
                }
            }

            return count;
        }

        private int DumpTechs(IList techs)
        {
            int count = 0;
            double totalR = 0d, totalG = 0d, totalB = 0d, totalV = 0d, totalGP = 0d;

            foreach (object tech in techs)
            {
                if (tech == null) continue;
                count++;

                object price = Get(tech, "price");
                double r = GameResGet(price, "r");
                double g = GameResGet(price, "g");
                double b = GameResGet(price, "b");
                double v = GameResGet(price, "v");
                double gp = GameResGet(price, "gratitude_points");

                totalR += r;
                totalG += g;
                totalB += b;
                totalV += v;
                totalGP += gp;

                Logger.LogInfo(
                    "BGCR_TECH" +
                    "|id=" + Escape(Id(tech)) +
                    "|branch=" + Escape(Convert.ToString(Get(tech, "branch_type"), CultureInfo.InvariantCulture)) +
                    "|r=" + Escape(Format(r)) +
                    "|g=" + Escape(Format(g)) +
                    "|b=" + Escape(Format(b)) +
                    "|v=" + Escape(Format(v)) +
                    "|gp=" + Escape(Format(gp)) +
                    "|parents=" + Escape(ObjectIdList(Get(tech, "parents") as IEnumerable)) +
                    "|crafts=" + Escape(StringList(Get(tech, "crafts") as IEnumerable)) +
                    "|works=" + Escape(StringList(Get(tech, "works") as IEnumerable)) +
                    "|requires_dlc=" + Escape(Convert.ToString(Get(tech, "requires_dlc"), CultureInfo.InvariantCulture)) +
                    "|hidden=" + Escape(BoolString(Get(tech, "hidden"))) +
                    "|invisible=" + Escape(BoolString(Get(tech, "invisible"))));
            }

            Logger.LogInfo(
                "BGCR_TECH_TOTALS" +
                "|count=" + count.ToString(CultureInfo.InvariantCulture) +
                "|r=" + Format(totalR) +
                "|g=" + Format(totalG) +
                "|b=" + Format(totalB) +
                "|v=" + Format(totalV) +
                "|gp=" + Format(totalGP));

            return count;
        }

        private int DumpRelevantBlueprints(IList objectCrafts, IList techs)
        {
            int count = 0;

            foreach (object craft in objectCrafts)
            {
                if (craft == null) continue;

                string outObj = Convert.ToString(Get(craft, "out_obj"), CultureInfo.InvariantCulture) ?? "";
                if (!RelevantOutObjects.Contains(outObj)) continue;

                count++;
                string craftId = Id(craft);

                Logger.LogInfo(
                    "BGCR_BLUEPRINT" +
                    "|craft=" + Escape(craftId) +
                    "|out_obj=" + Escape(outObj) +
                    "|build_type=" + Escape(Convert.ToString(Get(craft, "build_type"), CultureInfo.InvariantCulture)) +
                    "|needs=" + Escape(ItemList(Get(craft, "needs") as IList)) +
                    "|builders=" + Escape(StringList(Get(craft, "builder_ids") as IEnumerable)) +
                    "|locked_builders=" + Escape(StringList(Get(craft, "locked_builders_ids") as IEnumerable)) +
                    "|techs=" + Escape(TechIdsForCraft(techs, craftId)) +
                    "|work_techs=" + Escape(TechIdsForWork(techs, outObj)));
            }

            return count;
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

        private static string TechPricesForCraft(IList techs, string craftId)
        {
            List<string> values = new List<string>();
            foreach (object tech in techs)
            {
                if (tech == null) continue;
                if (!ContainsString(Get(tech, "crafts") as IList, craftId)) continue;

                object price = Get(tech, "price");
                values.Add(
                    Id(tech) + ":" +
                    Format(GameResGet(price, "r")) + "/" +
                    Format(GameResGet(price, "g")) + "/" +
                    Format(GameResGet(price, "b")) + "/" +
                    Format(GameResGet(price, "v")) + "/" +
                    Format(GameResGet(price, "gratitude_points")));
            }
            return string.Join(";", values.ToArray());
        }

        private static string TechIdsForWork(IList techs, string workId)
        {
            List<string> ids = new List<string>();
            foreach (object tech in techs)
            {
                if (tech == null) continue;
                if (ContainsString(Get(tech, "works") as IList, workId))
                    ids.Add(Id(tech));
            }
            return string.Join(";", ids.ToArray());
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

        private static string Id(object obj)
        {
            return Get(obj, "id") as string ?? "";
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

        private static string Reward(IList output, string id)
        {
            if (output == null) return "0";

            double sum = 0d;
            foreach (object item in output)
            {
                if (!string.Equals(Id(item), id, StringComparison.Ordinal)) continue;
                sum += ToDouble(Get(item, "value"));
            }

            return Format(sum);
        }

        private static string ItemList(IList list)
        {
            if (list == null) return "";

            List<string> values = new List<string>();
            foreach (object item in list)
            {
                if (item == null) continue;
                values.Add(Id(item) + ":" + Number(Get(item, "value")));
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

        private static string ObjectIdList(IEnumerable values)
        {
            if (values == null) return "";

            List<string> result = new List<string>();
            foreach (object value in values)
            {
                string id = Id(value);
                if (!string.IsNullOrEmpty(id))
                    result.Add(id);
            }

            return string.Join(";", result.ToArray());
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

        private static string Number(object value)
        {
            return Format(ToDouble(value));
        }

        private static string Format(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
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

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("\\", "\\\\").Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }
    }
}
