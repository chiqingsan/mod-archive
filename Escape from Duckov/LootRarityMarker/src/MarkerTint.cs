using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LootRarityMarker
{
    /// <summary>
    /// 一个容器交互点（InteractMarker 实例）的染色单元。
    ///
    /// 做法：**不换材质、不换 shader、不 new Material**，只用 MaterialPropertyBlock 把
    /// 交互点材质里「本来就承载颜色」的那几个属性覆盖成本 mod 的颜色。
    /// 这么做的三个理由：
    ///   1. 零资源创建 —— 不产生新的 Material / Texture，不破坏 SRP Batcher 的材质批次；
    ///   2. 零污染 —— MPB 是 per-renderer 的覆盖层，写它不会改到共享材质，
    ///      别的容器、别的 mod、游戏本体都看不到任何变化；
    ///   3. 可完全还原 —— 清空 MPB 就等于回到游戏原样（见 Clear()）。
    ///
    /// 哪些属性算「承载颜色」：见 BuildProfile()。默认候选是两个从发行资源里挖出来的属性
    /// （_Tint / _Tint2），它们恰好是 InteractMarker 五个材质里唯一“颜色不同则外观不同”的属性。
    ///
    /// 关键细节 —— **HDR 亮度必须保留，而且可以再乘一个倍率**：
    /// 原版 _Tint 的值是超过 1 的（圆环 3.44、光点最高 3.0），那不是笔误，是 HDR 发光强度。
    /// 如果直接把 LDR 的红色 (1,0.18,0.18) 写进去，发光会被压暗一大截，看起来像“坏掉的贴图”。
    /// 所以这里是「只换色相、保留原亮度」：按原色 RGB 的最大分量等比缩放本 mod 的颜色。
    /// 白品质容器因此和原版几乎一模一样，红品质容器则是又亮又红。
    ///
    /// 在此基础上，Apply() 还接受一个 intensity 倍率（设置里的「发光强度」），
    /// 它直接乘在 RGB 亮度上 —— 因为交互点着色器是 Unlit + 半透明混合、写进 HDR 颜色缓冲，
    /// 所以这个倍率就是屏幕上的实际亮度，不需要动材质、不需要发光的后处理配合。
    ///
    /// 为什么倍率只乘 RGB、不乘 alpha：
    /// 通道混合是 SrcAlpha / OneMinusSrcAlpha，乘 alpha 也能变暗，但那是「更透」而不是「更暗」——
    /// 圆环会透出后面的景物，看起来像掉帧而不是像调暗。乘 RGB 才是干净的亮度调节。
    /// </summary>
    internal sealed class MarkerTint
    {
        /// <summary>RGB 最大分量低于它就认为这个属性是「黑的、没在用」，跳过不染。</summary>
        private const float BlackEpsilon = 0.02f;

        /// <summary>默认染色属性：从发行资源里验证过的两个真正承载交互点颜色的属性。</summary>
        public static readonly string[] DefaultColorProperties = { "_Tint", "_Tint2" };

        /// <summary>空 MPB，专门用来把渲染器还原成「没有任何覆盖」。静态共享，永不写入。</summary>
        private static readonly MaterialPropertyBlock EmptyBlock = new MaterialPropertyBlock();

        /// <summary>一个属性在某个渲染器上的覆盖配方：写到哪个属性、按多大亮度缩放、alpha 用多少。</summary>
        private struct Slot
        {
            public int Id;
            public float Magnitude;
            public float Alpha;
        }

        private static readonly Slot[] NoSlots = new Slot[0];

        private readonly InteractMarker _marker;
        private readonly Renderer[] _renderers;
        private readonly Slot[][] _slots;
        private readonly MaterialPropertyBlock _block = new MaterialPropertyBlock();
        private readonly bool _hasAnySlot;

        /// <summary>是否已经往这个交互点写过覆盖（还原时才知道要不要清）。</summary>
        public bool Touched { get; private set; }

        public MarkerTint(InteractMarker marker, string[] colorProperties)
        {
            _marker = marker;
            _renderers = marker.GetComponentsInChildren<Renderer>(true);
            _slots = new Slot[_renderers.Length][];

            string[] names = (colorProperties != null && colorProperties.Length > 0)
                ? colorProperties
                : DefaultColorProperties;

            // 属性名 → ID 只算一次，之后 HasProperty/GetColor/SetColor 全走 int 重载，不重复哈希字符串
            int[] ids = new int[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                ids[i] = Shader.PropertyToID(names[i]);
            }

            bool hasAny = false;
            for (int r = 0; r < _renderers.Length; r++)
            {
                _slots[r] = BuildRendererSlots(_renderers[r], ids);
                if (_slots[r].Length > 0)
                {
                    hasAny = true;
                }
            }
            _hasAnySlot = hasAny;
        }

        /// <summary>交互点还在不在（对象被销毁后 Unity 的 == null 会返回 true）。</summary>
        public bool IsAlive
        {
            get { return _marker != null; }
        }

        /// <summary>这个交互点上有没有可染的属性。false = 染色不会有效果，需要看诊断日志。</summary>
        public bool HasAnySlot
        {
            get { return _hasAnySlot; }
        }

        /// <summary>
        /// 按「保留原亮度、只换色相」把 color 应用到该交互点的所有渲染器上，
        /// 再把亮度整体乘上 intensity（1.0 = 游戏原样）。
        /// 只在判定结果变化时调用，不是每帧。
        /// </summary>
        public void Apply(Color color, float intensity)
        {
            if (!_hasAnySlot)
            {
                return;
            }

            for (int r = 0; r < _renderers.Length; r++)
            {
                Slot[] slots = _slots[r];
                if (slots.Length == 0)
                {
                    continue;
                }
                Renderer renderer = _renderers[r];
                if (renderer == null)
                {
                    continue;
                }

                _block.Clear(); // MPB 是复用的：先清掉上一轮的属性，只留这一次要写的
                for (int i = 0; i < slots.Length; i++)
                {
                    _block.SetColor(slots[i].Id, Rescale(color, slots[i].Magnitude, slots[i].Alpha, intensity));
                }
                renderer.SetPropertyBlock(_block);
            }
            Touched = true;
        }

        /// <summary>把交互点还原成游戏原样（清掉所有 MPB 覆盖）。</summary>
        public void Clear()
        {
            if (!Touched)
            {
                return;
            }
            Touched = false;
            for (int r = 0; r < _renderers.Length; r++)
            {
                Renderer renderer = _renderers[r];
                if (renderer == null)
                {
                    continue;
                }
                // 传一个空的 MPB = 该渲染器不再有任何属性覆盖，回到共享材质的值
                renderer.SetPropertyBlock(EmptyBlock);
            }
        }

        /// <summary>诊断用：把这个交互点上每个渲染器的材质与候选属性值列出来。</summary>
        public string Describe(string[] candidateNames)
        {
            string[] names = (candidateNames != null && candidateNames.Length > 0)
                ? candidateNames
                : DefaultColorProperties;

            StringBuilder sb = new StringBuilder();
            sb.Append("marker=").Append(_marker != null ? _marker.name : "<null>")
              .Append(" renderers=").Append(_renderers.Length)
              .Append(" tintable=").Append(_hasAnySlot);
            for (int r = 0; r < _renderers.Length; r++)
            {
                Renderer renderer = _renderers[r];
                if (renderer == null)
                {
                    continue;
                }
                sb.Append("\n    [").Append(r).Append("] ").Append(renderer.name)
                  .Append(" enabled=").Append(renderer.enabled)
                  .Append(" slots=").Append(_slots[r].Length);
                Material[] mats = renderer.sharedMaterials;
                if (mats == null)
                {
                    continue;
                }
                for (int m = 0; m < mats.Length; m++)
                {
                    Material mat = mats[m];
                    if (mat == null)
                    {
                        continue;
                    }
                    sb.Append("\n        mat ").Append(mat.name)
                      .Append(" shader=").Append(mat.shader != null ? mat.shader.name : "<null>");
                    for (int n = 0; n < names.Length; n++)
                    {
                        int id = Shader.PropertyToID(names[n]);
                        if (!mat.HasProperty(id))
                        {
                            sb.Append("\n          ").Append(names[n]).Append(" = <无此属性>");
                            continue;
                        }
                        Color c = mat.GetColor(id);
                        sb.Append("\n          ").Append(names[n]).Append(" = ")
                          .Append(c.r.ToString("F3")).Append(", ").Append(c.g.ToString("F3")).Append(", ")
                          .Append(c.b.ToString("F3")).Append(", ").Append(c.a.ToString("F3"));
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 找出这个渲染器上「承载颜色」的属性，并记下原来的亮度/透明度配方。
        /// 判定规则：属性存在、且原色不是黑的（黑色 = 该材质这个属性没在用，
        /// 比如 InteractMarkerShadow 的 _Tint 就是纯黑，染它只会把阴影染花）。
        /// </summary>
        private static Slot[] BuildRendererSlots(Renderer renderer, int[] ids)
        {
            if (renderer == null)
            {
                return NoSlots;
            }
            Material[] mats = renderer.sharedMaterials;
            if (mats == null || mats.Length == 0)
            {
                return NoSlots;
            }

            List<Slot> list = null;
            for (int n = 0; n < ids.Length; n++)
            {
                int id = ids[n];
                for (int m = 0; m < mats.Length; m++)
                {
                    Material mat = mats[m];
                    if (mat == null || !mat.HasProperty(id))
                    {
                        continue;
                    }
                    Color src = mat.GetColor(id);
                    float magnitude = Mathf.Max(src.r, Mathf.Max(src.g, src.b));
                    if (magnitude < BlackEpsilon)
                    {
                        break; // 这个材质上该属性是黑的 → 不参与染色，看下一个属性
                    }
                    if (list == null)
                    {
                        list = new List<Slot>(ids.Length);
                    }
                    list.Add(new Slot { Id = id, Magnitude = magnitude, Alpha = src.a });
                    break; // 一个属性只认第一个拥有它的材质（子网格共用同一个 MPB）
                }
            }
            return (list == null) ? NoSlots : list.ToArray();
        }

        /// <summary>
        /// 保留原亮度、只换色相，再乘整体亮度倍率：按原色最大分量等比缩放，alpha 用原值。
        /// intensity = 1 时结果和游戏原版的亮度完全一致。
        /// </summary>
        private static Color Rescale(Color color, float magnitude, float alpha, float intensity)
        {
            if (intensity < 0f)
            {
                intensity = 0f;
            }
            float m = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
            if (m <= 0.00001f)
            {
                // 本 mod 的颜色本身是黑的（理论上不会发生）：原样写回，至少不改变亮度
                return new Color(color.r, color.g, color.b, alpha);
            }
            float k = magnitude * intensity / m;
            return new Color(color.r * k, color.g * k, color.b * k, alpha);
        }
    }
}
