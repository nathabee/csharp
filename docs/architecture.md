
# architecture



## (planned) architecture:

csharp/
│
├── DialMock.slnx
│
├── DialMock/                (Blazor UI)
│
├── DialMock.Core/           (rules + geometry + drawing contract)
│
├── DialMock.Render.Svg/     (optional later)
│
├── DialMock.AutoCAD/        (future plugin project)
│
└── DialMock.Tests/


1 solution many project :
- DialMock is designed to run on a server : it is using the .NET blazor (Web UI framework)
- DialMock.Core : contain the core logic , is rendering object to te UI. Core is the source of the dial description, independent of rendering technology (ie Autodcad, SVG)




> A renderer-neutral dial engine reusable inside industrial CAD (AutoCAD plugin) and any other front-end.



---

## 1. Architectural Target State

You are building this:

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

## 2. Strict Coordinate Convention (Non-Negotiable)

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

## 3. Renderer-Neutral Geometry Contract

show interface

---

## 4. Dial Engine Contract

Core now produces **complete drawing primitives**:

* Outer arc
* Major ticks (lines)
* Needle (line)
* Labels (text)
* Title
* Unit

UI will not compute geometry anymore.

---

## 5. Rendering Adapters

### 5.1 SVG Renderer (Blazor project)

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

### 5.2 AutoCAD Renderer (Future)

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

## 6. Project Structure (Final Form)

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

## Notes


### Logic in the Core

logic should not be part of the UI, that should just do rerendering
so we make a neutral geometry contract inside :DialMock.Core/Models/DialLayoutData.cs

the service DialMock.Core/Services/DialGeometryCalculator.cs contains the logic
if you add a service you must register it in the UI: add the builder.Services.AddScoped<DialGeometryCalculator>(); inside the program.cs

in the page.*.razor calling the service we need to :
- inject : @inject DialGeometryCalculator Geometry
- 

 
---
### Architecture layer:
 
Geometry does not call anything
DialEngine uses Geometry
DialRuleEngine uses Models
UI calls RuleEngine + DialEngine


DialMock.Core
├── Engine
│   └── DialEngine.cs
├── Geometry
│   ├── Arc2.cs
│   ├── DialDrawing.cs
│   ├── Line2.cs
│   ├── Point2.cs
│   └── Text2.cs
├── Models
│   ├── DialRenderData.cs
│   ├── DialSpec.cs
│   └── ValidationResult.cs
└── Services
    └── DialRuleEngine.cs

#### Models

input and validation result
DialSpec
ValidationResult
DialRenderData for now

#### Geometry

dumb geometry types only
Point2, Line2, Arc2, Text2, DialDrawing
these are just data containers

#### Engine / Services

DialEngine = builds geometry
DialRuleEngine = validates and prepares non-geometry display data


