using System.Collections.Generic;
using Rhino.DocObjects;

namespace BabelRhino8.Domain;

public sealed class BabelModelSpec
{
    public BabelModelSpec(
        string displayName,
        double sidewallDepthInches,
        bool includeFrontFace,
        IlluminationType illumination,
        string sidewallColorHex,
        string faceColorHex,
        string illumColorHex,
        IReadOnlyList<ObjRef> sourceObjects)
    {
        DisplayName = displayName;
        SidewallDepthInches = sidewallDepthInches;
        IncludeFrontFace = includeFrontFace;
        Illumination = illumination;
        SidewallColorHex = sidewallColorHex;
        FaceColorHex = faceColorHex;
        IllumColorHex = illumColorHex;
        SourceObjects = sourceObjects;
    }

    public string DisplayName { get; }
    public double SidewallDepthInches { get; }
    public bool IncludeFrontFace { get; }
    public IlluminationType Illumination { get; }
    public string SidewallColorHex { get; }
    public string FaceColorHex { get; }
    public string IllumColorHex { get; }
    public IReadOnlyList<ObjRef> SourceObjects { get; }
}
