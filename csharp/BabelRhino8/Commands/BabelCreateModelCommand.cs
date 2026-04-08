using BabelRhino8.Domain;
using BabelRhino8.Infrastructure;
using BabelRhino8.Panels;
using BabelRhino8.Services;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input;
using Rhino.Input.Custom;

namespace BabelRhino8.Commands;

public sealed class BabelCreateModelCommand : Command
{
    public override string EnglishName => "BabelCreateModel";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
        var go = new GetObject();
        go.SetCommandPrompt("Select source surfaces or polysurfaces for Babel");
        go.GeometryFilter = ObjectType.Surface | ObjectType.PolysrfFilter;
        go.GroupSelect = true;
        go.EnablePreSelect(true, true);
        go.GetMultiple(1, 0);

        if (go.CommandResult() != Result.Success)
        {
            return go.CommandResult();
        }

        var depthInches = 4.0;
        if (RhinoGet.GetNumber("Sidewall depth (inches)", false, ref depthInches) != Result.Success)
        {
            return Result.Cancel;
        }

        var includeFace = true;
        var faceOption = new GetOption();
        faceOption.SetCommandPrompt("Include front face");
        var yesIndex = faceOption.AddOption("Yes");
        faceOption.AddOption("No");
        var faceResult = faceOption.Get();
        if (faceResult == GetResult.Option)
        {
            includeFace = faceOption.OptionIndex() == yesIndex;
        }
        else if (faceResult != GetResult.Nothing)
        {
            return Result.Cancel;
        }

        var illum = IlluminationType.NonIllum;
        var illumOption = 2;
        var illumGetter = new GetOption();
        illumGetter.SetCommandPrompt("Illumination type");
        illumGetter.AddOption("FrontIllum");
        illumGetter.AddOption("ReverseIllum");
        illumGetter.AddOption("NonIllum");

        if (illumGetter.Get() == GetResult.Option)
        {
            illumOption = illumGetter.OptionIndex() - 1;
        }

        if (illumOption == 0)
        {
            illum = IlluminationType.FrontIllum;
        }
        else if (illumOption == 1)
        {
            illum = IlluminationType.ReverseIllum;
        }

        var sidewallColorHex = "#C8C8C8";
        var faceColorHex = includeFace ? "#FFFFFF" : string.Empty;
        var illumColorHex = illum == IlluminationType.NonIllum ? string.Empty : "#FFFFFF";

        var refs = new ObjRef[go.ObjectCount];
        for (var i = 0; i < go.ObjectCount; i++)
        {
            refs[i] = go.Object(i);
        }

        var service = new BabelModelService(
            new RhinoLayerService(),
            new RhinoMaterialService(),
            new RhinoGeometryService(),
            new UnitConverter());

        try
        {
            var spec = new BabelModelSpec(
                "model",
                depthInches,
                includeFace,
                illum,
                sidewallColorHex,
                faceColorHex,
                illumColorHex,
                refs);

            var result = service.CreateModel(doc, spec);
            RhinoApp.WriteLine($"Babel model created: {result.GroupName}");
            BabelPanels.OpenPanel();
            return Result.Success;
        }
        catch (System.Exception ex)
        {
            RhinoApp.WriteLine($"Babel failed: {ex}");
            return Result.Failure;
        }
    }
}
