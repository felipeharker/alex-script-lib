using BabelRhino8.Panels;
using Rhino;
using Rhino.PlugIns;

namespace BabelRhino8;

public sealed class BabelRhinoPlugin8 : PlugIn
{
    public static BabelRhinoPlugin8 Instance { get; private set; } = null!;

    public BabelRhinoPlugin8()
    {
        Instance = this;
    }

    protected override LoadReturnCode OnLoad(ref string errorMessage)
    {
        BabelPanels.RegisterPanel(this);
        RhinoApp.WriteLine("BabelRhino8 plugin loaded.");
        return LoadReturnCode.Success;
    }
}
