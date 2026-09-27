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
        public const string PluginName = "BGCR Balance Dump";
        public const string PluginVersion = "0.1.0";

        private static readonly BindingFlags Inst = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly BindingFlags Stat = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly HashSet<string> GraveTypes = new HashSet<string>(StringComparer.Ordinal)
        {
            "GraveStone",
            "GraveFence",
            "GraveCover"
        };

        private Assembly _gameAssembly;
        private bool _dumped;

        private void Awake()
        {
            Logger.LogInfo("BGCR Balance Dump 0.1.0 loaded. Read-only research probe.");
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

                    bool gameStarted = ToBool(GetStatic(mainGameType, "game_started"));
                    object balance = GetStatic(balanceType, "me");

                    if (gameStarted && balance != null)
                    {
                        try
                        {
                            Dump(balance);
                            _dumped = true;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("BGCR_DATA_ERROR|" + Escape(ex.ToString()));
                            _dumped = true;
                        }

                        yield break;
                    }
                }

                yield return null;
            }
        }

        private void Dump(object balance)
        {
            IList items = Get(balance, "items_data") as IList;
            IList crafts = Get(balance, "craft_data") as IList;
            IList techs = Get(balance, "techs_data") as IList;

            if (items == null || crafts == null || techs == null)
            {
                throw new InvalidOperationException("GameBalance items_data/craft_data/techs_data not available.");
            }

            Logger.LogInfo("BGCR_DATA_BEGIN|probe=0.1.0|target=GraveyardKeeper-1.407|source=loaded-GameBalance");
            Logger.LogInfo("BGCR_DATA_SOURCE|decompile_reference=Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9");

            int decorationCount = 0;
            int producerCount = 0;
            int consumerCount = 0;
            int studyCount = 0;

            foreach (object itemDef in items)
            {
                if (itemDef == null) continue;

                string type = Convert.ToString(Get(itemDef, "type"), CultureInfo.InvariantCulture);
                if (!GraveTypes.Contains(type)) continue;

                string itemId = Id(itemDef);
                if (string.IsNullOrEmpty(itemId)) continue;

                decorationCount++;

                string displayName = TryDisplayName(itemDef);
                string quality = Number(Get(itemDef, "quality"));

                Logger.LogInfo(
                    "BGCR_ITEM" +
                    "|id=" + Escape(itemId) +
                    "|type=" + Escape(type) +
                    "|name=" + Escape(displayName) +
                    "|quality=" + Escape(quality));

                object survey = CallNoArgs(itemDef, "GetSurveyCraft");
                if (survey != null)
                {
                    studyCount++;
                    Logger.LogInfo(CraftLine("BGCR_STUDY", itemId, survey, null));
                }

                foreach (object craft in crafts)
                {
                    if (craft == null) continue;
                    string craftId = Id(craft);
                    if (string.IsNullOrEmpty(craftId)) continue;

                    bool produces = ContainsItemId(Get(craft, "output") as IList, itemId);
                    bool consumes = ContainsItemId(Get(craft, "needs") as IList, itemId);

                    if (produces && !craftId.StartsWith("surv:", StringComparison.Ordinal))
                    {
                        producerCount++;
                        Logger.LogInfo(CraftLine("BGCR_PRODUCER", itemId, craft, techs));
                    }

                    if (consumes && !craftId.StartsWith("surv:", StringComparison.Ordinal))
                    {
                        consumerCount++;
                        Logger.LogInfo(CraftLine("BGCR_CONSUMER", itemId, craft, techs));
                    }
                }
            }

            Logger.LogInfo(
                "BGCR_DATA_DONE" +
                "|decorations=" + decorationCount.ToString(CultureInfo.InvariantCulture) +
                "|producers=" + producerCount.ToString(CultureInfo.InvariantCulture) +
                "|consumers=" + consumerCount.ToString(CultureInfo.InvariantCulture) +
                "|studies=" + studyCount.ToString(CultureInfo.InvariantCulture));
        }

        private string CraftLine(string kind, string itemId, object craft, IList techs)
        {
            string craftId = Id(craft);
            IList outputs = Get(craft, "output") as IList;

            string techIds = "";
            string techPrices = "";
            if (techs != null)
            {
                List<string> ids = new List<string>();
                List<string> prices = new List<string>();

                foreach (object tech in techs)
                {
                    if (tech == null) continue;
                    IList unlockedCrafts = Get(tech, "crafts") as IList;
                    if (!ContainsString(unlockedCrafts, craftId)) continue;

                    string techId = Id(tech);
                    ids.Add(techId);

                    object price = Get(tech, "price");
                    prices.Add(techId + ":" + (price == null ? "" : Convert.ToString(price, CultureInfo.InvariantCulture)));
                }

                techIds = JoinEscaped(ids);
                techPrices = JoinEscaped(prices);
            }

            return kind +
                "|item=" + Escape(itemId) +
                "|craft=" + Escape(craftId) +
                "|craft_type=" + Escape(Convert.ToString(Get(craft, "craft_type"), CultureInfo.InvariantCulture)) +
                "|craft_in=" + Escape(StringList(Get(craft, "craft_in") as IEnumerable)) +
                "|needs=" + Escape(ItemList(Get(craft, "needs") as IList)) +
                "|outputs=" + Escape(ItemList(outputs)) +
                "|red=" + Escape(Reward(outputs, "r")) +
                "|green=" + Escape(Reward(outputs, "g")) +
                "|blue=" + Escape(Reward(outputs, "b")) +
                "|energy_expr=" + Escape(ExpressionRaw(Get(craft, "energy")) ) +
                "|energy_const=" + Escape(ExpressionConstant(Get(craft, "energy"))) +
                "|time_expr=" + Escape(ExpressionRaw(Get(craft, "craft_time"))) +
                "|time_const=" + Escape(ExpressionConstant(Get(craft, "craft_time"))) +
                "|hidden=" + Escape(BoolString(Get(craft, "hidden"))) +
                "|one_time=" + Escape(BoolString(Get(craft, "one_time_craft"))) +
                "|transfer_needs_to_wgo=" + Escape(BoolString(Get(craft, "transfer_needs_to_wgo"))) +
                "|set_out_wgo_params_on_start=" + Escape(BoolString(Get(craft, "set_out_wgo_params_on_start"))) +
                "|techs=" + Escape(techIds) +
                "|tech_prices=" + Escape(techPrices);
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
            for (Type t = type; t != null; t = t.BaseType)
            {
                FieldInfo field = t.GetField(name, Stat);
                if (field != null) return field.GetValue(null);

                PropertyInfo property = t.GetProperty(name, Stat);
                if (property != null && property.CanRead) return property.GetValue(null, null);
            }

            return null;
        }

        private static object CallNoArgs(object obj, string name)
        {
            if (obj == null) return null;
            MethodInfo method = obj.GetType().GetMethods(Inst)
                .FirstOrDefault(m => m.Name == name && m.GetParameters().Length == 0);
            return method == null ? null : method.Invoke(obj, null);
        }

        private static string Id(object obj)
        {
            return Get(obj, "id") as string ?? "";
        }

        private static bool ContainsItemId(IList list, string id)
        {
            if (list == null) return false;

            foreach (object item in list)
            {
                if (string.Equals(Id(item), id, StringComparison.Ordinal)) return true;
            }

            return false;
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

            return sum.ToString("0.###", CultureInfo.InvariantCulture);
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

        private static string JoinEscaped(List<string> values)
        {
            return values == null ? "" : string.Join(";", values.ToArray());
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

        private static string ExpressionConstant(object expression)
        {
            string raw = ExpressionRaw(expression);
            if (string.IsNullOrWhiteSpace(raw)) return "";

            float value;
            return float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                ? value.ToString("0.###", CultureInfo.InvariantCulture)
                : "";
        }

        private static string TryDisplayName(object itemDef)
        {
            try
            {
                MethodInfo method = itemDef.GetType().GetMethods(Inst)
                    .FirstOrDefault(m =>
                    {
                        if (m.Name != "GetItemName") return false;
                        ParameterInfo[] p = m.GetParameters();
                        return p.Length == 1 && p[0].ParameterType == typeof(bool);
                    });

                if (method == null) return "";
                return Convert.ToString(method.Invoke(itemDef, new object[] { false }), CultureInfo.InvariantCulture) ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static string BoolString(object value)
        {
            return ToBool(value) ? "true" : "false";
        }

        private static bool ToBool(object value)
        {
            if (value == null) return false;
            try { return Convert.ToBoolean(value, CultureInfo.InvariantCulture); }
            catch { return false; }
        }

        private static double ToDouble(object value)
        {
            if (value == null) return 0d;
            try { return Convert.ToDouble(value, CultureInfo.InvariantCulture); }
            catch { return 0d; }
        }

        private static string Number(object value)
        {
            return ToDouble(value).ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("\\", "\\\\").Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }
    }
}
