using System.Collections.Generic;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace BabelRhino8.Services;

public interface IGeometryService
{
    IReadOnlyList<Brep> ExtractSourceBreps(IEnumerable<ObjRef> sourceObjects);
    IReadOnlyList<System.Guid> BuildSidewalls(RhinoDoc doc, IEnumerable<Brep> sourceBreps, Vector3d direction, double depth, ObjectAttributes attributes);
    IReadOnlyList<System.Guid> BuildTranslatedFaces(RhinoDoc doc, IEnumerable<Brep> sourceBreps, Vector3d direction, double distance, ObjectAttributes attributes);
}
