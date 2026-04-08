using BabelRhino8.Domain;
using Rhino;

namespace BabelRhino8.Services;

public interface IBabelModelService
{
    BabelBuildResult CreateModel(RhinoDoc doc, BabelModelSpec spec);
}
