using System.Drawing;
using Rhino;
using Rhino.DocObjects;

namespace BabelRhino8.Services;

public sealed class RhinoMaterialService : IMaterialService
{
    public int EnsureMaterialIndex(RhinoDoc doc, string materialName, Color color)
    {
        var existingIndex = doc.Materials.Find(materialName, true);
        if (existingIndex >= 0)
        {
            var existing = doc.Materials[existingIndex];
            existing.DiffuseColor = color;
            existing.Name = materialName;
            doc.Materials.Modify(existing, existingIndex, true);
            return existingIndex;
        }

        var material = new Material
        {
            Name = materialName,
            DiffuseColor = color
        };

        return doc.Materials.Add(material);
    }
}
