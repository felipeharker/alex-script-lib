using System;
using System.Collections.Generic;
using System.Drawing;
using BabelRhino8.Domain;
using BabelRhino8.Infrastructure;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace BabelRhino8.Services;

public sealed class BabelModelService : IBabelModelService
{
    private readonly ILayerService _layerService;
    private readonly IMaterialService _materialService;
    private readonly IGeometryService _geometryService;
    private readonly IUnitConverter _unitConverter;

    public BabelModelService(
        ILayerService layerService,
        IMaterialService materialService,
        IGeometryService geometryService,
        IUnitConverter unitConverter)
    {
        _layerService = layerService;
        _materialService = materialService;
        _geometryService = geometryService;
        _unitConverter = unitConverter;
    }

    public BabelBuildResult CreateModel(RhinoDoc doc, BabelModelSpec spec)
    {
        if (spec.SourceObjects.Count == 0)
        {
            throw new InvalidOperationException("Select one or more surfaces before running Babel.");
        }

        var modelKey = LayerNaming.NextModelKey(doc);
        var sourceBreps = _geometryService.ExtractSourceBreps(spec.SourceObjects);
        if (sourceBreps.Count == 0)
        {
            throw new InvalidOperationException("No valid surface/brep input found in the current selection.");
        }

        var cplane = doc.Views.ActiveView?.ActiveViewport?.ConstructionPlane() ?? Plane.WorldXY;
        var direction = cplane.ZAxis;

        var sidewallColor = ColorParser.ParseOrDefault(spec.SidewallColorHex, Color.LightGray);
        var sidewallLayer = _layerService.EnsureLayer(doc, LayerNaming.Root, LayerNaming.Sidewalls(modelKey), sidewallColor);
        sidewallLayer.RenderMaterialIndex = _materialService.EnsureMaterialIndex(doc, LayerNaming.SidewallMaterial(modelKey), sidewallColor);
        doc.Layers.Modify(sidewallLayer, sidewallLayer.Index, true);

        var sidewallAttrs = new ObjectAttributes { LayerIndex = sidewallLayer.Index };
        var depth = _unitConverter.InchesToDoc(doc, spec.SidewallDepthInches);
        var objectIds = new List<Guid>();

        objectIds.AddRange(_geometryService.BuildSidewalls(doc, sourceBreps, direction, depth, sidewallAttrs));

        if (spec.IncludeFrontFace)
        {
            var faceColor = ColorParser.ParseOrDefault(spec.FaceColorHex, Color.White);
            var faceLayer = _layerService.EnsureLayer(doc, LayerNaming.Root, LayerNaming.Face(modelKey), faceColor);
            faceLayer.RenderMaterialIndex = _materialService.EnsureMaterialIndex(doc, LayerNaming.FaceMaterial(modelKey), faceColor);
            doc.Layers.Modify(faceLayer, faceLayer.Index, true);

            var faceAttrs = new ObjectAttributes { LayerIndex = faceLayer.Index };
            var faceOffset = depth + _unitConverter.InchesToDoc(doc, 0.03125);
            objectIds.AddRange(_geometryService.BuildTranslatedFaces(doc, sourceBreps, direction, faceOffset, faceAttrs));
        }

        if (spec.Illumination != IlluminationType.NonIllum)
        {
            var illumColor = ColorParser.ParseOrDefault(spec.IllumColorHex, Color.White);
            var illumLayerName = spec.Illumination == IlluminationType.FrontIllum
                ? LayerNaming.FrontIllum(modelKey)
                : LayerNaming.ReverseIllum(modelKey);

            var illumLayer = _layerService.EnsureLayer(doc, LayerNaming.Root, illumLayerName, illumColor);
            var illumAttrs = new ObjectAttributes { LayerIndex = illumLayer.Index };

            var offset = _unitConverter.InchesToDoc(doc, 0.0625);
            if (spec.Illumination == IlluminationType.FrontIllum)
            {
                offset += depth;
            }
            else
            {
                offset *= -1;
            }

            objectIds.AddRange(_geometryService.BuildTranslatedFaces(doc, sourceBreps, direction, offset, illumAttrs));
        }

        var groupName = LayerNaming.GroupName(modelKey, spec.DisplayName);
        var groupIndex = doc.Groups.Add(groupName);
        doc.Groups.AddToGroup(groupIndex, objectIds);
        doc.Views.Redraw();

        return new BabelBuildResult(modelKey, groupName, objectIds);
    }
}
