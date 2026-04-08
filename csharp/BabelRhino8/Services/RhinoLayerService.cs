using System;
using System.Drawing;
using Rhino;
using Rhino.DocObjects;

namespace BabelRhino8.Services;

public sealed class RhinoLayerService : ILayerService
{
    public Layer EnsureLayer(RhinoDoc doc, string rootLayerName, string childLayerName, Color color)
    {
        var rootLayer = doc.Layers.FindName(rootLayerName);
        var rootIndex = rootLayer?.Index ?? -1;
        if (rootIndex < 0)
        {
            rootIndex = doc.Layers.Add(rootLayerName, Color.White);
            if (rootIndex < 0)
            {
                throw new InvalidOperationException($"Failed to create root layer: {rootLayerName}");
            }

            rootLayer = doc.Layers[rootIndex];
        }

        if (rootLayer is null)
        {
            throw new InvalidOperationException($"Root layer lookup failed: {rootLayerName}");
        }

        foreach (var layer in doc.Layers)
        {
            if (!string.Equals(layer.Name, childLayerName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (layer.ParentLayerId != rootLayer.Id)
            {
                continue;
            }

            if (layer.Color != color)
            {
                layer.Color = color;
                doc.Layers.Modify(layer, layer.Index, false);
            }

            return layer;
        }

        var newLayer = new Layer
        {
            Name = childLayerName,
            ParentLayerId = rootLayer.Id,
            Color = color
        };

        var index = doc.Layers.Add(newLayer);
        if (index < 0)
        {
            throw new InvalidOperationException($"Failed to create layer: {rootLayerName}::{childLayerName}");
        }

        return doc.Layers[index];
    }
}
