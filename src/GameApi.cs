using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace BetterGraveCraftingRewards
{
    internal static class GameApi
    {
        internal const BindingFlags Inst =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        internal const BindingFlags Stat =
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        internal static Type MainGameType { get; private set; }
        internal static Type GameBalanceType { get; private set; }
        internal static Type CraftComponentType { get; private set; }
        internal static Type ItemType { get; private set; }

        private static Assembly _gameAssembly;
        private static ConstructorInfo _itemConstructor;

        internal static void Bind()
        {
            _gameAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a =>
                    string.Equals(
                        a.GetName().Name,
                        "Assembly-CSharp",
                        StringComparison.Ordinal));

            if (_gameAssembly == null)
                throw new InvalidOperationException("Assembly-CSharp was not found.");

            MainGameType = RequireType("MainGame");
            GameBalanceType = RequireType("GameBalance");
            CraftComponentType = RequireType("CraftComponent");
            ItemType = RequireType("Item");

            _itemConstructor = ItemType.GetConstructor(
                Inst,
                null,
                new[] { typeof(string), typeof(int) },
                null);

            if (_itemConstructor == null)
                throw new MissingMethodException("Item(string,int)");
        }

        internal static Type RequireType(string name)
        {
            Type direct = _gameAssembly.GetType(name, false);
            if (direct != null)
                return direct;

            Type found;
            try
            {
                found = _gameAssembly.GetTypes()
                    .FirstOrDefault(t => t != null && t.Name == name);
            }
            catch (ReflectionTypeLoadException ex)
            {
                found = ex.Types
                    .FirstOrDefault(t => t != null && t.Name == name);
            }

            if (found == null)
                throw new TypeLoadException(name);

            return found;
        }

        internal static MethodInfo RequireMethod(
            Type type,
            string name,
            int parameterCount)
        {
            MethodInfo method = type.GetMethods(Inst)
                .FirstOrDefault(m =>
                    m.Name == name
                    && m.GetParameters().Length == parameterCount);

            if (method == null)
                throw new MissingMethodException(type.FullName, name);

            return method;
        }

        internal static object Get(object obj, string name)
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

        internal static void Set(object obj, string name, object value)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            for (Type t = obj.GetType(); t != null; t = t.BaseType)
            {
                FieldInfo field = t.GetField(name, Inst);
                if (field != null)
                {
                    field.SetValue(obj, value);
                    return;
                }

                PropertyInfo property = t.GetProperty(name, Inst);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(obj, value, null);
                    return;
                }
            }

            throw new MissingMemberException(obj.GetType().FullName, name);
        }

        internal static object GetStatic(Type type, string name)
        {
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

        internal static string Id(object obj)
        {
            return Get(obj, "id") as string ?? string.Empty;
        }

        internal static IList CraftData
        {
            get
            {
                object balance = GetStatic(GameBalanceType, "me");
                return Get(balance, "craft_data") as IList;
            }
        }

        internal static object Player
        {
            get
            {
                object mainGame = GetStatic(MainGameType, "me");
                return Get(mainGame, "player");
            }
        }

        internal static bool ReadBool(object obj, string name)
        {
            object value = Get(obj, name);
            return value != null && Convert.ToBoolean(value);
        }

        internal static bool IsPlayer(object wgo)
        {
            return ReadBool(wgo, "is_player");
        }

        internal static bool CompletionAwardsTechPointsToPlayer(
            object craftComponent,
            object craft,
            object other)
        {
            if (IsPlayer(other))
                return true;

            if (ReadBool(craft, "is_auto"))
                return true;

            object station = Get(craftComponent, "wgo");
            return ReadBool(station, "is_current_craft_gratitude");
        }

        internal static object NewItem(string id, int value)
        {
            return _itemConstructor.Invoke(new object[] { id, value });
        }

        internal static int PointTotal(IList output, string pointId)
        {
            if (output == null)
                return 0;

            int sum = 0;
            foreach (object item in output)
            {
                if (string.Equals(Id(item), pointId, StringComparison.Ordinal))
                    sum += Convert.ToInt32(Get(item, "value"));
            }

            return sum;
        }

        internal static bool OutputContains(IList output, string itemId)
        {
            if (output == null)
                return false;

            foreach (object item in output)
            {
                if (string.Equals(Id(item), itemId, StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        internal static void EnsurePointEntry(IList output, string pointId)
        {
            if (output == null)
                throw new InvalidOperationException("Craft output is null.");

            foreach (object item in output)
            {
                if (string.Equals(Id(item), pointId, StringComparison.Ordinal))
                    return;
            }

            output.Add(NewItem(pointId, 0));
        }

        internal static void SetPointTotal(
            IList output,
            string pointId,
            int target)
        {
            EnsurePointEntry(output, pointId);

            object first = null;
            foreach (object item in output)
            {
                if (!string.Equals(Id(item), pointId, StringComparison.Ordinal))
                    continue;

                if (first == null)
                {
                    first = item;
                    Set(item, "value", target);
                }
                else
                {
                    Set(item, "value", 0);
                }
            }
        }

        internal static void IncreasePointTotalPreservingShape(
            IList output,
            string pointId,
            int expectedCurrent,
            int target)
        {
            int actual = PointTotal(output, pointId);
            if (actual != expectedCurrent)
                throw new InvalidOperationException(
                    "Expected " + expectedCurrent + " " + pointId
                    + " but found " + actual + ".");

            int delta = target - actual;
            if (delta < 0)
                throw new InvalidOperationException(
                    "Study target would decrease " + pointId + ".");

            if (delta == 0)
                return;

            object recipient = null;
            int best = int.MinValue;

            foreach (object item in output)
            {
                if (!string.Equals(Id(item), pointId, StringComparison.Ordinal))
                    continue;

                int value = Convert.ToInt32(Get(item, "value"));
                if (recipient == null || value > best)
                {
                    recipient = item;
                    best = value;
                }
            }

            if (recipient == null)
            {
                output.Add(NewItem(pointId, target));
                return;
            }

            Set(recipient, "value", best + delta);
        }

        internal static int GetPlayerInt(string name, int fallback)
        {
            object player = Player;
            if (player == null)
                return fallback;

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

            float value = Convert.ToSingle(
                method.Invoke(player, new object[] { name, (float)fallback }));

            return (int)Math.Round(value);
        }

        internal static void SetPlayerInt(string name, int value)
        {
            object player = Player;
            if (player == null)
                throw new InvalidOperationException("MainGame player is unavailable.");

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

            method.Invoke(player, new object[] { name, (float)value });
        }
    }
}
