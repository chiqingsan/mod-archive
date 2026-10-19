using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace HeadshotBulletRefund
{
    /// <summary>
    /// 本 mod 的可配置数值。默认值 = 最初设计的行为（爆头返还 1 颗、不超弹匣容量）。
    ///
    /// 取值优先级（低 → 高）：
    ///   1. 这里的字段默认值
    ///   2. 与 dll 同目录的 config.json（没有 ModConfig 框架时也能配置）
    ///   3. ModConfig 框架的存档值（游戏内 Mod Settings 标签页里改过的值优先，见 ModConfigBridge）
    /// </summary>
    [Serializable]
    public class ModSettings
    {
        /// <summary>总开关。关掉后完全不返还（事件回调第一行就返回）。</summary>
        public bool enabled = true;

        /// <summary>只在爆头击杀时返还；关掉则任意击杀都返还。</summary>
        public bool headshotOnly = true;

        /// <summary>每次击杀返还几颗（1..MaxRefundCount）。</summary>
        public int refundCount = 1;

        /// <summary>是否绝不超过弹匣容量。关掉后可以装到容量之上（配合 refundCount 玩“越打越多”）。</summary>
        public bool respectMagazineCapacity = true;

        public const int MaxRefundCount = 10;

        public void Normalize()
        {
            if (refundCount < 1)
            {
                refundCount = 1;
            }
            else if (refundCount > MaxRefundCount)
            {
                refundCount = MaxRefundCount;
            }
        }

        /// <summary>
        /// 用 mod 目录下（与 dll 同级）的 config.json 覆盖默认值。
        /// 文件不存在或格式错误时静默使用代码里的默认值，不影响 mod 工作。
        /// </summary>
        public static ModSettings LoadFromModFolder()
        {
            ModSettings settings = new ModSettings();
            try
            {
                string path = Path.Combine(ModFolder, "config.json");
                if (File.Exists(path))
                {
                    // JsonUtility 由 UnityEngine.JSONSerializeModule 提供，游戏自带，无第三方依赖
                    JsonUtility.FromJsonOverwrite(File.ReadAllText(path, Encoding.UTF8), settings);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[HeadshotBulletRefund] 读取 config.json 失败，改用内置默认值：" + e.Message);
            }
            settings.Normalize();
            return settings;
        }

        private static string _modFolder;

        /// <summary>mod 自己的目录（dll 所在目录）。</summary>
        internal static string ModFolder
        {
            get
            {
                if (_modFolder == null)
                {
                    try
                    {
                        _modFolder = Path.GetDirectoryName(typeof(ModBehaviour).Assembly.Location);
                    }
                    catch (Exception)
                    {
                        _modFolder = null;
                    }
                    if (string.IsNullOrEmpty(_modFolder))
                    {
                        _modFolder = ".";
                    }
                }
                return _modFolder;
            }
        }
    }
}
