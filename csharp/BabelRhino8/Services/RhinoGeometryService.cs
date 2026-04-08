using System;
using System.Collections.Generic;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace BabelRhino8.Services;

public sealed class RhinoGeometryService : IGeometryService
{
    public IReadOnlyList<Brep> ExtractSourceBreps(IEnumerable<ObjRef> sourceObjects)
    {
        var breps = new List<Brep>();
        foreach (var source in sourceObjects)
        {
            var brep = source.Brep();
            if (brep is not null)
            {
                breps.Add(brep);
            }
        }

        return breps;
    }

    public IReadOnlyList<Guid> BuildSidewalls(
        RhinoDoc doc,
        IEnumerable<Brep> sourceBreps,
        Vector3d direction,
        double depth,
        ObjectAttributes attributes)
    {
        var ids = new List<Guid>();
        foreach (var brep in sourceBreps)
        {
            var xform = Transform.Translation(direction * depth);
            foreach (var face in brep.Faces)
            {
                var faceBrep = face.DuplicateFace(false);
                if (faceBrep is null)
                {
                    continue;
                }

                faceBrep.Transform(xform);
                var id = doc.Objects.AddBrep(faceBrep, attributes);
                if (id != Guid.Empty)
                {
                    ids.Add(id);
                }
            }
        }

        return ids;
    }

    public IReadOnlyList<Guid> BuildTranslatedFaces(
        RhinoDoc doc,
        IEnumerable<Brep> sourceBreps,
        Vector3d direction,
        double distance,
        ObjectAttributes attributes)
    {
        var ids = new List<Guid>();
        var xform = Transform.Translation(direction * distance);

        foreach (var brep in sourceBreps)
        {
            var copy = brep.DuplicateBrep();
            copy.Transform(xform);
            var id = doc.Objects.AddBrep(copy, attributes);
            if (id != Guid.Empty)
            {
                ids.Add(id);
            }
        }

        return ids;
    }
}
