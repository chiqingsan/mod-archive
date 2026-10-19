using System;
using System.Reflection;
using UnityEngine;

namespace LootRarityMarker
{
    /// <summary>
    /// 与社区 mod 菜单框架 ModConfig（Steam 创意工坊 3590674339，displayName「ModConfig」，
    /// 描述「Easy configuration via ingame settings」，API v1）对接的最小反射桥。
    ///
    /// 为什么用反射：ModConfig 是另一个 mod（运行时才被 ModManager 用 Assembly.LoadFrom 载入），
    /// 编译期引用它会让本 mod 在没装 ModConfig 时直接加载失败。这里全部走反射查找类型，
    /// 所以：
    ///   * 装了 ModConfig → 设置项出现在 主菜单 → 设置 → Mod Settings 标签页里；
    ///   * 没装 → 所有方法返回 false / 默认值，本 mod 继续用 config.json 或内置默认值工作。
    ///
    /// 对接的契约（均来自 ModConfig 反编译结果，见 D:\code\mod\Escape from Duckov\decompiled\ModConfig\）：
    ///   ModConfig.ModBehaviour.VERSION                                       (static int, 必须 == 1)
    ///   ModConfig.ModBehaviour.AddInputWithSlider(string modName, string key, string description, Type valueType, object defaultValue, Vector2? sliderRange)
    ///   ModConfig.ModBehaviour.AddBoolDropdownList(string modName, string key, string description, bool defaultValue)
    ///   ModConfig.ModBehaviour.AddOnOptionsChangedDelegate(Action&lt;string&gt;) / RemoveOnOptionsChangedDelegate(Action&lt;string&gt;)
    ///   ModConfig.OptionsManager_Mod.Load&lt;T&gt;(string key, T defaultValue) / Save&lt;T&gt;(string key, T value)
    /// 存储：EasySave3 的 &lt;存档目录&gt;/ModConfig.ES3，键名为 modName + "_" + key。
    /// 配置项在 ModConfig 的菜单建好之前注册会自动进队列，所以什么时候注册都不会丢。
    /// </summary>
    internal static class ModConfigBridge
    {
        private const string ModName = "LootRarityMarker";
        private const string KeyPrefix = ModName + "_";
        private const int ApiVersion = 1;

        private static bool _initDone;
        private static bool _available;
        private static Type _modBehaviourType;
        private static Type _optionsManagerType;
        private static MethodInfo _addSlider;
        private static MethodInfo _addBool;
        private static MethodInfo _addChanged;
        private static MethodInfo _removeChanged;
        private static MethodInfo _load;

        /// <summary>键名前缀：ModConfig 的选项变更回调回传的是这个完整键。</summary>
        public static string ChangedKeyPrefix
        {
            get { return KeyPrefix; }
        }

        /// <summary>查找并校验 ModConfig 的 API。只做一次；之后直接返回缓存结果。</summary>
        public static bool TryInit()
        {
            if (_initDone)
            {
                return _available;
            }
            _initDone = true;
            try
            {
                _modBehaviourType = FindType("ModConfig.ModBehaviour");
                _optionsManagerType = FindType("ModConfig.OptionsManager_Mod");
                if (_modBehaviourType == null || _optionsManagerType == null)
                {
                    return false; // 没装 / 没启用 ModConfig，正常情况
                }

                const BindingFlags S = BindingFlags.Static | BindingFlags.Public;

                // 版本门：ModConfig 用 VERSION 字段声明 API 版本，不匹配就不要乱调
                FieldInfo versionField = _modBehaviourType.GetField("VERSION", S);
                if (versionField != null && versionField.FieldType == typeof(int))
                {
                    int modVersion = (int)versionField.GetValue(null);
                    if (modVersion != ApiVersion)
                    {
                        Debug.LogWarning("[LootRarityMarker] ModConfig 的 API 版本是 v" + modVersion +
                                         "，本 mod 对接的是 v" + ApiVersion + "，本次不注册设置项（回退到 config.json / 默认值）。");
                        return false;
                    }
                }

                _addSlider = _modBehaviourType.GetMethod("AddInputWithSlider", S);
                _addBool = _modBehaviourType.GetMethod("AddBoolDropdownList", S);
                _addChanged = _modBehaviourType.GetMethod("AddOnOptionsChangedDelegate", S);
                _removeChanged = _modBehaviourType.GetMethod("RemoveOnOptionsChangedDelegate", S);
                _load = _optionsManagerType.GetMethod("Load", S);

                if (_addSlider == null || _addBool == null || _addChanged == null || _load == null)
                {
                    Debug.LogWarning("[LootRarityMarker] ModConfig 缺少预期的方法，本次不注册设置项（回退到 config.json / 默认值）。");
                    return false;
                }

                _available = true;
                Debug.Log("[LootRarityMarker] 已接上 ModConfig：设置项在 主菜单 → 设置 → Mod Settings。");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[LootRarityMarker] 连接 ModConfig 失败（不影响染色功能）：" + e.Message);
                return false;
            }
        }

        /// <summary>注册一个带滑条的数字输入项（int）。</summary>
        public static bool AddIntSlider(string key, string description, int defaultValue, int min, int max)
        {
            if (!_available || _addSlider == null)
            {
                return false;
            }
            try
            {
                _addSlider.Invoke(null, new object[]
                {
                    ModName, Prefix(key), description, typeof(int), defaultValue, new Vector2(min, max)
                });
                return true;
            }
            catch (Exception e)
            {
                Warn("注册数字设置项", key, e);
                return false;
            }
        }

        /// <summary>注册一个 是/否 下拉项。</summary>
        public static bool AddBoolDropdown(string key, string description, bool defaultValue)
        {
            if (!_available || _addBool == null)
            {
                return false;
            }
            try
            {
                _addBool.Invoke(null, new object[] { ModName, Prefix(key), description, defaultValue });
                return true;
            }
            catch (Exception e)
            {
                Warn("注册开关设置项", key, e);
                return false;
            }
        }

        /// <summary>注册选项变更回调；参数是 ModConfig 落盘用的完整键（modName_key）。</summary>
        public static bool AddChangedHandler(Action<string> handler)
        {
            if (!_available || _addChanged == null || handler == null)
            {
                return false;
            }
            try
            {
                _addChanged.Invoke(null, new object[] { handler });
                return true;
            }
            catch (Exception e)
            {
                Warn("注册设置变更回调", "-", e);
                return false;
            }
        }

        public static void RemoveChangedHandler(Action<string> handler)
        {
            if (!_available || _removeChanged == null || handler == null)
            {
                return;
            }
            try
            {
                _removeChanged.Invoke(null, new object[] { handler });
            }
            catch (Exception)
            {
                // 退订失败无害（进程退出时一并消失），不打扰玩家
            }
        }

        /// <summary>读取由 ModConfig 保存的值；不可用或出错时返回 defaultValue。</summary>
        public static T Load<T>(string key, T defaultValue)
        {
            if (!_available || _load == null)
            {
                return defaultValue;
            }
            try
            {
                MethodInfo generic = _load.MakeGenericMethod(typeof(T));
                object value = generic.Invoke(null, new object[] { Prefix(key), defaultValue });
                return (value is T) ? (T)value : defaultValue;
            }
            catch (Exception e)
            {
                Warn("读取设置", key, e);
                return defaultValue;
            }
        }

        private static string Prefix(string key)
        {
            return KeyPrefix + key;
        }

        private static void Warn(string what, string key, Exception e)
        {
            Debug.LogWarning("[LootRarityMarker] " + what + "失败(" + key + ")：" + e.Message);
        }

        /// <summary>在所有已加载程序集里按全名找类型（ModConfig 的 dll 名固定，但不去猜程序集名）。</summary>
        private static Type FindType(string fullName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    Type type = assemblies[i].GetType(fullName, false);
                    if (type != null)
                    {
                        return type;
                    }
                }
                catch (Exception)
                {
                    // 个别程序集反射会抛，跳过
                }
            }
            return null;
        }
    }
}
