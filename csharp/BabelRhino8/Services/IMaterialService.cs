using System.Drawing;
using Rhino;

namespace BabelRhino8.Services;

public interface IMaterialService
{
    int EnsureMaterialIndex(RhinoDoc doc, string materialName, Color color);
}
