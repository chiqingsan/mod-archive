using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QuickSell;

internal static class HintArtwork
{
    internal static Texture2D? Texture { get; private set; }
    internal static readonly Rect[] Regions = new Rect[7];

    internal static void Load()
    {
        var sources = new List<Texture2D>(4);
        try
        {
            foreach (string name in new[] { "Demand", "Stolen", "Contraband" })
            {
                using var input = typeof(HintArtwork).Assembly.GetManifestResourceStream("QuickSell.Highlights." + name + ".png")!;
                using var bytes = new MemoryStream();
                input.CopyTo(bytes);
                var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                sources.Add(source);
                if (!ImageConversion.LoadImage(source, bytes.ToArray())) throw new InvalidDataException("高亮图片无法解码：" + name);
            }
            // Pixel-art badge: a blue chip with a light upward learning arrow.
            var learning = new Texture2D(192, 192, TextureFormat.RGBA32, false);
            sources.Add(learning);
            string[] pixels = { "..########..", ".#BBBBBBBB#.", "#BBBBWWBBBB#", "#BBBWWWWBBB#", "#BBWWBBWWBB#", "#BBBBWWBBBB#", "#BBBBWWBBBB#", "#BBBBWWBBBB#", "#BBBBWWBBBB#", "#BBBBBBBBBB#", ".#BBBBBBBB#.", "..########.." };
            var colors = new Color32[192 * 192];
            for (int y = 0; y < 192; y++) for (int x = 0; x < 192; x++)
                colors[(191 - y) * 192 + x] = pixels[y / 16][x / 16] switch { '#' => new Color32(24, 20, 35, 255), 'B' => new Color32(69, 150, 188, 255), 'W' => new Color32(239, 232, 197, 255), _ => new Color32(0, 0, 0, 0) };
            learning.SetPixels32(colors); learning.Apply();
            // Decode and pack only on enable. All visible items share one small,
            // non-readable texture; no image loading or pixel scans while holding.
            Texture = new Texture2D(2, 2, TextureFormat.RGBA32, false)
            { name = "QuickSellHighlights", filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
            var packed = Texture.PackTextures(sources.ToArray(), 2, 512, true);
            // Approved 1254x1254 source art. Tight opaque bounds plus two source
            // pixels of padding; demand corners are sampled independently.
            Regions[0] = Region(packed[0], 185, 918, 152, 151); // top left
            Regions[1] = Region(packed[0], 918, 918, 152, 151); // top right
            Regions[2] = Region(packed[0], 185, 185, 152, 151); // bottom left
            Regions[3] = Region(packed[0], 918, 185, 152, 151); // bottom right
            Regions[4] = Region(packed[1], 276, 225, 784, 757);
            Regions[5] = Region(packed[2], 154, 203, 946, 847);
            Regions[6] = packed[3];
        }
        finally
        {
            foreach (var source in sources) UnityEngine.Object.Destroy(source);
        }
    }

    private static Rect Region(Rect packed, float x, float y, float width, float height)
        => new(packed.x + x / 1254 * packed.width, packed.y + y / 1254 * packed.height,
            width / 1254 * packed.width, height / 1254 * packed.height);

    internal static void Unload()
    {
        if (Texture != null) UnityEngine.Object.Destroy(Texture);
        Texture = null;
    }
}
