# Babel for Rhino 8 (C#) — Build, Compile, and Install Guide

This guide translates the workflow in `scripts/babel-app-notes.md` into a maintainable Rhino 8 plugin architecture using C# and Visual Studio.


## 0) Canonical project location (post-integration)

The Rhino plugin source now lives in **`scripts/csharp/BabelRhino8/`** (Visual Studio solution + maintainable service architecture).

- Keep using this directory as the single source of truth for future work.
- The older experimental copy in `scripts/csharp/babel-rhino8/` has been removed to avoid duplicate sources.

---

## 1) Scope and goals

Based on the notes, Babel should:

- Run as a Rhino panel (dockable UI)
- Let users create/update/delete/select **Babel models**
- Build model parts from selected source surfaces:
  - Sidewalls
  - Optional front face
  - Optional illumination geometry
- Assign layer structure, colors, and materials consistently
- Stay compatible with Rhino 8 and be maintainable as features grow

---

## 2) Recommended technology stack (Rhino 8 compatible)

- **Language:** C#
- **Framework target:** `net7.0-windows` (current Rhino 8 plugin baseline)
- **Rhino SDK:** `RhinoCommon` via Rhino plugin template / Rhino package references
- **UI:** Eto.Forms panel (Rhino-native cross-platform UI toolkit)
- **Data format:** JSON for saved model metadata (versioned schema)
- **Tests:**
  - Unit tests for pure business logic (naming, offsets, validation)
  - Optional Rhino.Inside-based integration tests for geometry commands

> Why this stack: It keeps geometry logic and UI separated, minimizes Rhino API coupling in core logic, and supports long-term updates.

---

## 3) Suggested plugin architecture

Use a layered approach so UI, domain logic, and Rhino document operations are decoupled:

```text
BabelRhino8/
  BabelRhino8.csproj
  Plugin/
    BabelPlugin.cs                 # Rhino PlugIn entry
  Commands/
    CreateBabelModelCommand.cs     # Optional command entrypoint
  Panels/
    BabelPanel.cs                  # Eto UI panel
  Domain/
    BabelModelSpec.cs              # Input/output model definition
    IlluminationType.cs
    LayerNaming.cs                 # sidewalls-a, face-a, f-illum-a rules
    UnitsPolicy.cs                 # inch/mm conversions and tolerances
  Services/
    IBabelModelService.cs
    BabelModelService.cs           # orchestration + validation
    RhinoLayerService.cs           # layer creation + colors
    RhinoMaterialService.cs        # plaster material provisioning
    RhinoGeometryService.cs        # extrusion + offsets + grouping
    RhinoSelectionService.cs       # source surface retrieval
  Persistence/
    BabelModelRecord.cs
    BabelStorageService.cs         # save/load metadata in document strings or JSON
  Infrastructure/
    Logging.cs
    RhinoDocExtensions.cs
```

### Design principles

- **Single responsibility:** each service handles one concern
- **Dependency inversion:** panel depends on interfaces, not concrete Rhino calls
- **Versioned data model:** add `SchemaVersion` in persisted records
- **Idempotent operations:** re-running update should not duplicate layers/materials unexpectedly
- **Deterministic naming:** centralize naming in `LayerNaming`

---

## 4) Mapping the app notes to implementation

From `babel-app-notes.md`, map each workflow stage to explicit methods:

- `CreateNewModel(...)`
  - Validate user selected source surfaces
  - Allocate model key (`a`, `b`, `c`...) based on existing Babel models
- `BuildSidewalls(depth, color)`
  - Ensure `model::sidewalls-x` layer exists
  - Extrude source surfaces by depth
  - Ensure material `sw-x` (plaster + color)
- `BuildFace(faceColor, includeFrontFace)`
  - If disabled, skip
  - Ensure `model::face-x` layer exists
  - Place face at `sidewallDepth + 0.03125 in`
  - Ensure material `face-x`
- `BuildIllumination(type, color?)`
  - `front-illum`: create `model::f-illum-x` at `+0.0625 in`
  - `reverse-illum`: create `model::r-illum-x` at `-0.0625 in`
  - `non-illum`: no illumination geometry
- `FinalizeModel(name)`
  - Group all generated objects
  - Persist metadata for update/delete/rebuild

### Unit and tolerance policy

Rhino files can be inches, mm, etc. Store offsets in inches but convert using document units:

- Face offset: `0.03125 in`
- Illum offset: `0.0625 in`

Use one conversion utility and never hardcode unit multipliers inline.

---

## 5) Visual Studio setup (step-by-step)

## Prerequisites

1. Rhino 8 installed
2. Visual Studio 2022 (17.8+ recommended)
3. .NET 7 SDK installed
4. Rhino developer tools/template support:
   - Install Rhino/Grasshopper developer templates if available
   - If template is unavailable, use a class library and add Rhino references manually

## Create the plugin project

1. Open Visual Studio → **Create a new project**.
2. Choose **RhinoCommon Plug-In** template (preferred).
3. Project name: `BabelRhino8`.
4. Target framework: `net7.0-windows`.
5. Confirm plugin metadata (name/version/company).

If no Rhino template exists:

1. Create **Class Library** targeting `net7.0-windows`.
2. Add references to Rhino 8 assemblies (usually from Rhino install path) or official NuGet/package method recommended by McNeel.
3. Ensure output is a Rhino plugin assembly (`.rhp` when packaged by Rhino tooling).

---

## 6) Minimal code skeleton (recommended starting point)

### `BabelPlugin.cs`

```csharp
using Rhino;
using Rhino.PlugIns;

namespace BabelRhino8.Plugin;

public class BabelPlugin : PlugIn
{
    public static BabelPlugin Instance { get; private set; } = null!;

    public BabelPlugin()
    {
        Instance = this;
    }

    protected override LoadReturnCode OnLoad(ref string errorMessage)
    {
        RhinoApp.WriteLine("BabelRhino8 loaded.");
        return LoadReturnCode.Success;
    }
}
```

### `CreateBabelModelCommand.cs`

```csharp
using Rhino;
using Rhino.Commands;

namespace BabelRhino8.Commands;

public class CreateBabelModelCommand : Command
{
    public override string EnglishName => "BabelCreateModel";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
        // TODO: open panel/workflow and call IBabelModelService
        RhinoApp.WriteLine("BabelCreateModel started.");
        return Result.Success;
    }
}
```

### `BabelModelSpec.cs`

```csharp
namespace BabelRhino8.Domain;

public enum IlluminationType
{
    FrontIllum,
    ReverseIllum,
    NonIllum
}

public sealed record BabelModelSpec(
    string ModelKey,
    double SidewallDepthInches,
    bool IncludeFrontFace,
    IlluminationType IlluminationType,
    string SidewallColorHex,
    string? FaceColorHex,
    string? IllumColorHex
);
```

---

## 7) Compile and debug in Rhino 8

1. Set build configuration to `Debug`.
2. In project debug settings, set **Start external program** to Rhino 8 executable.
3. Build solution.
4. Launch debug; Rhino starts.
5. In Rhino command line, run your command (e.g., `BabelCreateModel`).
6. Verify:
   - expected layers/material names
   - geometry offsets
   - grouping and metadata persistence

Tip: Keep a dedicated test `.3dm` document with known input surfaces.

---

## 8) Install in Rhino 8

## Developer install (local)

1. In Rhino 8, run `PlugInManager`.
2. Choose **Install** and browse to your built plugin output (`.rhp` or template-defined output).
3. Enable plugin if not enabled automatically.
4. Restart Rhino if requested.

## Team distribution

- Produce a versioned release artifact (zip/msi/internal package)
- Include:
  - plugin binary
  - changelog
  - compatibility matrix (Rhino version + OS)
  - migration notes for persisted metadata schema changes

---

## 9) Maintainability checklist

Before each release:

- [ ] Geometry operations are in services, not panel code
- [ ] All layer/material naming comes from one naming module
- [ ] Unit conversions are centralized
- [ ] Non-destructive update path tested (edit existing model)
- [ ] Delete path removes/archives dependent objects safely
- [ ] Persisted records include schema version
- [ ] Log key operations for supportability

---

## 10) Rhino 8 compatibility guardrails

- Avoid undocumented APIs where possible
- Encapsulate any render-engine-specific calls (e.g., V-Ray light creation) behind an adapter interface so Babel still runs without V-Ray
- Keep a fallback mode when renderer API is unavailable:
  - create illum surfaces/layers
  - skip mesh-light creation with clear warning
- Pin and document tested Rhino 8 build numbers in release notes

---

## 11) Suggested next implementation milestone

Implement this order for a robust MVP:

1. Command + panel shell
2. Source surface selection and validation
3. Sidewall generation + layer/material assignment
4. Optional face generation
5. Illumination surface generation
6. Metadata persistence + list/update/delete in panel
7. Optional V-Ray adapter for mesh light creation

This sequence gives early usable value while preserving architecture quality.
