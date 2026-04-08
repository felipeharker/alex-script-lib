using System;
using System.Drawing;

namespace BabelRhino8.Infrastructure;

public static class ColorParser
{
    public static Color ParseOrDefault(string? hex, Color fallback)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return fallback;
        }

        var value = hex.Trim().TrimStart('#');
        if (value.Length != 6)
        {
            return fallback;
        }

        try
        {
            var r = Convert.ToInt32(value[..2], 16);
            var g = Convert.ToInt32(value.Substring(2, 2), 16);
            var b = Convert.ToInt32(value.Substring(4, 2), 16);
            return Color.FromArgb(r, g, b);
        }
        catch
        {
            return fallback;
        }
    }
}
