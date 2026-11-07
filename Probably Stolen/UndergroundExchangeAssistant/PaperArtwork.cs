using System;
using System.Collections.Generic;

namespace UndergroundExchangeAssistant;

internal static class PaperArtwork
{
    internal const float AnchorX = .44f, AnchorY = .42f, RelativeWidth = .40f;
    // The rim hugs the sheet exactly like the game's own selection boxes do.
    internal const float HoverScale = 1.00f;
    internal const int TargetWidth = 64;
    internal readonly struct Pixel
    {
        internal readonly byte R, G, B, A;
        internal Pixel(byte r, byte g, byte b, byte a = 255) { R = r; G = g; B = b; A = a; }
    }

    internal sealed class Artwork
    {
        internal readonly int Width, Height;
        internal readonly Pixel[] Normal, Outline;
        internal Artwork(int width, int height, Pixel[] normal, Pixel[] outline)
        { Width = width; Height = height; Normal = normal; Outline = outline; }
    }

    // Tight alpha bounds, nearest-neighbour sampling, identical padded canvases.
    // Source pixels use Unity's bottom-to-top order. The approved PNG stays intact.
    internal static Artwork Build(Pixel[] source, int width, int height)
    {
        if (width <= 0 || height <= 0 || source.Length != width * height)
            throw new ArgumentException("图片尺寸与像素数据不一致");
        int left = width, bottom = height, right = -1, top = -1;
        for (int y = 0; y < height; y++) for (int x = 0; x < width; x++)
        {
            if (source[y * width + x].A < 128) continue;
            left = Math.Min(left, x); right = Math.Max(right, x);
            bottom = Math.Min(bottom, y); top = Math.Max(top, y);
        }
        if (right < left) throw new ArgumentException("材料单图片没有可见内容");
        int faceHeight = Math.Max(1, (int)Math.Round((top - bottom + 1d) * TargetWidth / (right - left + 1)));
        const int padding = 3;
        int outputWidth = TargetWidth + padding * 2, outputHeight = faceHeight + padding * 2;
        var normal = new Pixel[outputWidth * outputHeight];
        for (int y = 0; y < faceHeight; y++) for (int x = 0; x < TargetWidth; x++)
        {
            int sx = left + Math.Min(right - left, (int)((x + .5) * (right - left + 1) / TargetWidth));
            int sy = bottom + Math.Min(top - bottom, (int)((y + .5) * (top - bottom + 1) / faceHeight));
            var pixel = source[sy * width + sx];
            if (pixel.A >= 128) normal[(y + padding) * outputWidth + x + padding] = new Pixel(pixel.R, pixel.G, pixel.B);
        }
        return new Artwork(outputWidth, outputHeight, normal, Outline(normal, outputWidth, outputHeight));
    }

    // Outline only exterior transparency. Interior holes and printed detail do
    // not acquire highlights. Eight neighbours make stepped diagonal edges join.
    internal static Pixel[] Outline(Pixel[] normal, int width, int height)
    {
        var exterior = new bool[normal.Length];
        var queue = new Queue<int>();
        for (int x = 0; x < width; x++) { Visit(x); Visit((height - 1) * width + x); }
        for (int y = 0; y < height; y++) { Visit(y * width); Visit(y * width + width - 1); }
        while (queue.Count > 0)
        {
            int at = queue.Dequeue(), x = at % width, y = at / width;
            if (x > 0) Visit(at - 1); if (x + 1 < width) Visit(at + 1);
            if (y > 0) Visit(at - width); if (y + 1 < height) Visit(at + width);
        }
        var outline = new Pixel[normal.Length];
        var cream = new Pixel(225, 209, 156);
        // One pixel wide in art space: the sheet is drawn close to 1:1 on screen,
        // so this reads as a single-pixel selection rim, like the game's own.
        const int reach = 1;
        for (int y = 0; y < height; y++) for (int x = 0; x < width; x++)
        {
            int at = y * width + x;
            if (!exterior[at]) continue;
            for (int dy = -reach; dy <= reach; dy++) for (int dx = -reach; dx <= reach; dx++)
            {
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < width && ny >= 0 && ny < height && normal[ny * width + nx].A >= 128)
                {
                    outline[at] = cream;
                    dy = reach; break;
                }
            }
        }
        return outline;

        void Visit(int at)
        {
            if (exterior[at] || normal[at].A >= 128) return;
            exterior[at] = true; queue.Enqueue(at);
        }
    }
}
