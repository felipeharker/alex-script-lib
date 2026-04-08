using Rhino;
using RhinoMath = Rhino.RhinoMath;

namespace BabelRhino8.Services;

public sealed class UnitConverter : IUnitConverter
{
    public double InchesToDoc(RhinoDoc doc, double inches)
    {
        var scale = RhinoMath.UnitScale(Rhino.UnitSystem.Inches, doc.ModelUnitSystem);
        return inches * scale;
    }
}
