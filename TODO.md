# TODO list for 1.1.x


* Phase 1 done: `DialMock.CadModel`
* Phase 2 done: `DialAutoCADPlugin`
* Phase 3 done: `AutoCadMock` console host calling the plugin and producing a CAD summary


 
---



## Phase 4 — tighten plugin boundary for external-host usage

Goal: make `AutoCadMock` behave more like a real consumer of the plugin DLL, not like an internal app that knows Core input types.

### Tasks

1. Introduce a plugin-side request model, for example:

   * `DialAutoCADPlugin/Models/DialCadRequest.cs`

2. Change the public plugin API from:

   * `CadDrawing Build(DialSpec spec)`

   to:

   * `CadDrawing Build(DialCadRequest request)`

3. Map `DialCadRequest` internally to `DialSpec` inside `DialAutoCADPlugin`

4. Remove the direct `DialMock.Core` reference from `AutoCadMock`

5. Update `AutoCadMock` to construct `DialCadRequest` instead of `DialSpec`

### Expected result

* `AutoCadMock` references only:

  * `DialAutoCADPlugin`
  * `DialMock.CadModel`
* `AutoCadMock` no longer knows Core input types
* plugin boundary becomes closer to a real external DLL contract

---

## Phase 5 — improve CAD mapping semantics

Goal: make CAD output more explicit and more useful for downstream CAD/export workflows.

### Tasks

1. Keep existing mappings:

   * `Line2 -> CadLine`
   * `Arc2 -> CadArc`
   * `Text2 -> CadText`

2. Add plugin-owned CAD extras deliberately and explicitly:

   * center circle
   * optional title text
   * optional unit text

3. Stop relying only on “last line = needle” once a cleaner distinction exists

   * either by explicit mapping convention
   * or by richer internal classification before mapping

4. Expand semantic layers as needed:

   * `DIAL_ARC`
   * `DIAL_TICKS`
   * `DIAL_LABELS`
   * `DIAL_NEEDLE`
   * `DIAL_CENTER`
   * `DIAL_META`

5. Review text defaults for CAD output:

   * text height
   * rotation handling
   * alignment strategy if needed later

### Expected result

* CAD drawing is more readable in downstream tools
* layer semantics are stable
* CAD output is no longer just a raw mirror of Core primitives

---

## Phase 6 — introduce DXF export service

Goal: export `CadDrawing` to DXF through a reusable service, not ad hoc host logic.

### Tasks

1. Add an export abstraction, for example:

   * `ICadDrawingExporter`

2. Decide export placement:

   * either in `DialAutoCADPlugin`
   * or in a dedicated project such as `DialMock.Dxf`

3. Implement first DXF writer for:

   * layers
   * lines
   * arcs
   * text
   * circles if already added

4. Keep DXF as an export format only

   * internal abstraction remains `CadDrawing`

5. Make export callable from `AutoCadMock` through the plugin/exporter contract

### Expected result

* `CadDrawing` can be exported to a real `.dxf` file
* exporter logic is reusable
* host does not contain DXF transformation rules

---

## Phase 7 — extend AutoCadMock from summary host to export host

Goal: turn `AutoCadMock` into a practical verification host for the plugin pipeline.

### Tasks

1. Keep the existing summary output

2. Add DXF file generation to disk

3. Add output folder conventions

4. Add simple command-line options later if useful, for example:

   * output path
   * sample selection
   * alternate request values

5. Keep `AutoCadMock` focused on orchestration:

   * build request
   * call plugin
   * call exporter
   * save file
   * print diagnostics

### Expected result

* `AutoCadMock` becomes the practical end-to-end mock CAD host
* it remains a host, not a second business layer

---

## Phase 8 — validation in external CAD viewer

Goal: verify that exported CAD output is geometrically and semantically correct.

### Tasks

1. Open DXF in a free CAD viewer

2. Verify:

   * arc orientation
   * text placement
   * label spacing
   * needle direction
   * scale consistency
   * layer separation

3. Compare visual result against the Blazor SVG preview

4. Note mismatches between SVG and CAD expectations

5. Fix mapping/export conventions as needed

### Expected result

* CAD output is visually trustworthy
* geometry is validated outside the codebase
* SVG and CAD paths are known to be aligned or intentionally different

---

## Phase 9 — add CAD-path tests

Goal: protect the new branch with tests before the exporter and mapping grow further.

### Tasks

1. Add tests for `DialAutoCADPlugin`

2. Verify:

   * valid request produces `CadDrawing`
   * invalid request fails predictably
   * expected entity counts
   * expected layer assignment
   * needle entity classification
   * center/meta entities when introduced

3. Add exporter tests once DXF is implemented:

   * expected sections exist
   * expected entities are written
   * output is non-empty and structurally valid

### Expected result

* CAD path becomes regression-safe
* future refactors are less risky

---

## Phase 10 — decide how far Core cleanup should go

Goal: review remaining architectural fuzziness without destabilizing the existing working app.

### Tasks

1. Reassess whether `DialRenderData` should remain in Core
2. Keep Core renderer-neutral
3. Avoid pulling CAD concerns back into Core
4. Decide whether current `DialDrawing` shape remains sufficient
5. Only refactor Core if there is clear pressure from:

   * CAD mapping
   * SVG rendering
   * duplicated interpretation rules

### Expected result

* Core stays stable and neutral
* cleanup happens only when justified

---

# Current architectural guardrails

These should remain true during all remaining phases:

* `DialMock.Core` stays free of Blazor and CAD host logic
* `DialAutoCADPlugin` consumes Core and produces CAD-neutral output
* `AutoCadMock` calls the plugin, not Core engines directly
* DXF is an export format, not the internal model
* AutoCAD SDK types do not enter the neutral CAD model
* host apps orchestrate; they do not own transformation logic

---
 