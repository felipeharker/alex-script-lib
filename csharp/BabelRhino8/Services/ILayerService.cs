using System.Drawing;
using Rhino;
using Rhino.DocObjects;

namespace BabelRhino8.Services;

public interface ILayerService
{
    Layer EnsureLayer(RhinoDoc doc, string rootLayerName, string childLayerName, Color color);
}
