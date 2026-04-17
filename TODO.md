# TODO list
 

 To DO LIST for 1.1.x 

 

## Architecture validation

### What matches your expectations

The existing split is already meaningful:

* **DialMock.Core** contains the dial domain logic and primitive geometry generation.
* **DialMock** is a separate UI/rendering application.
* The current dependency direction is correct:

  * `DialMock.Core` is referenced by `DialMock`
  * Core does not reference Blazor
  * UI rendering lives outside Core

That part is sound.

### What is currently mixed or too early-bound

There are two architectural issues in the current solution:

#### 1. Core is neutral with respect to UI, but not yet neutral with respect to downstream rendering semantics

`DialMock.Core` currently returns this model:

* `Point2`
* `Line2`
* `Arc2`
* `Text2`
* `DialDrawing`

That is acceptable as an intermediate step, but it is already a drawing-oriented output model, not purely business/domain logic. In practice, this is fine for your project, because your stated goal is “geometry generation → neutral drawing model.” So I would not fight that.

But I would be precise about the distinction:

* `DialMock.Core` is **renderer-neutral**
* it is **not** “no-output-model”
* it already defines a neutral 2D drawing contract

That means the next CAD layer should consume `DialDrawing`, not bypass it.

#### 2. `DialRenderData` is currently mixed into Core even though part of it is presentation-facing

`DialRenderData` contains:

* title
* unit
* min/max
* preview value
* tick count

That is not catastrophic, but architecturally it is fuzzy. It is not pure validation output, and it is not pure geometry. It exists mainly because the SVG preview needs display metadata.

For the CAD phase, I would avoid growing that pattern further. Do not let `DialAutoCADPlugin` depend on `DialRenderData` unless there is a clear need.

## What is present in the ZIP

The ZIP contains these projects only:

* `DialMock.Core`
* `DialMock`

I did **not** find:

* `DialAutoCADPlugin`
* `AutoCadMock`
* a solution file in the ZIP root

So this is not yet the next-phase scaffold. It is still the first-phase base.

## My verdict on the current architecture

Yes, the current architecture is aligned enough to proceed, with one important adjustment:

**Do not let the plugin layer invent its own direct business logic.**
The plugin layer should translate from Core’s neutral drawing output into a CAD-oriented model.

That means your next phase should be based on this pipeline:

`DialSpec -> DialMock.Core -> DialDrawing -> CAD mapping layer -> CadDrawing -> DXF export`

That is the correct separation.

---

## Proposed project structure additions

I agree with your preview almost entirely. For a portfolio-quality result, I would add **three** projects, not just two.

### Recommended structure

```text
DialMock.Core
    domain + dial geometry generation
    outputs DialDrawing

DialMock.CadModel
    CAD-neutral entity model
    CadDrawing, CadLine, CadArc, CadCircle, CadText, CadLayer

DialAutoCADPlugin
    mapping/application layer
    consumes DialMock.Core
    produces DialMock.CadModel
    optionally coordinates export services

AutoCadMock
    host/test app
    calls DialAutoCADPlugin only
    writes DXF / runs export scenario

DialMock
    Blazor preview
    consumes DialMock.Core
```

## Why `DialMock.CadModel` should be separate

This is the key architectural point.

If you put `CadDrawing`, `CadLine`, `CadArc`, etc. directly inside `DialAutoCADPlugin`, then the plugin project becomes both:

* the reusable integration layer
* the owner of the CAD model contract

That is too much responsibility.

A separate `DialMock.CadModel` gives you:

* a stable CAD-neutral contract
* easier testing
* future reuse by:

  * DXF exporter
  * mock viewer
  * future real AutoCAD adapter
  * maybe later SVG-from-CAD or PDF export

So yes: **Step 1 should be `DialMock.CadModel`**.

---

## Dependency direction I recommend

```text
DialMock.Core
    ↑
DialAutoCADPlugin
    ↑
AutoCadMock

DialMock.CadModel
    ↑
DialAutoCADPlugin
    ↑
AutoCadMock

DialMock.Core
    ↑
DialMock
```

More explicitly:

* `DialMock.Core` references nobody
* `DialMock.CadModel` references nobody
* `DialAutoCADPlugin` references:

  * `DialMock.Core`
  * `DialMock.CadModel`
* `AutoCadMock` references:

  * `DialAutoCADPlugin`
  * possibly `DialMock.CadModel` only if needed for display/export inspection
* `DialMock` references:

  * `DialMock.Core`

I would keep `DialMock` completely independent from the CAD branch.

---

## First minimal interfaces

Keep these small. Do not over-abstract on day one.

### In `DialMock.CadModel`

Entities first, no service abstractions yet:

```text
CadDrawing
CadEntity (base/marker)
CadLine
CadArc
CadCircle
CadText
CadLayer
```

Minimal common concerns:

* geometry
* layer name
* maybe line type / color later
* no AutoCAD SDK types
* no DXF writer logic here

### In `DialAutoCADPlugin`

Start with only two service interfaces.

#### 1. Core-to-CAD mapping

```csharp
public interface IDialCadBuilder
{
    CadDrawing Build(DialSpec spec);
}
```

This is the main application service.

Internally it can:

* validate spec
* call `DialEngine`
* map `DialDrawing` to `CadDrawing`

#### 2. Optional export abstraction

If you want DXF in the plugin layer from the start:

```csharp
public interface ICadDrawingExporter
{
    void Export(CadDrawing drawing, Stream output);
}
```

or

```csharp
public interface ICadDrawingExporter
{
    string ExportToString(CadDrawing drawing);
}
```

For the first pass, I prefer `Stream output`.

### Internal mapper interface

Only if you want one extra seam for testing:

```csharp
internal interface IDialDrawingToCadMapper
{
    CadDrawing Map(DialDrawing drawing, DialSpec spec);
}
```

This one can remain internal. It does not need to be public API yet.

---

## Minimal first entity set

I would start with these fields only.

### `CadDrawing`

* `IReadOnlyList<CadLayer> Layers`
* `IReadOnlyList<CadEntity> Entities`

or even simpler:

* `List<CadEntity> Entities`

At the beginning, layers can be implicit by entity `LayerName`.

### `CadLine`

* `Start`
* `End`
* `LayerName`

### `CadArc`

* `Center`
* `Radius`
* `StartAngleDeg`
* `SweepAngleDeg`
* `LayerName`

### `CadCircle`

* `Center`
* `Radius`
* `LayerName`

### `CadText`

* `Position`
* `Content`
* `Height`
* `RotationDeg`
* `LayerName`

### `CadLayer`

* `Name`

That is enough for first DXF export.

---

## Recommended layer naming from day one

Even for MVP, do not dump everything into layer `0`.

Use semantic layers immediately:

* `DIAL_ARC`
* `DIAL_TICKS`
* `DIAL_LABELS`
* `DIAL_NEEDLE`
* `DIAL_CENTER`
* `DIAL_META`

This makes DXF output inspectable in a real CAD viewer and makes the project look engineered, not improvised.

---

## Step-by-step implementation plan

## Phase 1 — stabilize architecture

1. Add a solution file if missing.
2. Create `DialMock.CadModel`.
3. Move no existing Core geometry yet.
4. Keep Core unchanged while introducing the CAD branch.

## Phase 2 — define CAD-neutral model

5. Implement:

   * `CadPoint2`
   * `CadEntity`
   * `CadLine`
   * `CadArc`
   * `CadCircle`
   * `CadText`
   * `CadLayer`
   * `CadDrawing`
6. Keep model small and serialization-friendly.

## Phase 3 — create plugin layer

7. Create `DialAutoCADPlugin`.
8. Add `IDialCadBuilder`.
9. Implement a first `DialCadBuilder` using:

   * `DialRuleEngine`
   * `DialEngine`
   * mapper from `DialDrawing` to `CadDrawing`

## Phase 4 — mapping strategy

10. Map Core output into CAD entities:

* `Line2 -> CadLine`
* `Arc2 -> CadArc`
* `Text2 -> CadText`

11. Add plugin-owned CAD extras not present in Core:

* center circle
* optional title/unit text

12. Assign semantic layers.

This is important: the plugin is allowed to enrich the drawing for CAD use, but it should do so explicitly and predictably.

## Phase 5 — DXF export

13. Decide where DXF export lives.

For clean architecture, I recommend one of these two options:

### Option A — export in `DialAutoCADPlugin`

Good for now if you want fewer projects.

### Option B — separate `DialMock.Dxf`

Better long term if you expect multiple CAD outputs.

For your current phase, Option A is acceptable.
For portfolio quality, Option B is stronger.

My recommendation:

* start with exporter in `DialAutoCADPlugin`
* keep it behind `ICadDrawingExporter`
* split later only if it grows

## Phase 6 — host application

14. Create `AutoCadMock` as a console app first.
15. It should:

* create `DialSpec`
* call `IDialCadBuilder`
* export DXF
* write file to disk

16. No direct call from host to Core.

## Phase 7 — validation

17. Open DXF in a free viewer.
18. Verify:

* arc orientation
* text positions
* layer separation
* scale consistency
* needle direction

---

## Architectural decisions I recommend now

## 1. Keep Core unchanged at first

Do not refactor Core before the CAD branch exists.
You do not yet have enough pressure to justify restructuring it.

## 2. Do not make DXF the internal model

DXF is an export format, not your internal abstraction.

Internal model:

* `CadDrawing`

External format:

* DXF

That distinction matters.

## 3. Do not make `AutoCadMock` the exporter owner

`AutoCadMock` should be a host, not the place where CAD logic lives.

Its job is:

* wiring
* invoking plugin services
* saving output
* maybe displaying basic diagnostics

## 4. Avoid premature AutoCAD terminology

Do not create types like:

* `AcadEntity`
* `AcadDocument`
* `AutoCadLayerService`

You are not integrating with AutoCAD yet.
Use generic CAD terminology.

---

## One refinement to your preview

You wrote:

```text
AutoCadMock
    consumes CadDrawing
    exports DXF
```

I would tighten that to:

```text
AutoCadMock
    calls DialAutoCADPlugin
    receives CadDrawing or export result
```

and preferably:

```text
AutoCadMock
    calls DialAutoCADPlugin services
    does not own CAD transformation rules
```

If `AutoCadMock` starts exporting DXF itself, the host begins to absorb reusable behavior that belongs lower in the stack.

So the cleaner version is:

* `DialAutoCADPlugin` builds `CadDrawing`
* optionally `DialAutoCADPlugin` also exports DXF
* `AutoCadMock` orchestrates and writes files

--- 