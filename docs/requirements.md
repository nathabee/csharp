# 1. Architectural Target State

You are building this:
A renderer-neutral dial engine reusable inside industrial CAD (AutoCAD plugin) and any other front-end.


```
Dial Engine (Core)
        ↓
Renderer Adapter Layer
        ↓
Concrete Targets
   - SVG (Blazor preview)
   - AutoCAD plugin
   - (Later: DXF export, PDF, etc.)
```

Core must know **nothing** about:

* SVG
* Blazor
* AutoCAD
* HTML
* Databases
* UI

Core must only know geometry and dial logic.

---

# 2. Strict Coordinate Convention (Non-Negotiable)

Define this once and never change it:

* Origin: (0,0) = dial center
* X axis: right
* Y axis: up (mathematical, NOT SVG)
* Angles: degrees
* 0° = positive X axis
* Positive angle = CCW

This matches:

* Mathematics
* AutoCAD
* Most CAD systems

SVG will adapt. AutoCAD will not.

So Core uses **mathematical coordinate system**.

---

# 3. Renderer-Neutral Geometry Contract

interface are defined in `DialMock.Core/Geometry/`
 
---

# 4. Dial Engine Contract

```csharp
public interface IDialEngine
{
    ValidationResult Validate(DialSpec spec);
    DialDrawing BuildDrawing(DialSpec spec);
}
```

Concrete implementation:

```csharp
public class DialEngine : IDialEngine
```

Core now produces **complete drawing primitives**:

* Outer arc
* Major ticks (lines)
* Needle (line)
* Labels (text)
* Title
* Unit

UI will not compute geometry anymore.

---

# 5. Rendering Adapters

## 5.1 SVG Renderer (Blazor project)

Create:

```
DialMock.Render.Svg (optional project)
```

Or keep inside UI project for now.

```csharp
public class SvgDialRenderer
{
    public string Render(DialDrawing drawing)
}
```

This class:

* Converts Y-up to Y-down
* Converts Arc2 to SVG path
* Converts Line2 to <line>
* Converts Text2 to <text>

All coordinate transformation happens here.

Core stays clean.

---

## 5.2 AutoCAD Renderer (Future)

Separate project:

```
DialMock.AutoCAD
```

References:

* DialMock.Core
* Autodesk.AutoCAD.DatabaseServices

Create:

```csharp
public class AutoCadDialRenderer
{
    public void Render(DialDrawing drawing, Database db)
}
```

This:

* Creates Arc entities
* Creates Line entities
* Creates DBText entities

No math.
Just mapping primitives.

---

# 6. Project Structure (Final Form)

```
csharp/
│
├── DialMock.slnx
│
├── DialMock.Core/
│   ├── Geometry/
│   ├── Engine/
│   ├── Models/
│   └── Validation/
│
├── DialMock/                (Blazor UI)
│
├── DialMock.AutoCAD/        (Plugin project)
│
├── DialMock.Tests/
│
└── docs/
```

This is industry-grade separation.

---

# 7. Phased Refactor Plan

We are currently at:

Step 3 — Refactor Core to output primitives

Here is the concrete execution order:

---

### Phase A — Remove Geometry from Razor

Delete:

* Polar()
* Angle math inside UI
* Tick loops in Razor

UI should only:

```csharp
var drawing = Engine.BuildDrawing(spec);
var svg = Renderer.Render(drawing);
```

Nothing else.

---

### Phase B — Move All Geometry to Core

Inside `DialEngine.BuildDrawing()`:

* Calculate angles
* Convert to points
* Create Line2 objects
* Create Arc2 object
* Create Text2 objects

Core now fully owns geometry.

---

### Phase C — Introduce Unit Tests

In `DialMock.Tests`:

* Test that needle is inside expected angle
* Test tick count
* Test arc sweep
* Test value-to-angle mapping

You now test geometry independent of UI.

This is critical before AutoCAD integration.

---

### Phase D — Create AutoCAD Plugin Skeleton

Later:

```bash
dotnet new classlib -n DialMock.AutoCAD
```

Target:

```
net48
```

Reference AutoCAD SDK assemblies.

Create:

```csharp
[CommandMethod("DIALMOCK")]
public void CreateDial()
```

Inside:

* Create DialSpec
* Call Core engine
* Call AutoCadDialRenderer

Done.

---

# 8. Industrial Considerations

Because you want industrial integration:

Add:

* Units abstraction (mm, inch)
* Tolerance handling
* Precision rounding strategy
* Scaling support
* Layer naming convention support

These go in Core later.

---
 