# Dial Architecture

## Goal

Build a reusable dial engine that can serve multiple hosts:

- `DialMock` for fast browser preview with SVG
- `AutoCadMock` for demo/testing of CAD-oriented output
- `DialAutoCADPlugin` for real AutoCAD integration

The shared dial logic must stay independent from UI and from AutoCAD-specific APIs.

---

## Core Idea

`DialAutoCADPlugin` is the reusable CAD/plugin integration layer.

It consumes the shared dial logic from `DialMock.Core` and exposes CAD-oriented behavior that a host can call.

`AutoCadMock` is a demo/test host that exercises the same plugin layer outside the real AutoCAD environment.

This allows us to validate plugin behavior without requiring AutoCAD for every test, while keeping the integration logic reusable for a real AutoCAD host.

---

## Architecture Overview

```mermaid
graph TD
    A[Dial specification input<br/>title, unit, range, ticks, preview value]

    A --> B[DialMock.Core<br/>shared dial logic]

    B --> C[DialMock<br/>Blazor web app]
    C --> D[SVG renderer]
    D --> E[Browser demo]

    B --> F[DialAutoCADPlugin<br/>reusable CAD/plugin integration layer]
    F --> G[Neutral CAD entity model<br/>lines, arcs, text, layers, inserts]

    G --> H[AutoCadMock<br/>demo/test host]
    H --> I[DXF export or mock CAD preview]
    I --> J[Free CAD viewer]

    G --> K[Real AutoCAD host]
    K --> L[AutoCAD managed API adapter]
    L --> M[Real AutoCAD DB objects]

    subgraph Shared Logic
        B
        F
        G
    end

    subgraph Demo Hosts
        C
        D
        E
        H
        I
        J
    end

    subgraph Real CAD Environment
        K
        L
        M
    end
````

---

## Main Projects

### `DialMock.Core`

Reusable shared logic.

Contains:

* dial rules
* validation
* scale calculations
* domain model
* geometry generation
* neutral CAD/entity model

This project must not depend on:

* Blazor
* SVG
* AutoCAD
* HTML
* UI framework code

---

### `DialMock`

Blazor web application for preview and quick simulation.

Responsibilities:

* provide browser UI
* let the user enter dial parameters
* call shared logic from `DialMock.Core`
* render SVG preview for fast visual feedback

This is a standalone web application.

It is started with:

```bash
dotnet DialMock.dll
```

or inside Docker through:

```dockerfile
ENTRYPOINT ["dotnet", "DialMock.dll"]
```

So `DialMock.dll` is an ASP.NET Core application assembly, not a plugin DLL.

---

### `DialAutoCADPlugin`

Reusable AutoCAD-oriented integration layer.

Responsibilities:

* consume the dial logic from `DialMock.Core`
* expose plugin-oriented services and integration points
* build CAD-oriented entities or instructions
* later translate them into real AutoCAD database objects through the AutoCAD API

This project should ideally contain:

* no web UI
* no demo-only concerns
* no Blazor code

It should remain reusable by both:

* `AutoCadMock`
* real AutoCAD

---

### `AutoCadMock`

Demo/test host for the plugin layer.

Responsibilities:

* provide a user-facing test environment
* call `DialAutoCADPlugin`
* simulate host behavior outside real AutoCAD
* display or export the generated CAD-oriented result

Possible outputs:

* DXF export
* structured CAD description
* mock CAD preview

`AutoCadMock` is not real AutoCAD. It is a host used to test the same plugin layer in a controlled environment.

---

## DLL Types

### 1. Class Library DLL

Example:

* shared logic library
* plugin library
* helper library

This is not launched directly by the user.

It is loaded by another process or referenced by another application.

Typical examples:

* AutoCAD loads `DialAutoCADPlugin.dll`
* `DialMock` references `DialMock.Core.dll`
* `AutoCadMock` references `DialAutoCADPlugin.dll`

---

### 2. Application DLL

Example:

* ASP.NET Core web app
* console app

This is started by the .NET host.

Typical example:

```bash
dotnet DialMock.dll
```

So:

* `DialMock.dll` is an application DLL
* `DialAutoCADPlugin.dll` is a class library DLL

They are both `.dll` files, but they play very different roles.

---

## Runtime Model

### `DialMock`

Host process:

* `.NET runtime`
* `ASP.NET Core`
* `Kestrel`

Flow:

* Docker starts the .NET runtime
* the runtime loads `DialMock.dll`
* `Program.cs` configures the web host
* Kestrel serves routes, Razor components, static assets, and interactive Blazor UI

---

### `DialAutoCADPlugin`

Host process:

* real AutoCAD

Flow:

* AutoCAD loads the plugin DLL
* AutoCAD calls plugin entry points through its managed API
* the plugin uses shared logic from `DialMock.Core`
* the plugin creates CAD-oriented output and eventually real AutoCAD entities

This is not started with `dotnet DialAutoCADPlugin.dll`.

It is loaded by AutoCAD.

---

### `AutoCadMock`

Host process:

* its own host application

Flow:

* the mock application starts normally
* it references `DialAutoCADPlugin`
* it calls the same plugin layer that real AutoCAD would use
* it displays or exports the resulting CAD-oriented output

So you run:

```bash
dotnet AutoCadMock.dll
```

not:

```bash
dotnet AutoCadMock.dll DialAutoCADPlugin.dll
```

The plugin DLL is loaded automatically as a dependency.

---

## Dependency Direction

The intended dependency chain is:

```text
DialMock.Core
    ↑
DialAutoCADPlugin
    ↑
AutoCadMock
```

and separately:

```text
DialMock.Core
    ↑
DialMock
```

For real integration:

```text
DialMock.Core
    ↑
DialAutoCADPlugin
    ↑
Real AutoCAD host
```

Important rule:

* `AutoCadMock` must call `DialAutoCADPlugin`
* it must not bypass the plugin and go directly to `DialMock.Core` for CAD generation

That way the mock host tests the same plugin layer that the real AutoCAD host will use.

---

## Neutral CAD Entity Model

Inside the shared/plugin architecture, we use a neutral CAD-oriented model.

Examples:

* `CadLine`
* `CadArc`
* `CadCircle`
* `CadText`
* `CadLayer`
* `CadInsert`

This is not yet a real AutoCAD runtime object.

It is our own representation of CAD geometry and structure.

Why this matters:

* `DialMock` can render the shared logic as SVG
* `AutoCadMock` can display or export CAD-oriented output
* `DialAutoCADPlugin` can translate the same model into real AutoCAD entities

This is stronger than pretending the mock host is real AutoCAD.

---

## Output Strategy

### Browser Preview

`DialMock` renders:

* SVG

Purpose:

* fast visual feedback
* easy demo access from a browser
* simple preview of dial rules and rendering behavior

---

### CAD-Oriented Demo Output

`AutoCadMock` should preferably export:

* DXF

Why DXF:

* suitable for 2D dial geometry
* viewable in free tools
* useful as an exchange/demo format
* much easier to expose on a project page than a proprietary CAD runtime object

So the likely demo chain is:

* `DialMock` -> quick SVG simulation
* `AutoCadMock` -> DXF-oriented demo/export
* free viewer -> inspect the exported result

---

### Real AutoCAD Output

`DialAutoCADPlugin` should later create:

* real AutoCAD database objects
* lines
* arcs
* text
* layers
* inserts / blocks

through the AutoCAD managed API.

---

## Summary

Current understanding:

* `DialMock.dll` is a standalone ASP.NET Core web application assembly
* `DialAutoCADPlugin.dll` is a class library intended to be loaded by AutoCAD
* `AutoCadMock` is a separate host application that exercises the same plugin layer outside real AutoCAD
* `DialMock.Core` remains the shared reusable logic base

This gives us:

* a reusable architecture
* a realistic plugin-oriented design
* a demo host for development and portfolio presentation
* a clear path toward real AutoCAD integration
 


 ## dll CALL

In the industry 2 possible DLL call pattern.
We eill start here by patternA


 ### Pattern A — host-driven interaction
 
 The CAD user is in AutoCAD, triggers a command, enters/selects values, and the DLL generates the result inside the CAD session. This matches Autodesk’s command-based plugin model. ([Autodesk][1])
 
 In your simulation, that would mean:
 
 * `AutoCadMock` has a UI
 * user edits values there
 * `AutoCadMock` calls the DLL
 * DLL produces DXF now, and later maybe native AutoCAD objects
 
 ### Pattern B — workflow / PLM-driven
 
 A PLM or workflow system determines the item, revision, or configuration, and then a CAD/export process is invoked with those values. In Teamcenter-style environments, CAD tools often operate on PLM-managed data and workflows, rather than on manually typed inputs alone. ([Autodesk Forums][2])
 
 In that case the host would:
 
 * receive prepared values from a file, database, or workflow step
 * call the DLL with those values
 * generate the result
 
 That is more enterprise-like, but also more complex.


 # Short explanation of what we are doing (architecture statement)

You can use this in `architecture.md` or README.

## Host-Driven CAD Plugin Workflow

This project follows a **host-driven interaction model**, similar to how AutoCAD plugins are typically used in industry.

In this model, the user interacts with a host application, and the reusable CAD logic is implemented in a DLL.

### Workflow

1. The user enters dial parameters in a host application.
2. The host builds a structured request (`DialCadRequest`).
3. The host calls the reusable plugin DLL (`DialAutoCADPlugin`).
4. The plugin generates CAD geometry.
5. The plugin exports the drawing to DXF.
6. The host displays status information and stores the result.

### Current simulation

Since real AutoCAD integration is not yet implemented, the system uses a mock host:

```text
User
  ↓
AutoCadMock (desktop UI host)
  ↓
DialAutoCADPlugin.dll
  ↓
DXF output
```

### Responsibilities

**AutoCadMock (Host)**

* collects user input
* builds `DialCadRequest`
* calls the plugin DLL
* saves output files
* displays status information

**DialAutoCADPlugin (DLL)**

* validates request data
* generates CAD geometry
* maps geometry to CAD entities
* exports DXF output

**DialMock.Core**

* contains reusable dial logic
* contains geometry generation rules
* independent of CAD and UI

---

# What “headless” means

This is important — and often misunderstood.

## Headless = no user interface

A **headless program** runs:

* without a window
* without user input dialogs
* without interactive UI
* usually inside scripts, CI pipelines, or containers

It receives instructions automatically.

---

# Your current interactive mode

This is **not headless**.

Example:

```text
User opens AutoCadMock.exe
User enters values in form
User clicks "Generate"
DXF is created
```

That is:

```text
Interactive Mode
(UI present)
```