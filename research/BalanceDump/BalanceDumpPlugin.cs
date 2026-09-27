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
        public const string PluginName = "BGCR Material Dump";
        public const string PluginVersion = "0.2.0";

        private static readonly BindingFlags Inst = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly BindingFlags Stat = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private static readonly HashSet<string> GraveTypes = new HashSet<string>(StringComparer.Ordinal)
        {
            "GraveStone",
            "GraveFence",
            "GraveCover"
        };
        private static readonly HashSet<string> NonMaterialIds = new HashSet<string>(StringComparer.Ordinal)
        {
            "r", "g", "b", "v", "gratitude_points", "energy", "hp", "progress", "money", "durability"
        };

        private Assembly _gameAssembly;
        private bool _dumped;

        private void Awake()
        {
            Logger.LogInfo("BGCR Material Dump 0.2.0 loaded. Read-only research probe.");
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
                            DumpMaterialGraph(balance);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("BGCR_MATERIAL_ERROR|" + Escape(ex.ToString()));
                        }

                        _dumped = true;
                        yield break;
                    }
                }

                yield return null;
            }
        }

        private void DumpMaterialGraph(object balance)
        {
            IList items = Get(balance, "items_data") as IList;
            IList crafts = Get(balance, "craft_data") as IList;
            IList techs = Get(balance, "techs_data") as IList;

            if (items == null || crafts == null || techs == null)
                throw new InvalidOperationException("GameBalance items_data/craft_data/techs_data not available.");

            Dictionary<string, object> itemById = new Dictionary<string, object>(StringComparer.Ordinal);
            HashSet<string> graveIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (object itemDef in items)
            {
                if (itemDef == null) continue;
                string id = Id(itemDef);
                if (string.IsNullOrEmpty(id)) continue;
                if (!itemById.ContainsKey(id)) itemById[id] = itemDef;

                string type = Convert.ToString(Get(itemDef, "type"), CultureInfo.InvariantCulture);
                if (GraveTypes.Contains(type)) graveIds.Add(id);
            }

            Dictionary<string, object> craftById = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (object craft in crafts)
            {
                if (craft == null) continue;
                string id = Id(craft);
                if (!string.IsNullOrEmpty(id) && !craftById.ContainsKey(id)) craftById[id] = craft;
            }

            Queue<KeyValuePair<string, int>> queue = new Queue<KeyValuePair<string, int>>();
            Dictionary<string, int> depthByMaterial = new Dictionary<string, int>(StringComparer.Ordinal);

            foreach (string graveId in graveIds.OrderBy(x => x, StringComparer.Ordinal))
            {
                object primary;
                if (!craftById.TryGetValue(graveId, out primary)) continue;

                IList needs = Get(primary, "needs") as IList;
                if (needs == null) continue;

                foreach (object need in needs)
                {
                    string materialId = Id(need);
                    if (!IsMaterialCandidate(materialId, graveIds)) continue;
                    EnqueueIfNew(materialId, 0, depthByMaterial, queue);
                }
            }

            Logger.LogInfo(
                "BGCR_MATERIAL_BEGIN|probe=0.2.0|target=GraveyardKeeper-1.407|source=loaded-GameBalance" +
                "|direct_materials=" + depthByMaterial.Count.ToString(CultureInfo.InvariantCulture));

            int materialCount = 0;
            int producerCount = 0;
            int rootCount = 0;
            int maxDepth = 0;

            while (queue.Count > 0)
            {
                KeyValuePair<string, int> current = queue.Dequeue();
                string materialId = current.Key;
                int depth = current.Value;
                materialCount++;
                if (depth > maxDepth) maxDepth = depth;

                object itemDef;
                itemById.TryGetValue(materialId, out itemDef);

                Logger.LogInfo(
                    "BGCR_MATERIAL" +
                    "|id=" + Escape(materialId) +
                    "|depth=" + depth.ToString(CultureInfo.InvariantCulture) +
                    "|name=" + Escape(TryDisplayName(itemDef)) +
                    "|type=" + Escape(itemDef == null ? "" : Convert.ToString(Get(itemDef, "type"), CultureInfo.InvariantCulture)) +
                    "|quality=" + Escape(itemDef == null ? "" : Number(Get(itemDef, "quality"))));

                List<object> producers = new List<object>();
                foreach (object craft in crafts)
                {
                    if (craft == null) continue;
                    string craftId = Id(craft);
                    if (!IsUsefulProducerCraft(craftId, craft)) continue;
                    if (!ContainsItemId(Get(craft, "output") as IList, materialId)) continue;
                    producers.Add(craft);
                }

                if (producers.Count == 0)
                {
                    rootCount++;
                    Logger.LogInfo(
                        "BGCR_MATERIAL_ROOT" +
                        "|id=" + Escape(materialId) +
                        "|depth=" + depth.ToString(CultureInfo.InvariantCulture));
                    continue;
                }

                foreach (object craft in producers.OrderBy(c => Id(c), StringComparer.Ordinal))
                {
                    producerCount++;
                    string craftId = Id(craft);
                    IList outputs = Get(craft, "output") as IList;
                    IList needs = Get(craft, "needs") as IList;

                    Logger.LogInfo(
                        "BGCR_MATERIAL_CRAFT" +
                        "|target=" + Escape(materialId) +
                        "|depth=" + depth.ToString(CultureInfo.InvariantCulture) +
                        "|craft=" + Escape(craftId) +
                        "|craft_type=" + Escape(Convert.ToString(Get(craft, "craft_type"), CultureInfo.InvariantCulture)) +
                        "|craft_in=" + Escape(StringList(Get(craft, "craft_in") as IEnumerable)) +
                        "|needs=" + Escape(ItemList(needs)) +
                        "|outputs=" + Escape(ItemList(outputs)) +
                        "|red=" + Escape(Reward(outputs, "r")) +
                        "|green=" + Escape(Reward(outputs, "g")) +
                        "|blue=" + Escape(Reward(outputs, "b")) +
                        "|energy_expr=" + Escape(ExpressionRaw(Get(craft, "energy"))) +
                        "|energy_const=" + Escape(ExpressionConstant(Get(craft, "energy"))) +
                        "|time_expr=" + Escape(ExpressionRaw(Get(craft, "craft_time"))) +
                        "|time_const=" + Escape(ExpressionConstant(Get(craft, "craft_time"))) +
                        "|techs=" + Escape(TechIdsForCraft(techs, craftId)) +
                        "|tech_prices=" + Escape(TechPricesForCraft(techs, craftId)));

                    if (needs == null) continue;
                    foreach (object need in needs)
                    {
                        string nextId = Id(need);
                        if (!IsMaterialCandidate(nextId, graveIds)) continue;
                        EnqueueIfNew(nextId, depth + 1, depthByMaterial, queue);
                    }
                }
            }

            Logger.LogInfo(
                "BGCR_MATERIAL_DONE" +
                "|materials=" + materialCount.ToString(CultureInfo.InvariantCulture) +
                "|producer_crafts=" + producerCount.ToString(CultureInfo.InvariantCulture) +
                "|roots=" + rootCount.ToString(CultureInfo.InvariantCulture) +
                "|max_depth=" + maxDepth.ToString(CultureInfo.InvariantCulture));
        }

        private static bool IsMaterialCandidate(string id, HashSet<string> graveIds)
        {
            return !string.IsNullOrEmpty(id) &&
                   !NonMaterialIds.Contains(id) &&
                   !graveIds.Contains(id) &&
                   !id.StartsWith("story:", StringComparison.Ordinal);
        }

        private static bool IsUsefulProducerCraft(string craftId, object craft)
        {
            if (string.IsNullOrEmpty(craftId)) return false;
            if (craftId.StartsWith("surv:", StringComparison.Ordinal) ||
                craftId.StartsWith("set_", StringComparison.Ordinal) ||
                craftId.StartsWith("rem_", StringComparison.Ordinal) ||
                craftId.StartsWith("fix_", StringComparison.Ordinal) ||
                craftId.StartsWith("destroy_", StringComparison.Ordinal))
                return false;

            string craftType = Convert.ToString(Get(craft, "craft_type"), CultureInfo.InvariantCulture);
            return !string.Equals(craftType, "Fixing", StringComparison.Ordinal);
        }

        private static void EnqueueIfNew(
            string id,
            int depth,
            Dictionary<string, int> depthByMaterial,
            Queue<KeyValuePair<string, int>> queue)
        {
            int knownDepth;
            if (depthByMaterial.TryGetValue(id, out knownDepth) && knownDepth <= depth) return;

            depthByMaterial[id] = depth;
            queue.Enqueue(new KeyValuePair<string, int>(id, depth));
        }

        private static string TechIdsForCraft(IList techs, string craftId)
        {
            List<string> ids = new List<string>();
            foreach (object tech in techs)
            {
                if (tech == null) continue;
                if (!ContainsString(Get(tech, "crafts") as IList, craftId)) continue;
                ids.Add(Id(tech));
            }
            return string.Join(";", ids.ToArray());
        }

        private static string TechPricesForCraft(IList techs, string craftId)
        {
            List<string> prices = new List<string>();
            foreach (object tech in techs)
            {
                if (tech == null) continue;
                if (!ContainsString(Get(tech, "crafts") as IList, craftId)) continue;

                string id = Id(tech);
                object price = Get(tech, "price");
                prices.Add(id + ":" + (price == null ? "" : Convert.ToString(price, CultureInfo.InvariantCulture)));
            }
            return string.Join(";", prices.ToArray());
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

        private static bool ContainsItemId(IList list, string id)
        {
            if (list == null) return false;
            foreach (object item in list)
                if (string.Equals(Id(item), id, StringComparison.Ordinal)) return true;
            return false;
        }

        private static bool ContainsString(IList list, string value)
        {
            if (list == null) return false;
            foreach (object item in list)
                if (string.Equals(Convert.ToString(item, CultureInfo.InvariantCulture), value, StringComparison.Ordinal)) return true;
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
            if (itemDef == null) return "";

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
                return Convert.ToString(method.Invoke(itemDef, new object[] { true }), CultureInfo.InvariantCulture) ?? "";
            }
            catch
            {
                return "";
            }
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
