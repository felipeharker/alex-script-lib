using Eto.Forms;
using Rhino;

namespace BabelRhino8.Panels;

public sealed class BabelPanel : Panel
{
    public BabelPanel()
    {
        Content = new StackLayout
        {
            Padding = 8,
            Spacing = 6,
            Items =
            {
                new Label { Text = "Babel" },
                new Label { Text = "Use command: BabelCreateModel" }
            }
        };
    }
}

public static class BabelPanels
{
    public static void RegisterPanel(Rhino.PlugIns.PlugIn plugin)
    {
        RhinoApp.WriteLine("Babel panel registration is disabled for compatibility mode.");
    }

    public static void OpenPanel()
    {
        // Intentionally no-op in compatibility mode.
    }
}
