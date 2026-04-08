using System;
using System.Collections.Generic;
using System.Linq;
using Rhino;

namespace BabelRhino8.Infrastructure;

public static class LayerNaming
{
    public const string Root = "model";

    public static string Sidewalls(string key) => $"sidewalls-{key}";
    public static string Face(string key) => $"face-{key}";
    public static string FrontIllum(string key) => $"f-illum-{key}";
    public static string ReverseIllum(string key) => $"r-illum-{key}";

    public static string SidewallMaterial(string key) => $"sw-{key}";
    public static string FaceMaterial(string key) => $"face-{key}";
    public static string GroupName(string key, string displayName) => $"babel-{key}-{displayName}";

    public static string NextModelKey(RhinoDoc doc)
    {
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var layer in doc.Layers)
        {
            if (!layer.Name.StartsWith("sidewalls-", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var maybe = layer.Name.Replace("sidewalls-", string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(maybe))
            {
                keys.Add(maybe);
            }
        }

        for (var c = 'a'; c <= 'z'; c++)
        {
            var key = c.ToString();
            if (!keys.Contains(key))
            {
                return key;
            }
        }

        return DateTime.UtcNow.ToString("yyyyMMddHHmmss");
    }
}
