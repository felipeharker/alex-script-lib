using System;
using System.Collections.Generic;

namespace BabelRhino8.Domain;

public sealed class BabelBuildResult
{
    public BabelBuildResult(string modelKey, string groupName, IReadOnlyList<Guid> objectIds)
    {
        ModelKey = modelKey;
        GroupName = groupName;
        ObjectIds = objectIds;
    }

    public string ModelKey { get; }
    public string GroupName { get; }
    public IReadOnlyList<Guid> ObjectIds { get; }
}
