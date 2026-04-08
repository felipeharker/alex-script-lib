using Rhino;

namespace BabelRhino8.Services;

public interface IUnitConverter
{
    double InchesToDoc(RhinoDoc doc, double inches);
}
