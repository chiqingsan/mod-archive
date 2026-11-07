using System;
using System.Collections.Generic;
using System.Text;

namespace FastLoad;

/// <summary>一次读档的精简化解档计划：源文件、时间戳、缓存的大库存串。</summary>
internal sealed class SavePlan
{
    internal string SourceFile = "";

    internal DateTime SourceStamp;

    internal bool PreloadReady;

    internal bool SlimReady;

    internal int RawLength;

    internal int SlimLength;

    internal int PreloadLength;

    /// <summary>被精简掉的库存 JSON（已反转义的真实内容），按 playerStore 字段名索引。</summary>
    internal readonly Dictionary<string, string> InvJson = new(StringComparer.Ordinal);
}

/// <summary>
/// 读档文件的外科手术：把 15.8M 字符的存档改写成约 0.8M 字符的精简副本
/// （去掉最大的几个库存 JSON，并把 storeStation 提到 playerStore 前面），
/// 同时生成只含 networkUpgrade 的预览迷你文件。
/// 只做字符串层面的剪切/拼接，不解析物品数据，不改游戏反序列化逻辑。
/// </summary>
internal static class SaveSurgeon
{
    internal const string PreloadFile = "FastLoad_preload.es3";

    internal const string SlimFile = "FastLoad_slim.es3";

    /// <summary>精简副本里清空的库存字段（其余字段原样保留）。</summary>
    private static readonly string[] StripFields =
    {
        "mainInvJSON",
        "soldInvJSON",
        "docInvJSON",
        "showcaseInvJSON",
    };

    private static readonly HashSet<string> StripSet = new(StripFields, StringComparer.Ordinal);

    internal static SavePlan? Current { get; private set; }

    /// <summary>与 SaveFiles.ResolveLoadFile 一致的源文件解析，但用 ES3 判断存在性。</summary>
    internal static string? ResolveSource(NewGameData? ngd)
    {
        if (ngd == null || ngd.saveSlot == -1)
        {
            return null;
        }

        try
        {
            if (!string.IsNullOrEmpty(ngd.saveFile) && ES3.FileExists(ngd.saveFile))
            {
                return ngd.saveFile;
            }
        }
        catch
        {
            // 忽略，走槽位回退。
        }

        try
        {
            return SaveFiles.Slot(ngd.saveSlot);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>取（必要时构建）当前源文件的精简方案。任何失败都返回 null，调用方回退原版。</summary>
    internal static SavePlan? GetPlan(string? sourceFile)
    {
        if (string.IsNullOrEmpty(sourceFile))
        {
            return null;
        }

        try
        {
            var stamp = ES3.GetTimestamp(sourceFile);
            var cached = Current;
            if (cached != null
                && cached.SlimReady
                && string.Equals(cached.SourceFile, sourceFile, StringComparison.Ordinal)
                && cached.SourceStamp == stamp)
            {
                return cached;
            }

            var plan = Build(sourceFile, stamp);
            Current = plan;
            return plan;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"精简读档文件构建失败（回退原版）: {ex.Message}");
            return null;
        }
    }

    private static SavePlan Build(string sourceFile, DateTime stamp)
    {
        var raw = ES3.LoadRawString(sourceFile);
        if (string.IsNullOrEmpty(raw))
        {
            throw new InvalidOperationException("源存档为空");
        }

        var top = SkipWs(raw, 0);
        if (top >= raw.Length || raw[top] != '{')
        {
            throw new InvalidOperationException("源存档不是 JSON 对象");
        }

        if (!TryFindProperty(raw, top, "playerStore", out _, out var ps, out var pe))
        {
            throw new InvalidOperationException("缺少 playerStore");
        }

        if (!TryFindProperty(raw, top, "storeStation", out _, out var ss, out var se))
        {
            throw new InvalidOperationException("缺少 storeStation");
        }

        var pobj = SkipWs(raw, ps);
        if (pobj >= raw.Length || raw[pobj] != '{')
        {
            throw new InvalidOperationException("playerStore 不是对象");
        }

        if (!TryFindProperty(raw, pobj, "__type", out _, out var ts, out var te))
        {
            throw new InvalidOperationException("playerStore 缺少 __type");
        }

        if (!TryFindProperty(raw, pobj, "value", out _, out var vs, out var ve))
        {
            throw new InvalidOperationException("playerStore 缺少 value");
        }

        var typeLiteral = raw.Substring(ts, te - ts);
        var vobj = SkipWs(raw, vs);
        if (vobj >= raw.Length || raw[vobj] != '{')
        {
            throw new InvalidOperationException("playerStore.value 不是对象");
        }

        if (!TryFindProperty(raw, vobj, "networkUpgrade", out _, out var ns, out var ne))
        {
            throw new InvalidOperationException("playerStore 缺少 networkUpgrade");
        }

        var netRaw = raw.Substring(ns, ne - ns);

        // 预览迷你文件：只要 networkUpgrade 字典（SHOWCASE_I/II、DOSSIER_UPGRADE、TRASHCAN_UPGRADE 都在里面）。
        var preload = "{\"playerStore\":{\"__type\":" + typeLiteral + ",\"value\":{\"networkUpgrade\":" + netRaw + "}}}\n";

        // 精简副本：清空最大的几个库存 JSON，storeStation 放前面，第二次 LoadInto 不必跳过整个 playerStore。
        var invJson = new Dictionary<string, string>(StringComparer.Ordinal);
        var inner = new StringBuilder((int)Math.Max(1024, (ve - vs) * 0.9));
        inner.Append('{');
        var first = true;
        var pos = SkipWs(raw, vobj + 1);
        while (pos < raw.Length && raw[pos] != '}')
        {
            if (raw[pos] != '"')
            {
                throw new InvalidOperationException($"playerStore 字段解析异常（位置 {pos}）");
            }

            var nameEnd = ReadStringEnd(raw, pos);
            var name = Unescape(raw, pos + 1, nameEnd - pos - 2);
            var colon = SkipWs(raw, nameEnd);
            if (colon >= raw.Length || raw[colon] != ':')
            {
                throw new InvalidOperationException($"playerStore 字段 {name} 缺少冒号");
            }

            var valStart = SkipWs(raw, colon + 1);
            var valEnd = ScanValueEnd(raw, valStart);

            if (!first)
            {
                inner.Append(',');
            }

            first = false;
            inner.Append('"').Append(name).Append("\":");
            if (StripSet.Contains(name))
            {
                invJson[name] = Unescape(raw, valStart + 1, valEnd - valStart - 2);
                inner.Append("\"\"");
            }
            else
            {
                inner.Append(raw, valStart, valEnd - valStart);
            }

            pos = SkipWs(raw, valEnd);
            if (pos < raw.Length && raw[pos] == ',')
            {
                pos = SkipWs(raw, pos + 1);
            }
        }

        inner.Append('}');

        var station = raw.Substring(ss, se - ss);
        var slim = "{\"storeStation\":" + station + ",\"playerStore\":{\"__type\":" + typeLiteral + ",\"value\":" + inner + "}}\n";

        ES3.SaveRaw(preload, PreloadFile);
        ES3.SaveRaw(slim, SlimFile);

        var plan = new SavePlan
        {
            SourceFile = sourceFile,
            SourceStamp = stamp,
            PreloadReady = true,
            SlimReady = true,
            RawLength = raw.Length,
            PreloadLength = preload.Length,
            SlimLength = slim.Length,
        };

        foreach (var pair in invJson)
        {
            plan.InvJson[pair.Key] = pair.Value;
        }

        if (Core.DumpStats?.Value == true)
        {
            Core.Log?.Log($"[FastLoad] 精简存档构建：{raw.Length:N0} → {slim.Length:N0} 字符（-{100.0 * (raw.Length - slim.Length) / raw.Length:F1}%），预览 {preload.Length:N0} 字符，缓存库存 {invJson.Count} 个");
        }

        return plan;
    }

    /// <summary>与 ES3 JSON reader 一致的转义还原：\b\f\n\r\t 特殊，其余反斜杠丢弃。</summary>
    internal static string Unescape(string s, int start, int count)
    {
        if (count <= 0)
        {
            return string.Empty;
        }

        var end = start + count;
        var hasEscape = false;
        for (var i = start; i < end; i++)
        {
            if (s[i] == '\\')
            {
                hasEscape = true;
                break;
            }
        }

        if (!hasEscape)
        {
            return s.Substring(start, count);
        }

        var sb = new StringBuilder(count);
        for (var i = start; i < end; i++)
        {
            var c = s[i];
            if (c == '\\' && i + 1 < end)
            {
                var next = s[i + 1];
                switch (next)
                {
                    case 'b': sb.Append('\b'); break;
                    case 'f': sb.Append('\f'); break;
                    case 'n': sb.Append('\n'); break;
                    case 'r': sb.Append('\r'); break;
                    case 't': sb.Append('\t'); break;
                    default: sb.Append(next); break;
                }

                i++;
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    private static int SkipWs(string s, int i)
    {
        while (i < s.Length)
        {
            var c = s[i];
            if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
            {
                i++;
                continue;
            }

            return i;
        }

        return i;
    }

    /// <summary>i 指向开引号，返回闭引号之后的位置。</summary>
    private static int ReadStringEnd(string s, int i)
    {
        if (i >= s.Length || s[i] != '"')
        {
            throw new InvalidOperationException($"期望字符串（位置 {i}）");
        }

        var j = i + 1;
        while (j < s.Length)
        {
            var c = s[j];
            if (c == '\\')
            {
                j += 2;
                continue;
            }

            if (c == '"')
            {
                return j + 1;
            }

            j++;
        }

        throw new InvalidOperationException("字符串未闭合");
    }

    /// <summary>i 指向值的第一个字符，返回值的结束位置（不含）。</summary>
    private static int ScanValueEnd(string s, int i)
    {
        if (i >= s.Length)
        {
            throw new InvalidOperationException("值超出范围");
        }

        var c = s[i];
        if (c == '"')
        {
            return ReadStringEnd(s, i);
        }

        if (c == '{' || c == '[')
        {
            var depth = 0;
            var j = i;
            while (j < s.Length)
            {
                var ch = s[j];
                if (ch == '"')
                {
                    j = ReadStringEnd(s, j);
                    continue;
                }

                if (ch == '{' || ch == '[')
                {
                    depth++;
                }
                else if (ch == '}' || ch == ']')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return j + 1;
                    }
                }

                j++;
            }

            throw new InvalidOperationException("对象/数组未闭合");
        }

        var k = i;
        while (k < s.Length)
        {
            var ch = s[k];
            if (ch == ',' || ch == '}' || ch == ']' || ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
            {
                break;
            }

            k++;
        }

        return k;
    }

    /// <summary>在 objStart 指向的对象里查找属性，返回值的起止位置。</summary>
    private static bool TryFindProperty(string s, int objStart, string name, out int nameStart, out int valStart, out int valEnd)
    {
        nameStart = -1;
        valStart = -1;
        valEnd = -1;

        if (objStart < 0 || objStart >= s.Length || s[objStart] != '{')
        {
            return false;
        }

        var pos = SkipWs(s, objStart + 1);
        while (pos < s.Length && s[pos] != '}')
        {
            if (s[pos] != '"')
            {
                return false;
            }

            var nameEnd = ReadStringEnd(s, pos);
            var colon = SkipWs(s, nameEnd);
            if (colon >= s.Length || s[colon] != ':')
            {
                return false;
            }

            var v = SkipWs(s, colon + 1);
            if (NameEquals(s, pos, nameEnd, name))
            {
                nameStart = pos;
                valStart = v;
                valEnd = ScanValueEnd(s, v);
                return true;
            }

            pos = SkipWs(s, ScanValueEnd(s, v));
            if (pos < s.Length && s[pos] == ',')
            {
                pos = SkipWs(s, pos + 1);
            }
        }

        return false;
    }

    private static bool NameEquals(string s, int quoteStart, int quoteEnd, string name)
    {
        var len = quoteEnd - quoteStart - 2;
        if (len != name.Length)
        {
            return false;
        }

        for (var i = 0; i < len; i++)
        {
            if (s[quoteStart + 1 + i] != name[i])
            {
                return false;
            }
        }

        return true;
    }
}
